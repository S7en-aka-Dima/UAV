using DevExpress.Mvvm;
using Modelling_Client.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Modelling_Client.ViewModels
{
    class PrimaryRadarViewModel
    {
        public PrimaryRadarViewModel()
        {

        }

        public ICommand<object> DM => new DelegateCommand<object>((window) => { MessageBox.Show(window.ToString()); });

    }
}
