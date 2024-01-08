using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assistant.Tools.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Assistant.ViewModels.Main
{
    public partial class NonClientAreaViewModel : ViewModelBase
    {
        [ObservableProperty]
        public String versionInfo;

        public NonClientAreaViewModel()
        {
            VersionInfo = VersionHelper.GetVersion();
        }
    }
}
