using CommonTypeModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Abstract
{
    public interface IRackRepository
    {
        Task<IEnumerable<Rack>> GetAllRacks();
        Task<Rack> GetRackByName(string name);
        Task<Rack> GetRackById(string Id);
        Task AddRack(Rack model);
        Task<bool> UpdateRack(Rack model);
        Task<bool> RemoveRackByName(string name);
        Task<bool> RemoveRackById(string Id);
        Task<bool> RemoveAllRacks();
    }
}
