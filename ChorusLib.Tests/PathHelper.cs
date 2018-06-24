using System;
using System.IO;
using NUnit.Framework;

namespace ChorusLib.Tests
{
    static class PathHelper
    {
        public static string SongDownloadPath { get; set; } = Path.GetTempPath();

        public static DirectoryInfo GetExpectedSongDirectory(Song song)
        {
            string expectedSongPath = Path.Combine(SongDownloadPath, song.FolderName);
            return new DirectoryInfo(expectedSongPath);
        }

        public static string GetLocalFileUri(string filePath)
        {
            return new Uri(filePath).AbsoluteUri;
        }

        public static string GetSongPath(string relativePath)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, relativePath);
        }
    }
}