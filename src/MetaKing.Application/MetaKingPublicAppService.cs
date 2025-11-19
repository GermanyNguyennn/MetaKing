using MetaKing.Localization;
using Volo.Abp.Application.Services;

namespace MetaKing;

/* Inherit your application services from this class.
 */
public abstract class MetaKingPublicAppService : ApplicationService
{
    protected MetaKingPublicAppService()
    {
        LocalizationResource = typeof(MetaKingResource);
    }
}
