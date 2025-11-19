using MetaKing.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace MetaKing.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MetaKingEntityFrameworkCoreModule),
    typeof(MetaKingApplicationContractsModule)
)]
public class MetaKingDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
