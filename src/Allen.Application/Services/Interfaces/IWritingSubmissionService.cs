namespace Allen.Application;

public interface IWritingSubmissionService
{
    IAsyncEnumerable<string> SubmitSentenceAsync(LearningWritingSubmitModel model);
    Task<OperationResult> IeltsWritingSubmitAsync(Guid userId, IeltsWritingSubmitModel model);
}