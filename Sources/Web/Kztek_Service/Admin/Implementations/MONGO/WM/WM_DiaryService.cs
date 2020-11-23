using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models.WM;
using Kztek_Service.Admin.Interfaces;
using Kztek_Service.Admin.Interfaces.WM;

namespace Kztek_Service.Admin.Implementations.MONGO.WM
{
    public class WM_DiaryService : IWM_DiaryService
    {
        private IWM_DiaryRepository _WM_DiaryRepository;
        private ISY_UserService _SY_UserService;

        public WM_DiaryService(IWM_DiaryRepository _WM_DiaryRepository, ISY_UserService _SY_UserService)
        {
            this._WM_DiaryRepository = _WM_DiaryRepository;
            this._SY_UserService = _SY_UserService;
        }

        public async Task<MessageReport> Create(WM_Diary obj)
        {
            return await _WM_DiaryRepository.Add(obj);
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

                return await _WM_DiaryRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<List<WM_Diary>> GetAllByUserId_ScheduleId(string userid, string scheduleid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine(", 'ScheduleId': { '$eq': '" + scheduleid + "' }");
            query.AppendLine("}");

            return await _WM_DiaryRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<WM_Diary> GetById(string id)
        {
            return await _WM_DiaryRepository.GetOneById(id);
        }

        public async Task<List<WM_DiaryCustomViewModel>> GetDiaryByUser(string scheduleid)
        {
            //Lấy danh sách diary tất cả về
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'ScheduleId': { '$eq': '" + scheduleid + "' }");
            query.AppendLine("}");

            var diaries = await _WM_DiaryRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            //Lấy danh sách người dùng có liên quan
            var users = await _SY_UserService.GetAllByIds(diaries.Select(n => n.UserId).ToList());

            //Mapping lại dữ liệu
            var custom = new List<WM_DiaryCustomViewModel>();
            if (users.Any())
            {
                foreach (var item in users)
                {
                    custom.Add(new WM_DiaryCustomViewModel()
                    {
                        Username = item.Username,
                        DataByUser = diaries.Where(n => n.UserId == item.Id).ToList()
                    });
                }
            }

            return custom;
        }

        public async Task<MessageReport> Update(WM_Diary obj)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + obj.Id + "' }");
            query.AppendLine("}");

            return await _WM_DiaryRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), obj);
        }
    }
}