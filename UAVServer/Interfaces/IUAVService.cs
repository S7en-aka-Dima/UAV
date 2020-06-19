using Modelling_Client.Models;

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows.Media;

namespace UAVServer
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IUAVService" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IServiceCallBack))]
    public interface IUAVService
    {
        /// <summary>
        /// Выполняет подключение к серверу и возвращает уникальнй номер клиента
        /// </summary>
        /// <returns>Возвращает целое число</returns>
        [OperationContract]
        int Connect(int id);

        /// <summary>
        /// Выполняет отключение от сервиса определённого клиента по уникальному номеру <paramref name="id"/>
        /// </summary>
        /// <param name="id">Уникальный номер клиента</param>
        [OperationContract(IsOneWay = true)]
        void Disconnect(int id);

        [Obsolete("Не рекомендуется использовать из-за возможной неполноты данных, дождитесь ответа сервера", true)]
        /// <summary>
        /// Передает итерации не пренадлежащие клиенту
        /// </summary>
        /// <param name="id">Уникальный номер клиента</param>
        /// <returns>возвращает список <list type="List<UAVBase>"/></returns>
        [OperationContract]
        List<List<UAVBase>> GetData(int id);

        [Obsolete("Не рекомендуется использовать из-за возможной неполноты данных, дождитесь ответа сервера", true)]
        /// <summary>
        /// Передает все данные последней итерации клиенту
        /// </summary>
        /// <returns>возвращает список <list type="List<UAVBase>"/></returns>
        [OperationContract]
        List<List<UAVBase>> GetAllData();

        /// <summary>
        /// Передаёт данные текущей итерации на сервер для обмена 
        /// </summary>
        /// <param name="uav">Расширенный класс содержищий основной класс UAVBase/>
        /// <param name="id"></param>
        [OperationContract(IsOneWay = true)]
        void SendValues(List<UAVBase> uav, int id);

        [OperationContract(IsOneWay = true)]
        void SendValues1(string n);

        /// <summary>
        /// сообщает серверу о готовности клиента к моделированию
        /// </summary>
        /// <param name="id">Уникальный номер</param>
        [OperationContract(IsOneWay = true)]
        void LetsStart(int id);

        /// <summary>
        /// Принудительный запуск всех клиентов
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void ForcedStart();

        /// <summary>
        /// Принудительная остановка моделирования всех пользователей
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void StopModeling();

        [OperationContract]
        Color GetColor(int CountUAVs = 0);
    }

    [ServiceContract]
    public interface IServiceCallBack
    {
        /// <summary>
        /// Возвращает клиенту список типа Dictionary<int, UAVBase>, где первый столбец - id клиента
        /// </summary>
        /// <param name="data"></param>
        [OperationContract(IsOneWay = true)]
        void SendValuesCallBack(Dictionary<int, List<UAVBase>> data);

        [OperationContract(IsOneWay = true)]
        void SendValuesCallBack1(string srt);

        [OperationContract(IsOneWay = true)]
        void GetStart();

        [OperationContract(IsOneWay = true)]
        void Stop();
    }
}
