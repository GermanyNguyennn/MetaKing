using MetaKing.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MetaKing.Admin.Permissions;

public class MetaKingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        //Catalog
        var catalogGroup = context.AddGroup(MetaKingPermissions.CatalogGroupName, L("Permission:Catalog"));

        //Manufacture
        var manufacturerPermission = catalogGroup.AddPermission(MetaKingPermissions.Manufacturer.Default, L("Permission:Catalog.Manufacturer"));
        manufacturerPermission.AddChild(MetaKingPermissions.Manufacturer.Create, L("Permission:Catalog.Manufacturer.Create"));
        manufacturerPermission.AddChild(MetaKingPermissions.Manufacturer.Update, L("Permission:Catalog.Manufacturer.Update"));
        manufacturerPermission.AddChild(MetaKingPermissions.Manufacturer.Delete, L("Permission:Catalog.Manufacturer.Delete"));

        //Product Category
        var productCategoryPermission = catalogGroup.AddPermission(MetaKingPermissions.ProductCategory.Default, L("Permission:Catalog.ProductCategory"));
        productCategoryPermission.AddChild(MetaKingPermissions.ProductCategory.Create, L("Permission:Catalog.ProductCategory.Create"));
        productCategoryPermission.AddChild(MetaKingPermissions.ProductCategory.Update, L("Permission:Catalog.ProductCategory.Update"));
        productCategoryPermission.AddChild(MetaKingPermissions.ProductCategory.Delete, L("Permission:Catalog.ProductCategory.Delete"));

        //Add product
        var productPermission = catalogGroup.AddPermission(MetaKingPermissions.Product.Default, L("Permission:Catalog.Product"));
        productPermission.AddChild(MetaKingPermissions.Product.Create, L("Permission:Catalog.Product.Create"));
        productPermission.AddChild(MetaKingPermissions.Product.Update, L("Permission:Catalog.Product.Update"));
        productPermission.AddChild(MetaKingPermissions.Product.Delete, L("Permission:Catalog.Product.Delete"));
        productPermission.AddChild(MetaKingPermissions.Product.AttributeManage, L("Permission:Catalog.Product.AttributeManage"));

        //Add attribute
        var attributePermission = catalogGroup.AddPermission(MetaKingPermissions.Attribute.Default, L("Permission:Catalog.Attribute"));
        attributePermission.AddChild(MetaKingPermissions.Attribute.Create, L("Permission:Catalog.Attribute.Create"));
        attributePermission.AddChild(MetaKingPermissions.Attribute.Update, L("Permission:Catalog.Attribute.Update"));
        attributePermission.AddChild(MetaKingPermissions.Attribute.Delete, L("Permission:Catalog.Attribute.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MetaKingResource>(name);
    }
}
