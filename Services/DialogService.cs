using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assistant.ViewModels.Dialogs;
using Assistant.Views.Dialogs;
using HandyControl.Controls;
using HandyControl.Tools.Extension;

namespace Assistant.Services
{
    public enum MessageLevel { Info, Success, Warning, Error }
    public class DialogService
    {
        public DialogService()
        {

        }

        public async Task<Boolean> ShowTextAsync(String title, String content)
        {
            return await Dialog.Show(new TextDialog
            {
                DataContext = new TextDialogViewModel
                {
                    Title = title,
                    Content = content
                }
            })
            .GetResultAsync<Boolean>();
        }

        public void Message(MessageLevel level, String message = "", String token = "")
        {
            switch (level)
            {
                case MessageLevel.Info:
                    Growl.Info(message, token);
                    break;
                case MessageLevel.Success:
                    Growl.Success(message, token);
                    break;
                case MessageLevel.Warning:
                    Growl.Warning(message, token);
                    break;
                case MessageLevel.Error:
                    Growl.Error(message, token);
                    break;
            }
        }
    }
}
