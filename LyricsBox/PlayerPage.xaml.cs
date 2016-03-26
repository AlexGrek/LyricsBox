using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Search;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace LyricsBox
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class PlayerPage : Page
    {

        public PlayerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var player = CorePlayer.Current;
            if (player.MusicSource != null)
            {
                artistName.Text = player.ShortName();
            }
                
            
            
        }

        private void lrcList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lrcList.ScrollIntoView(lrcList.SelectedItem, ScrollIntoViewAlignment.Leading);
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            settings.Flyout.ShowAt(sender as FrameworkElement);
        }
    }
}
