using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Service.Admin.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Kztek_Service.Admin.Implementations.SQLSERVER
{
    public class SY_MenuFunctionService : ISY_MenuFunctionService
    {
        private ISY_MenuFunctionRepository _SY_MenuFunctionRepository;
        private ISY_Map_Role_MenuRepository _SY_Map_Role_MenuRepository;
        private ISY_Map_User_RoleRepository _SY_Map_User_RoleRepository;

        private ISY_UserRepository _SY_UserRepository;

        public SY_MenuFunctionService(ISY_MenuFunctionRepository _SY_MenuFunctionRepository, ISY_Map_Role_MenuRepository _SY_Map_Role_MenuRepository, ISY_Map_User_RoleRepository _SY_Map_User_RoleRepository, ISY_UserRepository _SY_UserRepository)
        {
            this._SY_MenuFunctionRepository = _SY_MenuFunctionRepository;
            this._SY_Map_Role_MenuRepository = _SY_Map_Role_MenuRepository;
            this._SY_Map_User_RoleRepository = _SY_Map_User_RoleRepository;
            this._SY_UserRepository = _SY_UserRepository;
        }

        public async Task<IEnumerable<SY_MenuFunction>> GetAllActiveByUserId(HttpContext context, SessionModel model)
        {
            var query = from n in _SY_MenuFunctionRepository.Table
                        where n.Active == true
                        select n;


            if (model != null && model.isAdmin == false)
            {
                var auths = AuthHelper.MenuFunctionByUserId(model, context).Result;

                var list = auths.Select(n => n.Id).ToList();

                query = query.Where(n => list.Contains(n.Id));
            }

            return await Task.FromResult(query);
        }

        public async Task<List<SY_MenuFunction>> GetAll()
        {
            var data = _SY_MenuFunctionRepository.Table;
            return await Task.FromResult(data.ToList());
        }

        public async Task<List<SY_MenuFunction>> GetAllActive()
        {
            var data = from n in _SY_MenuFunctionRepository.Table
                       where n.Active == true
                       select n;

            return await Task.FromResult(data.ToList());
        }

        public async Task<List<SY_MenuFunction>> GetAllActiveOrder()
        {
            var data = from n in _SY_MenuFunctionRepository.Table
                       where n.Active == true
                       orderby n.SortOrder
                       select n;

            return await Task.FromResult(data.ToList());
        }

        public async Task<SY_MenuFunction> GetById(string id)
        {
            return await _SY_MenuFunctionRepository.GetOneById(id);
        }

        public async Task<SY_MenuFunction_Submit> GetCustomById(string id)
        {
            var model = new SY_MenuFunction_Submit();

            var obj = GetById(id).Result;
            if (obj != null)
            {
                model = GetCustomByModel(obj).Result;
            }

            return await Task.FromResult(model);
        }

        public async Task<SY_MenuFunction_Submit> GetCustomByModel(SY_MenuFunction model)
        {
            var obj = new SY_MenuFunction_Submit()
            {
                ActionName = model.ActionName,
                Active = model.Active,
                ControllerName = model.ControllerName,
                Icon = model.Icon,
                Id = model.Id,
                MenuName = model.MenuName,
                MenuType = model.MenuType,
                ParentId = model.ParentId,
                SortOrder = model.SortOrder
            };

            return await Task.FromResult(obj);
        }

        public async Task<MessageReport> Create(SY_MenuFunction model)
        {
            return await _SY_MenuFunctionRepository.Add(model);
        }

        public async Task<MessageReport> Update(SY_MenuFunction model)
        {
            return await _SY_MenuFunctionRepository.Update(model);
        }

        public async Task<MessageReport> Delete(string ids)
        {

            var query = from n in _SY_MenuFunctionRepository.Table
                        where ids.Contains(n.Id)
                        select n;


            var result = await _SY_MenuFunctionRepository.Remove_Multi(query);

            return result;
        }

        public async Task<string> GetBreadcrumb(string id, string parentid, string lastvalue)
        {
            var list = GetAllActiveOrder().Result;

            if (string.IsNullOrWhiteSpace(parentid))
            {
                lastvalue += id;
            }
            else
            {
                var objParent = list.FirstOrDefault(n => n.Id.Equals(parentid));
                if (objParent != null)
                {
                    lastvalue += objParent.Id + ",";

                    var str = GetBreadcrumb(objParent.Id, objParent.ParentId.ToString(), lastvalue);

                    lastvalue = str.Result;
                }
            }

            return await Task.FromResult(lastvalue);
        }

        private List<string> GetListIdByUserId(string id)
        {
            //Danh sách quyền
            var roles = from r in _SY_Map_User_RoleRepository.Table
                        where r.UserId == id
                        select r.RoleId;

            var menus = from m in _SY_Map_Role_MenuRepository.Table
                        where roles.Contains(m.RoleId)
                        select m.MenuId;

            return menus.ToList();
        }

        public async Task<MessageReport> CreateMap(SY_Map_Role_Menu model)
        {
            return await _SY_Map_Role_MenuRepository.Add(model);
        }

        public async Task<MessageReport> DeleteMap(string roleid)
        {
            var t = from n in _SY_Map_Role_MenuRepository.Table
                    where n.RoleId == roleid
                    select n;

            return await _SY_Map_Role_MenuRepository.Remove_Multi(t);
        }

        public async Task<List<SY_MenuFunction_Submit>> GetAllCustomActiveOrder()
        {
            var dt = new List<SY_MenuFunction_Submit>();

            var data = from n in _SY_MenuFunctionRepository.Table
                       where n.Active == true
                       orderby n.SortOrder
                       select n;

            var k = data.ToList();

            foreach (var item in k)
            {
                dt.Add(GetCustomByModel(item).Result);
            }

            return await Task.FromResult(dt);
        }


    }
}