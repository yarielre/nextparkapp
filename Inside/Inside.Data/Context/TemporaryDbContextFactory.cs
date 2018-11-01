using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Inside.Data.Context
{
    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<InsideContext>
    {
        public IConfiguration Configuration { get; set; }
        public InsideContext CreateDbContext(string[] args)
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddJsonFile("appsettings.json");
            Configuration = configurationBuilder.Build();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            var builder = new DbContextOptionsBuilder<InsideContext>();
            builder.UseSqlServer(
                connectionString,
                optionsBuilder =>
                    optionsBuilder.MigrationsAssembly(typeof(InsideContext).GetTypeInfo().Assembly.GetName().Name));
            return new InsideContext(builder.Options);
        }
    }
}
//Online: "Data Source=149.202.41.48,1433;Initial Catalog=insidedevdb;User Id=sa;Password=Wisegar.1;TrustServerCertificate=True;"