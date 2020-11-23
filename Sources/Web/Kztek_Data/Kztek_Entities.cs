using System;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
//using MySql.Data.MySqlClient;

namespace Kztek_Data
{
    public class Kztek_Entities : DbContext
    {
        private IMongoClient _MongoClient;
        private IMongoDatabase _MongoDatabase;

        public Kztek_Entities(DbContextOptions<Kztek_Entities> options) : base(options)
        {

        }

        public Kztek_Entities()
        {
            var mongoUrl = new MongoUrl(AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:DefaultConnection").Result);

            _MongoClient = new MongoClient(mongoUrl.ToString().Replace(@"/" + mongoUrl.DatabaseName, ""));

            _MongoDatabase = _MongoClient.GetDatabase(mongoUrl.DatabaseName);
        }

        public DbSet<SY_User> SY_Users { get; set; }

        public DbSet<SY_Role> SY_Roles { get; set; }

        public DbSet<SY_MenuFunction> SY_MenuFunctions { get; set; }

        public DbSet<SY_Map_User_Role> SY_Map_User_Roles { get; set; }

        public DbSet<SY_Map_Role_Menu> SY_Map_Role_Menus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SY_User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Username).IsRequired();
            });

            modelBuilder.Entity<SY_Role>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<SY_MenuFunction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MenuName).IsRequired();
            });

            modelBuilder.Entity<SY_Map_User_Role>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<SY_Map_Role_Menu>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }

        public async Task<IMongoCollection<T>> GetCollection<T>()
        {
            var existed = await CollectionExistsAsync(typeof(T).Name);
            if (existed == false)
            {
                _MongoDatabase.CreateCollection(typeof(T).Name);
            }

            return await Task.FromResult(_MongoDatabase.GetCollection<T>(typeof(T).Name));
        }

        private async Task<bool> CollectionExistsAsync(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);

            //filter by collection name
            var collections = await _MongoDatabase.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });

            //check for existence
            return await collections.AnyAsync();
        }
    }
}
