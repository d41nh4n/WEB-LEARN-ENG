using System.Text.RegularExpressions;

namespace Allen.API;

public class CreateOrUpdateQuestionModelValidator : AbstractValidator<CreateOrUpdateQuestionModel>
{
    public CreateOrUpdateQuestionModelValidator()
    {
        RuleFor(x => x.ModuleType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(LearningModuleType)).Contains(value));

        RuleFor(x => x.ModuleItemId)
            .NotNull().WithMessage(ErrorMessageBase.Required);

        RuleFor(x => x.QuestionType)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(QuestionType)).Contains(value));


        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(1000).WithMessage(ErrorMessageBase.MaxLength);

        When(x => RequiresOptionsAndAnswerTypes.Contains(x.QuestionType), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);

            RuleFor(x => x.CorrectAnswer)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
        });

        RuleFor(x => x.File)
            .Must((model, file) =>
            {
                if (RequiresContentUrlTypes.Contains(model.QuestionType))
                {
                    // Bắt buộc phải có file
                    if (file == null || file.Length <= 0)
                        return false;

                    // Kiểm tra tên file
                    var fileName = file.FileName;
                    var regex = new Regex(@"^[a-zA-Z0-9._-]+$");
                    return regex.IsMatch(fileName);
                }

                // Nếu không phải Listening/Speaking thì phải null
                return file == null;
            })
            .WithMessage("File chỉ được phép có nếu QuestionType là Listening hoặc Speaking và tên file không chứa khoảng trắng hoặc ký tự đặc biệt.");
    }
    private static readonly string[] RequiresOptionsAndAnswerTypes = new[]
    {
        nameof(QuestionType.MultipleChoice),
        nameof(QuestionType.Ordering),
        nameof(QuestionType.Matching),
        nameof(QuestionType.FillInBlank),
        nameof(QuestionType.ShortAnswer),
        nameof(QuestionType.Listening),
        nameof(QuestionType.Speaking)
    };

    private static readonly string[] RequiresContentUrlTypes = new[]
    {
        nameof(QuestionType.Listening),
        nameof(QuestionType.Speaking)
    };
}
