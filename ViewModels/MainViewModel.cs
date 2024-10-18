using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Windows;
using Assistant.Services;
using Assistant.Data.Models;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Assistant.Views;
using HandyControl.Data;

namespace Assistant.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private UIElement? mainView;

        [ObservableProperty]
        private ObservableCollection<TabItem> tabs;

        [ObservableProperty]
        private Int32 tabIndex;

        private readonly NavigationService navigator;
        private readonly DialogService dialog;
        private readonly ILogger logger;
        public MainViewModel(NavigationService navigator, DialogService dialog, ILogger logger)
        {
            this.navigator = navigator;
            this.dialog = dialog;
            this.logger = logger;

            Tabs = new ObservableCollection<TabItem>();
            TabIndex = 0;

            navigator.MainViewChanged += (s, e) =>
            {
                MainView = e.View;
            };

            // navigator.NavigateTo<IndexView>();
            logger.Information("软件启动");
            Welcome();
        }

        private void Welcome()
        {
            var tab = new TabItem {
                Header = LangKeys.Welcome,
                Content = App.Current.Services.GetRequiredService<WelcomeView>(),
                Icon = "\ue633",
            };
            Tabs.Add(tab);
            TabIndex = Tabs.Count - 1;
        }

        [RelayCommand]
        public async Task TestAsync()
        {
            Boolean result = await dialog.ShowTextAsync(LangKeys.Message, LangKeys.UpdateComplete);
            dialog.Message(MessageLevel.Info, result.ToString());
        }

        [RelayCommand]
        public void Navigate(Type type)
        {
            navigator.NavigateTo(type);
        }

        [RelayCommand]
        public void Close(CancelRoutedEventArgs args)
        {
            var tab = args.OriginalSource as TabItem;
            var view = tab!.Content as System.Windows.Controls.UserControl;
            var model = view!.DataContext as TabItemViewModelBase;
            model!.Close();
        }

        [RelayCommand]
        public void OpenSerialPort()
        {
            var tab = new TabItem
            {
                Header = LangKeys.Serial,
                Content = App.Current.Services.GetRequiredService<SerialPortView>(),
                Icon = "\ue62c",
            };
            Tabs.Add(tab);
            TabIndex = Tabs.Count - 1;
        }

        [RelayCommand]
        public void OpenMine()
        {
            var tab = new TabItem
            {
                Header = LangKeys.Mine,
                Content = App.Current.Services.GetRequiredService<MineView>(),
                Icon = "\ue704",
            };
            Tabs.Add(tab);
            TabIndex = Tabs.Count - 1;
        }

        [RelayCommand]
        public void OpenSettings()
        {
            var tab = new TabItem
            {
                Header = LangKeys.Settings,
                Content = App.Current.Services.GetRequiredService<SettingsView>(),
                Icon = "\ue795",
            };
            Tabs.Add(tab);
            TabIndex = Tabs.Count - 1;
        }

        [RelayCommand]
        public void OpenSocket()
        {
            var tab = new TabItem
            {
                Header = LangKeys.Socket,
                Content = App.Current.Services.GetRequiredService<SocketView>(),
                Icon = "\ue65f",
            };
            Tabs.Add(tab);
            TabIndex = Tabs.Count - 1;
        }
    }
}
