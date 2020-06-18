using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelling_Client.Models.Перечисления
{
    public enum UAVsWorkMode : ushort
    {
        [Display(Name = "Сыграть в ленивеца")]
        None,
        [Display(Name = "Добавить")]
        Add,
        [Display(Name = "Удалить")]
        Delete,
        [Display(Name = "Сохранить")]
        Serialize,
        [Display(Name = "Загрузить")]
        Deserialize
    }
}
