using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using DynamicApp.Mobile.Models;
using DynamicApp.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.StateSquid;

namespace DynamicApp.Mobile.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public BaseViewModel()
        {
            Logger = DependencyService.Get<LoggerService>();

            OpenLinkInBrowserCommand = new Command<string>(async url => await OpenLinkInBrowser(url));
            OpenLinkInExternalBrowserCommand = new Command<string>(async url => await OpenLinkInExternalBrowser(url));
        }

        public LoggerService Logger { get; set; }

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        bool showError = false;
        public bool ShowError
        {
            get { return showError; }
            set { SetProperty(ref showError, value); }
        }

        string errorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        State currentState = State.Loading;
        public State CurrentState
        {
            get { return currentState; }
            set { SetProperty(ref currentState, value); }
        }

        public Command OpenLinkInBrowserCommand { get; private set; }

        private async Task OpenLinkInBrowser(string destination)
        {
            try
            {
                var url = destination;
                if (!url.StartsWith("http"))
                {
                    url = "https://" + url;
                }
                await Browser.OpenAsync(new Uri(url), BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception exception)
            {
                await Logger.Error("Bad URL", exception);
            }
        }

        public Command OpenLinkInExternalBrowserCommand { get; private set; }

        private async Task OpenLinkInExternalBrowser(string destination)
        {
            try
            {
                var url = destination;
                if (!url.StartsWith("http"))
                {
                    url = "https://" + url;
                }
                await Browser.OpenAsync(new Uri(url), BrowserLaunchMode.External);
            }
            catch (Exception exception)
            {
                await Logger.Error("Bad URL", exception);
            }
        }

        protected bool CanExecute()
        {
            return !IsBusy;
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
