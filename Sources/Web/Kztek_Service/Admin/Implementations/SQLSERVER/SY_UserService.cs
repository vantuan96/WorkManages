using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Library.Security;
using Kztek_Model.Models;
using Kztek_Service.Admin.Interfaces;
using X.PagedList;

namespace Kztek_Service.Admin.Implementations.SQLSERVER
{
    public class SY_UserService : ISY_UserService
    {
        private ISY_UserRepository _SY_UserRepository;
        private ISY_Map_User_RoleRepository _SY_Map_User_RoleRepository;

        public SY_UserService(ISY_UserRepository _SY_UserRepository, ISY_Map_User_RoleRepository _SY_Map_User_RoleRepository)
        {
            this._SY_UserRepository = _SY_UserRepository;
            this._SY_Map_User_RoleRepository = _SY_Map_User_RoleRepository;
        }

        public async Task<List<SY_User>> GetAll()
        {
            var data = _SY_UserRepository.Table;
            return await Task.FromResult(data.ToList());
        }

        public async Task<GridModel<SY_User>> GetPaging(string key, int pageNumber, int pageSize)
        {

            var query = from n in _SY_UserRepository.Table
                        select n;

            if (!string.IsNullOrWhiteSpace(key))
            {
                key = key.ToLower();
                query = query.Where(n => n.Name.ToLower().Contains(key) || n.Username.ToLower().Contains(key));
            }

            var pageList = query.ToPagedList(pageNumber, pageSize);
            
            //
            // var model = new GridModel<SY_User>()
            // {
            //     Data = pageList.ToList(),
            //     PageIndex = pageNumber,
            //     PageSize = pageSize,
            //     TotalIem = pageList.TotalItemCount,
            //     TotalPage = pageList.PageCount
            // };

            var model = GridModelHelper<SY_User>.GetPage(pageList.ToList(), pageNumber, pageSize, pageList.TotalItemCount, pageList.PageCount);

            return await Task.FromResult(model);
        }

        public async Task<SY_User> GetById(string id)
        {
            return await _SY_UserRepository.GetOneById(id);
        }

        public async Task<SY_User> GetByUsername(string username)
        {
            var query = from n in _SY_UserRepository.Table
                        where n.Username == username
                        select n;

            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<SY_User> GetByUsername_notId(string username, string id)
        {
            var query = from n in _SY_UserRepository.Table
                        where n.Username == username && n.Id != id
                        select n;

            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<SY_User_Submit> GetCustomById(string id)
        {
            var model = new SY_User_Submit();

            var obj = await _SY_UserRepository.GetOneById(id);
            if (obj != null)
            {
                model = await GetCustomByModel(obj);
            }

            return model;
        }

        public async Task<SY_User_Submit> GetCustomByModel(SY_User model)
        {
            var obj = new SY_User_Submit()
            {
                Id = model.Id,
                Active = model.Active,
                Name = model.Name,
                Roles = new List<string>(),
                Username = model.Username,
                isAdmin = model.isAdmin,
                Avatar = model.Avatar
            };

            obj.Roles = (from n in _SY_Map_User_RoleRepository.Table
                         where n.UserId == model.Id
                         select n.RoleId).ToList();

            return await Task.FromResult(obj);
        }

        public async Task<MessageReport> Create(SY_User model)
        {
            return await _SY_UserRepository.Add(model);
        }

        public async Task<MessageReport> Update(SY_User model)
        {
            return await _SY_UserRepository.Update(model);
        }

        public async Task<MessageReport> Delete(string id)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var obj = GetById(id);
            if (obj.Result != null)
            {
                return await _SY_UserRepository.Remove(obj.Result);
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public Task<MessageReport> Login(AuthModel model, out SY_User user)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Kiểm tra username
                var objUser = GetByUsername(model.Username).Result;
                if (objUser == null)
                {
                    user = null;
                    result = new MessageReport(false, "Tài khoản không tồn tại");
                    return Task.FromResult(result);
                }

                if (objUser.Active == false)
                {
                    user = null;
                    result = new MessageReport(false, "Tài khoản bị khóa");
                    return Task.FromResult(result);
                }

                //Giải mã
                var pass = CryptoHelper.DecryptPass_User(objUser.Password, objUser.PasswordSalat);

                //Check mật khẩu
                if (pass != model.Password)
                {
                    user = null;
                    result = new MessageReport(false, "Mật khẩu không khớp");
                    return Task.FromResult(result);
                }

                //Gán lại user
                user = objUser;
                result = new MessageReport(true, "Đăng nhập thành công");
            }
            catch (System.Exception ex)
            {
                user = null;
                result = new MessageReport(false, ex.Message);
            }

            return Task.FromResult(result);
        }

        public Task<List<SY_User>> GetAllActive()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<SY_User>> GetAllByIds(List<string> ids)
        {
            throw new System.NotImplementedException();
        }
    }
}