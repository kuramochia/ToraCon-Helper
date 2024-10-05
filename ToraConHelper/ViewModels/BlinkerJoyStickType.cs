using System.ComponentModel;
using ToraConHelper.Views;

namespace ToraConHelper.ViewModels;

[TypeConverter(typeof(EnumDescriptionConverter))]
public enum BlinkerJoyStickType 
{
    [Description("左レバー(Button40,41)")]
    LeftStick,
    [Description("右レバー(Button46,47)")]
    RightStick
}
