using Volo.Abp.Modularity;

namespace MetaKing.Admin;

[DependsOn(
    typeof(MetaKingAdminApplicationModule),
    typeof(MetaKingDomainTestModule)
)]
public class MetaKingApplicationTestModule : AbpModule
{

}
