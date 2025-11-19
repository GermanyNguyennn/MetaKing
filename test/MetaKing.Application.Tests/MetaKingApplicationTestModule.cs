using MetaKing;
using Volo.Abp.Modularity;

namespace MetaKing;

[DependsOn(
    typeof(MetaKingPublicApplicationModule),
    typeof(MetaKingDomainTestModule)
)]
public class MetaKingApplicationTestModule : AbpModule
{

}
