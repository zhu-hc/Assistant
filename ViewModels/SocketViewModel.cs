using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows;
using Assistant.Data.Models;
using Assistant.Services;
using Assistant.Tools.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentScheduler;
using Microsoft.VisualBasic.Logging;
using Serilog;
using TouchSocket.Core;
using TouchSocket.Sockets;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Assistant.ViewModels
{
    public partial class SocketViewModel : TabItemViewModelBase
    {
        public IMessenger messenger = new WeakReferenceMessenger();

        [ObservableProperty]
        private Boolean isHexReceive;

        [ObservableProperty]
        private Boolean isLogMode;

        [ObservableProperty]
        private Boolean isAutoNewline;

        [ObservableProperty]
        private Boolean isIgnoreReceive;

        [ObservableProperty]
        private Boolean isHexTransmit;

        [ObservableProperty]
        private Boolean isLoopTransmit;

        [ObservableProperty]
        private String address;

        [ObservableProperty]
        private String port;

        [ObservableProperty]
        private String time;

        [ObservableProperty]
        private String protocol;

        [ObservableProperty]
        private UInt32 txByteCount;

        [ObservableProperty]
        private UInt32 rxByteCount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FrameCount))]
        private UInt32 txFrameCount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FrameCount))]
        private UInt32 rxFrameCount;

        public String FrameCount => $"{RxFrameCount}/{TxFrameCount}";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BtnName))]
        private Boolean isOpen;

        [ObservableProperty]
        private String clientText = "";

        [ObservableProperty]
        private String udpTarget = "127.0.0.1:8080";

        public String BtnName => IsOpen ? LangKeys.Close : LangKeys.Open;

        public BindingList<String> Clients { get; } = new BindingList<String>();

        private TcpClient tcpClient = new TcpClient();
        private TcpService tcpService = new TcpService();
        private UdpSession udpSession = new UdpSession();

        private Timer timer = new Timer();
        private readonly DialogService dialog;
        private readonly ILogger logger;
        public SocketViewModel(DialogService dialog, ILogger logger)
        {
            this.dialog = dialog;
            this.logger = logger;

            IsOpen = false;
            Port = "8080";
            Time = "1000";
            Protocol = "";
            Address = "";
            IsLogMode = true;

            // 成功连接到服务器
            tcpClient.Connected = (client, e) =>
            {
                IsOpen = true;
                return EasyTask.CompletedTask;
            };
            // 从服务器断开连接
            tcpClient.Closed = (client, e) =>
            {
                IsOpen = false;
                return EasyTask.CompletedTask;
            };
            // 从服务器收到信息
            tcpClient.Received = (client, e) =>
            {
                Received("Recv from Server", e.ByteBlock.Span.ToArray());
                return EasyTask.CompletedTask;
            };

            // 有客户端成功连接
            tcpService.Connected = (client, e) =>
            {
                UpdateClients(client);
                return EasyTask.CompletedTask;
            };
            // 有客户端断开连接
            tcpService.Closed = (client, e) =>
            {
                UpdateClients(client);
                return EasyTask.CompletedTask;
            };
            // 从客户端收到信息
            tcpService.Received = (client, e) =>
            {
                Received($"Recv from {client.GetIPPort()}", e.ByteBlock.Span.ToArray());
                return EasyTask.CompletedTask;
            };

            timer.Elapsed += new ElapsedEventHandler((s, e) => {
                messenger.Send("", "Transmit");
            });
        }

        public override void Close()
        {
            base.Close();
        }

        [RelayCommand]
        public void ResetCount()
        {
            TxByteCount = RxByteCount = TxFrameCount = RxFrameCount = 0;
        }

        [RelayCommand]
        public void ClearTransmit()
        {
            messenger.Send("", "ClearTransmit");
        }

        [RelayCommand]
        public void ClearReceive()
        {
            messenger.Send("", "ClearReceive");
        }

        [RelayCommand]
        public async Task Open()
        {
            switch (Protocol)
            {
                case "UDP": await OpenUdp(); break;
                case "TCP Client": await OpenTcpClient(); break;
                case "TCP Server": await OpenTcpServer(); break;
            }
        }

        [RelayCommand]
        public async Task CloseClient()
        {
            if (IsOpen == false) return;

            switch (Protocol)
            {
                case "UDP":
                    break;
                case "TCP Client": 
                    break;
                case "TCP Server":
                    foreach (var item in tcpService.Clients)
                    {
                        if (ClientText.Contains("ALL") || ClientText == item.GetIPPort())
                            await item.CloseAsync();
                    }
                    break;
            }
        }

        [RelayCommand]
        public async Task Transmit(String content)
        {
            try
            {
                if (String.IsNullOrEmpty(content)) throw new Exception(I18NExtension.Translate(LangKeys.EnterContent));
                if (!IsOpen) throw new Exception(I18NExtension.Translate(LangKeys.PleaseOpenSocket));

                byte[] bytes;
                if (IsHexTransmit)
                {
                    if (!HexStringExtension.IsValid(content)) throw new Exception(I18NExtension.Translate(LangKeys.PleaseInputHex));
                    bytes = HexStringExtension.ToBytes(content);
                }
                else
                {
                    bytes = content.ToUTF8Bytes();
                }

                String title = "";
                switch (Protocol)
                {
                    case "TCP Client":
                        title = "Send to Server";
                        await tcpClient.SendAsync(bytes); 
                        break;
                    case "TCP Server":
                        if (tcpService.Clients.Count == 0) throw new Exception(I18NExtension.Translate(LangKeys.NoClient));
                        title = $"Send to {ClientText}";
                        foreach (var item in tcpService.Clients)
                        {
                            if (ClientText.Contains("ALL") || ClientText == item.GetIPPort())
                            {
                                await tcpService.SendAsync(item.Id, bytes);
                            }
                        }
                        break;
                    case "UDP":
                        await udpSession.SendAsync(EndPointExtension.ToEndPoint(UdpTarget), bytes);
                        break;
                }

                TxByteCount += (UInt32)bytes.Length;
                TxFrameCount++;
                if (IsLogMode)
                {
                    if (IsHexReceive == false)
                    {
                        content = Encoding.UTF8.GetString(bytes);
                    }
                    else
                    {
                        for (Int32 i = 0; i < bytes.Length; i++)
                        {
                            content += bytes[i].ToString("X2") + " ";
                        }
                    }
                    messenger.Send($"[{DateTime.Now.ToString()}]# {title} >>>\r\n{content}\r\n", "Text");
                }
            }
            catch (Exception ex)
            {
                timer.Enabled = false;
                IsLoopTransmit = false;
                dialog.Message(MessageLevel.Error, ex.Message);
            }
        }

        [RelayCommand]
        public void Auto()
        {
            try
            {
                if (IsLoopTransmit)
                {
                    timer.Interval = Convert.ToInt32(Time);
                    timer.Enabled = true;
                }
                else
                {
                    timer.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                timer.Enabled = false;
                IsLoopTransmit = false;
                dialog.Message(MessageLevel.Error, ex.Message);
            }
        }

        private async Task OpenUdp()
        {
            try
            {
                if (IsOpen == false)
                {
                    udpSession = new UdpSession();
                    udpSession.Received = (c, e) =>
                    {
                        Received($"Recv from {e.EndPoint}", e.ByteBlock.Span.ToArray());
                        return EasyTask.CompletedTask;
                    };

                    await udpSession.SetupAsync(new TouchSocketConfig()
                        .SetBindIPHost($"{Address}:{Port}")
                    );
                    await udpSession.StartAsync();
                    IsOpen = true;
                }
                else
                {
                    await udpSession.StopAsync();
                    IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                dialog.Message(MessageLevel.Error, ex.Message);
            }
        }

        private async Task OpenTcpClient()
        {
            try
            {
                if (IsOpen == false)
                {
                    await tcpClient.SetupAsync(new TouchSocketConfig()
                        .SetRemoteIPHost($"{Address}:{Port}")
                        .ConfigurePlugins(a => a.UseTcpReconnection())
                    );
                    await tcpClient.ConnectAsync();
                }
                else
                {
                    await tcpClient.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                dialog.Message(MessageLevel.Error, ex.Message);
            }
        }

        private async Task OpenTcpServer()
        {
            try
            {
                if (IsOpen == false)
                {
                    await tcpService.SetupAsync(new TouchSocketConfig()
                        .SetListenIPHosts($"tcp://{Address}:{Port}")
                    );
                    await tcpService.StartAsync();
                    IsOpen = true;
                }
                else
                {
                    await tcpService.StopAsync();
                    IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                dialog.Message(MessageLevel.Error, ex.Message);
            }
        }

        private void Received(String log, byte[] bytes)
        {
            RxByteCount += (UInt32)bytes.Length;
            RxFrameCount++;
            if (IsIgnoreReceive == false)
            {
                String content = "";
                String result = "";
                if (IsHexReceive == false)
                {
                    content = Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    for (Int32 i = 0; i < bytes.Length; i++)
                    {
                        content += bytes[i].ToString("X2") + " ";
                    }
                }
                if (IsAutoNewline) result = "\r\n";
                if (IsLogMode == true)
                {
                    result += $"[{DateTime.Now.ToString()}]# {log} <<<\r\n{content}\r\n";
                }
                else
                {
                    result += content;
                }

                messenger.Send(result, "Text");
            }
        }

        private void UpdateClients(TcpSessionClient client)
        {
            List<String> lists = new List<String>();
            lists.Add($"ALL Client({tcpService.Clients.Count})");
            foreach (var item in tcpService.Clients)
            {
                lists.Add(item.GetIPPort());
            }
            messenger.Send(lists.ToArray(), "Clients");
        }
    }
}
