using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChorusLib
{
    public class ChorusApi : IChorusApi
    {
        private readonly string _apiUrl;

        public ChorusApi(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public async Task<List<Song>> SearchAsync(SongProps filter, int page = 1) =>
            await SearchAsync($"{BuildSearchQuery(filter)}", page);

        public async Task<List<Song>> SearchAsync(string filter, int page = 1)
        {
            var jsonResponse = await GetJsonSearchResponseAsync(filter, page);
            var searchResult = JsonConvert.DeserializeObject<ChorusSearchResult>(jsonResponse, new SongJsonConverter());
            return searchResult.Songs;
        }

        /// <summary>
        /// Returns a query string in accordance to the Chorus API specs
        /// </summary>
        /// <param name="props">Song props for the query</param>
        /// <param name="page">Result page</param>
        /// <returns>A space-delimited query string</returns>
        private string BuildSearchQuery(SongProps props)
        {
            var queryString = new StringBuilder();

            queryString.Append(props.ToQueryString());

            // difficulties -> flags
            // https://github.com/Paturages/chorus/blob/a18731cedb144b95c17f734b97a85c2ec1274d38/src/utils/db.js#L121
            foreach (var instrument in props.Instruments ?? Enumerable.Empty<SongProps.Instrument>())
                queryString.Append($" diff_{ instrument.GetName() }=15"); // available levels

            return queryString.ToString().Trim();
        }



        private async Task<string> GetJsonSearchResponseAsync(string searchQuery, int page = 1) =>
            await GetJsonResponseAsync("search", $"query={searchQuery}&from={(page - 1) * 20}");

        private async Task<string> GetJsonResponseAsync(string action, string query)
        {
            UriBuilder uriBuilder = new UriBuilder("https", $"{_apiUrl}/{action}")
            {
                Query = $"{query}"
            };
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpWebRequest request = WebRequest.CreateHttp(uriBuilder.Uri);
            request.ProtocolVersion = HttpVersion.Version11;
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
    }
}