using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assistant.Services;
using CommunityToolkit.Mvvm.Input;
using Serilog;

namespace Assistant.ViewModels
{
    public partial class SerialPortViewModel : TabItemViewModelBase
    {
        private readonly DialogService dialog;
        private readonly ILogger logger;
        public SerialPortViewModel(DialogService dialog, ILogger logger)
        {
            this.dialog = dialog;
            this.logger = logger;
        }

        public override void Close()
        {
            base.Close();
        }

        [RelayCommand]
        public void SimpleTransmit(Object[] paras)
        {
            try
            {
                String content = (String)paras[0];
                Boolean isHex = (Boolean)paras[1];
                dialog.Message(MessageLevel.Info, $"{content} {isHex}");
            }
            catch (Exception ex)
            {
                dialog.Message(MessageLevel.Error, ex.Message);
            }
        }
    }
}
