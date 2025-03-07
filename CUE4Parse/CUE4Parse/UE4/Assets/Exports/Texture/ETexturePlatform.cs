using System.ComponentModel;

namespace CUE4Parse.UE4.Assets.Exports.Texture
{
    public enum ETexturePlatform
    {
        [Description("桌面版/移动版")]
        DesktopMobile,
        [Description("微软Xbox One系列/索尼PS4/5")]
        XboxAndPlaystation,
        [Description("任天堂Switch")]
        NintendoSwitch
    }
}
