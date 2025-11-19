using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MetaKing.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class MetaKingDbContextFactory : IDesignTimeDbContextFactory<MetaKingDbContext>
{
    public MetaKingDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        MetaKingEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<MetaKingDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new MetaKingDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../MetaKing.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
