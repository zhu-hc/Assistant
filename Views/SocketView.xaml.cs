﻿using System;
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
                vm.messenger.Register<SocketView, String, String>(this, "Clear", (s, e) => {
                    Application.Current.Dispatcher.BeginInvoke(() => {
                        TextEditor.Clear();
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
