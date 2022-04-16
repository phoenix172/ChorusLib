using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChorusLib
{
    internal static class Helpers
    {
        public static string ToQueryString(this SongProps props)
        {
            return $"name=\"{props.Name}\"" +
                   $" artist=\"{props.Artist}\"" +
                   $" album=\"{props.Album}\"" +
                   $" genre=\"{props.Genre}\"";
        }
        public static string GetName(this SongProps.Instrument instrument) =>
            Enum.GetName(typeof(SongProps.Instrument), instrument).ToLowerInvariant();
    }
}
