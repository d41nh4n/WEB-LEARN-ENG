using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace Allen.Application;

[RegisterService(typeof(IWritingSubmissionService))]
public class WritingSubmissionService(
    IWritingRepository _writingRepo,
    IGeminiService _geminiService,
    IWritingSubmissionRepository _submissionRepo,
    IUserPointService _userPointService,
    IUnitOfWork _uow
) : IWritingSubmissionService
{
    const int CostPerIeltsAss = 5;
    const int CostPerSentence = 1;

    public async Task<OperationResult> IeltsWritingSubmitAsync(Guid userId, IeltsWritingSubmitModel model)
    {
        await _uow.Repository<UserEntity>().GetByIdAsync(userId);
        var userPoint = await _uow.Repository<UserPointEntity>().GetByConditionAsync(x => x.UserId == userId);
        if (userPoint == null || userPoint.TotalPoints < CostPerIeltsAss)
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.InsufficientPoints, userPoint?.TotalPoints ?? 0, CostPerIeltsAss));

        try
        {
            var writing = await _writingRepo.GetByIdAsync(model.WritingId);
            var descriptionContent = $"Question: {writing.ContentEN}";
            if (!string.IsNullOrWhiteSpace(writing.SourceUrl))
            {
                descriptionContent += $"\n\n[TASK IMAGE SOURCE URL]: {writing.SourceUrl}";
            }
            var aiResponse = await _geminiService.AIAgentIeltsWritingAsync(new GeminiRequest
            {
                Prompt = model.Content,
                Description = descriptionContent,
                SessionId = model.WritingTaskType
		    });

            var cleaned = aiResponse
                .Replace("```json", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("```", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Trim();

            if (!JsonHelper.IsValidJson(cleaned))
                throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.InvalidFormat, "AI Response"));

            var result = JsonHelper.Deserialize<IeltsWritingResponse>(cleaned);
            if (result == null)
                throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.Invalid, "AI Response"));

            FullResponse fullResponse = null!;
            await _uow.ExecuteWithTransactionAsync(async () =>
            {
                fullResponse = await SaveIeltsScoresAsync(userId, result, model);
                await _userPointService.UsePointsInternalAsync(userId, CostPerIeltsAss);

                await _uow.SaveChangesAsync();
            });

            return OperationResult.SuccessResult("Submit successfully", fullResponse);
        }
        catch (Exception ex)
        {
            return OperationResult.Failure($"Submission failed: {ex.Message ?? ex.InnerException?.Message}");
        }
    }

    private async Task<FullResponse> SaveIeltsScoresAsync(Guid userId, IeltsWritingResponse result, IeltsWritingSubmitModel model)
    {
        var (tr, cc, lr, ga) = (result.TaskResponse, result.CoherenceCohesion, result.LexicalResource, result.GrammaticalAccuracy);
        var raw = (tr + cc + lr + ga) / 4.0m;
        var overall = Math.Round(raw * 2, MidpointRounding.AwayFromZero) / 2;

        var writing = await _writingRepo.GetByIdAsync(model.WritingId);
        var attemptRepo = _uow.Repository<UserTestAttemptEntity>();

        var attempt = await attemptRepo.GetByConditionAsync(a => a.UserId == userId && a.LearningUnitId == writing.LearningUnitId);

        if (attempt == null)
        {
            attempt = new UserTestAttemptEntity
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LearningUnitId = writing.LearningUnitId,
                OverallBand = overall,
                TimeSpent = model.TimeSpent ?? 0
            };
            await attemptRepo.AddAsync(attempt);
        }
        else
        {
            attempt.OverallBand = overall;
            attempt.TimeSpent = model.TimeSpent ?? attempt.TimeSpent;
        }

        var submission = new WritingSubmissionEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AttemptId = attempt.Id,
            WritingId = model.WritingId,
            Content = JsonSerializer.Serialize(result.Feedback, 
                new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) }),
            TaskResponse = tr,
            CoherenceCohesion = cc,
            LexicalResource = lr,
            GrammaticalAccuracy = ga
        };

        await _submissionRepo.AddAsync(submission);

        return new FullResponse
        {
            WritingSubmissionId = submission.Id,
            IeltsWritingResponse = result,
            TestAttempt = new TestAttempt
            {
                Id = attempt.Id,
                UserId = attempt.UserId,
                LearningUnitId = attempt.LearningUnitId,
                OverallBand = attempt.OverallBand,
                TimeSpent = attempt.TimeSpent
            }
        };
    }

    public async IAsyncEnumerable<string> SubmitSentenceAsync(LearningWritingSubmitModel model)
    {
        var writing = await _writingRepo.GetByIdAsync(model.WritingId);

        var userPoint = await _uow.Repository<UserPointEntity>().GetByConditionAsync(x => x.UserId == model.UserId);
        if (userPoint == null || userPoint.TotalPoints < CostPerSentence)
        {
            var msg = ErrorMessageBase.Format(ErrorMessageBase.InsufficientPoints, userPoint?.TotalPoints ?? 0, CostPerSentence);
            var error = JsonSerializer.Serialize(new { Success = false, Message = msg });
            yield return error + "\n";
            yield break;
        }

        var contentSource = (model.Mode == WritingModeType.VietToEng)
            ? writing.ContentEN
            : writing.ContentVN;

        var sentences = SplitSentences(contentSource!);
        if (model.SentenceIndex >= sentences.Count)
            throw new ValidationException("Sentence Index has exceeded the number of sentences in the article");

        await _uow.ExecuteWithTransactionAsync(async () =>
        {
            var used = await _userPointService.UsePointsInternalAsync(model.UserId, CostPerSentence);
            await _uow.SaveChangesAsync();
        });

        var currentSentence = sentences[model.SentenceIndex];
        await foreach (var chunk in _geminiService.AIAgentWritingAsync(new GeminiRequest
        {
            Prompt = model.Content,
            Description = currentSentence
        }))
        {
            yield return chunk;
        }

        bool isCorrect = model.Content == currentSentence;
        decimal point = isCorrect ? 10 : 0;

        var jsonString = JsonSerializer.Serialize(new { Point = point, IsCorrect = isCorrect });
        jsonString = jsonString + "\n";
        //Console.WriteLine("Yield return JSON chunk: " + jsonString);
        yield return jsonString;
    }

    private List<string> SplitSentences(string content)
    {
        var pattern = @"(?<!a\.m)(?<!p\.m)(?<=[\.!\?])\s+";
        var sentences = Regex.Split(content, pattern)
        .Select(s => s.Trim())
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .ToList();

        return sentences;
    }
}