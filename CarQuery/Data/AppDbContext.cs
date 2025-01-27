using CarQuery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarQuery.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        }

        public DbSet<Car> Car {  get; set; }
        public DbSet<Carousel> Carousel { get; set; }
        public DbSet<CarouselSlide> CarouselSlide { get; set; }
        public DbSet<Image> Image { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Relação de Carousel com CarouselSlide
            modelBuilder.Entity<CarouselSlide>()
                .HasOne(cs => cs.Carousel)
                .WithMany(c => c.CarouselSlides)
                .HasForeignKey(cs => cs.CarouselId)
                .OnDelete(DeleteBehavior.Cascade); //Excluir o carousel exclui todos os CarouselSlide relacionados

            //Relação de CarouselSlide com Image
            modelBuilder.Entity<CarouselSlide>()
                .HasOne(cs => cs.Image)
                .WithMany()
                .HasForeignKey(cs => cs.ImageId)
                .OnDelete(DeleteBehavior.Restrict); //Ao excluir CarouselSlide, o Image não será excluído

            //Relação de Image com Car
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Car)
                .WithMany(c => c.Images)
                .HasForeignKey(i => i.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            //Relação CarouselSlide e Car
            modelBuilder.Entity<CarouselSlide>()
                .HasOne(cs => cs.Car)
                .WithMany()
                .HasForeignKey(cs => cs.CarId)
                .OnDelete(DeleteBehavior.Restrict); //Não exclui o Car ao excuir o CarouselSlide
        }
    }
}
