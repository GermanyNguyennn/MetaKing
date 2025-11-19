using MetaKing.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MetaKing.Admin.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MetaKingAdminController : AbpControllerBase
{
    protected MetaKingAdminController()
    {
        LocalizationResource = typeof(MetaKingResource);
    }
}
