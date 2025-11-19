using Xunit;

namespace MetaKing.EntityFrameworkCore;

[CollectionDefinition(MetaKingTestConsts.CollectionDefinitionName)]
public class MetaKingEntityFrameworkCoreCollection : ICollectionFixture<MetaKingEntityFrameworkCoreFixture>
{

}
