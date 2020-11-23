using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NC_WorkmanagementSelfHost.Helpers
{
    public class ApiHelper
    {
        public static HttpClient client;

        static ApiHelper()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
        }

        public static async Task<T> ConvertResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var t = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                return t;
            }

            return default;
        }

        public static async Task<HttpResponseMessage> HttpGet(string url, string token = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                return await client.GetAsync(url);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<HttpResponseMessage> HttpPost<T>(string url, T obj, string token = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var content = JsonConvert.SerializeObject(obj);

                var data = new StringContent(content, Encoding.UTF8, "application/json");

                return await client.PostAsync(url, data);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
