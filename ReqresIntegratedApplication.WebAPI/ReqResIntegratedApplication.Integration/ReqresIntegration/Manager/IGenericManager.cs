using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;




namespace ReqResIntegratedApplication.Integration.ReqresIntegration.Manager
{
  public interface IGenericManager<T> where T : class
    {
        Task<T> Get(int id);
        Task<List<T>> GetAll();
        Task<T> Post();
    }
}
