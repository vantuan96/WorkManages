using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models;
using Kztek_Service.Admin.Interfaces;

namespace Kztek_Service.Admin.Implementations.MONGO
{
    public class SY_RoleService : ISY_RoleService
    {
        private ISY_RoleRepository _SY_RoleRepository;
        private ISY_Map_Role_MenuRepository _SY_Map_Role_MenuRepository;
        private ISY_Map_User_RoleRepository _SY_Map_User_RoleRepository;

        public SY_RoleService(ISY_RoleRepository _SY_RoleRepository, ISY_Map_Role_MenuRepository _SY_Map_Role_MenuRepository, ISY_Map_User_RoleRepository _SY_Map_User_RoleRepository)
        {
            this._SY_RoleRepository = _SY_RoleRepository;
            this._SY_Map_Role_MenuRepository = _SY_Map_Role_MenuRepository;
            this._SY_Map_User_RoleRepository = _SY_Map_User_RoleRepository;
        }

        public async Task<List<SY_Role>> GetAll()
        {
            return await Task.FromResult(_SY_RoleRepository.Table.ToList());
        }

        public async Task<List<SY_Role>> GetAllActiveOrder()
        {
            //query
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'Active': true");
            query.AppendLine("}");

            return await _SY_RoleRepository.GetMany(MongoHelper.ConvertQueryStringToDocument(query.ToString()), "RoleName", false);
        }

        public async Task<SY_Role> GetById(string id)
        {
            return await _SY_RoleRepository.GetOneById(id);
        }

        public async Task<SY_Role_Submit> GetCustomById(string id)
        {
            var model = new SY_Role_Submit();

            var obj = await _SY_RoleRepository.GetOneById(id);
            if (obj != null)
            {
                model = await GetCustomByModel(obj);
            }

            return model;
        }

        public async Task<SY_Role_Submit> GetCustomByModel(SY_Role model)
        {
            var obj = new SY_Role_Submit()
            {
                Id = model.Id.ToString(),
                Active = model.Active,
                Description = model.Description,
                RoleName = model.RoleName,
                MenuFunctions = new List<string>()
            };

            obj.MenuFunctions = (from n in _SY_Map_Role_MenuRepository.Table
                                 where n.RoleId == model.Id
                                 select n.MenuId).ToList();

            return await Task.FromResult(obj);
        }

        public async Task<MessageReport> Create(SY_Role model)
        {
            return await _SY_RoleRepository.Add(model);
        }

        public async Task<MessageReport> Update(SY_Role model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _SY_RoleRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }

        public async Task<MessageReport> Delete(string id)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var obj = await GetById(id);
            if (obj != null)
            {
                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'_id': { '$eq': '" + id + "' }");
                query.AppendLine("}");

                return await _SY_RoleRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<MessageReport> CreateMap(SY_Map_User_Role model)
        {
            return await _SY_Map_User_RoleRepository.Add(model);
        }

        public async Task<MessageReport> DeleteMap(string userid)
        {
            //query
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine("}");

            return await _SY_Map_User_RoleRepository.Remove_Multi(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }
    }
}