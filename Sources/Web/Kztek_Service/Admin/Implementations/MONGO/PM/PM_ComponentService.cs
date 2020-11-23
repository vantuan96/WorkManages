using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models.PM;
using Kztek_Service.Admin.Interfaces.PM;
using MongoDB.Driver;

namespace Kztek_Service.Admin.Implementations.MONGO.PM
{
    public class PM_ComponentService : IPM_ComponentService
    {
        private IPM_ComponentRepository _PM_ComponentRepository;

        public PM_ComponentService(IPM_ComponentRepository _PM_ComponentRepository)
        {
            this._PM_ComponentRepository = _PM_ComponentRepository;
        }

        public async Task<MessageReport> Create(PM_Component model)
        {
            return await _PM_ComponentRepository.Add(model);
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

                result = await _PM_ComponentRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

                //Tìm các thằng con => update lên thằng cha của bản ghi xóa.
                if (result.isSuccess)
                {
                    await GetChildByComponent(obj);
                }

                return result;
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<MessageReport> DeleteByProjectId(string projectid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'ProjectId': { '$eq': '" + projectid + "' }");
            query.AppendLine("}");

            return await _PM_ComponentRepository.Remove_Multi(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<PM_Component>> GetAllByIds(List<string> ids, int status = 0)
        {
            var count = 0;

            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'Status': {'$eq': " + status + "}");

            query.AppendLine(", '_id': { '$in': [");

            foreach (var item in ids)
            {
                count++;
                query.AppendLine(string.Format("'{0}'{1}", item, count == ids.Count ? "" : ","));
            }

            query.AppendLine("]}");

            query.AppendLine("}");

            return await _PM_ComponentRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<PM_Component>> GetAllByProjectId(string projectid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'ProjectId': { '$eq': '" + projectid + "' }");

            query.AppendLine("}");

            return await _PM_ComponentRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<PM_Component>> GetAllCurrentByIds(List<string> ids, int status = 0)
        {
            var time = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            var count = 0;

            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'Status': {'$eq': " + status + "}");

            query.AppendLine("'DateStart': {'$lt': ISODate('" + time + "T00:00:00.000+07:00')}");

            query.AppendLine(", '_id': { '$in': [");

            foreach (var item in ids)
            {
                count++;
                query.AppendLine(string.Format("'{0}'{1}", item, count == ids.Count ? "" : ","));
            }

            query.AppendLine("]}");

            query.AppendLine("}");

            return await _PM_ComponentRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<PM_Component_Submit>> GetAllCustomByProjectId(string projectid)
        {
            var custom = new List<PM_Component_Submit>();

            var data = await GetAllByProjectId(projectid);

            foreach (var item in data)
            {
                custom.Add(await GetCustomByModel(item));
            }

            return custom;
        }

        public async Task<PM_Component> GetByCode(string projectid, string code)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'ProjectId': { '$eq': '" + projectid + "' }");
            query.AppendLine(", 'Code': { '$eq': '" + code + "' }");

            query.AppendLine("}");

            var dt = await _PM_ComponentRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));


            return dt.FirstOrDefault();
        }

        public async Task<PM_Component> GetById(string id)
        {
            return await _PM_ComponentRepository.GetOneById(id);
        }

        public async Task<PM_Component_Submit> GetCustomById(string id)
        {
            var model = new PM_Component_Submit();

            var obj = await GetById(id);
            if (obj != null)
            {
                model = await GetCustomByModel(obj);
            }

            return await Task.FromResult(model);
        }

        public async Task<PM_Component_Submit> GetCustomByModel(PM_Component model)
        {
            var obj = new PM_Component_Submit()
            {
                Id = model.Id,
                ParentId = model.ParentId,
                Title = model.Title,
                Code = model.Code,
                DateCreated = model.DateCreated,
                DateEnd = model.DateEnd,
                DateStart = model.DateStart,
                Description = model.Description,
                Label = model.Label,
                Note = model.Note,
                ProjectId = model.ProjectId,
                Status = model.Status
            };

            return await Task.FromResult(obj);
        }

        public async Task<MessageReport> MComplete(string id)
        {
            var obj = await GetById(id);
            if (obj != null)
            {
                obj.Status = 1;
            }

            return await Update(obj);
        }

        public async Task<MessageReport> Update(PM_Component model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _PM_ComponentRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }

        private async Task<int> GetChildByComponent(PM_Component obj)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'ParentId': { '$eq': '" + obj.Id + "' }");

            query.AppendLine("}");

            var k = await _PM_ComponentRepository.GetCollection();

            var update = Builders<PM_Component>.Update
                        .Set(p => p.ParentId, obj.ParentId);

            var a = await k.UpdateManyAsync(MongoHelper.ConvertQueryStringToDocument(query.ToString()), update);

            return 1;
        }
    }
}
