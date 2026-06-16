using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using DroneRental.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DroneRental.Api.Tests.TestInfrastructure
{
    public  class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "this-is-a-test-secret-key-with-enough-length",
                    ["Jwt:Issuer"] = "DroneRental.Tests",
                    ["Jwt:Audience"] = "DroneRental.Tests",
                    ["Jwt:ExpiresInMinutes"] = "120"
                });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<DroneRentalDbContext>>();
                services.RemoveAll<DroneRentalDbContext>();

                
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                services.AddSingleton<DbConnection>(connection);
                

                services.AddDbContext<DroneRentalDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });
                var serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DroneRentalDbContext>();
                dbContext.Database.EnsureCreated();
            });
                
            
        }
    }
}
