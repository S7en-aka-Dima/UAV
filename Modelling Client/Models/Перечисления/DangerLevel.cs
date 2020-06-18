using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
