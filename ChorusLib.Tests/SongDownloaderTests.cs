using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using static ChorusLib.Tests.PathHelper;
using static ChorusLib.Tests.TestData.TestSongs;

namespace ChorusLib.Tests
{
    [TestFixture]
    public class SongDownloaderTests
    {
        [TestCaseSource(nameof(GetSongs))]
        public async Task Download_Song_DownloadsSongToLocation(Song testSong)
        {
            var songDirectory = GetExpectedSongDirectory(testSong);

            await DownloadSongAsync(testSong);

            Assert.That(songDirectory.Exists);
            Assert.That(songDirectory.GetFiles(), Has.Length.Positive);
        }

        public static IEnumerable<Song> GetSongs()
        {
            yield return MakeArchivedSong();
            yield return MakeUnpackedSong();
        }

        private async Task DownloadSongAsync(Song testSong, bool overrideExisting = true)
        {
            SongDownloader downloader = new SongDownloader(SongDownloadPath, overrideExisting);
            await downloader.DownloadAsync(testSong);
        }
    }
}