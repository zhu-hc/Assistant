using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Tools.Extension;

namespace Assistant.ViewModels.Dialogs
{
    public partial class TextDialogViewModel : ViewModelBase, IDialogResultable<Boolean>
    {
        [ObservableProperty]
        private String title;

        [ObservableProperty]
        private String content;

        public TextDialogViewModel()
        {
            Title = Content = "";
            CloseAction = () => { };
        }

        [RelayCommand]
        public void Confirm()
        {
            Result = true;
            CloseAction();
        }

        [RelayCommand]
        public void Cancel()
        {
            Result = false;
            CloseAction();
        }

        public Boolean Result { get; set; }
        public Action CloseAction { get; set; }
    }
}
