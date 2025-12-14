using Meilisearch;

namespace Allen.Application;

[RegisterService(typeof(IMeiliSearchService<>))]
public class MeiliSearchService<T> : IMeiliSearchService<T>
{
    private readonly IMeiliSearchRepository<T> _repository;

    public MeiliSearchService(IMeiliSearchRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task IndexAsync(T entity) => await _repository.IndexAsync(entity);

    public async Task<List<VocabularyMLSModel>> SearchAsync(string word)
    {
        // Simple validation - chỉ trả về "Invalid input"
        if (!IsValidEnglishSearch(word))
        {
            throw new BadRequestException(ErrorMessageBase.Format(ErrorMessageBase.Invalid, nameof(word)));
        }

        // Clean input
        var cleanedWord = CleanEnglishSearchTerm(word);

        // Execute search
        var result = await _repository.SearchAsync(cleanedWord!);

       return result.ToList() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, word));
    }

    private bool IsValidEnglishSearch(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var trimmed = input.Trim();

        // Độ dài cơ bản
        if (trimmed.Length < 2 || trimmed.Length > 25)
            return false;

        // Chỉ cho phép: chữ cái, dấu cách, dấu gạch ngang, apostrophe
        foreach (char c in trimmed)
        {
            if (!char.IsLetter(c) && c != ' ' && c != '-' && c != '\'')
                return false;
        }

        // Không được toàn khoảng trắng
        if (trimmed.All(char.IsWhiteSpace))
            return false;

        // Không được bắt đầu/kết thúc bằng dấu đặc biệt
        if (trimmed.StartsWith("-") || trimmed.EndsWith("-") ||
            trimmed.StartsWith("'") || trimmed.EndsWith("'"))
            return false;

        // Không có nhiều dấu gạch ngang liên tiếp
        if (trimmed.Contains("--"))
            return false;

        return true;
    }

    private string? CleanEnglishSearchTerm(string input)
    {
        return input?.Trim()
                    .ToLowerInvariant()
                    .Replace("  ", " ")
                    .Replace(" - ", "-")
                    .Replace("- ", "-")
                    .Replace(" -", "-");
    }

    public Task DeleteAsync(Guid id) => _repository.DeleteAsync(id);

    public async Task<TaskResource> AddVocabularyAsync(VocabularyMLSModel vocabularyMLSModel)
    {
        try
        {
            //  Gọi repository để thêm document vào Meilisearch
            var taskResource = await _repository.AddVocabularyAsync(vocabularyMLSModel);

            //  Lấy client để chờ task hoàn tất
            var client = _repository.GetCurrentClientAsync();

            //  Chờ Meilisearch xử lý xong (đảm bảo đã index)
            var completedTask = await client.WaitForTaskAsync(taskResource.Uid);

            //  Kiểm tra trạng thái sau khi hoàn tất
            if (completedTask.Status.Equals(TaskInfoStatus.Failed))
            {
                throw new InternalServerException(ErrorMessageBase.CreateFailure, ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(VocabularyMLSModel), vocabularyMLSModel.Word!));
            }

            // 5️⃣ Nếu thành công → trả về thông tin task
            return completedTask;
        }
        catch (MeilisearchApiError ex)
        {
            // Lỗi từ Meilisearch API (ví dụ schema sai, index chưa tồn tại)
            throw new Exception($"Lỗi từ Meilisearch API: {ex.Message}", ex);
        }
        catch (HttpRequestException ex)
        {
            // Không thể kết nối đến Meilisearch server
            throw new Exception($"Không thể kết nối tới Meilisearch server: {ex.Message}", ex);
        }
        catch (TimeoutException ex)
        {
            // Server mất quá nhiều thời gian để xử lý
            throw new Exception("Meilisearch phản hồi quá lâu.", ex);
        }
        catch (Exception ex)
        {
            // Lỗi không xác định khác
            throw new Exception($"Lỗi không xác định khi thêm từ vựng: {ex.Message}", ex);
        }
    }

    public async Task<TaskResource> UpdateVocabularyAsync(VocabularyMLSModel vocabularyMLSModel)
    {
        try
        {
            // Gọi repository để cập nhật document trong Meilisearch
            var taskResource = await _repository.UpdateVocabularyAsync(vocabularyMLSModel);

            // Lấy client để chờ task hoàn tất
            var client = _repository.GetCurrentClientAsync();

            // Chờ Meilisearch xử lý xong (đảm bảo đã index)
            var completedTask = await client.WaitForTaskAsync(taskResource.Uid);

            // Kiểm tra trạng thái sau khi hoàn tất
            if (completedTask.Status.Equals(TaskInfoStatus.Failed))
            {
                throw new InternalServerException(ErrorMessageBase.UpdateFailure, ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(VocabularyMLSModel), vocabularyMLSModel.Word!));
            }

            // Nếu thành công → trả về thông tin task
            return completedTask;
        }
        catch (MeilisearchApiError ex)
        {
            // Lỗi từ Meilisearch API (ví dụ schema sai, index chưa tồn tại)
            throw new Exception($"Lỗi từ Meilisearch API khi cập nhật: {ex.Message}", ex);
        }
        catch (HttpRequestException ex)
        {
            // Không thể kết nối đến Meilisearch server
            throw new Exception($"Không thể kết nối tới Meilisearch server khi cập nhật: {ex.Message}", ex);
        }
        catch (TimeoutException ex)
        {
            // Server mất quá nhiều thời gian để xử lý
            throw new Exception("Meilisearch phản hồi quá lâu khi cập nhật.", ex);
        }
        catch (Exception ex)
        {
            // Lỗi không xác định khác
            throw new Exception($"Lỗi không xác định khi cập nhật từ vựng: {ex.Message}", ex);
        }
    }
}
