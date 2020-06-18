﻿using DevExpress.Mvvm.POCO;

using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using Modelling_Client.Models.Перечисления;
using Modelling_Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Excel = Microsoft.Office.Interop.Excel;

namespace Modelling_Client.Models
{
    public class Modelling : BaseViewModel
    {
        private string path;

        private int
            numberOfIterations,
            thisClientID;

        private bool
            isMultipleuser = false,
            simulate = false;

        private ObservableCollection<Iteration> iterations = new ObservableCollection<Iteration>();
        private ObservableCollection<UAVBase> uavs = new ObservableCollection<UAVBase>
        {
            new UAVBase(
                new UAVSettings{ID = 1, Radius = 10, Speed = 5, X = 1111.111, Y = 1234.202, Z = 133.002},
                new ObservableCollection<RouteSegment>
                {
                    new RouteSegment(1111.111, 1234.202, 133.002, 222.369, 2148.551, 2),
                    new RouteSegment(1111.111, 1234.202, 133.002, 222.369, 2148.551, 2)
                }),
            new UAVBase(
                new UAVSettings{ID = 2, Radius = 10, Speed = 5, X = 1111.111, Y = 1234.202, Z = 133.002},
                new ObservableCollection<RouteSegment>
                {
                    new RouteSegment(1111.111, 1234.202, 133.002, 222.369, 2148.551, 2),
                    new RouteSegment(1111.111, 1234.202, 133.002, 222.369, 2148.551, 2)
                })
        };

        public Modelling()
        {
            path = $@"{DateTime.UtcNow.ToString("dd(ddd)-MM-yyyy HH-mm-ss")}";
            uavs[0].DangerID = 1;
            uavs[0].DangerClientID = 32;
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

                if (value)
                {
                    Random rdm = new Random();

                    for (int i = 0; i < rdm.Next(50); i++)
                        iterations.Add(new Iteration(uavs));
                }

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
        public int ThisClientID
        {
            get => thisClientID;
            set { thisClientID = value; OnPropertyChanged(); }
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
        public void SaveSettingsIntoFile(bool FlagSave)
        {
            // Проверка директории directory. Если нет, то создать
            Directory.CreateDirectory($@"Сохранённые настройки\");
            string fileName = $@"Сохранённые настройки\{path} {uavs.Count}-UAVs";

            if (isMultipleuser) fileName += $" MULTY";
            else fileName += $" SOLO";

            fileName += ".xml";

            // обработчик с какой кнопки
            if (FlagSave)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = path;
                saveFileDialog.Filter = "Файл сохранения моделирования XML (*.xml)|*.xml";
                if ((bool)saveFileDialog.ShowDialog())
                {
                    SerializeOrDeserialize(saveFileDialog.FileName, UAVsWorkMode.Serialize);
                }
            }
            else
            {
                SerializeOrDeserialize(fileName, UAVsWorkMode.Serialize);
            }
        }
        public void SaveIntoExcel()
        {
            SerializeOrDeserialize($@"{Environment.CurrentDirectory}\Сохранённые настройки\Все моделирования.xlsx", UAVsWorkMode.SaveIntoExcel);

            GC.Collect();
        }

        private void SerializeOrDeserialize(string filePath, UAVsWorkMode mode)
        {
            if (filePath == string.Empty) return;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ObservableCollection<UAVBase>));

            switch (mode)
            {
                case UAVsWorkMode.Deserialize:
                    using (var file = new FileStream(filePath, FileMode.Open))
                    {
                        ObservableCollection<UAVBase> data = xmlSerializer.Deserialize(file) as ObservableCollection<UAVBase>;

                        foreach (var item in data)
                        {
                            item.Route.RemoveAt(0);

                            item.CanChange = item.ClientID == thisClientID;
                        }

                        uavs = data;

                        OnPropertyChanged("AllUAVBases");
                    }

                    break;

                case UAVsWorkMode.Serialize:
                    using (var file = new FileStream(filePath, FileMode.Create))
                        xmlSerializer.Serialize(file, uavs);

                    break;

                /*
                 * НЕ ТРОГАТЬ ЕСЛИ ПЛОХО ЗНАЕШЬ БИБЛИОТЕКУ EXCEL!!!!!
                 * 
                 * Происходит:
                 *  1)  проверка на существование файла - открытие файла
                 *  2)  проверка запущен ли excel - запуск приложения
                 */
                case UAVsWorkMode.SaveIntoExcel:
                    SaveIntoExcelAsync(filePath);
                    break;
            }

            GC.Collect();
        }

        private async Task SaveIntoExcelAsync(string filePath)
        {
            await Task.Run(() => SaveIntoExcel(filePath));
        }
        private void SaveIntoExcel(string filePath)
        {
            Excel.Application excel;
            Workbook workbook;
            Worksheet worksheet;

            //  Проверка запущен ли Excel и запуск приложения
            try
            {
                excel = (Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
            }
            catch
            {
                excel = new Excel.Application();
            }

            //  Проверка наличия файла и добавление новой страницы
            try
            {
                workbook = excel.Workbooks.get_Item("Все моделирования.xlsx");
                worksheet = (Worksheet)workbook.Worksheets.Add(After: workbook.Worksheets[workbook.Worksheets.Count]);
            }
            catch
            {
                //  Открытие закрытого файла по пути или создание новой книги и добавление новой страницы
                try
                {
                    workbook = excel.Workbooks.Open(filePath, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", true, false, 0, true, 1, 0);
                    worksheet = (Worksheet)workbook.Worksheets.Add(After: workbook.Worksheets[workbook.Worksheets.Count]);
                }
                catch
                {
                    workbook = excel.Workbooks.Add();
                    worksheet = (Worksheet)workbook.ActiveSheet;
                    //  сохранение книги по указанному пути
                    workbook.SaveAs(filePath);
                }
            }

            worksheet.Name = DateTime.UtcNow.ToString("dd(ddd) HH-mm-ss");

            char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            int
                numClient = 0,
                i = 0,  //  номер толбца
                j = 0,  //  номер строки
                curClientID = thisClientID;

            Dictionary<int, int> clientwithnum = new Dictionary<int, int>();
            clientwithnum.Add(numClient, curClientID);

            #region Заполнение файла
            foreach (var uav in uavs)
            {
                //  отрисовка следующего бпла того же клиента
                if (curClientID == uav.ClientID)
                {
                    if (numClient == 0) i = 0;
                    else i = (numClient) * 8;

                    int drawn = 0;
                    for (int f = 0; f < uavs.IndexOf(uav); f++)
                        if (uavs[f].ClientID == uav.ClientID)
                            drawn++;

                    j = (5 * iterations.Count + 3) * drawn + 1;
                }
                else
                {
                    //  проверка, отрисован ли уже бил этот клиент
                    if (clientwithnum.ContainsValue(uav.ClientID))
                    {
                        var client = clientwithnum.FirstOrDefault(k => k.Value == uav.ClientID);
                        //curClientID = client.Value;
                        numClient = client.Key;

                        //  подсчёт отрисованных бпла
                        int drawn = 0;
                        for (int f = 0; f < uavs.IndexOf(uav); f++)
                            if (uavs[f].ClientID == uav.ClientID)
                                drawn++;

                        j = (5 * iterations.Count + 3) * drawn + 1;
                    }
                    else
                    {
                        //  Отрисовка нового клиента в новый столбец
                        numClient = clientwithnum.Keys.Count;
                        clientwithnum.Add(numClient, uav.ClientID);
                        j = 1;
                    }

                    curClientID = uav.ClientID;
                    i = numClient * 8;
                }

                Color color = Color.FromArgb(uav.Color.A, uav.Color.R, uav.Color.G, uav.Color.B);

                //  Заполнение шапки таблицы, заличка нужным - обводка - номер бпла и номер клиента
                worksheet.get_Range($"{letters[i]}{j}:{letters[i + 1]}{j}").Interior.Color = color;
                worksheet.get_Range($"{letters[i]}{j}:{letters[i + 1]}{j}").Borders.Weight = Excel.XlBorderWeight.xlMedium;
                worksheet.get_Range($"{letters[i]}{j}:{letters[i + 1]}{j}").Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                worksheet.get_Range($"{letters[i]}{j}:{letters[i]}{j}").Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlLineStyleNone;
                i += 2;
                worksheet.Cells[j, i + 1] = $"Беспилотный летательный аппарат №{uav.Settings.ID} клиент {uav.ClientID}";
                worksheet.get_Range($"{letters[i]}{j}:{letters[i + 4]}{j}").Merge(Type.Missing);
                worksheet.get_Range($"{letters[i]}{j}:{letters[i + 4]}{j}").Borders.Weight = Excel.XlBorderWeight.xlMedium;
                worksheet.get_Range($"{letters[i]}{j}:{letters[i + 4]}{j}").Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                i -= 1;
                j++;

                //  сетка для загоровков таблицы
                HeadStyle(worksheet, $"{letters[i + 1]}{j - 1}:{letters[i + 5]}{j - 1}");
                worksheet.get_Range($"{letters[i - 1]}{j}:{letters[i - 1]}{j}").Borders[XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlLineStyleNone;
                worksheet.get_Range($"{letters[i + 1]}{j - 1}:{letters[i + 5]}{j - 1}").Interior.Color = XlColorIndex.xlColorIndexNone;

                //  Изменение размера столбцов
                worksheet.get_Range($"{letters[i - 1]}{j}:{letters[i - 1]}{j}").ColumnWidth = 8;
                worksheet.get_Range($"{letters[i]}{j}:{letters[i]}{j}").ColumnWidth = 2;
                worksheet.get_Range($"{letters[i + 1]}{j}:{letters[i + 1]}{j}").ColumnWidth = 18;
                worksheet.get_Range($"{letters[i + 2]}{j}:{letters[i + 2]}{j}").ColumnWidth = 15;
                worksheet.get_Range($"{letters[i + 3]}{j}:{letters[i + 3]}{j}").ColumnWidth = 15;
                worksheet.get_Range($"{letters[i + 4]}{j}:{letters[i + 4]}{j}").ColumnWidth = 15;
                worksheet.get_Range($"{letters[i + 5]}{j}:{letters[i + 5]}{j}").ColumnWidth = 15;

                //  заполнение шапки таблицы
                worksheet.Cells[j, i] = $"Итерации";
                worksheet.Cells[j, i + 2] = $"Местоположение";
                worksheet.Cells[j, i + 3] = $"Скорость";
                worksheet.Cells[j, i + 4] = $"Ускорение";
                worksheet.Cells[j, i + 5] = $"Макс. скорость";
                worksheet.Cells[j, i + 6] = $"Мин. скорость";


                HeadStyle(worksheet, $"{letters[i - 1]}{j}:{letters[i + 5]}{j}");
                worksheet.get_Range($"{letters[i - 1]}{j}:{letters[i]}{j}").Merge();

                for (int iterationIndex = 0; iterationIndex < iterations.Count; iterationIndex++)
                {
                    byte rowAxes = 0;

                    //  Объединение ячеек и подписи номера итерации
                    worksheet.get_Range($"{letters[i - 1]}{j + 1}:{letters[i - 1]}{j + 1 + 4}").Merge();

                    //  Выделение контура всей итерации линией средней толщины
                    worksheet.get_Range($"{letters[i - 1]}{j + 1}:{letters[i + 5]}{j + 1 + 4}").Borders.LineStyle = XlLineStyle.xlContinuous;
                    worksheet.get_Range($"{letters[i - 1]}{j + 1}:{letters[i + 5]}{j + 1 + 4}").BorderAround2(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium);
                    worksheet.get_Range($"{letters[i]}{j + 1}:{letters[i + 5]}{j + 1 + 2}").BorderAround2(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium);

                    //  Все ячейки во всей итерации сенрируются
                    worksheet.get_Range($"{letters[i - 1]}{j + 1}:{letters[i + 5]}{j + 1 + 4}").VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    worksheet.get_Range($"{letters[i - 1]}{j + 1}:{letters[i + 5]}{j + 1 + 4}").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    //  Красим итерацию при столкновении
                    if (uav.DangerLevel == DangerLevel.SafeLevel)
                    {
                        worksheet.get_Range($"{letters[i - 1]}{j + 1}:{letters[i + 5]}{j + 1 + 4}").Interior.Color = Color.FromArgb(198, 239, 206);
                        worksheet.get_Range($"{letters[i - 1]}{j + 1}:{letters[i + 5]}{j + 1 + 4}").Font.Color = Color.FromArgb(0, 97, 0);
                    }
                    else if (uav.DangerLevel == DangerLevel.Crash)
                    {
                        worksheet.get_Range($"{letters[i - 1]}{j + 1}:{letters[i + 5]}{j + 1 + 4}").Interior.Color = Color.FromArgb(255, 199, 206);
                        worksheet.get_Range($"{letters[i - 1]}{j + 1}:{letters[i + 5]}{j + 1 + 4}").Font.Color = Color.FromArgb(156, 0, 6);
                    }

                    //  3) Написание всей небходимой информации для каждого бпла
                    for (int row = j + 1; row <= j + 5; row++)
                    {
                        rowAxes++;

                        //  Построчная отрисовка
                        switch (rowAxes % 5)
                        {
                            case 1:
                                worksheet.Cells[row, i] = iterationIndex + 1;

                                worksheet.Cells[row, i + 1] = $"X";
                                worksheet.Cells[row, i + 2] = uav.Settings.X;
                                worksheet.Cells[row, i + 3] = uav.Settings.SpeedX;
                                worksheet.Cells[row, i + 4] = uav.Settings.AccelX;
                                worksheet.Cells[row, i + 5] = "-";
                                worksheet.Cells[row, i + 6] = "-";
                                break;

                            case 2:
                                worksheet.Cells[row, i + 1] = $"Y";
                                worksheet.Cells[row, i + 2] = uav.Settings.Y;
                                worksheet.Cells[row, i + 3] = uav.Settings.SpeedY;
                                worksheet.Cells[row, i + 4] = uav.Settings.AccelY;
                                worksheet.Cells[row, i + 5] = "-";
                                worksheet.Cells[row, i + 6] = "-";
                                break;

                            case 3:
                                worksheet.Cells[row, i + 1] = $"Z";
                                worksheet.Cells[row, i + 2] = uav.Settings.Z;
                                worksheet.Cells[row, i + 3] = uav.Settings.SpeedZ;
                                worksheet.Cells[row, i + 4] = uav.Settings.AccelZ;
                                worksheet.Cells[row, i + 5] = "-";
                                worksheet.Cells[row, i + 6] = "-";
                                break;

                            case 4:
                                worksheet.Cells[row, i + 2] = "Текущая угроза";
                                worksheet.Cells[row, i + 3] = "Скорость";
                                worksheet.Cells[row, i + 4] = "Статус";
                                worksheet.Cells[row, i + 5] = "Макс. скорость";
                                worksheet.Cells[row, i + 6] = "Мин. скорость";

                                //  Стил для шапки
                                HeadStyle(worksheet, $"{letters[i + 1]}{row}:{letters[i + 5]}{row}");
                                worksheet.get_Range($"{letters[i]}{row}:{letters[i]}{row}").Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlLineStyleNone;
                                worksheet.get_Range($"{letters[i]}{row}:{letters[i]}{row + 1}").Borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlLineStyleNone;
                                worksheet.get_Range($"{letters[i + 1]}{row}:{letters[i + 5]}{row + 1}").BorderAround2(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium);
                                break;

                            case 0:
                                if (uav.DangerID != 0 && uav.ClientID != uav.DangerClientID)
                                    worksheet.Cells[row, i + 2] = $"БПЛА {uav.DangerID}\nКлиент {uav.DangerClientID}";
                                else if (uav.DangerID == uav.Settings.ID && uav.ClientID == uav.DangerClientID && uav.DangerID != 0)
                                    worksheet.Cells[row, i + 2] = $"Ошибка!";
                                else
                                    worksheet.Cells[row, i + 2] = $"Угрозы нет";

                                worksheet.Cells[row, i + 3] = uav.Settings.Speed;
                                //  Описание
                                worksheet.Cells[row, i + 4] = $"{GetDescription(uav.DangerLevel)}";
                                //worksheet.Cells[row, i + 4] = $"{GetDescription(uav.DangerLevel)}\n{Enum.GetName(typeof(DangerLevel), uav.DangerLevel)}";
                                //worksheet.Cells[row, i + 4] = $"{Enum.GetName(typeof(DangerLevel), uav.DangerLevel)}";

                                //  Название и описание
                                worksheet.Cells[row, i + 5] = uav.Settings.MaxSpeed;
                                worksheet.Cells[row, i + 6] = uav.Settings.MinSpeed;
                                break;
                        }
                    }
                    j += 5;
                }
                j++;
            }
            #endregion

            excel.Visible = true;
            workbook.Save();
        }

        private void HeadStyle(Worksheet worksheet, string StartCell, string EndCell)
        {
            worksheet.get_Range($"{StartCell}:{EndCell}").Font.Bold = true;
            worksheet.get_Range($"{StartCell}:{EndCell}").Borders.LineStyle = XlLineStyle.xlContinuous;
            worksheet.get_Range($"{StartCell}:{EndCell}").BorderAround2(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium);

            worksheet.get_Range($"{StartCell}:{EndCell}").VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            worksheet.get_Range($"{StartCell}:{EndCell}").HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            worksheet.get_Range($"{StartCell}:{EndCell}").Interior.Color = Color.FromArgb(0, 255, 255);
            worksheet.get_Range($"{StartCell}:{EndCell}").Font.Color = Color.Black;
        }
        private void HeadStyle(Worksheet worksheet, string CellRange)
        {
            worksheet.get_Range(CellRange).Font.Bold = true;
            worksheet.get_Range(CellRange).Borders.LineStyle = XlLineStyle.xlContinuous;
            worksheet.get_Range(CellRange).BorderAround2(XlLineStyle.xlContinuous, XlBorderWeight.xlMedium);

            worksheet.get_Range(CellRange).VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            worksheet.get_Range(CellRange).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            worksheet.get_Range(CellRange).Interior.Color = Color.FromArgb(0, 255, 255);
            worksheet.get_Range(CellRange).Font.Color = Color.Black;
        }
        private string GetDescription(Enum enumElement)
        {
            Type type = enumElement.GetType();

            MemberInfo[] memInfo = type.GetMember(enumElement.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return enumElement.ToString();
        }
    }
}