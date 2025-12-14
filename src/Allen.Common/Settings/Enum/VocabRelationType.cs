namespace Allen.Common;

public enum VocabRelationType
{
    Synonym = 1,        // Đồng nghĩa
    Antonym = 2,        // Trái nghĩa
    Hypernym = 3,       // Từ bao quát (ví dụ: "animal" là hypernym của "dog")
    Hyponym = 4,        // Từ cụ thể hơn (ví dụ: "dog" là hyponym của "animal")
    Related = 5,        // Có liên quan ngữ nghĩa
}
