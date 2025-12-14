namespace Allen.Domain;

[Table("ReadingParagraphs")]
public class ReadingParagraphEntity : EntityBase<Guid>
{
	public Guid PassageId { get; set; }
	public ReadingPassageEntity Passage { get; set; } = null!;

	public int Order { get; set; }                 // Thứ tự đoạn
	public string OriginalText { get; set; } = null!;
	public string Transcript { get; set; } = null!; // Bản dịch/diễn giải
}
