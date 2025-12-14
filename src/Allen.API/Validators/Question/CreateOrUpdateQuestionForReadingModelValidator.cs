namespace Allen.API;

public class CreateOrUpdateQuestionForReadingModelValidator : AbstractValidator<CreateOrUpdateQuestionForReadingModel>
{
    public CreateOrUpdateQuestionForReadingModelValidator()
    {
        RuleFor(x => x.ModuleItemId)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .NotEqual(Guid.Empty).WithMessage("ModuleItemId cannot be an empty GUID.");

        RuleFor(x => x.Prompt)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .MaximumLength(500).WithMessage(ErrorMessageBase.MaxLength);

        RuleFor(x => x.QuestionType)
            .NotNull().WithMessage(ErrorMessageBase.Required)
            .Must(value => Enum.GetNames(typeof(QuestionForReadingType)).Contains(value));

		RuleFor(x => x)
	        .Must(x =>
		        !(x.StartTextIndex.HasValue && x.EndTextIndex.HasValue)
		        || x.StartTextIndex <= x.EndTextIndex
	        )
	        .WithMessage("StartTextIndex cannot be greater than EndTextIndex.");


		When(x => x.QuestionType == QuestionForReadingType.SingleChoice.ToString(), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);

            RuleFor(x => x.CorrectAnswer)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);

            RuleFor(x => x.Label)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);

            RuleForEach(x => x.SubQuestions)
                .Null();
        });

        When(x => x.QuestionType == QuestionForReadingType.MultipleChoice.ToString(), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);

            RuleForEach(x => x.SubQuestions)
                .SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
                .When(x => x.SubQuestions != null && x.SubQuestions.Any());

            //RuleFor(x => x.TableMetadata).Null();
        });

        // ================================
        // 🟦 2️⃣ True/False/NotGiven
        // ================================
        When(x => x.QuestionType == QuestionForReadingType.TrueFalse.ToString(), () =>
        {
            //RuleFor(x => x.SubQuestions).Null();
            //RuleFor(x => x.TableMetadata).Null();
        });

        // ================================
        // 🟨 3️⃣ Fill In Blank / Sentence Completion
        // ================================
        When(x => x.QuestionType == QuestionForReadingType.FillInBlank.ToString(), () =>
        {
            RuleFor(x => x.Options).Null();
            RuleForEach(x => x.SubQuestions)
                .SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            //RuleFor(x => x.TableMetadata).Null();
        });

        // ================================
        // 🟥 4️⃣ Matching
        // ================================
        When(x => x.QuestionType == QuestionForReadingType.Matching.ToString(), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleForEach(x => x.SubQuestions)
                .SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleFor(x => x.CorrectAnswer).Null();
        });

		When(x => x.QuestionType == QuestionForReadingType.SummaryCompletion.ToString(), () =>
		{
			RuleFor(x => x.Prompt)
				.NotEmpty().WithMessage(ErrorMessageBase.Required);
			RuleForEach(x => x.SubQuestions)
				.SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
				.NotEmpty().WithMessage(ErrorMessageBase.Required);
		});

		// ================================
		// 🟪 5️⃣ Map / Diagram Labeling
		// ================================
		When(x => x.QuestionType == QuestionForReadingType.MapLabeling.ToString(), () =>
        {
            RuleFor(x => x.Options)
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleForEach(x => x.SubQuestions)
                .SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
                .NotEmpty().WithMessage(ErrorMessageBase.Required);
            RuleFor(x => x.CorrectAnswer).Null();
        });

        //// ================================
        //// 🟫 6️⃣ TableCompletion
        //// ================================
        //When(x => x.QuestionType == QuestionForReadingType.TableCompletion.ToString(), () =>
        //{
        //    RuleForEach(x => x.SubQuestions)
        //        .SetValidator(new CreateSubQuestionForListeningIeltsModelValidator())
        //        .NotEmpty().WithMessage(ErrorMessageBase.Required);
        //    RuleFor(x => x.Options).Null();
        //    RuleFor(x => x.CorrectAnswer).Null();
        //});
    }
}
