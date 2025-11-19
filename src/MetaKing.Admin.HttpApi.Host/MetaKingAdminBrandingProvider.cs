using Microsoft.Extensions.Localization;
using MetaKing.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MetaKing.Admin;

[Dependency(ReplaceServices = true)]
public class MetaKingAdminBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MetaKingResource> _localizer;

    public MetaKingAdminBrandingProvider(IStringLocalizer<MetaKingResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["MetaKing"];
}
