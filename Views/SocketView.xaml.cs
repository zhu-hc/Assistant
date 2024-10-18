using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using Assistant.Tools.Helpers;
using Assistant.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace Assistant.Views
{
    /// <summary>
    /// SocketView.xaml 的交互逻辑
    /// </summary>
    public partial class SocketView : UserControl
    {
        public SocketView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.DataContext is SocketViewModel vm)
            {
                vm.messenger.Register<SocketView, String, String>(this, "Text", (s, e) => {
                    Application.Current.Dispatcher.BeginInvoke(() => {
                        TextEditor.AppendText(e);
                        TextEditor.ScrollToEnd();
                    });
                });
                vm.messenger.Register<SocketView, String, String>(this, "ClearReceive", (s, e) => {
                    Application.Current.Dispatcher.BeginInvoke(() => {
                        TextEditor.Clear();
                    });
                });
                vm.messenger.Register<SocketView, String, String>(this, "ClearTransmit", (s, e) => {
                    Application.Current.Dispatcher.BeginInvoke(() => {
                        tbContent.Clear();
                    });
                });
                vm.messenger.Register<SocketView, String[], String>(this, "Clients", (s, e) => {
                    Application.Current.Dispatcher.BeginInvoke(() => {
                        clients.Items.Clear();
                        foreach (var item in e)
                        {
                            clients.Items.Add(item);
                        }
                        clients.SelectedIndex = 0;
                    });
                });
                vm.messenger.Register<SocketView, String, String>(this, "Transmit", (s, e) => {
                    Application.Current.Dispatcher.BeginInvoke(() => {
                        mainTransmit.Command.Execute(mainTransmit.CommandParameter);
                    });
                });
            }
        }

        private void cbIP_DropDownOpened(object sender, EventArgs e)
        {
            if (sender != null && sender is ComboBox comboBox)
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                comboBox.Items.Clear();
                comboBox.Items.Add("127.0.0.1");
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        comboBox.Items.Add(ip.ToString());
                    }
                }
            }
        }
    }
}
