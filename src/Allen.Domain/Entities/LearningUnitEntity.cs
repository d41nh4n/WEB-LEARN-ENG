namespace Allen.Domain;

/// <summary>
/// Đại diện cho một "Unit học" trong hệ thống.
/// Một Unit là một nhóm nội dung học lớn, gồm nhiều Step (bài học nhỏ) 
/// mà người học phải hoàn thành theo thứ tự.
///
/// Ví dụ:
/// - Unit: "Basic Grammar 1" (Grammar skill)
/// - Unit: "Daily Conversations" (Listening skill)
///
/// Chức năng chính:
/// 1. Chứa danh sách các Step (UnitSteps) để học tuần tự.
/// 2. Lưu thông tin mô tả về Unit như tên, độ khó, kỹ năng.
/// 3. Liên kết với tiến trình học của từng người dùng (UserUnitProgresses).
/// 4. Có thể mở rộng cho nhiều kỹ năng (Listening, Reading, Writing, Speaking) 
///    thông qua thuộc tính SkillType.
/// </summary>
[Table("LearningUnits")]
public class LearningUnitEntity : EntityBase<Guid>
{
	public Guid? CategoryId { get; set; }
	public CategoryEntity? Category { get; set; }

	public string Title { get; set; } = null!;
	public string? Description { get; set; }
	public LevelType Level { get; set; }
	public SkillType? SkillType { get; set; }
	public LearningUnitType? LearningUnitType { get; set; }
	public LearningUnitStatusType LearningUnitStatusType { get; set; } = LearningUnitStatusType.Draft;

	public ICollection<ReadingPassageEntity> ReadingPassages { get; set; } = [];
	public ICollection<SpeakingEntity> Speakings { get; set; } = [];
    public ICollection<WritingEntity> Writings { get; set; } = [];
    public ICollection<ListeningEntity> Listenings { get; set; } = [];
    //public ICollection<UnitContentEntity> UnitContents { get; set; } = [];
	//public ICollection<UserUnitProgressEntity> UserUnitProgresses { get; set; } = [];
}