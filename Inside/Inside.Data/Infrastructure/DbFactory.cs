using System;
using System.Collections.Generic;
using System.IO;
using Inside.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Inside.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        private InsideContext _dbContext;
        public IConfiguration Configuration { get; set; }

        public InsideContext Init()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddJsonFile("appsettings.json");
            Configuration = configurationBuilder.Build();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            if (_dbContext != null) return _dbContext;
            var dbContextBuilder = new DbContextOptionsBuilder();
            dbContextBuilder.UseSqlServer(connectionString);
            _dbContext = new InsideContext(dbContextBuilder.Options);
            return _dbContext;
        }

        protected override void DisposeCore()
        {
            _dbContext?.Dispose();
        }
    }
}
//Online: "Data Source=149.202.41.48,1433;Initial Catalog=insidedevdb;User Id=sa;Password=Wisegar.1;TrustServerCertificate=True;"