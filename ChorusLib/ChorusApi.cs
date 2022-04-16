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

            // make it somewhat dynamic by getting all string properties
            foreach (var property in props.GetType().GetProperties().Where(x => x.PropertyType == typeof(string)))
            {
                var value = property.GetValue(props) as string;
                if (!string.IsNullOrEmpty(value)) queryString.Append($" {property.Name.ToLowerInvariant()}=\"{value}\"");
            }

            // difficulties -> flags
            // https://github.com/Paturages/chorus/blob/a18731cedb144b95c17f734b97a85c2ec1274d38/src/utils/db.js#L121
            foreach (var instrument in props.Instruments ?? Enumerable.Empty<SongProps.Instrument>())
            {
                //queryString += $" tier_{ GetInstrumentName(instrument)}=Dgt0"; // rb2 difficulty
                queryString.Append($" diff_{ GetInstrumentName(instrument)}=15"); // available levels
            }

            return queryString.ToString().Trim();
        }

        private static string GetInstrumentName(SongProps.Instrument instrument) =>
            Enum.GetName(typeof(SongProps.Instrument), instrument).ToLowerInvariant();

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
            using (var response = (HttpWebResponse) await request.GetResponseAsync())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
                return await streamReader.ReadToEndAsync();
            }
        }

        /// <summary>
        /// Extracts instrument data from the Chorus API result to the Song model
        /// </summary>
        private class SongJsonConverter : JsonConverter<Song>
        {
            public override bool CanWrite => false;

            public override Song ReadJson(JsonReader reader, Type objectType, Song existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject props = JObject.Load(reader);

                var song = props.ToObject<Song>();

                song.Instruments =
                    (Enum.GetValues(typeof(SongProps.Instrument)) as SongProps.Instrument[])
                        .Where(instrument => (props["hashes"] as JObject).ContainsKey(GetInstrumentName(instrument)))
                        .ToList();

                return song;
            }

            public override void WriteJson(JsonWriter writer, Song value, JsonSerializer serializer) => throw new NotImplementedException();
        }
    }
}