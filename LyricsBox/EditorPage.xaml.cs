using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace LyricsBox
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class EditorPage : Page
    {
        public EditorPage()
        {
            InitializeComponent();
        }

        private void lrcList_ItemClick(object sender, SelectionChangedEventArgs e)
        {
            if (CorePlayer.Current.Lyrics == null)
                return;

            var x = this.DataContext as Models.EditorPageViewModel;
            x.ItemClicked(lrcList.SelectedIndex);
        }

        private async void Save_Clicked(object sender, object e)
        {
            if (CorePlayer.Current.Lyrics != null)
            {
                if (CorePlayer.Current.Lyrics.IsSourceAvailable())
                    await CorePlayer.Current.Lyrics.UpdateSourceAsync();
                else
                {
                    saveAs.Flyout.ShowAt(sender as AppBarButton);
                }
            }
        }

        //save file manually
        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (CorePlayer.Current.Lyrics == null)
                return;
            var savePicker = new FileSavePicker();
            savePicker.DefaultFileExtension = ".lrc";
            savePicker.FileTypeChoices.Add("Lyrics file", new List<string>() { ".lrc" });
            savePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            savePicker.SuggestedFileName = CorePlayer.Current.MusicSource.DisplayName + ".lrc";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CorePlayer.Current.Lyrics.Source = file;
                await CorePlayer.Current.Lyrics.UpdateSourceAsync();
            }
        }

        //SaveAs button
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var but = sender as AppBarButton;
            but.Flyout.ShowAt(but);
        }
    }
}
