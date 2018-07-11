using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;

namespace ChorusLib
{
    public class SongDownloader
    {
        const string ArchiveKey = "archive";
        private readonly string _downloadLocation;
        private readonly bool _overrideExisting;
        private readonly IFileDownloader _downloader;

        public SongDownloader(string downloadLocation, bool overrideExisting = true)
        {
            _downloadLocation = downloadLocation;
            _overrideExisting = overrideExisting;
            _downloader = new WebRequestDownloader();
        }

        public void Download(Song song)
        {
            if (song.DirectLinks.ContainsKey(ArchiveKey))
            {
                DownloadAndUnpackSong(song);
            }
            else
            {
                DownloadUnpackedSong(song);
            }
        }

        private void DownloadUnpackedSong(Song song)
        {
            if (!EnsureSongFolder(song, out string songFolder))
                return;
            foreach (var fileLink in song.DirectLinks.Values)
            {
                _downloader.DownloadFile(fileLink, songFolder);
            }
        }

        private bool EnsureSongFolder(Song song, out string songFolder)
        {
            songFolder = GetSongFolder(song);
            if (Directory.Exists(songFolder))
            {
                if (_overrideExisting)
                    Directory.Delete(songFolder, true);
                else
                    return false;
            }
            Directory.CreateDirectory(songFolder);
            return true;
        }

        private void DownloadAndUnpackSong(Song song)
        {
            string archiveLink = song.DirectLinks[ArchiveKey];
            if (!EnsureSongFolder(song, out string songFolder)) return;
            string archivePath = _downloader.DownloadFile(archiveLink, songFolder);
            ExtractArchive(archivePath, songFolder);
        }

        private string GetSongFolder(Song song)
        {
            return Path.Combine(_downloadLocation, song.FolderName);
        }

        private void ExtractArchive(string archivePath, string extractPath)
        {
            using (var archiveStream = File.OpenRead(archivePath))
            using (var reader = ReaderFactory.Open(archiveStream))
            {
                reader.WriteAllToDirectory(extractPath);
            }
        }
    }
}
