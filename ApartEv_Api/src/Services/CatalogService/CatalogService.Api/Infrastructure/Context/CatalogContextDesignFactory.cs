using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Infrastructure.Context
{
    public class CatalogContextDesignFactory : IDesignTimeDbContextFactory<CatalogContext>
    {
        public CatalogContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CatalogContext>()
                .UseSqlServer("Data Source=c_sqlserver;Initial Catalog=;Persist Security Info=True;User ID=;Password=");//TODO SQL bağlantı linki

            return new CatalogContext(optionsBuilder.Options);
        }
    }
}
