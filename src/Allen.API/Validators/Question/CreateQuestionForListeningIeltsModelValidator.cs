using FluentValidation;

namespace Allen.API;

public class CreateQuestionForListeningIeltsModelValidator : AbstractValidator<CreateQuestionForListeningIeltsModel>
{
    public CreateQuestionForListeningIeltsModelValidator()
    {
        RuleFor(x => x.QuestionType)
            .NotEmpty().WithMessage("QuestionType is required.")
            .Must(value => Enum.GetNames(typeof(QuestionType)).Contains(value)
                || value is "SentenceCompletion" or "SummaryCompletion")
            .WithMessage("Invalid question type.");

        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage("Prompt is required.");

        RuleFor(x => x).Custom((model, context) =>
        {
            // Nếu parse được enum, lưu vào biến qType
            Enum.TryParse<QuestionType>(model.QuestionType, true, out var qType);

            // ✅ MultipleChoice → cần Options & CorrectAnswer
            if (qType == QuestionType.MultipleChoice)
            {
                if (model.Options == null || !model.Options.Any())
                    context.AddFailure(nameof(model.Options), "Options are required and cannot be empty.");

                if (string.IsNullOrWhiteSpace(model.CorrectAnswer))
                    context.AddFailure(nameof(model.CorrectAnswer), "CorrectAnswer is required for MultipleChoice.");
            }

            // ✅ TrueFalse → cần CorrectAnswer (Options có thể có True/False)
            else if (qType == QuestionType.TrueFalse)
            {
                if (string.IsNullOrWhiteSpace(model.CorrectAnswer))
                    context.AddFailure(nameof(model.CorrectAnswer), "CorrectAnswer is required for True/False.");
            }

            // ✅ Matching, Ordering, FillInBlank, ShortAnswer, SentenceCompletion, SummaryCompletion
            // chỉ cần SubQuestions hợp lệ, KHÔNG ép Options ở question cha
            else if (
                qType is QuestionType.Matching or QuestionType.Ordering or QuestionType.FillInBlank or QuestionType.ShortAnswer
                || model.QuestionType is "SentenceCompletion" or "SummaryCompletion"
            )
            {
                if (model.SubQuestions == null || !model.SubQuestions.Any())
                    context.AddFailure(nameof(model.SubQuestions), "SubQuestions are required and cannot be empty.");
                else
                {
                    var subValidator = new CreateSubQuestionForListeningIeltsModelValidator();
                    for (int i = 0; i < model.SubQuestions.Count; i++)
                    {
                        var sub = model.SubQuestions[i];
                        var result = subValidator.Validate(sub);
                        foreach (var err in result.Errors)
                        {
                            context.AddFailure($"SubQuestions[{i}].{err.PropertyName}", err.ErrorMessage);
                        }
                    }
                }
            }

            // ✅ Listening / Speaking → chỉ cần Prompt, không Options/SubQuestions
            else if (qType is QuestionType.Listening or QuestionType.Speaking)
            {
                if (model.Options != null && model.Options.Any())
                    context.AddFailure(nameof(model.Options), "Listening/Speaking should not have options.");
                if (model.SubQuestions != null && model.SubQuestions.Any())
                    context.AddFailure(nameof(model.SubQuestions), "Listening/Speaking should not have sub-questions.");
            }
        });
    }
}
