using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChorusLib
{
    public class Song : SongProps
    {
        public int Id { get; set; }
        public IDictionary<string, string> DirectLinks { get; set; }

        public string FolderName => !String.IsNullOrEmpty(Artist) ? $"{Artist} - {Name}" : $"{Name}";

        public override string ToString()
        {
            return $"{Id} - {Artist} - {Name}";
        }
    }

    public class SongProps
    {
        public string Artist { get; set; }
        public string Name { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
    }
}