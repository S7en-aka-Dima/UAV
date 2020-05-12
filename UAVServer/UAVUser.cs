using System.ServiceModel;

namespace UAVServer
{
    public class UAVUser
    {
        /// <summary>
        /// Задаёт/возвращает готовность клиента к моделированию
        /// </summary>
        public bool Ready { get; set; }

        /// <summary>
        /// Задаёт/возвращает уникальный номер клиента на время сессии
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Задаётся автоматически и позволяет работать с серисом путём обращения к экземпляру этого класса
        /// </summary>
        public OperationContext operationContext { get; set; }
    }
}
