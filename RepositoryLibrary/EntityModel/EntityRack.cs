using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.EntityModel
{
    public class EntityRack
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
