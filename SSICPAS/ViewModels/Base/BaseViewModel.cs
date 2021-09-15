using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using SSICPAS.Configuration;
using SSICPAS.Core.Interfaces;
using SSICPAS.Services;
using SSICPAS.Services.Interfaces;
using SSICPAS.Views.Menu;
using Xamarin.Forms;

namespace SSICPAS.ViewModels.Base
{
    public abstract class BaseViewModel : BaseBindableObject
    {
        private bool _isLoading;
        private static int _isCommandInProgress = 0; // 0 = false, other values = true. (original C convention)

        protected static INavigationService _navigationService => IoCContainer.Resolve<INavigationService>();
        protected static ISettingsService _settingsService => IoCContainer.Resolve<ISettingsService>();

        public virtual ICommand BackCommand => new Command(async () => await ThreadSafeExecuteOnceAsync(_navigationService.PopPage));

        public ICommand HelpButtonCommand => new Command(async () => await ExecuteOnceAsync(async () => await _navigationService.PushPage(new HelpPage())));
        
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                RaisePropertyChanged(() => IsLoading);
            }
        }
        
        public BaseViewModel()
        {
            Debug.Print($"{this.GetType().Name} created");
        }

        public LocaleService Strings => LocaleService.Current;

        protected void SetIsLoading(bool value)
        {
            IsLoading = value;
        }

        public virtual Task ExecuteOnReturn(object data)
        {
            return Task.FromResult(false);
        }

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Use ThreadSafeExecuteOnceAsync instead
        /// </summary>
        /// <param name="awaitableTask"></param>
        /// <returns></returns>
        [Obsolete]
        protected async Task ExecuteOnceAsync(Func<Task> awaitableTask)
        {
            if (IsLoading) return;
            SetIsLoading(true);

            try
            {
                await awaitableTask();
            }
            finally
            {
                SetIsLoading(false);
            }
        }

        public static async Task ThreadSafeExecuteOnceAsync(Func<Task> awaitableTask)
        {
            if (Interlocked.CompareExchange(ref _isCommandInProgress, 1, 0) != 0)
                return;

            try
            {
                await awaitableTask();
            }
            finally
            {
                Interlocked.Exchange(ref _isCommandInProgress, 0);
            }
        }
    }
}
