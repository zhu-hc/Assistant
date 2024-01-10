using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Markup;
using Assistant.Data.Models;
using Assistant.Services;
using Assistant.Tools.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FluentScheduler;
using HandyControl.Tools;
using Serilog;
using Timer = System.Timers.Timer;

namespace Assistant.ViewModels
{
    public partial class SerialPortViewModel : TabItemViewModelBase
    {
        public IMessenger messenger = new WeakReferenceMessenger();

        [ObservableProperty]
        private String? portName;

        [ObservableProperty]
        private String? baudRate;

        [ObservableProperty]
        private String? dataBits;

        [ObservableProperty]
        private String? stopBits;

        [ObservableProperty]
        private String? parity;

        [ObservableProperty]
        private UInt32 txCount;

        [ObservableProperty]
        private UInt32 rxCount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BtnName))]
        private Boolean isOpen;

        [ObservableProperty]
        private Boolean isHexShow;

        [ObservableProperty]
        private Boolean autoEnabled;

        [ObservableProperty]
        private String autoInterval;

        public String BtnName => IsOpen ? LangKeys.Close : LangKeys.Open;

        private SerialPort sp = new SerialPort();
        private Timer timer = new Timer();

        private readonly DialogService dialog;
        private readonly ILogger logger;
        public SerialPortViewModel(DialogService dialog, ILogger logger)
        {
            this.dialog = dialog;
            this.logger = logger;

            BaudRate = "115200";
            TxCount = RxCount = 0;
            IsOpen = false;
            AutoInterval = "1000";
            AutoEnabled = false;

            sp.ErrorReceived += (s, e) => {
                IsOpen = sp.IsOpen;
                dialog.Message(MessageLevel.Error, $"Error: {e.EventType}");
            };

            sp.DataReceived += (s, e) => {
                if (!sp.IsOpen) return;
                Int32 count = sp.BytesToRead;
                RxCount += (UInt32)count;

                Byte[] data = new byte[count];
                sp.Read(data, 0, data.Length);

                String ReceiveData = "";
                if (IsHexShow)
                {
                    for (Int32 i = 0; i < data.Length; i++)
                    {
                        ReceiveData += data[i].ToString("X2") + " ";
                    }
                }
                else
                {
                    ReceiveData = Encoding.UTF8.GetString(data);
                }
                messenger.Send(ReceiveData, "Text");
            };

            JobManager.AddJob(
                () => IsOpen = sp.IsOpen,
                s => s.ToRunEvery(200).Milliseconds()
            );

            timer.Elapsed += new ElapsedEventHandler((s, e) => {
                messenger.Send("", "Transmit");
            });
        }

        public override void Close()
        {
            base.Close();
        }

        private void WriteBytes(Byte[] dat)
        {
            if (sp.IsOpen)
            {
                sp.Write(dat, 0, dat.Length);
                TxCount += (UInt32)dat.Length;
            }
        }

        [RelayCommand]
        public void ClearTxCount() => TxCount = 0;

        [RelayCommand]
        public void ClearRxCount() => RxCount = 0;

        [RelayCommand]
        public void Open()
        {
            try
            {
                if (sp.IsOpen)
                    sp.Close();
                else
                {
                    sp.PortName = PortName;
                    sp.BaudRate = Convert.ToInt32(BaudRate);
                    sp.DataBits = DataBits switch
                    {
                        "5 Bit" => 5,
                        "6 Bit" => 6,
                        "7 Bit" => 7,
                        "8 Bit" => 8,
                        _ => throw new Exception(I18NExtension.Translate(LangKeys.DataBitsError)),
                    };
                    sp.StopBits = StopBits switch
                    {
                        "0 Bit" => System.IO.Ports.StopBits.None,
                        "1 Bit" => System.IO.Ports.StopBits.One,
                        "2 Bit" => System.IO.Ports.StopBits.Two,
                        "1.5 Bit" => System.IO.Ports.StopBits.OnePointFive,
                        _ => throw new Exception(I18NExtension.Translate(LangKeys.StopBitsError)),
                    };
                    sp.Parity = Parity switch
                    {
                        "None" => System.IO.Ports.Parity.None,
                        "Odd" => System.IO.Ports.Parity.Odd,
                        "Even" => System.IO.Ports.Parity.Even,
                        "Mark" => System.IO.Ports.Parity.Mark,
                        "Space" => System.IO.Ports.Parity.Space,
                        _ => throw new Exception(I18NExtension.Translate(LangKeys.CheckBitsError)),
                    };
                    sp.Open();
                    String str = $"{PortName},{BaudRate},{DataBits},{StopBits},{Parity}";
                    dialog.Message(MessageLevel.Info, str);
                }
            }
            catch (Exception ex)
            {
                dialog.Message(MessageLevel.Error, ex.Message);
            }
            finally
            {
                IsOpen = sp.IsOpen;
            }
        }

        [RelayCommand]
        public void Clear()
        {
            messenger.Send("", "Clear");
        }

        [RelayCommand]
        public void SimpleTransmit(Object[] paras)
        {
            try
            {
                String content = (String)paras[0];
                Boolean isHex = (Boolean)paras[1];
                if (String.IsNullOrEmpty(content)) throw new Exception(I18NExtension.Translate(LangKeys.EnterContent));
                if (!sp.IsOpen) throw new Exception(I18NExtension.Translate(LangKeys.PleaseOpenSerial));
                if (isHex)
                {
                    if (!HexStringExtension.IsValid(content)) throw new Exception(I18NExtension.Translate(LangKeys.PleaseInputHex));
                    WriteBytes(HexStringExtension.ToBytes(content));
                }
                else
                {
                    WriteBytes(Encoding.UTF8.GetBytes(content));
                }
            }
            catch (Exception ex)
            {
                timer.Enabled = false;
                AutoEnabled = false;
                dialog.Message(MessageLevel.Error, ex.Message);
            }
        }

        [RelayCommand]
        public void Auto()
        {
            try
            {
                if (AutoEnabled)
                {
                    timer.Interval = Convert.ToInt32(AutoInterval);
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
                AutoEnabled = false;
                dialog.Message(MessageLevel.Error, ex.Message);
            }
        }
    }
}
