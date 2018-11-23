using CommonTypeModel;
using Microsoft.Extensions.Options;
using Model;
using MongoDB.Driver;
using MongoDB.Bson;
using Repository.Abstract;
using Repository.EntityModel;
using RepositoryLibrary;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Concrete
{
    public class RackRepository : IRackRepository
    {
        // Parameterized constructor
        private readonly MongoDataContext _dbcontext;
        public RackRepository(IOptions<Settings> settings)
        {
            _dbcontext = new MongoDataContext(settings);
        }

        public async Task AddRack(Rack model)
        {
            try
            {
                EntityRack entityrack = new EntityRack { Name = model.Name };
                await _dbcontext.racks.InsertOneAsync(entityrack);
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
				Log.Logger.Information("Repository GetAllRacks Method Started...");
				List<Rack> racklist = new List<Rack>();
                List<EntityRack> entityracklist = await _dbcontext.racks.Find(x => true).ToListAsync();
                // Convert EntityRack to Rack model
                foreach (EntityRack rack in entityracklist)
                {
                    Rack item = new Rack { Id = rack.Id.ToString(), Name = rack.Name };
                    racklist.Add(item);
                }

                return racklist;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<Rack> GetRackById(string Id)
        {
			try
			{
				Rack rack = null;
				var filter = Builders<EntityRack>.Filter.Eq("_id", ObjectId.Parse(Id));
				EntityRack erack = await _dbcontext.racks.Find(filter).FirstOrDefaultAsync();
				if (erack != null)
				rack = new Rack { Id = erack.Id.ToString(), Name = erack.Name };
				return rack;
			}
	        catch(Exception)
            {
                throw;
            }
		}

		public async Task<Rack> GetRackByName(string name)
        {
			try
			{
				Rack rack = null;
				var filter = Builders<EntityRack>.Filter.Eq("Name", name);
				EntityRack erack = await _dbcontext.racks.Find(filter).FirstOrDefaultAsync();
				if (erack != null)
					rack = new Rack { Id = erack.Id.ToString(), Name = erack.Name };

				return rack;
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
				var deleteresult = await _dbcontext.racks.DeleteManyAsync(new BsonDocument());
				return deleteresult.IsAcknowledged;
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
				var filter = Builders<EntityRack>.Filter.Eq("_id", ObjectId.Parse(Id));
			    var deleteresult = await _dbcontext.racks.DeleteOneAsync(filter);
				return deleteresult.IsAcknowledged;
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
				var filter = Builders<EntityRack>.Filter.Eq("Name", name);
				var deleteresult = await _dbcontext.racks.DeleteOneAsync(filter);
				return deleteresult.IsAcknowledged;
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
				var filter = Builders<EntityRack>.Filter.Eq("_id", ObjectId.Parse(model.Id));
				var rack = _dbcontext.racks.Find(filter).FirstOrDefaultAsync();
				if (rack.Result == null)
					return false;

				var update = Builders<EntityRack>.Update
											  .Set(x => x.Name, model.Name);
	
				var updateresult = await _dbcontext.racks.UpdateOneAsync(filter, update);
				return updateresult.IsAcknowledged;
			}
			catch (Exception)
			{
				throw;
			}
		}
    }
}