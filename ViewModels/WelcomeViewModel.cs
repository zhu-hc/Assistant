using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Assistant.ViewModels
{
    public partial class WelcomeViewModel : TabItemViewModelBase
    {
        public WelcomeViewModel()
        {

        }

        public override void Close()
        {
            base.Close();
        }
    }
}
