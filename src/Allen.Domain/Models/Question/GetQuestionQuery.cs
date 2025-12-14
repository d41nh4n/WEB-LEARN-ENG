namespace Allen.Domain;

public class GetQuestionQuery
{
    public Guid ModuleItemId { get; set; }

    public LearningModuleType ModuleType { get; set; }
}
