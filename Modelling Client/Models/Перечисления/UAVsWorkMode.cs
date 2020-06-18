using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelling_Client.Models.Перечисления
{
    public enum UAVsWorkMode : ushort
    {
        [Description("Сыграть в ленивеца")]
        None,
        [Description("Добавить")]
        Add,
        [Description("Удалить")]
        Delete,
        [Description("Сохранить")]
        Serialize,
        [Description("Загрузить")]
        Deserialize,
        [Description("Закгрузить в Excel")]
        SaveIntoExcel
    }
}
