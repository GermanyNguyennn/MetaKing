using MetaKing.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MetaKing.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MetaKingController : AbpControllerBase
{
    protected MetaKingController()
    {
        LocalizationResource = typeof(MetaKingResource);
    }
}
