using System.Threading.Tasks;

namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Manager
{
    public interface IGenericManager<T> where T : class
    {
        Task<T?> Get(int id);
        Task<T?> GetAll(int page = 1, int perPage = 6);
        Task<TResult?> Post<TRequest, TResult>(TRequest requestBody)
            where TRequest : class
            where TResult : class;
    }
}
