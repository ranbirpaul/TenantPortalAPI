using CommonTypeModel;
using Repository.Abstract;
using Serilog;
using Service.Abstract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Concrete
{
    public class RackService : IRackService
    {
        private readonly IRackRepository _rep; 
        public RackService(IRackRepository rep)
        {
            _rep = rep;
        }

        public async Task AddRack(Rack model)
        {
            try
            {
                await _rep.AddRack(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Rack>> GetAllRacks()
        {
            try
            {
				Log.Logger.Information("Service GetAllRacks Method Started...");
				//throw new NullReferenceException("Racks not found!");
				return await _rep.GetAllRacks();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Rack> GetRackById(string Id)
        {
			try
			{
				return await _rep.GetRackById(Id);
			}
			catch (Exception)
			{
				throw;
			}
		}

        public async Task<Rack> GetRackByName(string name)
        {
			try
			{
				return await _rep.GetRackByName(name);
			}
			catch (Exception)
			{
				throw;
			}
		}

        public async Task<bool> RemoveAllRacks()
        {
			try
			{
				return await _rep.RemoveAllRacks();
			}
			catch (Exception)
			{
				throw;
			}

		}

        public async Task<bool> RemoveRackById(string Id)
        {
			try
			{
				return await _rep.RemoveRackById(Id);
			}
			catch (Exception)
			{
				throw;
			}
		}

        public async Task<bool> RemoveRackByName(string name)
        {
			try
			{
				return await _rep.RemoveRackByName(name);
			}
			catch (Exception)
			{
				throw;
			}
		}

        public async Task<bool> UpdateRack(Rack model)
        {
			try
			{
				return await _rep.UpdateRack(model);
			}
			catch (Exception)
			{
				throw;
			}
		}
    }
}