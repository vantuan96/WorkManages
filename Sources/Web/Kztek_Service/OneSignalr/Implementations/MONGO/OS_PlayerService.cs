using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Configs;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models.OneSignalr;
using Kztek_Service.OneSignalr.Interfaces;
using Newtonsoft.Json;

namespace Kztek_Service.OneSignalr.MONGO.Implementations
{
    public class OS_PlayerService : IOS_PlayerService
    {
        private IOS_PlayerRepository _OS_PlayerRepository;

        public OS_PlayerService(IOS_PlayerRepository _OS_PlayerRepository)
        {
            this._OS_PlayerRepository = _OS_PlayerRepository;
        }

        public async Task<MessageReport> Create(OS_Player model)
        {
            return await _OS_PlayerRepository.Add(model);
        }

        public async Task<MessageReport> Delete(string id)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var obj = await GetByPlayerId(id);
            if (obj != null)
            {
                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'PlayerId': { '$eq': '" + id + "' }");
                query.AppendLine("}");

                return await _OS_PlayerRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<OS_Player> GetByPlayerId(string id)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'PlayerId': { '$eq': '" + id + "' }");
            query.AppendLine("}");

            var dt = await _OS_PlayerRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return dt.FirstOrDefault();
        }

        public async Task<OS_Player> GetByPlayerId_UserId(string id, string userid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'PlayerId': { '$eq': '" + id + "' }");
            query.AppendLine(", 'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine("}");

            var dt = await _OS_PlayerRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return dt.FirstOrDefault();
        }

        public async Task<List<OS_Player>> GetPlayerIdsByUserIds(List<string> ids)
        {
            var count = 0;

            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'UserId': { '$in': [");

            foreach (var item in ids)
            {
                count++;
                query.AppendLine(string.Format("'{0}'{1}", item, count == ids.Count ? "" : ","));
            }

            query.AppendLine("]}");

            query.AppendLine("}");

            return await _OS_PlayerRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<MessageReport> SendMessage(OneSignalrMessage model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                using (var client = new HttpClient())
                {
                    var url = new Uri("https://onesignal.com/api/v1/notifications");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", OneSignalConfig.APISercetKey);
                    var obj = new
                    {
                        app_id = OneSignalConfig.AppID,
                        headings = new { en = model.Title },
                        contents = new { en = model.Description },
                        include_player_ids = model.PlayerIds,
                        content_available = true,
                        data = new { View = model.View, ID = model.Id },
                    };
                    var json = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);


                    if (response.IsSuccessStatusCode)
                    {
                        result = new MessageReport(true, "Gửi thành công");
                    } else
                    {
                        result = new MessageReport(false, await response.Content.ReadAsStringAsync());
                    }
                }

                
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        public async Task<MessageReport> SendNotification(OneSignalrMessage model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                using (var client = new HttpClient())
                {
                    var url = new Uri("https://onesignal.com/api/v1/notifications");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", OneSignalConfig.APISercetKey);

                    var obj = new
                    {
                        app_id = OneSignalConfig.AppID,
                        headings = new { en = model.Title },
                        contents = new { en = model.Description },
                        included_segments = new string[] { "All" },
                        content_available = true,
                        data = new { View = model.View, ID = "" },
                    };

                    var json = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        result = new MessageReport(true, "Gửi thành công");
                    }
                    else
                    {
                        result = new MessageReport(false, await response.Content.ReadAsStringAsync());
                    }
                }


            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        public async Task<MessageReport> SendVoip(string fromUser, string toUser, List<string> playerids)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                using (var client = new HttpClient())
                {
                    var url = new Uri("https://onesignal.com/api/v1/notifications");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", OneSignalConfig.APISercetKey);

                    var obj = new
                    {
                        app_id = OneSignalConfig.AppID,
                        headings = new { en = fromUser },
                        contents = new { en = fromUser + " gọi cho bạn" },
                        include_player_ids = playerids.ToArray(),
                        content_available = true,
                        data = new { View = toUser, ID = "" },
                        apns_push_type_override = "voip"
                    };

                    var json = JsonConvert.SerializeObject(obj);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        result = new MessageReport(true, "Gửi thành công");
                    }
                    else
                    {
                        result = new MessageReport(false, await response.Content.ReadAsStringAsync());
                    }
                }


            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        public async Task<MessageReport> Update(OS_Player model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _OS_PlayerRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}
