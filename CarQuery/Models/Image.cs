using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CarQuery.Models
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }
        public string ImgPath { get; set; }
        public int CarId { get; set; }
        
        [JsonIgnore]
        public Car Car { get; set; }

        public Image()
        {

        }
        public Image(string imgPath, Car car)
        {
            ImgPath = imgPath;
            Car = car;
            
        }
        
    }
}
