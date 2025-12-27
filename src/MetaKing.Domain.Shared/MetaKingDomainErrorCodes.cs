namespace MetaKing;

public static class MetaKingDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */
    public const string ProductNameAlreadyExists = "MetaKing:ProductNameAlreadyExists";
    public const string ProductCodeAlreadyExists = "MetaKing:ProductCodeAlreadyExists";

    public const string ProductIsNotExists = "MetaKing:ProductIsNotExists";
    public const string ProductAttributeIdIsNotExists = "MetaKing:ProductAttributeIdIsNotExists";

    public const string ProductAttributeValueIsNotValid = "MetaKing:ProductAttributeValueIsNotValid";

    public const string RoleNameAlreadyExists = "MetaKing:RoleNameAlreadyExists";
}
