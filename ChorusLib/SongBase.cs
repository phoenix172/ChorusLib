namespace ChorusLib
{
    public class SongBase
    {
        public string Artist { get; set; }
        public string Name { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }

        public override string ToString()
        {
            return $"{Artist} - {Name}";
        }
    }
}