using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

        public async Task DownloadAsync(Song song)
        {
            try
            {
                if (!EnsureSongFolder(song, out string songFolder))
                    return;
                if (song.DirectLinks.ContainsKey(ArchiveKey))
                {
                    await DownloadAndUnpackSongAsync(song, songFolder);
                }
                else
                {
                    await DownloadUnpackedSongAsync(song, songFolder);
                }
            }
            catch (Exception e)
            {
                throw new SongDownloadException(song, e);
            }
        }

        private async Task DownloadUnpackedSongAsync(Song song, string songFolder)
        {
            foreach (var fileLink in song.DirectLinks.Values)
            {
                await _downloader.DownloadFileAsync(fileLink, songFolder);
            }
        }

        private async Task DownloadAndUnpackSongAsync(Song song, string songFolder)
        {
            string archiveLink = song.DirectLinks[ArchiveKey];
            string archivePath = await _downloader.DownloadFileAsync(archiveLink, songFolder);
            await ExtractArchiveAsync(archivePath, songFolder);
            File.Delete(archivePath);
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

        private string GetSongFolder(Song song)
        {
            return Path.Combine(_downloadLocation, song.FolderName);
        }

        private async Task ExtractArchiveAsync(string archivePath, string extractPath)
        {
            await Task.Run(() =>
            {
                using (var archiveStream = File.OpenRead(archivePath))
                using (var reader = ReaderFactory.Open(archiveStream))
                {
                    reader.WriteAllToDirectory(extractPath);
                }
            });
        }
    }

    public class SongDownloadException : Exception
    {
        public Song Song { get; }

        public SongDownloadException(Song song, Exception innerException)
            : base($"An error occured while downloading song '{song}'", innerException)
        {
            Song = song;
        }
    }
}