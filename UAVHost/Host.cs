using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Threading;

namespace UAVHost
{
    class Host
    {
        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("***Debug version***");
#else
            Console.Clear();

            const string stop = "stop";
#endif
            Console.WriteLine(
                $"Запуск сервера..." +
                $"\nВсе файлы тут: {Path.GetDirectoryName(Path.GetFullPath("UAVHost.exe"))}");

            using (var host = new ServiceHost(typeof(UAVServer.UAVService)))
            {
                try
                {
                    host.Open(TimeSpan.FromSeconds(10));
                    Console.WriteLine("Служба запущена!");
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine($"Обнаружена ошибка: {ex.Message}");
                    host.Abort();
                    Console.WriteLine(
                        "<---------->" +
                        "\nСлужба выключена!" +
                        "\n<---------->" +
                        "\nДля выхода дажмите любой клавишу");
                    Console.ReadKey();
                }
#if !DEBUG
                Console.WriteLine($"Введите '{stop}' для завершения работы");

                while (stop != Console.ReadLine().ToLower()) { Console.WriteLine("Введите '{stop}' для завершения работы"); }

                Console.WriteLine("Завершение работ!");
#else
                Console.WriteLine("Для выхода дажмите любой клавишу");
                Console.ReadKey();
#endif
                host.Close();
            }
        }
    }

    public class Rezult : IAsyncResult
    {
        public bool IsCompleted => throw new NotImplementedException();

        public WaitHandle AsyncWaitHandle => throw new NotImplementedException();

        public object AsyncState => throw new NotImplementedException();

        public bool CompletedSynchronously => throw new NotImplementedException();
    }
}
