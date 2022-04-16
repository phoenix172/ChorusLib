using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace ChorusLib
{
    internal class SongJsonConverter : JsonConverter<Song>
    {
        public override bool CanWrite => false;

        public override Song ReadJson(JsonReader reader, Type objectType, Song existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject props = JObject.Load(reader);

            var song = props.ToObject<Song>();

            song.Instruments =
                (Enum.GetValues(typeof(SongProps.Instrument)) as SongProps.Instrument[])
                    .Where(instrument => (props["hashes"] as JObject).ContainsKey(instrument.GetName()))
                    .ToList();

            return song;
        }

        public override void WriteJson(JsonWriter writer, Song value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
