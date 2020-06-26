using Modelling_Client.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace UAVServer
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "UAVService" в коде и файле конфигурации.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class UAVService : IUAVService
    {
        List<UAVUser> users = new List<UAVUser>();
        Dictionary<int, List<UAVBase>> uavByUserID;

        int
            iterations = 1,
            modellings = 0;
        private int nextID = 1;


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

                Console.WriteLine($"{user.ID} подключился");
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
        public Color GetColor(int CountUAVs = 0)
        {
            int step = CountUAVs >= 12 ? 5 : 10;

            Random random = new Random();
            var color = new Color();

            color.A = Convert.ToByte(random.Next(CountUAVs * step, 200));
            color.R = Convert.ToByte(random.Next(CountUAVs * step, 255));
            color.G = Convert.ToByte(random.Next(CountUAVs * step, 255));
            color.B = Convert.ToByte(random.Next(CountUAVs * step, 255));

            return color;
        }

        private async Task SaveIterationsAsync(Dictionary<int, List<UAVBase>> data)
        {
            string message = await Task.Run(() => SaveIterations(data));

            Console.WriteLine($"{message}");
        }
        private async Task<Dictionary<int, List<UAVBase>>> GetOldIterationAsync(DateTime dateTimeModelling, int modellingNum, int iterationNum)
        {
            return await Task<Dictionary<int, List<UAVBase>>>.Run(() => GetOldIteration(dateTimeModelling, modellingNum, iterationNum));
        }
        private string SaveIterations(Dictionary<int, List<UAVBase>> data)
        {
            string path = string.Empty;

            if (!Directory.Exists($@"{DateTime.Now.ToShortDateString()}"))
            {
                iterations = 1;

                Directory.CreateDirectory($@"{DateTime.Now.ToShortDateString()}\{modellings}");
            }
#if DEBUG
            Console.WriteLine($@"{DateTime.Now.ToShortDateString()}\{iterations}");
#endif
            //foreach (var item in data)
                using (var file = new FileStream($@"{DateTime.Now.ToShortDateString()}\{modellings}\{iterations}.xml", FileMode.Create))
                {
                    try
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Dictionary<int, List<UAVBase>>));
                        xmlSerializer.Serialize(file, data);
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
            uavByUserID.Clear();

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

            if (users.Count == isReady)
            {
                ForcedStart();

                modellings++;
            }
        }
        public void StopModeling()
        {
            foreach (var user in users)
                user.operationContext.GetCallbackChannel<IServiceCallBack>().Stop();
        }

        public bool CanRepeatModelling(DateTime dateTimeModelling, int modellingNum, int iterationNum)
        {
            uavByUserID.Clear();
            GetOldIteration($@"{dateTimeModelling.ToShortDateString()}\{modellingNum}\{iterationNum}.xml");

            return uavByUserID != null && users.Count >= uavByUserID.Keys.Count;
        }
        public void RepeatModelling(DateTime dateTimeModelling, int modellingNum, int iterationNum)
        {
            if (!CanRepeatModelling(dateTimeModelling, modellingNum, iterationNum))
                return;

            for (int i = 0; i < users.Count; i++)
                users[i].operationContext.GetCallbackChannel<IServiceCallBack>().RepeatOldModelling(uavByUserID[i]);
        }
        public Dictionary<int, List<UAVBase>> GetOldIteration(DateTime dateTimeModelling, int modellingNum, int iterationNum)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Dictionary<int, List<UAVBase>>));
            try
            {
                using (var file = new FileStream($@"{dateTimeModelling.ToShortDateString()}\{modellingNum}\{iterationNum}.xml", FileMode.Open))
                {
                    uavByUserID = xmlSerializer.Deserialize(file) as Dictionary<int, List<UAVBase>>;
                }
            }
            catch (FileNotFoundException fnfe)
            {
                Console.WriteLine(
                    $"{fnfe.Message}\n" +
                    $"Файл {fnfe.FileName} не найден, невозможная передать данные!");

                uavByUserID = null;
            }

            return uavByUserID;
        }
        public void GetOldIteration(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Dictionary<int, List<UAVBase>>));
            try
            {
                using (var file = new FileStream($@"{path}", FileMode.Open))
                {
                    uavByUserID = xmlSerializer.Deserialize(file) as Dictionary<int, List<UAVBase>>;
                }
            }
            catch (FileNotFoundException fnfe)
            {
                Console.WriteLine(
                    $"{fnfe.Message}\n" +
                    $"Файл {fnfe.FileName} не найден, невозможная передать данные!");

                uavByUserID = null;
            }
        }

        public List<List<UAVBase>> GetData(int id)
        {
            if (id == 0) return null;

            List<List<UAVBase>> allData = new List<List<UAVBase>>();

            foreach (var item in uavByUserID)
                if (item.Key != id)
                    allData.Add(item.Value);

            return allData;
        }
        public List<List<UAVBase>> GetAllData()
        {
            List<List<UAVBase>> allData = new List<List<UAVBase>>();

            foreach (var item in uavByUserID)
                allData.Add(item.Value);

            return allData;
        }

        public void SendValues(List<UAVBase> uav, int id)
        {
            if (id == 0) return;

            if(uavByUserID == null)
                uavByUserID = new Dictionary<int, List<UAVBase>>();

            if (uavByUserID.ContainsKey(id)) 
                uavByUserID[id] = uav;
            else
                uavByUserID.Add(id, uav);

            if (uavByUserID.Count == users.Count)
            {
                SaveIterationsAsync(uavByUserID);
#if DEBUG
                Console.WriteLine($"{uavByUserID.Count} {users.Count}");
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
