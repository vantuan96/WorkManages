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
using Kztek_Service.Api.Interfaces;

namespace Kztek_Service.Api.Implementations.MONGO
{
    public class CustomerService : ICustomerService
    {
        private IMN_CustomerRepository _MN_CustomerRepository;
        private IMN_CustomerGroupRepository _MN_CustomerGroupRepository;
        private IMN_ContactRepository _MN_ContactRepository;

        public CustomerService(IMN_CustomerRepository _MN_CustomerRepository, IMN_CustomerGroupRepository _MN_CustomerGroupRepository, IMN_ContactRepository _MN_ContactRepository)
        {
            this._MN_CustomerRepository = _MN_CustomerRepository;
            this._MN_CustomerGroupRepository = _MN_CustomerGroupRepository;
            this._MN_ContactRepository = _MN_ContactRepository;
        }

        public async Task<GridModel<MN_CustomerCustomView>> GetPagingByFirst(string key, int pageNumber, int pageSize)
        {
            var custom = new List<MN_CustomerCustomView>();

            var query = new StringBuilder();

            var k = await GetPaging(key, "", pageNumber, pageSize);

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

        private async Task<GridModel<MN_Customer>> GetPaging(string key, string customergroupid, int pageNumber, int pageSize)
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

        public async Task<MN_CustomerDetailCustomView> GetCustomById(string id)
        {
            var model = new MN_CustomerDetailCustomView();

            try
            {
                //Lấy người khách hàng
                var objCustomer = await _MN_CustomerRepository.GetOneById(id);

                //Lấy nhóm khách hàng
                var objCustomerGroup = await _MN_CustomerGroupRepository.GetOneById(objCustomer.CustomerGroupId);

                //Lấy danh sách contacts
                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'CustomerId' : { '$eq': '" + objCustomer.Id + "' }");
                query.AppendLine("}");

                var listContacts = await _MN_ContactRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

                //Mapping
                model = new MN_CustomerDetailCustomView()
                {
                    Id = objCustomer.Id,
                    Name = objCustomer.Name,
                    Description = objCustomer.Description,
                    Note = objCustomer.Note,
                    CustomerGroupName = objCustomerGroup.Name,
                    Contacts = listContacts
                };

            }
            catch
            {
                model = new MN_CustomerDetailCustomView()
                {
                    Id = "",
                    Name = "Không tồn tại",
                    Description = "Không tồn tại",
                    Note = "Không tồn tại",
                    CustomerGroupName = "",
                    Contacts = new List<MN_Contact>()
                };
            }

            return model;
        }
    }
}
