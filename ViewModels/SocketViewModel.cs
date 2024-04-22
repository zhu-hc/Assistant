using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

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

        private Socket? socket = null;
        private TcpClient? tcpClient = null;
        private UdpClient? udpClient = null;
        public SocketViewModel()
        {
            Port = "8080";
            Time = "1000";
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

        }

        [RelayCommand]
        public void ClearReceive()
        {
            messenger.Send("", "Clear");
        }
    }
}
