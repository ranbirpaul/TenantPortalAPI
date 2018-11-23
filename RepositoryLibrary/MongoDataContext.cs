using Model;
using MongoDB.Driver;
using Repository.EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace RepositoryLibrary
{
    public class MongoDataContext
    {
        // Declaring mongo db
        private readonly IMongoDatabase _database;
        public MongoDataContext(IOptions<Settings> dbsettings)
        {
            try
            {
                var MongoUsername = dbsettings.Value.UserName;
                var MongoPassword = dbsettings.Value.Password;
                var MongoDatabaseName = dbsettings.Value.Database;

                // Creating credentials  
                var credential = MongoCredential.CreateCredential(MongoDatabaseName, MongoUsername, MongoPassword);

                // Creating MongoClientSettings  
                var settings = new MongoClientSettings
                {
                    Credentials = new[] { credential },
                    Server = new MongoServerAddress(dbsettings.Value.Host, Convert.ToInt16(dbsettings.Value.Port))
                };

                var client = new MongoClient(settings);
                if (client != null)
                    _database = client.GetDatabase(MongoDatabaseName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Add collections
        public IMongoCollection<EntityRack> racks => _database.GetCollection<EntityRack>("Racks");
    }
}
