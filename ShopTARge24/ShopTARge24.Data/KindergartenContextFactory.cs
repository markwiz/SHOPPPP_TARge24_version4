using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ShopTARge24.Data
{
    public class KindergartenContextFactory : IDesignTimeDbContextFactory<KindergartenContext>
    {
        public KindergartenContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<KindergartenContext>();

            
            optionsBuilder.UseSqlServer(
                "Server=10.11.15.206,1433;Database=ShopTARge24DevClean;User Id=sa;Password=Str0ng!Passw0rd;Encrypt=True;TrustServerCertificate=True;");

            return new KindergartenContext(optionsBuilder.Options);
        }
    }
}
