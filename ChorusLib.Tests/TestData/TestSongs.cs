using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static ChorusLib.Tests.PathHelper;

namespace ChorusLib.Tests.TestData
{
    static class TestSongs
    {
        public static Song MakeArchivedSong()
        {
            string songPath = GetSongPath(@"TestData\TestSong1.rar");
            return new Song
            {
                Artist="Gosho",
                Name = "TestSong1Archived",
                DirectLinks = new Dictionary<string, string>
                {
                    {"archive", GetLocalFileUri(songPath)}
                }
            };
        }

        public static Song MakeUnpackedSong()
        {
            string songPath = GetSongPath(@"TestData\TestSong2");
            var files = Directory.GetFiles(songPath).ToDictionary(
                x => Path.GetFileName(x), x => GetLocalFileUri(x));
            return new Song
            {
                Artist="Pesho",
                Name = "TestSong2Unpacked",
                DirectLinks = files
            };
        }
    }
}
