using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using NUnit.Framework;
using static ChorusLib.Tests.PathHelper;
using static ChorusLib.Tests.TestData.TestSongs;

namespace ChorusLib.Tests
{
    [TestFixture]
    public class SongDownloaderTest
    {
        [TestCaseSource(nameof(GetSongs))]
        public void Download_Song_DownloadsSongToLocation(Song testSong)
        {
            var songDirectory = GetExpectedSongDirectory(testSong);

            DownloadSong(testSong);

            Assert.That(songDirectory.Exists);
            Assert.That(songDirectory.GetFiles(), Has.Length.Positive);
        }

        public static IEnumerable<Song> GetSongs()
        {
            yield return MakeArchivedSong();
            yield return MakeUnpackedSong();
        }

        private void DownloadSong(Song testSong, bool overrideExisting = true)
        {
            SongDownloader downloader = new SongDownloader(SongDownloadPath, overrideExisting);
            downloader.Download(testSong);
        }
    }
}