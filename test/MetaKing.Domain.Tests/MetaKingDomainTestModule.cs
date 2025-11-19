using Volo.Abp.Modularity;

namespace MetaKing;

[DependsOn(
    typeof(MetaKingDomainModule),
    typeof(MetaKingTestBaseModule)
)]
public class MetaKingDomainTestModule : AbpModule
{

}
