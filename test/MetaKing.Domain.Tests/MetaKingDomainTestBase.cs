using Volo.Abp.Modularity;

namespace MetaKing;

/* Inherit from this class for your domain layer tests. */
public abstract class MetaKingDomainTestBase<TStartupModule> : MetaKingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
