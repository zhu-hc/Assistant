using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assistant.Data;
using HandyControl.Properties.Langs;
using HandyControl.Themes;
using HandyControl.Tools;

namespace Assistant.Views.Main
{
    /// <summary>
    /// NonClientAreaContent.xaml 的交互逻辑
    /// </summary>
    public partial class NonClientAreaContent
    {
        public NonClientAreaContent()
        {
            InitializeComponent();
        }

        private void ButtonConfig_OnClick(object sender, RoutedEventArgs e) => PopupConfig.IsOpen = true;

        private void ButtonSkins_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button button && button.Tag is ApplicationTheme tag)
            {
                PopupConfig.IsOpen = false;
                if (tag.Equals(GlobalData.Config.Theme)) return;
                GlobalData.Config.Theme = tag;
                GlobalData.Save();
                ((App)Application.Current).UpdateSkin(tag);
            }
        }

        private void ButtonLangs_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button { Tag: string langName })
            {
                PopupConfig.IsOpen = false;
                if (langName.Equals(GlobalData.Config.Lang))
                {
                    return;
                }

                ConfigHelper.Instance.SetLang(langName);
                I18NExtension.Culture = new CultureInfo(langName);

                GlobalData.Config.Lang = langName;
                GlobalData.Save();
            }
        }
    }
}
