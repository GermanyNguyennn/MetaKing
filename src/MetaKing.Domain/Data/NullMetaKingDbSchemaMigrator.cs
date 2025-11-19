using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MetaKing.Data;

/* This is used if database provider does't define
 * IMetaKingDbSchemaMigrator implementation.
 */
public class NullMetaKingDbSchemaMigrator : IMetaKingDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
