using System;
using System.Linq;
using System.Windows.Input;
using LedMatrixIde.Helpers;
using LedMatrixIde.Views;
using Prism.Commands;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LedMatrixIde.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private NavigationView _navigationView;

        public ICommand ItemInvokedCommand { get; }

        public ShellViewModel(INavigationService navigationServiceInstance)
        {
            _navigationService = navigationServiceInstance;
            this.ItemInvokedCommand = new DelegateCommand<NavigationViewItemInvokedEventArgs>(this.OnItemInvoked);
        }

        private NavigationViewItem _selected;
        public NavigationViewItem Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                this.SetProperty(ref _selected, value);
            }
        }

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get
            {
                return _isPaneOpen;
            }
            set
            {
                this.SetProperty(ref _isPaneOpen, value);
            }
        }

        public void Initialize(Frame frame, NavigationView navigationView)
        {
            _navigationView = navigationView;
            frame.Navigated += this.Frame_Navigated;
        }

        private void OnItemInvoked(NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                _navigationService.Navigate("Settings", null);
                return;
            }

            NavigationViewItem item = _navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(menuItem => (string)menuItem.Content == (string)args.InvokedItem);

            string pageKey = item.GetValue(NavigationHelper.NavigateToProperty) as string;

            _navigationService.Navigate(pageKey, null);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(SettingsPage))
            {
                this.Selected = _navigationView.SettingsItem as NavigationViewItem;
                return;
            }

            this.Selected = _navigationView.MenuItems
                            .OfType<NavigationViewItem>()
                            .FirstOrDefault(menuItem => this.IsMenuItemForPageType(menuItem, e.SourcePageType));
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            string sourcePageKey = sourcePageType.Name;
            sourcePageKey = sourcePageKey.Substring(0, sourcePageKey.Length - 4);
            string pageKey = menuItem.GetValue(NavigationHelper.NavigateToProperty) as string;
            return pageKey == sourcePageKey;
        }
    }
}
