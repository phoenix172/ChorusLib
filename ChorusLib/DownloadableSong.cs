using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChorusLib
{
    public class DownloadableSong : SongBase
    {
        public int Id { get; set; }
        public IDictionary<string, string> DirectLinks { get; set; }

        public string FolderName => !String.IsNullOrEmpty(Artist) ? $"{Artist} - {Name}" : $"{Name}";

        public override string ToString()
        {
            return $"{Id} - {Artist} - {Name}";
        }
    }
}