namespace Allen.Application;

public interface IServiceBase<T> where T :  class
{
	Task<List<T>> GetUsersWithPagingAsync();
}
