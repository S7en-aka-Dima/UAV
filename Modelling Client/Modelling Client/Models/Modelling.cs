using Microsoft.Win32;

using Modelling_Client.Models.Перечисления;
using Modelling_Client.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Modelling_Client.Models
{
    public class Modelling : BaseViewModel
    {
        private string path;

        private int numberOfIterations;

        private bool
            isMultipleuser = false,
            simulate = false;

        private ObservableCollection<Iteration> iterations = new ObservableCollection<Iteration>();
        private ObservableCollection<UAVBase> uavs = new ObservableCollection<UAVBase>
        {
            new UAVBase(new Models.UAVSettings{ID = 1, Radius = 10, Speed = 5}, new ObservableCollection<RouteSegment>{ new RouteSegment(1111.111, 1234.202, 133.002, 222.369, 2148.551, 2), new RouteSegment(1111.111, 1234.202, 133.002, 222.369, 2148.551, 2) }),
            new UAVBase(new Models.UAVSettings{ID = 2, Radius = 10, Speed = 5}, new ObservableCollection<RouteSegment>{ new RouteSegment(1111.111, 1234.202, 133.002, 222.369, 2148.551, 2), new RouteSegment(1111.111, 1234.202, 133.002, 222.369, 2148.551, 2) })
        };

        public Modelling()
        {
            path = $@"Сохранённые настройки\{DateTime.UtcNow.ToString("ddd-MM-YYYY HH-mm-ss")} {uavs.Count}UAVs";
            if (isMultipleuser) path += $" MULTY ";
            else path += $" SOLO ";
        }

        public string Path
        {
            get => path;
            set { path = value; }
        }
        public bool Simulate
        {
            get => simulate;
            set
            {
                simulate = value;
                OnPropertyChanged();
            }
        }
        public bool EnabledToChangeUAVs => !simulate;
        public bool IsMultipleuser
        {
            get => isMultipleuser;
            set
            {
                isMultipleuser = value;
                OnPropertyChanged();
            }
        }
        public int NumberOfIterations
        {
            get => numberOfIterations;
            set { numberOfIterations = value; OnPropertyChanged(); }
        }
        public ObservableCollection<UAVBase> AllUAVBases
        {
            get => uavs;
            set { uavs = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Iteration> Iterations
        {
            get => iterations;
            set { iterations = value; OnPropertyChanged(); }
        }

        public void Test()
        {
            MessageBox.Show("!");
        }
        public void OpenSettingsFile()
        {
            string filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
                SerializeOrDeserialize(openFileDialog.FileName, UAVsWorkMode.Deserialize);
        }
        /*
         * Создать функцию для сохранения файла (savefiledialog класс для сохранения) которая будет вызываться двумя кнопками и срабатывать по-разному
         *  назваит аналогично открытию файла: SaveSettingsIntoFile
         *  для сохранения использовать фенкцию SerializeOrDeserialize
         *  
         * Сделать проверку на деррикторию для создания пути по умолчанию (путь по умолчанию можно писать так: "... \<название папки>\"
         *  для удобства можно перед строкой писать @, например, @"\<папка/файл>", без собачки так: "\\<папка/файл>"
         *  
         * Если путь не указан, сохранять в деррикторию по умолчанию
         */

        private void SerializeOrDeserialize(string filePath, UAVsWorkMode mode)
        {
            if (filePath == string.Empty) return;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<UAVBase>));

            switch (mode)
            {
                case UAVsWorkMode.Deserialize:
                    using (var file = new FileStream(filePath, FileMode.Open))
                        uavs = xmlSerializer.Deserialize(file) as ObservableCollection<UAVBase>;

                    break;

                case UAVsWorkMode.Serialize:
                    using (var file = new FileStream(filePath, FileMode.Create))
                        xmlSerializer.Serialize(file, uavs);

                    break;
            }
        }
    }
}
