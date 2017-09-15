using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using TestFlask.Models.Entity;

namespace TestFlask.Data.Repos
{
    public interface IMongoRepo<T> where T : IMongoEntity
    {
    }

    public abstract class MongoRepo<T> : IMongoRepo<T> where T : IMongoEntity
    {
        protected IMongoDatabase db;
        protected string collectionName;

        private IMongoCollection<T> collection;

        //Made protected to restrict different use cases directly from business/controller classes 
        //it makes mocking, unit testing harder
        //(it's like opening an IQueryable interface to biz layer)
        protected IMongoCollection<T> Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = db.GetCollection<T>(collectionName);
                }

                return collection;
            }
        }

        public MongoRepo(IMongoDatabase mongoDb, string pCollectionName)
        {
            db = mongoDb;
            collectionName = pCollectionName;
        }
    }
}
