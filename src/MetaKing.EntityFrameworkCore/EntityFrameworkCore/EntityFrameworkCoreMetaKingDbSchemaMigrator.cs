using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MetaKing.Data;
using Volo.Abp.DependencyInjection;

namespace MetaKing.EntityFrameworkCore;

public class EntityFrameworkCoreMetaKingDbSchemaMigrator
    : IMetaKingDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreMetaKingDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the MetaKingDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<MetaKingDbContext>()
            .Database
            .MigrateAsync();
    }
}
