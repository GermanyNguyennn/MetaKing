using Volo.Abp.Modularity;

namespace MetaKing.Admin;

public abstract class MetaKingApplicationTestBase<TStartupModule> : MetaKingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
