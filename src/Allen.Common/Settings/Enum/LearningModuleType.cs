namespace Allen.Common;

/// <summary>
/// Loại module học mà một câu hỏi hoặc nội dung có thể thuộc về.
/// Dùng để tái sử dụng Question & Answer cho nhiều module khác nhau.
/// </summary>
public enum LearningModuleType
{
    /// <summary>
    /// Bài học dạng UnitStep (kiểu Duolingo).
    /// </summary>
    UnitStep = 1,

    /// <summary>
    /// Bài nghe thuộc kỹ năng Listening (Listening Lesson).
    /// </summary>
    ListeningLesson = 2,

    /// <summary>
    /// Đoạn văn/bài đọc thuộc kỹ năng Reading.
    /// </summary>
    ReadingPassage = 3,

    /// <summary>
    /// Nhiệm vụ viết (Writing Task).
    /// </summary>
    WritingTask = 4,

    /// <summary>
    /// Bài luyện nói (Speaking Practice).
    /// </summary>
    SpeakingPractice = 5
}
