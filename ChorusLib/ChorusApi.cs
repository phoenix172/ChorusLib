using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace ChorusLib
{
    public interface IChorusApi
    {
        List<Song> Search(SongProps filter);
    }

    public class ChorusApi : IChorusApi
    {
        private readonly string _apiUrl;

        public ChorusApi(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public List<Song> Search(SongProps filter)
        {
            string searchQuery = BuildSearchQuery(filter);
            var jsonResponse = GetJsonSearchResponse(searchQuery);
            var searchResult = JsonConvert.DeserializeObject<ChorusSearchResult>(jsonResponse);
            return searchResult.Songs;
        }

        private string BuildSearchQuery(SongProps songProps)
        {
            return $"name=\"{songProps.Name}\" artist=\"{songProps.Artist}\"" +
                   $" album=\"{songProps.Album}\" genre=\"{songProps.Genre}\"";
        }

        private string GetJsonSearchResponse(string searchQuery)
        {
            return GetJsonResponse("search", $"query={searchQuery}");
        }

        private string GetJsonResponse(string action, string query)
        {
            UriBuilder uriBuilder = new UriBuilder("https", $"{_apiUrl}/{action}");
            uriBuilder.Query = $"{query}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            var response = (HttpWebResponse)request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}