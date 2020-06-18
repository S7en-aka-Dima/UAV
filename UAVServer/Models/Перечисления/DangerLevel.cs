using System.ComponentModel;

namespace Modelling_Client.Models.Перечисления
{
    public enum DangerLevel : byte
    {
        [Description("Безопасный")]
        SafeLevel = 0,
        [Description("Столкновение")]
        Crash
    }
}
