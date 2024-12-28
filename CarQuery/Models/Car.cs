namespace CarQuery.Models
{
    public class Car
    {
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Power { get; set; }
        public string EnginePosition { get; set; }
        public string Cylinders { get; set; }
        public int TopSpeed { get; set; }
        public int Doors { get; set; }
        public double Price { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public List<Image> Images { get; set; }
        public Sound Sound { get; set; }


    }
}
