using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Assistant.Services
{
    public class NavigationEventArgs : EventArgs
    {
        public UIElement View { get; private set; }

        public NavigationEventArgs(UIElement view)
        {
            View = view;
        }
    }

    public class NavigationService
    {
        public event EventHandler<NavigationEventArgs>? MainViewChanged;

        private void OnMainViewChanged(NavigationEventArgs e)
        {
            MainViewChanged?.Invoke(this, e);
        }

        public void NavigateTo<T>() where T : UIElement
        {
            OnMainViewChanged(new NavigationEventArgs(App.Current.Services.GetRequiredService<T>()));
        }

        public void NavigateTo(Type type)
        {
            OnMainViewChanged(new NavigationEventArgs((UIElement)App.Current.Services.GetRequiredService(type)));
        }
    }
}
