using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopTARge24.Data;

namespace ShopTARge24.KindergartenTest
{
    public abstract class TestBase : IDisposable
    {
        protected readonly ServiceProvider Provider;
        protected readonly ShopTARge24Context DbContext;

        protected TestBase()
        {
            var services = new ServiceCollection();

            // InMemory versioon ShopTARge24Contextist
            services.AddDbContext<ShopTARge24Context>(options =>
                options.UseInMemoryDatabase($"ShopTARge24TestDb_{Guid.NewGuid()}"));

            Provider = services.BuildServiceProvider();
            DbContext = Provider.GetRequiredService<ShopTARge24Context>();

            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
            Provider.Dispose();
        }
    }
}
