using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LyricsBox
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool _active = false;
        public  static MainPage Current; //to access main page from other pages
        public double SongLength { get; set; }
        

        public MainPage()
        {
            this.InitializeComponent();
            
            Current = this;
        }

        public enum Pages { Home, Player, Editor, Debugger}

        public void NavigateToPage(Pages page)
        {
            if (!_active)
                return;

            /*
            switch (page)
            {
                case Pages.Home:
                    contentFrame.Navigate(typeof(BlankPage));
                    break;
                case Pages.Player:
                    contentFrame.Navigate(typeof(PlayerPage));
                    break;
            }

    */
            switch (page)
            {
                case Pages.Home:
                    hamburgerMenuList.SelectedIndex = 0;
                    break;
                case Pages.Player:
                    hamburgerMenuList.SelectedIndex = 1;
                    break;
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (true)
            {
                this.MySplitView.IsPaneOpen = !this.MySplitView.IsPaneOpen;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _active = true;
            contentFrame.Navigate(typeof(BlankPage));
            CorePlayer.Current.ExceptionHandler += HandleError;
        }

        async void HandleError(Exception ex)
        {
            var dialog = new MessageDialog("Error: " + ex.Message);
            await dialog.ShowAsync();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_active)
                return;
            switch (hamburgerMenuList.SelectedIndex)
            {
                case 0:
                    contentFrame.Navigate(typeof(BlankPage));
                    break;
                case 1:
                    contentFrame.Navigate(typeof(PlayerPage));
                    break;
                case 2:
                    contentFrame.Navigate(typeof(EditorPage));
                    break;
            }

            MySplitView.IsPaneOpen = false;
        }

        
    }
}
