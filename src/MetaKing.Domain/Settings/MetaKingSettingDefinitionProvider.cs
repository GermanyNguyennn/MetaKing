using Volo.Abp.Settings;

namespace MetaKing.Settings;

public class MetaKingSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MetaKingSettings.MySetting1));
    }
}
