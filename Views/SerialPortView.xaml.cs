using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Assistant.Tools.Helpers;
using Assistant.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using HandyControl.Tools;
using HandyControl.Tools.Extension;

namespace Assistant.Views
{
    /// <summary>
    /// SerialPortView.xaml 的交互逻辑
    /// </summary>
    public partial class SerialPortView : UserControl
    {
        public SerialPortView()
        {
            InitializeComponent();
        }

        private void cbPort_DropDownOpened(object sender, EventArgs e)
        {
            if (sender != null && sender is ComboBox comboBox)
            {
                String[] list = SerialPortHelper.GetPortNames();
                //添加串口项目  
                comboBox.Items.Clear();
                foreach (String s in list)
                {
                    //获取有多少个COM口  
                    comboBox.Items.Add(s);
                }
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is SerialPortViewModel vm)
            {
                vm.messenger.Register<SerialPortView, String, String>(this, "Text", (s, e) => {
                    Application.Current.Dispatcher.BeginInvoke(() => {
                        TextEditor.AppendText(e);
                        TextEditor.ScrollToEnd();
                    });
                });

                vm.messenger.Register<SerialPortView, String, String>(this, "Clear", (s, e) => {
                    Application.Current.Dispatcher.BeginInvoke(() => {
                        TextEditor.Clear();
                    });
                });

                vm.messenger.Register<SerialPortView, String, String>(this, "Transmit", (s, e) => {
                    Application.Current.Dispatcher.BeginInvoke(() => {
                        mainTransmit.Command.Execute(mainTransmit.CommandParameter);
                    });
                });
            }
        }
    }
}
