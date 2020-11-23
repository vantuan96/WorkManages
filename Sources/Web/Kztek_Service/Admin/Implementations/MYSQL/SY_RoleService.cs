using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository;
using Kztek_Model.Models;
using Kztek_Service.Admin.Interfaces;

namespace Kztek_Service.Admin.Implementations.MYSQL
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
            var data = _SY_RoleRepository.Table;
            return await Task.FromResult(data.ToList());
        }

        public async Task<List<SY_Role>> GetAllActiveOrder()
        {
            var data = from n in _SY_RoleRepository.Table
                       where n.Active == true
                       orderby n.RoleName
                       select n;

            return await Task.FromResult(data.ToList());
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
            return await _SY_RoleRepository.Update(model);
        }

        public async Task<MessageReport> Delete(string id)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var obj = GetById(id);
            if (obj.Result != null)
            {
                return await _SY_RoleRepository.Remove(obj.Result);
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
            var t = from n in _SY_Map_User_RoleRepository.Table
                    where n.UserId == userid
                    select n;

            return await _SY_Map_User_RoleRepository.Remove_Multi(t);
        }
    }
}