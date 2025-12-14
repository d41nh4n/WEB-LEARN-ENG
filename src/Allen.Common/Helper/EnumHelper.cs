using System.ComponentModel.DataAnnotations;

namespace Allen.Common;

public class EnumHelper
{
    public static IEnumerable<object> GetEnumList<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>().Select(e => new
        {
            //intValue = Convert.ToInt32(e),
            value = e.ToString()
        });
    }
    public static string GetDisplayName(Enum enumValue)
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())[0]
            .GetCustomAttribute<DisplayAttribute>()?.Name ?? enumValue.ToString();
    }
}
