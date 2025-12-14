namespace Allen.API;

public class CreateOrUpdateQuestionForListeningModelValidator
    
    : AbstractValidator<CreateOrUpdateQuestionForListeningModel>
{
    public CreateOrUpdateQuestionForListeningModelValidator()
    {
        RuleFor(x => x.ModuleItemId)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .NotEqual(Guid.Empty).WithMessage("ModuleItemId cannot be an empty GUID.");

        RuleFor(x => x.Prompt)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(500).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.QuestionType)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(QuestionForListeningType)).Contains(value));

        When(x => x.QuestionType == QuestionForListeningType.SingleChoice.ToString(), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);

            RuleFor(x => x.CorrectAnswer)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);

            RuleFor(x => x.Label)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);

            RuleFor(x => x.ContentUrl).Null();


            RuleForEach(x => x.SubQuestions)
                .Null();

            RuleFor(x => x.TableMetadata).Null();
        });


        When(x => x.QuestionType == QuestionForListeningType.MultipleChoice.ToString(), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);

            // Trường hợp MCQ có subquestions (Questions 13–14)
            RuleForEach(x => x.SubQuestions)
                .SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
                .When(x => x.SubQuestions != null && x.SubQuestions.Any());

            // Trường hợp MCQ đơn (chỉ 1 câu hỏi)
            When(x => x.SubQuestions == null || !x.SubQuestions.Any(), () =>
            {
                RuleFor(x => x.CorrectAnswer)
                    .NotEmpty().WithMessage(ErrorMessageBase.Required);
            });

            RuleFor(x => x.TableMetadata).Null();
        });

        // ================================
        // 🟦 2️⃣ True/False/NotGiven
        // ================================
        When(x => x.QuestionType == QuestionForListeningType.TrueFalse.ToString(), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleFor(x => x.CorrectAnswer)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleFor(x => x.SubQuestions).Null();
            RuleFor(x => x.TableMetadata).Null();
        });

        // ================================
        // 🟨 3️⃣ Fill In Blank / Sentence Completion
        // ================================
        When(x => x.QuestionType == QuestionForListeningType.FillInBlank.ToString(), () =>
               {
                   RuleFor(x => x.Options).Null();
                   RuleForEach(x => x.SubQuestions)
                       .SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
                       .NotEmpty().WithMessage(ErrorMessageBase.Required);
                   RuleFor(x => x.TableMetadata).Null();
               });

        // ================================
        // 🟥 4️⃣ Matching
        // ================================
        When(x => x.QuestionType == QuestionForListeningType.Matching.ToString(), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleForEach(x => x.SubQuestions)
                .SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleFor(x => x.CorrectAnswer).Null();
            RuleFor(x => x.TableMetadata).Null();
        });

        // ================================
        // 🟪 5️⃣ Map / Diagram Labeling
        // ================================
        When(x => x.QuestionType == QuestionForListeningType.MapLabeling.ToString(), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleFor(x => x.ContentUrl)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleForEach(x => x.SubQuestions)
                .SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleFor(x => x.CorrectAnswer).Null();
            RuleFor(x => x.TableMetadata).Null();
        });

        // ================================
        // 🟫 6️⃣ TableCompletion
        // ================================
        When(x => x.QuestionType == QuestionForListeningType.TableCompletion.ToString(), () =>
        {
            RuleFor(x => x.TableMetadata)
                .NotNull().WithMessage(ErrorMessageBase.Required);
            RuleForEach(x => x.SubQuestions)
                .SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleFor(x => x.Options).Null();
            RuleFor(x => x.CorrectAnswer).Null();
        });
    }
    //private static readonly string[] RequiresOptionsAndAnswerTypes = new[]
    //{
    //    nameof(QuestionForListeningType.MultipleChoice),
    //    nameof(QuestionForListeningType.FillInBlank),
    //    nameof(QuestionForListeningType.TrueFalse),
    //};

    //private static readonly string[] RequiresSubQuestionOnly = new[]
    //{
    //    nameof(QuestionForListeningType.MapLabeling),
    //    nameof(QuestionForListeningType.Matching),
    //};
    //private static readonly string[] RequiresTableMetadataTypes = new[]
    //{
    //    nameof(QuestionForListeningType.TableCompletion),
    //};
}
