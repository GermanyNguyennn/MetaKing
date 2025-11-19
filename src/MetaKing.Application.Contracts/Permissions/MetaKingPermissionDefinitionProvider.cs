using MetaKing.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace MetaKing.Permissions;

public class MetaKingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MetaKingPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(MetaKingPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MetaKingResource>(name);
    }
}
