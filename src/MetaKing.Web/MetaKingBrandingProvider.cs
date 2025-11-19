using Microsoft.Extensions.Localization;
using MetaKing.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MetaKing;

[Dependency(ReplaceServices = true)]
public class MetaKingBrandingProvider : DefaultBrandingProvider
{
    //private IStringLocalizer<MetaKingResource> _localizer;

    //public MetaKingBrandingProvider(IStringLocalizer<MetaKingResource> localizer)
    //{
    //    _localizer = localizer;
    //}

    //public override string AppName => _localizer["AppName"];

    public override string AppName => "MetaKing";
}
