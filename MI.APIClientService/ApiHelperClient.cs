using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MI.APIClientService
{
    public class ApiHelperClient
    {
        public async Task<T> PostAsync<T>(string url, object Model)
        {
            using (var http = new HttpClient())
            {
                var httpContent = new StringContent(JsonConvert.SerializeObject(Model), Encoding.UTF8, "application/json");
                //await异步等待回应
                var response = await http.PostAsync(url, httpContent);

                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();

                //await异步读取
                string Result = await response.Content.ReadAsStringAsync();

                var Item = JsonConvert.DeserializeObject<T>(Result);

                return Item;
            }              
        }

        public async Task PostAsync(string url, string requestMessage)
        {
            using (var http = new HttpClient())
            {
                var httpContent = new StringContent(requestMessage, Encoding.UTF8, "application/json");
                //await异步等待回应
                var response = await http.PostAsync(url, httpContent);

                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<T> GetAsync<T>(string url)
        {
            using (var http = new HttpClient())
            {
                var response = await http.GetAsync(url);
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();

                var Result = await response.Content.ReadAsStringAsync();

                var Items = JsonConvert.DeserializeObject<T>(Result);

                return Items;
            }
        }

    }
}
