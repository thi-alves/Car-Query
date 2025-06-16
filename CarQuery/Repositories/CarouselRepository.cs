using System.Data;
using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
using CarQuery.ViewModels.CarouselSlidesViewModel;
using CarQuery.ViewModels.CarouselViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (carousel != null)
                {
                    var operationSucceeded = await ResolvePositionConflict(carousel.Position);
                    if (operationSucceeded)
                    {
                        await _context.Carousel.AddAsync(carousel);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                }
                await transaction.RollbackAsync();
                return false;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (carousel == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                if (previousPosition != carousel.Position)
                {
                    bool operationResult = await ResolvePositionConflict(carousel.Position, previousPosition);
                    if (operationResult == false)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                _context.Carousel.Update(carousel);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
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
        public async Task<bool> ResolvePositionConflict(int newPosition, int previousPosition = 0)
        {
            try
            {
                int maxPosition = await CountCarousel();
                if (newPosition <= maxPosition && newPosition != 0)
                {
                    Carousel carouselAtTargetPosition = await _context.Carousel
                        .FirstOrDefaultAsync(c => c.Position == newPosition);

                    if (carouselAtTargetPosition == null)
                    {
                        return true;
                    }
                    else if (previousPosition != 0)
                    {
                        carouselAtTargetPosition.Position = (short)previousPosition;
                    }
                    else
                    {
                        maxPosition++;
                        carouselAtTargetPosition.Position = (short)maxPosition;
                    }

                    return true;
                }
                //nesse caso, ou ainda não existem carousels no sistema, ou o carousel a ser adicionado ocupará a última position
                else if (newPosition == maxPosition + 1)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCarousel(int carouselId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Carousel carousel = await _context.Carousel
                    .Include(c => c.CarouselSlides)
                    .ThenInclude(cs => cs.Image)
                    .FirstOrDefaultAsync(c => c.CarouselId == carouselId);

                if (carousel == null)
                {
                    return false;
                }

                int removedPosition = carousel.Position;
                var carouselsToReposition = await _context.Carousel
                    .Where(c => c.Position > removedPosition)
                    .OrderBy(c => c.Position)
                    .ToListAsync();
                _context.Carousel.Remove(carousel);

                //reorganiza as posições dos carousels restantes
                foreach (var c in carouselsToReposition)
                {
                    c.Position = (short)(c.Position - 1);
                }

                int rowsAffected = await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (transaction.GetDbTransaction().Connection?.State == ConnectionState.Open)
                {
                    await transaction.CommitAsync();
                }
            }
        }

        public async Task DeleteAllCarouselsSlidesByImage(Image img)
        {
            var carouselsSlides = await _context.CarouselSlide
                .Where(c => c.Image.ImageId == img.ImageId)
                .ToListAsync();

            _context.CarouselSlide.RemoveRange(carouselsSlides);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CarouselDisplayViewModel>> GetAllVisibleCarouselsToDisplay()
        {
            var carousels = await _context.Carousel
                .Select(c => new
                {
                    c.Title,
                    c.Position,
                    c.IsVisible,

                    CarouselSlides = c.CarouselSlides.Select(cs => new
                    {
                        CarId = cs.CarId,
                        CarModel = cs.Car.Model,
                        CarShortDescription = cs.Car.ShortDescription,
                        ImagePath = cs.Image.ImgPath
                    })
                    .ToList()
                })
                .Where(c => c.IsVisible == true)
                .OrderBy(c => c.Position)
                .ToListAsync();

            List<CarouselDisplayViewModel> carouselsVm = new List<CarouselDisplayViewModel>();

            foreach (var carousel in carousels)
            {
                CarouselDisplayViewModel newCarousel = new CarouselDisplayViewModel(carousel.Title);
                foreach (var carouselSlide in carousel.CarouselSlides)
                {
                    CarouselSlideViewModel carouselSlideViewModel = new CarouselSlideViewModel(carouselSlide.CarId, carouselSlide.CarModel, carouselSlide.CarShortDescription, carouselSlide.ImagePath);
                    newCarousel.CarouselSlides.Add(carouselSlideViewModel);
                }
                carouselsVm.Add(newCarousel);
            }

            return carouselsVm;
        }
    }
}
