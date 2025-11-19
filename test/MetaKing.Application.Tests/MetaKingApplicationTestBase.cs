using Volo.Abp.Modularity;

namespace MetaKing;

public abstract class MetaKingApplicationTestBase<TStartupModule> : MetaKingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
