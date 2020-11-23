using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models.MN;
using Kztek_Service.Admin.Interfaces.MN;

namespace Kztek_Service.Admin.Implementations.MONGO.MN
{
    public class MN_CustomerService : IMN_CustomerService
    {
        private IMN_CustomerRepository _MN_CustomerRepository;
        private IMN_CustomerGroupRepository _MN_CustomerGroupRepository;

        public MN_CustomerService(IMN_CustomerRepository _MN_CustomerRepository, IMN_CustomerGroupRepository _MN_CustomerGroupRepository)
        {
            this._MN_CustomerRepository = _MN_CustomerRepository;
            this._MN_CustomerGroupRepository = _MN_CustomerGroupRepository;
        }

        public async Task<MessageReport> Create(MN_Customer model)
        {
            return await _MN_CustomerRepository.Add(model);
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

                return await _MN_CustomerRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<MN_Customer> GetById(string id)
        {
            return await _MN_CustomerRepository.GetOneById(id);
        }

        public async Task<GridModel<MN_CustomerCustomView>> GetCustomPaging(string key, string customergroupid, int pageNumber, int pageSize)
        {
            var custom = new List<MN_CustomerCustomView>();

            var query = new StringBuilder();


            var k = await GetPaging(key, customergroupid, pageNumber, pageSize);

            //Danh sách nhóm khách hàng
            var customergroups = await GetCustomerGroupsByIds(k.Data.Select(n => n.CustomerGroupId).ToList());

            foreach (var item in k.Data)
            {
                //lấy nhóm khách hàng
                var objCustomerGroup = customergroups.FirstOrDefault(n => n.Id == item.CustomerGroupId);

                custom.Add(new MN_CustomerCustomView()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Note = item.Note,
                    Description = item.Description,
                    CustomerGroupName = objCustomerGroup != null ? objCustomerGroup.Name : ""
                });
            }

            var gridmodel = new GridModel<MN_CustomerCustomView>()
            {
                PageIndex = k.PageIndex,
                PageSize = k.PageSize,
                TotalIem = k.TotalIem,
                TotalPage = k.TotalPage,
                Data = custom
            };

            return gridmodel;
        }

        public async Task<GridModel<MN_Customer>> GetPaging(string key, string customergroupid, int pageNumber, int pageSize)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            if (!string.IsNullOrWhiteSpace(key))
            {
                query.AppendLine("'$or': [");

                query.AppendLine("{ 'Name': { '$in': [/" + key + "/i] } }");
                query.AppendLine(", { 'Description': { '$in': [/" + key + "/i] } }");

                query.AppendLine("]");
            }

            if (!string.IsNullOrWhiteSpace(customergroupid))
            {
                query.AppendLine(", 'CustomerGroupId': { '$eq': '" + customergroupid + "' }");
            }

            query.AppendLine("}");

            var sort = new StringBuilder();
            sort.AppendLine("{");
            sort.AppendLine("'CustomerGroupId': 1");
            sort.AppendLine(", 'Name': 1");
            sort.AppendLine("}");

            return await _MN_CustomerRepository.GetPaging(MongoHelper.ConvertQueryStringToDocument(query.ToString()), MongoHelper.ConvertQueryStringToDocument(sort.ToString()), pageNumber, pageSize);
        }

        public async Task<MessageReport> Update(MN_Customer model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _MN_CustomerRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }

        private async Task<List<MN_CustomerGroup>> GetCustomerGroupsByIds(List<string> ids)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'_id': { '$in': [");

            var count = 0;
            foreach (var item in ids)
            {
                count++;
                query.AppendLine(string.Format("'{0}'{1}", item, count == ids.Count ? "" : ","));
            }

            query.AppendLine("]}");

            query.AppendLine("}");

            return await _MN_CustomerGroupRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }
    }
}
