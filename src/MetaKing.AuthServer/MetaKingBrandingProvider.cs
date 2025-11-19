using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace MetaKing;

[Dependency(ReplaceServices = true)]
public class MetaKingBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "MetaKing";
}
