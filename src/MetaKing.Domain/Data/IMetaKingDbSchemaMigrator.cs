using System.Threading.Tasks;

namespace MetaKing.Data;

public interface IMetaKingDbSchemaMigrator
{
    Task MigrateAsync();
}
