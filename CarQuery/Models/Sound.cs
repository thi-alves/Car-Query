namespace CarQuery.Models
{
    public class Sound
    {
        public int SoundId { get; set; }
        public string SoundPath { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }

    }
}
