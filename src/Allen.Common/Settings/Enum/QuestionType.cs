namespace Allen.Common;

public enum QuestionType
{
    SingleChoice = 0,
    /// Câu hỏi trắc nghiệm nhiều lựa chọn (Multiple Choice).
    MultipleChoice = 1,

    /// Điền vào chỗ trống (Fill in the Blank).
    FillInBlank = 2,

    /// Đúng hoặc Sai (True/False).
    TrueFalse = 3,

    /// Ghép cặp (Matching).
    Matching = 4,

    /// Nối từ hoặc sắp xếp câu (Ordering).
    Ordering = 5,

    /// Câu hỏi yêu cầu trả lời tự do (Free Text / Short Answer).
    ShortAnswer = 6,

    /// Câu hỏi nghe rồi chọn đáp án (Listening Comprehension).
    Listening = 7,

    /// Câu hỏi nói (Speaking Practice).
    Speaking = 8,

    SentenceCompletion = 9,

    SummaryCompletion = 10,

    TableCompletion = 11,

    MapLabeling = 12,

	ParagraphCompletion = 13
}