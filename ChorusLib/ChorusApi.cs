using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChorusLib
{
    public class ChorusApi : IChorusApi
    {
        private readonly string _apiUrl;

        public ChorusApi(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public async Task<List<Song>> SearchAsync(SongProps filter)
        {
            string searchQuery = BuildSearchQuery(filter);
            return await SearchAsync(searchQuery);
        }

        public async Task<List<Song>> SearchAsync(string filter)
        {
            var jsonResponse = await GetJsonSearchResponseAsync(filter);
            var searchResult = JsonConvert.DeserializeObject<ChorusSearchResult>(jsonResponse);
            return searchResult.Songs;
        }

        private string BuildSearchQuery(SongProps songProps)
        {
            return $"name=\"{songProps.Name}\" artist=\"{songProps.Artist}\"" +
                   $" album=\"{songProps.Album}\" genre=\"{songProps.Genre}\"";
        }

        private async Task<string> GetJsonSearchResponseAsync(string searchQuery)
        {
            return await GetJsonResponseAsync("search", $"query={searchQuery}");
        }

        private async Task<string> GetJsonResponseAsync(string action, string query)
        {
            UriBuilder uriBuilder = new UriBuilder("https", $"{_apiUrl}/{action}")
            {
                Query = $"{query}"
            };
            HttpWebRequest request = WebRequest.CreateHttp(uriBuilder.Uri);
            
            using (var response = (HttpWebResponse) await request.GetResponseAsync())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}