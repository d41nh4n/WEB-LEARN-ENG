using System.Text.Json;

namespace Allen.Domain;
[Table("FlashCards")]
public class FlashCardEntity : EntityBase<Guid>
{
    public Guid DeckId { get; set; }
    public DeckEntity Deck { get; set; } = null!;

    // JSON content---------------------------------------------
    public string FrontContent { get; set; } = "[]";
    public string BackContent { get; set; } = "[]";

    // FSRS fields
    public FlashCardStateEntity CardState { get; set; } = null!;

    // Optional fields-------------------------------------------
    public string? Hint { get; set; }
    public string? PersonalNotes { get; set; }

    // States
    // true = active, false = suspended-------------------------
    public bool IsSuspended { get; set; } = false;
    public DateTime? LastClonedDate { get; set; }
    //Liên kết với vocabulary
    public Guid RelationVocabularyCardId { get; set; } = Guid.Empty;

    public static FlashCardEntity CloneFrom(FlashCardEntity source, Guid targetDeckId)
    {
        return new FlashCardEntity
        {
            Id = Guid.NewGuid(),
            DeckId = targetDeckId,

            // Copy nội dung
            FrontContent = source.FrontContent,
            BackContent = source.BackContent,
            Hint = source.Hint,

            // Reset trạng thái cá nhân
            PersonalNotes = null, // Notes cá nhân không nên clone
            IsSuspended = false,

            // Audit info
            LastClonedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,

            RelationVocabularyCardId = source.RelationVocabularyCardId
        };
    }

    public static FlashCardEntity CreateFromVocabulary(VocabularyEntity vocabulary, Guid targetDeckId)
    {
        var meanings = vocabulary.VocabularyMeanings.Take(3).ToList();

        var frontContents = new List<FlashCardContentsModel>();

        // A. Text: Từ vựng + Phát âm
        var firstMeaning = meanings.FirstOrDefault();
        string frontText = vocabulary.Word;
        if (firstMeaning != null && !string.IsNullOrEmpty(firstMeaning.Pronunciation))
        {
            frontText += $" [{firstMeaning.Pronunciation}]";
        }

        frontContents.Add(new FlashCardContentsModel
        {
            Type = "Text",
            Text = frontText
        });

        // B. Audio: Chỉ lấy audio link của nghĩa đầu tiên (nếu có)
        if (firstMeaning != null && !string.IsNullOrEmpty(firstMeaning.Audio))
        {
            frontContents.Add(new FlashCardContentsModel
            {
                Type = "Audio",
                Text = firstMeaning.Audio
            });
        }

        // Gom tất cả định nghĩa Tiếng Việt thành một chuỗi duy nhất, phân tách bằng dấu phẩy (hoặc chấm phẩy)
        var vnDefinitions = meanings
            .Where(m => !string.IsNullOrEmpty(m.DefinitionVN))
            .Select(m =>
            {
                // Thêm từ loại vào đầu nghĩa để phân biệt rõ ràng hơn
                string partOfSpeech = m.PartOfSpeech.HasValue ? $"({m.PartOfSpeech.ToString()}) " : "";
                return $"{partOfSpeech}{m.DefinitionVN}";
            })
            .ToList();

        string backText = string.Join("; ", vnDefinitions); // Dùng dấu chấm phẩy (;) để phân tách rõ ràng hơn dấu phẩy (,)

        var backContents = new List<FlashCardContentsModel>
    {
        new() {
            Type = "Text",
            Text = backText
        }
    };
        return new FlashCardEntity
        {
            Id = Guid.NewGuid(),
            DeckId = targetDeckId,
            // Chuyển đổi sang JSON string
            FrontContent = JsonSerializer.Serialize(frontContents),
            BackContent = JsonSerializer.Serialize(backContents),
            // Reset trạng thái cá nhân
            PersonalNotes = null, // Notes cá nhân không nên clone
            IsSuspended = false,
            // Audit info
            LastClonedDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            RelationVocabularyCardId = vocabulary.Id
        };
    }
}