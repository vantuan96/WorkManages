using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kztek_Data.Infrastructure
{
    public interface IMongoRepository<T> where T : class
    {
        Task<T> GetOneById(string id);

        IEnumerable<T> Table { get; }

        Task<IEnumerable<T>> GetMany(BsonDocument query);

        Task<List<T>> GetManyToList(BsonDocument query);

        Task<List<T>> GetMany(BsonDocument query, string sortColumn, bool desc = true);

        Task<GridModel<T>> GetPaging(BsonDocument query, BsonDocument sortColumns, int pageIndex, int pageSize);
        Task<GridModel<T>> GetPagings(BsonDocument query,  int pageIndex, int pageSize);

        Task<MessageReport> Add(T model);

        Task<MessageReport> Update(BsonDocument query, T model);

        Task<MessageReport> Remove(BsonDocument query);

        Task<MessageReport> Remove_Multi(BsonDocument query);

        Task<IMongoCollection<T>> GetCollection();
    }
}
