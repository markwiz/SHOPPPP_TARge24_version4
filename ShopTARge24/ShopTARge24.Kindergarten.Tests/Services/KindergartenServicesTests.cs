using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShopTARge24.ApplicationServices.Services;
using ShopTARge24.Core.Dto;
using ShopTARge24.Data;
using Xunit;

namespace ShopTARge24.Tests.Kindergarten 
{
    public class KindergartenServicesTests
    {
        private static ShopTARge24Context CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ShopTARge24Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ShopTARge24Context(options);
        }

        // 1) CREATE - Id tühi -> genereerib uue Guid'i ja teeb oige teksti
        [Fact]
        public async Task Create_GeneratesNewGuid_WhenIdIsEmpty()
        {
            using var context = CreateInMemoryContext();
            var service = new KindergartenServices(context);

            var dto = new KindergartenDto
            {
                Id = Guid.Empty,
                GroupName = "  orparid  ",
                ChildrenCount = 33,
                KindergartenName = "  sipsik  ",
                TeacherName = "  ulvi  "
            };

            var result = await service.Create(dto);

            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("orparid", result.GroupName);
            Assert.Equal("sipsik", result.KindergartenName);
            Assert.Equal("ulvi", result.TeacherName);
            Assert.Equal(33, result.ChildrenCount);
        }

        // 2) CREATE - kui Id on antud, kasutab seda
        [Fact]
        public async Task Create_UsesProvidedId()
        {
            using var context = CreateInMemoryContext();
            var service = new KindergartenServices(context);

            var givenId = Guid.NewGuid();

            var dto = new KindergartenDto
            {
                Id = givenId,
                GroupName = "orparid",
                ChildrenCount = 32,
                KindergartenName = "sipsik",
                TeacherName = "ulvi"
            };

            var result = await service.Create(dto);

            Assert.NotNull(result);
            Assert.Equal(givenId, result.Id);
        }

        // 3) UPDATE - olemasoleva entiteedi uuendamine
        [Fact]
        public async Task Update_UpdatesExistingEntity()
        {
            using var context = CreateInMemoryContext();

            var id = Guid.NewGuid();

            context.Kindergarten.Add(new ShopTARge24.Core.Domain.Kindergarten
            {
                Id = id,
                GroupName = "vana",
                ChildrenCount = 10,
                KindergartenName = "vana kg",
                TeacherName = "vana õpetaja",
                CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-5)
            });

            await context.SaveChangesAsync();

            var service = new KindergartenServices(context);

            var dto = new KindergartenDto
            {
                Id = id,
                GroupName = "  uus grupp  ",
                ChildrenCount = 20,
                KindergartenName = "  uus kg  ",
                TeacherName = "  uus õpetaja  "
            };

            var updated = await service.Update(dto);

            Assert.NotNull(updated);
            Assert.Equal("uus grupp", updated.GroupName);
            Assert.Equal(20, updated.ChildrenCount);
            Assert.Equal("uus kg", updated.KindergartenName);
            Assert.Equal("uus õpetaja", updated.TeacherName);
        }

        // 4) UPDATE - kui ei ole olemas, tagastab null
        [Fact]
        public async Task Update_ReturnsNull_WhenEntityNotFound()
        {
            using var context = CreateInMemoryContext();
            var service = new KindergartenServices(context);

            var dto = new KindergartenDto
            {
                Id = Guid.NewGuid(),
                GroupName = "midagi",
                ChildrenCount = 1,
                KindergartenName = "x",
                TeacherName = "y"
            };

            var result = await service.Update(dto);

            Assert.Null(result);
        }

        // 5) DELETE - kustutab lasteaia ja seotud failid
        [Fact]
        public async Task Delete_RemovesKindergartenAndItsFiles()
        {
            using var context = CreateInMemoryContext();

            var id = Guid.NewGuid();

            // Seedime lasteaia
            context.Kindergarten.Add(new ShopTARge24.Core.Domain.Kindergarten
            {
                Id = id,
                GroupName = "orparid",
                ChildrenCount = 33,
                KindergartenName = "sipsik",
                TeacherName = "ulvi",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

           
            context.FileToApis.Add(new ShopTARge24.Core.Domain.FileToApi
            {
                Id = Guid.NewGuid(),
                KindergartenId = id,
                ImageTitle = "test image",
                ImageData = new byte[] { 1, 2, 3 }  
            });

            await context.SaveChangesAsync();

            var service = new KindergartenServices(context);

            // Act
            var deleted = await service.Delete(id);

            // Assert
            Assert.NotNull(deleted);
            Assert.False(context.Kindergarten.Any(k => k.Id == id));
            Assert.False(context.FileToApis.Any(f => f.KindergartenId == id));
        }


        // 6) DETAIL - tagastab õige kirje
        [Fact]
        public async Task DetailAsync_ReturnsCorrectEntity()
        {
            using var context = CreateInMemoryContext();

            var id = Guid.NewGuid();

            context.Kindergarten.Add(new ShopTARge24.Core.Domain.Kindergarten
            {
                Id = id,
                GroupName = "orparid",
                ChildrenCount = 32,
                KindergartenName = "sipsik",
                TeacherName = "ulvi",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var service = new KindergartenServices(context);

            var result = await service.DetailAsync(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("orparid", result.GroupName);
            Assert.Equal(32, result.ChildrenCount);
            Assert.Equal("sipsik", result.KindergartenName);
            Assert.Equal("ulvi", result.TeacherName);
        }
    }
}
