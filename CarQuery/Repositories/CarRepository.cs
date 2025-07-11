﻿using System.Data;
using CarQuery.Data;
using CarQuery.Models;
using CarQuery.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CarQuery.Repositories
{
    public class CarRepository : ICarRepository
    {
        private string ServerPath { get; set; }
        private readonly AppDbContext _context;
        private readonly ICarouselRepository _carouselRepository;

        public CarRepository(IWebHostEnvironment system, AppDbContext context, ICarouselRepository carouselRepository)
        {
            ServerPath = system.WebRootPath;
            _context = context;
            _carouselRepository = carouselRepository;
        }

        public async Task<bool> AddCar(Car car)
        {
            await _context.Car.AddAsync(car);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Car> GetCarById(int carId)
        {
            Car car = await _context.Car
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.CarId == carId);

            return car;
        }

        public async Task<bool> UpdateCar(Car car)
        {
            if (car == null)
            {
                return false;
            }
            _context.Update(car);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCar(int carId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                Car car = await _context.Car
                    .Include(c => c.Images)
                    .FirstOrDefaultAsync(c => c.CarId == carId);

                if (car != null)
                {
                    foreach (var img in car.Images)
                    {
                        //exclui os CarouselSlides que usam as imagens do carro a ser deletado
                        await _carouselRepository.DeleteAllCarouselsSlidesByImage(img);
                    }

                    _context.Car.Remove(car);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    //a operação de deleção de imagens deve ser feito somente se as de banco de dados terem sido bem-sucedidas, pois não tem como recuperar as imagens caso façamos RollbackAsync
                    foreach (var img in car.Images)
                    {
                        string normalizedPath = (img.ImgPath).Replace("/", "\\");
                        string imgPath = ServerPath + normalizedPath;

                        if (System.IO.File.Exists(imgPath))
                        {
                            //deletando a imagem do servidor
                            System.IO.File.Delete(imgPath);
                        }
                    }
                    return true;
                }
                return false;
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

        public async Task<IEnumerable<Car>> SearchByModel(string model)
        {
            if (string.IsNullOrEmpty(model))
            {
                return Enumerable.Empty<Car>();
            }

            var car = await _context.Car.Include(c => c.Images)
            .Where(c => c.Model.StartsWith(model))
            .ToListAsync();

            return car;
        }

    }
}

