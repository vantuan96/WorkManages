using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Service.Admin.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Kztek_Service.Admin.Implementations.MONGO
{
    public class SY_MenuFunctionService : ISY_MenuFunctionService
    {
        private ISY_MenuFunctionRepository _SY_MenuFunctionRepository;
        private ISY_Map_Role_MenuRepository _SY_Map_Role_MenuRepository;

        public SY_MenuFunctionService(ISY_MenuFunctionRepository _SY_MenuFunctionRepository, ISY_Map_Role_MenuRepository _SY_Map_Role_MenuRepository)
        {
            this._SY_MenuFunctionRepository = _SY_MenuFunctionRepository;
            this._SY_Map_Role_MenuRepository = _SY_Map_Role_MenuRepository;
        }

        public async Task<MessageReport> Create(SY_MenuFunction model)
        {
            return await _SY_MenuFunctionRepository.Add(model);
        }

        public async Task<MessageReport> CreateMap(SY_Map_Role_Menu model)
        {
            return await _SY_Map_Role_MenuRepository.Add(model);
        }

        public async Task<MessageReport> Delete(string ids)
        {
            //
            var arr = ids.Split(';', StringSplitOptions.RemoveEmptyEntries);

            //query
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$in': [");

            var count = 0;
            foreach (var item in arr)
            {
                count++;
                query.AppendLine(string.Format("'{0}'{1}", item, count == arr.Length ? "" : ","));
            }

            query.AppendLine("]}");
            query.AppendLine("}");

            return await _SY_MenuFunctionRepository.Remove_Multi(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<MessageReport> DeleteMap(string roleid)
        {
            //query
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'RoleId': { '$eq': '" + roleid + "' }");
            query.AppendLine("}");

            return await _SY_Map_Role_MenuRepository.Remove_Multi(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<SY_MenuFunction>> GetAll()
        {
            return await Task.FromResult(_SY_MenuFunctionRepository.Table.ToList());
        }

        public async Task<List<SY_MenuFunction>> GetAllActive()
        {
            //query
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'Active': true");
            query.AppendLine("}");

            return await _SY_MenuFunctionRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<IEnumerable<SY_MenuFunction>> GetAllActiveByUserId(HttpContext context, SessionModel model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'Active': true");

            if (model != null && model.isAdmin == false)
            {
                var auths = AuthHelper.MenuFunctionByUserId(model, context).Result;

                query.AppendLine(", '_id': { '$in': [");

                var count = 0;
                foreach (var item in auths)
                {
                    count++;
                    query.AppendLine(string.Format("'{0}'{1}", item.Id, count == auths.Count ? "" : ","));
                }

                query.AppendLine("]}");
            }

            query.AppendLine("}");

            return await _SY_MenuFunctionRepository.GetMany(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<SY_MenuFunction>> GetAllActiveOrder()
        {
            //query
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'Active': true");
            query.AppendLine("}");

            return await _SY_MenuFunctionRepository.GetMany(MongoHelper.ConvertQueryStringToDocument(query.ToString()), "SortOrder", false);
        }

        public async Task<List<SY_MenuFunction_Submit>> GetAllCustomActiveOrder()
        {
            var dt = new List<SY_MenuFunction_Submit>();

            var dta = await GetAllActiveOrder();

            foreach (var item in dta)
            {
                dt.Add(await GetCustomByModel(item));
            }

            return await Task.FromResult(dt);
        }

        public async Task<string> GetBreadcrumb(string id, string parentid, string lastvalue)
        {
            var list = await GetAllActiveOrder();

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

        public async Task<SY_MenuFunction> GetById(string id)
        {
            return await _SY_MenuFunctionRepository.GetOneById(id);
        }

        public async Task<SY_MenuFunction_Submit> GetCustomById(string id)
        {
            var model = new SY_MenuFunction_Submit();

            var obj = await GetById(id);
            if (obj != null)
            {
                model = await GetCustomByModel(obj);
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

        public async Task<MessageReport> Update(SY_MenuFunction model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _SY_MenuFunctionRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}
