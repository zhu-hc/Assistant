using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;
using Assistant.Data;
using Assistant.Services;
using Assistant.ViewModels;
using Assistant.ViewModels.Main;
using Assistant.Views;
using Assistant.Views.Main;
using FluentScheduler;
using HandyControl.Themes;
using HandyControl.Tools;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Assistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }
        public static new App Current => (App)Application.Current;

        public App()
        {
            var container = new ServiceCollection();

            // Logger
            container.AddSingleton<ILogger>(sp => {
                return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("Logs/log.txt")
                .CreateLogger();
            });

            // Services
            container.AddSingleton<NavigationService>();
            container.AddSingleton<DialogService>();

            container.AddTransient<NonClientAreaViewModel>();
            container.AddTransient<NonClientAreaContent>(sp => {
                return new NonClientAreaContent
                {
                    DataContext = sp.GetRequiredService<NonClientAreaViewModel>()
                };
            });

            container.AddTransient<WelcomeViewModel>();
            container.AddTransient<WelcomeView>(sp => {
                return new WelcomeView
                {
                    DataContext = sp.GetRequiredService<WelcomeViewModel>()
                };
            });

            container.AddTransient<SerialPortViewModel>();
            container.AddTransient<SerialPortView>(sp => {
                return new SerialPortView
                {
                    DataContext = sp.GetRequiredService<SerialPortViewModel>()
                };
            });

            container.AddTransient<SocketViewModel>();
            container.AddTransient<SocketView>(sp => {
                return new SocketView
                {
                    DataContext = sp.GetRequiredService<SocketViewModel>()
                };
            });

            container.AddSingleton<MineViewModel>();
            container.AddSingleton<MineView>(sp => {
                return new MineView
                {
                    DataContext = sp.GetRequiredService<MineViewModel>()
                };
            });

            container.AddSingleton<SettingsViewModel>();
            container.AddSingleton<SettingsView>(sp => {
                return new SettingsView
                {
                    DataContext = sp.GetRequiredService<SettingsViewModel>()
                };
            });

            // MainWindow
            container.AddSingleton<MainViewModel>();
            container.AddSingleton<MainWindow>(sp => {
                return new MainWindow
                {
                    DataContext = sp.GetRequiredService<MainViewModel>()
                };
            });

            Services = container.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ApplicationHelper.IsSingleInstance();
            GlobalData.Init();
            if (GlobalData.Config.Theme != ApplicationTheme.Light)
            {
                UpdateSkin(GlobalData.Config.Theme);
            }
            ConfigHelper.Instance.SetLang(GlobalData.Config.Lang);
            I18NExtension.Culture = new CultureInfo(GlobalData.Config.Lang);
            ConfigHelper.Instance.SetWindowDefaultStyle();
            ConfigHelper.Instance.SetNavigationWindowDefaultStyle();

            // Init
            JobManager.Initialize();

            MainWindow = Services.GetRequiredService<MainWindow>();
            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            GlobalData.Save();
        }

        internal void UpdateSkin(ApplicationTheme theme)
        {
            var win = Services.GetRequiredService<MainWindow>();
            ThemeAnimationHelper.AnimateTheme(win, ThemeAnimationHelper.SlideDirection.Top, 0.3, 1, 0.5);
            ThemeManager.Current.ApplicationTheme = theme;
            ThemeAnimationHelper.AnimateTheme(win, ThemeAnimationHelper.SlideDirection.Bottom, 0.3, 0.5, 1);
        }
    }
}
