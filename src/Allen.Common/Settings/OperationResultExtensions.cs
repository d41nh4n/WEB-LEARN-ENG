namespace Allen.Common.Settings;
public static class OperationResultExtensions
{
    // Lấy data với type safety, nếu sai type thì trả default (null hoặc 0)
    public static T? GetData<T>(this OperationResult result)
    {
        if (result.Data is T value)
            return value;

        return default;
    }

    // Lấy data bắt buộc, sai type thì quăng exception rõ ràng
    public static T GetRequiredData<T>(this OperationResult result)
    {
        if (result.Data is T value)
            return value;

        throw new InvalidCastException(
            $"OperationResult.Data is not of expected type {typeof(T).Name}");
    }
}
