using MetaKing.Localization;
using Volo.Abp.Application.Services;

namespace MetaKing.Admin;

/* Inherit your application services from this class.
 */
public abstract class MetaKingAdminAppService : ApplicationService
{
    protected MetaKingAdminAppService()
    {
        LocalizationResource = typeof(MetaKingResource);
    }
}