using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UAVServer
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "UAVService" в коде и файле конфигурации.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class UAVService : IUAVService
    {
        List<UAVUser> users = new List<UAVUser>();
        Dictionary<int, TransportClass> uavByUserID;

        int iterations = 1;
        private int nextID = 1;

        private async Task SaveIterationsAsync(Dictionary<int, TransportClass> data)
        {
            string message = await Task.Run(() => SaveIterations(data));

            Console.WriteLine($"{message}"); 
        }

        //[TODO] сделать переподключение с возвращением клиенту (возможно с новым id) истории всех итераций
        public int Connect(int id)
        {
            if (id == 0)
                id = nextID++;

            if (!users.Contains(users.Find(u => u.ID == id)))
            {
                UAVUser user = new UAVUser
                {
                    ID = id,
                    Ready = false,
                    operationContext = OperationContext.Current
                };

                Console.WriteLine($"{id} подключился");
                users.Add(user);
            }
            else
                Console.WriteLine($"{id} переподключился");
            

            return id;
        }

        public void Disconnect(int id)
        {
            if (id == 0) return;
            Console.WriteLine($"{users.Find(u => u.ID == id && u != null).ID} отключился");
            users.Remove(users.Find(u => u.ID == id && u != null));
        }

        private string SaveIterations(Dictionary<int, TransportClass> data)
        {
            string path = string.Empty;

            if (!Directory.Exists($@"{DateTime.Now.ToShortDateString()}"))
                iterations = 1;
                
            Directory.CreateDirectory($@"{DateTime.Now.ToShortDateString()}\{iterations}");
#if DEBUG
            Console.WriteLine($@"{DateTime.Now.ToShortDateString()}\{iterations}");
#endif
            foreach (var item in data)
                using (var file = new FileStream($@"{DateTime.Now.ToShortDateString()}\{iterations}\{item.Key}.xml", FileMode.Create))
                {
                    try
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(TransportClass));
                        xmlSerializer.Serialize(file, item.Value);
                    }
                    catch(Exception se)
                    {
                        return $"Ошибка соханения:\n{se.Message}";
                    }
                }

            path = $@"Успешное сохранение\nДанные доступны по этому пути: {DateTime.Now.ToShortDateString()}\{iterations++}";

            return path;
        }

        #region Функции для обеспечения моделирования
        public void ForcedStart()
        {
            foreach (var user in users)
                user.operationContext.GetCallbackChannel<IServiceCallBack>().GetStart();
        }

        public void LetsStart(int id)
        {
            if (id == 0) return;

            int isReady = 0;

            foreach (var user in users)
            {
                if (user.ID == id)
                    user.Ready = true;

                if (user.Ready == true)
                    isReady++;
                else return;
            }

            if(users.Count == isReady)
                foreach (var user in users)
                    user.operationContext.GetCallbackChannel<IServiceCallBack>().GetStart();
        }

        public void StopModeling()
        {
            foreach (var user in users)
                user.operationContext.GetCallbackChannel<IServiceCallBack>().Stop();
        }

        public List<TransportClass> GetData(int id)
        {
            if (id == 0) return null;

            List<TransportClass> allData = new List<TransportClass>();

            foreach (var item in uavByUserID)
                if (item.Key != id)
                    allData.Add(item.Value);

            return allData;
        }

        public List<TransportClass> GetAllData()
        {
            List<TransportClass> allData = new List<TransportClass>();

            foreach (var item in uavByUserID)
                allData.Add(item.Value);

            return allData;
        }

        public void SendValues(TransportClass uav, int id)
        {
            if (id == 0) return;


            if(uavByUserID == null)
                uavByUserID = new Dictionary<int, TransportClass>();

            if (uavByUserID.ContainsKey(id)) 
                uavByUserID[id] = uav;
            else
                uavByUserID.Add(id, uav);
#if DEBUG
            Console.WriteLine($"{uavByUserID.Count} {users.Count}");
#endif
            if (uavByUserID.Count == users.Count)
            {
                SaveIterationsAsync(uavByUserID);
#if DEBUG
                Console.WriteLine($"{uavByUserID.Count} {users.Count} in if");

                foreach (var user in users)
                {
                    Console.WriteLine($"{uavByUserID.Count} {users.Count} in foreach");
                    user.operationContext.GetCallbackChannel<IServiceCallBack>().SendValuesCallBack(uavByUserID);
                    //Console.WriteLine(uav.UAV.X.ToString() + " 1");
                }
#else
                foreach (var user in users)
                {
                    user.operationContext.GetCallbackChannel<IServiceCallBack>().SendValuesCallBack(uavByUserID);
                }
#endif

                uavByUserID.Clear();

                //[TODO]    вызвать функцию для асинхронной сериализации
                //          отчищать uavByUserID от данных во избежании постоянной сериализации
                //          создание маски для файлов для определения возраста документа
                //          Console.WriteLine("Сохранение данных!");
            }
        }

        public void SendValues1(string n)
        {
            foreach (var user in users)
            {
                Console.WriteLine("---");
                user.operationContext.GetCallbackChannel<IServiceCallBack>().SendValuesCallBack1(n);
                Console.WriteLine("***");
            }
        }
#endregion
    }
}
