using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ChorusLib.Tests
{
    [TestFixture]
    public class ChorusApiTests
    {
        private IChorusApi _repo;
        [OneTimeSetUp]
        public void SetUp()
        {
            _repo = new ChorusApi("chorus.fightthe.pw/api");
        }

        [TestCase("heaven")]
        [TestCase("eagle")]
        [TestCase("future")]
        public async Task Search_SongName(string songName)
        {
            var result = await _repo.SearchAsync(new SongProps
            {
                Name = songName
            });

            if (result.Count == 0) Assert.Inconclusive("No songs found");

            Assert.That(result, Has.All.Matches<Song>(
                x => x.Name.ToLower().Contains(songName.ToLower())));
        }

        [TestCase("heaven", "helloween")]
        [TestCase("eagle", "helloween")]
        [TestCase("future", "helloween")]
        public async Task Search_SongNameAndArtist(string songName, string artist)
        {
            var result = await _repo.SearchAsync(new SongProps
            {
                Name = songName,
                Artist = artist
            });

            if (result.Count == 0) Assert.Inconclusive("No songs found");

            Assert.That(result, Has.All.Matches<SongProps>(
                x => x.Name.ToLower().Contains(songName.ToLower())
                && x.Artist.ToLower().Contains(artist.ToLower())));
        }

        private static readonly SongProps[] InstrumentCases = {
            new SongProps { Name = "eagle",
                Instruments = new List<SongProps.Instrument> { SongProps.Instrument.Drums, SongProps.Instrument.Guitar}
            }
        };

        [TestCaseSource(nameof(InstrumentCases))]
        public async Task Search_SongNameAndInstrument(SongProps props)
        {
            var result = await _repo.SearchAsync(props);

            if (result.Count == 0) Assert.Inconclusive("No songs found");

            Assert.Multiple(() =>
            {
                Assert.That(result, Has.All.Matches<SongProps>(
                    x => x.Name.ToLower().Contains(props.Name.ToLower())));
                foreach (var song in result)
                {
                    Assert.That(song.Instruments, Is.SupersetOf(props.Instruments));
                }
            });
        }

        [TestCase("heaven", "helloween")]
        [TestCase("eagle", "helloween")]
        [TestCase("future", "helloween")]
        public async Task Search_HasSongDetails(string songName, string artist)
        {
            var result = await _repo.SearchAsync(new SongProps
            {
                Name = songName,
                Artist = artist
            });

            if (result.Count == 0) Assert.Inconclusive("No songs found");

            Assert.That(result, Has.All.Matches<Song>(
                x => x.Id != default && x.DirectLinks != default(Dictionary<string, string>)));
        }
    }
}
