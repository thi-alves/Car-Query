using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CarQuery.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _context;
        public ImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public Image GetImageById(int imgId)
        {
            return _context.Image.FirstOrDefault(i => i.ImageId == imgId);
        }

        public async Task<List<Image>> GetImagesByCarId(int carId)
        {
            return await _context.Image
                .Where(i => i.CarId == carId)
                .OrderBy(i => i.ImageId)
                .ToListAsync();
        }
        public async Task DeleteImageById(int imageId)
        {
            Image img = _context.Image.FirstOrDefault(i => i.ImageId == imageId);

            if(img != null)
            {
                _context.Image.Remove(img);
                await _context.SaveChangesAsync();
            }
        }
    }
}
