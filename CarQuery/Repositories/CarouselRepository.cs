using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CarQuery.Repositories
{
    public class CarouselRepository : ICarouselRepository
    {
        private readonly AppDbContext _context;

        public CarouselRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<bool> CreateCarousel(Carousel carousel)
        {
            try
            {
                if (carousel != null)
                {
                    var operationSucceeded = await UpdateCarouselPosition(carousel.Position);

                    if (operationSucceeded)
                    {
                        await _context.Carousel.AddAsync(carousel);
                        int rowsAffected = await _context.SaveChangesAsync();

                        return rowsAffected > 0;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<int> CountCarousel()
        {
            int total = await _context.Carousel.CountAsync();
            return total;
        }

        public async Task<bool> UpdateCarouselPosition(int position)
        {
            try
            {
                int total = await CountCarousel();

                if (position <= total && position != 0)
                {
                    Carousel carousel = await _context.Carousel.FirstOrDefaultAsync(c => c.Position == position);

                    if (carousel != null)
                    {
                        total++;
                        carousel.Position = (short)total;

                        int rowsAffected = await _context.SaveChangesAsync();

                        return rowsAffected > 0;
                    }
                }
                else if((total == 0 || position == total+1) && position != 0)
                {
                    return true;
                }
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}
