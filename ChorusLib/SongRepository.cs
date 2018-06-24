using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace ChorusLib
{
    public class SongRepository
    {
        private readonly IChorusApi _chorusApi;

        public SongRepository(IChorusApi chorusApi)
        {
            _chorusApi = chorusApi;
        }

        public SongRepository(string chorusApiUrl)
        {
            _chorusApi = new ChorusApi(chorusApiUrl);
        }

        public List<Song> Search(SongProps filter)
        {
            return _chorusApi.Search(filter);
        }
    }

    public class ChorusSearchResult
    {
        public List<Song> Songs { get; set; }
    }
}
