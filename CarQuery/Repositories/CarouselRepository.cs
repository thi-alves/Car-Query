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

        public async Task<Carousel> GetCarouselById(int carouselId)
        {
            Carousel carousel = await _context.Carousel
                .Include(c => c.CarouselSlides)
                .ThenInclude(cs => cs.Image)
                .FirstOrDefaultAsync(c => c.CarouselId == carouselId);

            return carousel;
        }
       
        //previousPosition indica a posição atual do Carrossel a ser atualizado. Ou seja, é a posição que deve ser atualizada. 
        public async Task<bool> UpdateCarousel(Carousel carousel, int previousPosition)
        {
            Console.WriteLine("UpdateCarousel");
            if (carousel == null)
            {
                return false;
            }

            if(previousPosition != carousel.Position)
            {
                bool operationResult = await UpdateCarouselPosition(carousel.Position, previousPosition);
                
                if (operationResult == false)
                {
                    return false;
                }
            }

            _context.Carousel.Update(carousel);
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<int> CountCarousel()
        {
            int total = await _context.Carousel.CountAsync();
            return total;
        }

        /*
         * 1 -No caso de criação de um novo Carrossel, usar apenas o parâmetro newPosition, que indica a posição que esse novo Carrossel deve assumir.
         * 2 -No caso de edição/atualização de um Carrossel existente, usar os dois parâmetros. Onde newPosition indica a posição que o Carrossel a ser 
         * atualizado deve assumir, e previousPosition indica a posição que o Carrossel a ser atualizado possui no momento.
        */
        public async Task<bool> UpdateCarouselPosition(int newPosition, int previousPosition = 0)
        {
            try
            {
                int total = await CountCarousel();

                if (newPosition <= total && newPosition != 0)
                {
                    Carousel carousel = await _context.Carousel.FirstOrDefaultAsync(c => c.Position == newPosition);

                    int rowsAffected = 0;

                    if (carousel != null)
                    {
                        if (previousPosition != 0)
                        {
                            carousel.Position = (short)previousPosition;
                            rowsAffected = await _context.SaveChangesAsync();

                            return rowsAffected > 0;
                        }
                        total++;
                        carousel.Position = (short)total;
                        rowsAffected = await _context.SaveChangesAsync();

                        return rowsAffected > 0;
                    }
                }
                else if ((total == 0 || newPosition == total + 1) && newPosition != 0)
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

        public async Task<bool> DeleteCarousel(int carouselId)
        {
            Carousel carousel = await _context.Carousel
                .Include(c => c.CarouselSlides)
                .ThenInclude(cs => cs.Image)
                .FirstOrDefaultAsync(c => c.CarouselId == carouselId);

            if (carousel != null)
            {
                int removedPosition = carousel.Position;

                _context.Carousel.Remove(carousel);

                int rowsAffected = await _context.SaveChangesAsync();

                if (rowsAffected > 0)
                {
                    int totalCarousel = await CountCarousel();

                    if (removedPosition < totalCarousel)
                    {
                        var carousels = await _context.Carousel.Where(c => c.Position >= removedPosition).ToListAsync();

                        if (carousels != null)
                        {
                            foreach (Carousel c in carousels)
                            {
                                c.Position = (short)removedPosition;
                                removedPosition++;
                            }

                            rowsAffected = await _context.SaveChangesAsync();

                            return rowsAffected > 0;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
