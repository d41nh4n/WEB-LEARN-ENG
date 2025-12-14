using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Allen.Infrastructure;

[RegisterService(typeof(IVocabularyRepository))]
public class VocabularyRepository(
    SqlApplicationDbContext context
) : RepositoryBase<VocabularyEntity>(context), IVocabularyRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<QueryResult<VocabulariesModel>> GetVocabulariesAsync(QueryInfo queryInfo)
    {
        string searchText = queryInfo.SearchText?.ToLower() ?? "";
        var query = (from vocabulary in _context
                                 .Vocabularies.AsNoTracking()
                                 .Where(v => v.Word.ToLower().Contains(searchText))
                     select new VocabulariesModel
                     {
                         Id = vocabulary.Id,
                         Level = vocabulary.Level.ToString(),
                         Word = vocabulary.Word
                     });

        var entities = await query
            .OrderBy(x => x.Word)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<VocabulariesModel>
        {
            Data = entities,
            TotalCount = queryInfo.NeedTotalCount
                ? await query.CountAsync()
                : 0
        };
    }

    public async Task<VocabularyModel> GetVocabularyByIdAsync(Guid vocabId)
    {
        var vocabulary = await _context.Vocabularies
        .AsNoTracking()
        .Include(v => v.Topic)
        .Include(v => v.VocabularyMeanings)
        .Include(v => v.VocabularyTags)
            .ThenInclude(vt => vt.Tag)
        .FirstOrDefaultAsync(v => v.Id == vocabId);

        if (vocabulary == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity), vocabId));

        return new VocabularyModel
        {
            Id = vocabulary.Id,
            Topic = vocabulary.Topic != null ? new VocabularyTopicModel
            {
                Id = vocabulary.Topic.Id,
                TopicName = vocabulary.Topic.TopicName
            } : null,
            Word = vocabulary.Word,
            Level = vocabulary.Level.ToString(),
            VocabularyMeanings = vocabulary.VocabularyMeanings?.Select(meaning => new VocabularyMeaningModel
            {
                Id = meaning.Id,
                PartOfSpeech = meaning.PartOfSpeech.ToString(),
                Pronunciation = meaning.Pronunciation,
                Audio = meaning.Audio,
                DefinitionEN = meaning.DefinitionEN,
                DefinitionVN = meaning.DefinitionVN,
                Example1 = meaning.Example1,
                Example2 = meaning.Example2,
                Example3 = meaning.Example3
            }).ToList() ?? new List<VocabularyMeaningModel>(),
            Tags = vocabulary.VocabularyTags?.Select(vt => new TagModel
            {
                Id = vt.Tag?.Id ?? Guid.Empty,
                NameTag = vt.Tag?.NameTag ?? ""
            }).ToList() ?? new List<TagModel>()
        };
    }

    public async Task<VocabularyModel> GetVocabularyByWordAsync(string word)
    {
        var vocabulary = await _context.Vocabularies
         .AsNoTracking()
         .Include(v => v.Topic)
         .Include(v => v.VocabularyMeanings)
         .Include(v => v.VocabularyTags)
             .ThenInclude(vt => vt.Tag)
         .FirstOrDefaultAsync(v => v.Word == word);

        if (vocabulary == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity), word));

        return new VocabularyModel
        {
            Id = vocabulary.Id,
            Topic = vocabulary.Topic != null ? new VocabularyTopicModel
            {
                Id = vocabulary.Topic.Id,
                TopicName = vocabulary.Topic.TopicName
            } : null,
            Word = vocabulary.Word,
            Level = vocabulary.Level.ToString(),
            VocabularyMeanings = vocabulary.VocabularyMeanings?.Select(meaning => new VocabularyMeaningModel
            {
                Id = meaning.Id,
                PartOfSpeech = meaning.PartOfSpeech.ToString(),
                Pronunciation = meaning.Pronunciation,
                Audio = meaning.Audio,
                DefinitionEN = meaning.DefinitionEN,
                DefinitionVN = meaning.DefinitionVN,
                Example1 = meaning.Example1,
                Example2 = meaning.Example2,
                Example3 = meaning.Example3
            }).ToList() ?? new List<VocabularyMeaningModel>(),
            Tags = vocabulary.VocabularyTags?.Select(vt => new TagModel
            {
                Id = vt.Tag?.Id ?? Guid.Empty,
                NameTag = vt.Tag?.NameTag ?? ""
            }).ToList() ?? new List<TagModel>()
        };
    }
    ///======================== Advanced Functions ========================///

    public async Task<QuizVocabulariesResponeModel> GetQuizVocabulariesAsync(QuizVocabulariesRequestModel model)
    {
        var allSelectedIds = new List<Guid>();

        var levelWordCounts = new Dictionary<LevelType, int>
        {
            { LevelType.A1, model.NumberA1Words },
            { LevelType.A2, model.NumberA2Words },
            { LevelType.B1, model.NumberB1Words },
            { LevelType.B2, model.NumberB2Words },
            { LevelType.C1, model.NumberC1Words },
            { LevelType.C2, model.NumberC2Words }
        };

        // 1. Lặp qua từng cấp độ và lấy ID ngẫu nhiên (TUẦN TỰ)
        foreach (var (level, count) in levelWordCounts)
        {
            if (count > 0)
            {
                // Gọi hàm lấy ID ngẫu nhiên. Vì không dùng Task.WhenAll, 
                // các truy vấn này sẽ chạy tuần tự và an toàn trên DbContext Scoped.
                var randomIds = await GetRandomVocabularyIdsByLevelAsync(model.Topic, level, count);
                allSelectedIds.AddRange(randomIds);
            }
        }

        if (allSelectedIds.Count == 0)
        {
            return new QuizVocabulariesResponeModel { Vocabularies = new List<VocabularyModel>() };
        }

        // 2. Truy vấn tất cả Entity bằng các ID đã chọn (MỘT TRUY VẤN LỚN)
        var allVocabularies = await _context.Vocabularies
            .AsNoTracking()
            .Where(v => allSelectedIds.Contains(v.Id))
            .Include(v => v.Topic)
            .Include(v => v.VocabularyMeanings)
            .ToListAsync();

        // 3. Ánh xạ (Map) từ Entity sang Model và trả về
        var vocabularyModels = allVocabularies
            .Select(vocabulary => new VocabularyModel
            {
                Id = vocabulary.Id,
                Topic = vocabulary.Topic != null ? new VocabularyTopicModel
                {
                    Id = vocabulary.Topic.Id,
                    TopicName = vocabulary.Topic.TopicName
                } : null,
                Word = vocabulary.Word,
                Level = vocabulary.Level.ToString(),
                VocabularyMeanings = vocabulary.VocabularyMeanings?.Select(meaning => new VocabularyMeaningModel
                {
                    Id = meaning.Id,
                    PartOfSpeech = meaning.PartOfSpeech.ToString(),
                    Pronunciation = meaning.Pronunciation,
                    Audio = meaning.Audio,
                    DefinitionEN = meaning.DefinitionEN,
                    DefinitionVN = meaning.DefinitionVN,
                    Example1 = meaning.Example1,
                    Example2 = meaning.Example2,
                    Example3 = meaning.Example3
                }).ToList() ?? [],
                Tags = vocabulary.VocabularyTags?.Select(vt => new TagModel
                {
                    Id = vt.Tag?.Id ?? Guid.Empty,
                    NameTag = vt.Tag?.NameTag ?? ""
                }).ToList() ?? []
            }).ToList(); ;

        return new QuizVocabulariesResponeModel
        {
            Vocabularies = vocabularyModels
        };
    }

    /// <summary>
    /// Lấy ngẫu nhiên các ID từ vựng cho một cấp độ cụ thể.
    /// </summary>
    private async Task<List<Guid>> GetRandomVocabularyIdsByLevelAsync(Guid? topicId, LevelType level, int count)
    {
        var query = _context.Vocabularies
            .AsNoTracking()
            .Where(v => v.Level == level);

        if (topicId.HasValue)
        {
            query = query.Where(v => v.TopicId == topicId.Value);
        }

        // Lấy ngẫu nhiên ID
        var randomIds = await query
            .OrderBy(r => Guid.NewGuid()) // Sắp xếp ngẫu nhiên
            .Take(count)
            .Select(v => v.Id)            // Chỉ chiếu (Project) ID
            .ToListAsync();

        return randomIds;
    }

    public async Task<List<VocabularyEntity>> GetVocabulariesByIdsAsync(List<Guid> vocabIds)
    {
        return await _context.Vocabularies
            .AsNoTracking()
            .Include(v => v.VocabularyMeanings)
            .Where(v => vocabIds.Contains(v.Id))
            .ToListAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity)));
    }

    public async Task<QueryResult<VocabularyModel>> GetVocabulariesByTopicIdAsync(QueryInfo queryInfo, Guid topic)
    {
        // Bổ sung các using cần thiết (Giả định)
        // using Microsoft.EntityFrameworkCore;
        // using System.Linq;
        // using System.Collections.Generic;

        // --- BƯỚC 1: LỌC TRƯỚC (FIX LỖI LOGIC: KHÔNG DÙNG THAM SỐ topic) ---
        var vocabularyQuery = _context.Vocabularies
            .AsNoTracking()
            .Where(v => v.TopicId == topic); // <-- Lọc theo Topic ID

        if (!string.IsNullOrWhiteSpace(queryInfo.SearchText))
        {
            // Thực hiện tìm kiếm case-insensitive bằng cách chuyển đổi cả hai vế sang chữ thường.
            var searchText = queryInfo.SearchText.ToLower();
            vocabularyQuery = vocabularyQuery.Where(v => v.Word.ToLower().Contains(searchText));
        }

        // --- BƯỚC 2: PROJECT DỮ LIỆU ---
        var query = vocabularyQuery
            .Select(vocabulary => new VocabularyModel
            {
                Id = vocabulary.Id,
                // Topic: Projection này ổn nếu Topic không quá lớn
                Topic = vocabulary.Topic != null ? new VocabularyTopicModel
                {
                    Id = vocabulary.Topic.Id,
                    TopicName = vocabulary.Topic.TopicName
                } : null,
                Word = vocabulary.Word,
                // Lưu ý: .ToString() trên enum/int có thể cần được thực hiện client-side hoặc được EF Core hỗ trợ
                Level = vocabulary.Level.ToString(),

                // Vocabulary Meanings: Sử dụng LINQ to Objects (.Select().ToList()) bên trong Select
                // sẽ buộc EF Core phải dịch toàn bộ tập hợp này (hoặc client-side eval), 
                // nhưng trong projection complex này thường được chấp nhận.
                VocabularyMeanings = vocabulary.VocabularyMeanings
                    .Select(meaning => new VocabularyMeaningModel
                    {
                        Id = meaning.Id,
                        PartOfSpeech = meaning.PartOfSpeech.ToString(),
                        Pronunciation = meaning.Pronunciation,
                        Audio = meaning.Audio,
                        DefinitionEN = meaning.DefinitionEN,
                        DefinitionVN = meaning.DefinitionVN,
                        Example1 = meaning.Example1,
                        Example2 = meaning.Example2,
                        Example3 = meaning.Example3
                    }).ToList() ?? new List<VocabularyMeaningModel>(),

                // Tags
                Tags = vocabulary.VocabularyTags
                    .Select(vt => new TagModel
                    {
                        Id = vt.Tag!.Id, // Giả định vt.Tag không null sau khi đã lọc (hoặc dùng !/?? như dưới)
                        NameTag = vt.Tag.NameTag
                    }).ToList() ?? new List<TagModel>()
            });

        // --- BƯỚC 3: PHÂN TRANG VÀ SẮP XẾP (FIX LỖI RUNTIME: .OrderByDescending(x => queryInfo.OrderBy)) ---

        // **LỖI ĐÃ SỬA:** Dòng này được thay bằng sắp xếp cố định (ví dụ: theo Id) 
        // vì cú pháp OrderBy/OrderByDescending cũ không hợp lệ cho sắp xếp động.
        var entities = await query
            .OrderByDescending(x => x.Word) // <-- Đã thay thế sắp xếp động bị lỗi
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity)));

        // --- BƯỚC 4: TRẢ VỀ KẾT QUẢ ---
        return new QueryResult<VocabularyModel>
        {
            Data = entities,
            TotalCount = queryInfo.NeedTotalCount
                ? await vocabularyQuery.CountAsync() // Đếm trên query gốc (vocabularyQuery) để hiệu quả hơn
                : 0
        };
    }
}

