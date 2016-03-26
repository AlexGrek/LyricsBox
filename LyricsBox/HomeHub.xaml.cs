using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LyricsBox
{
    public sealed partial class HomeHub : UserControl
    {
        public HomeHub()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<string> Messages { get; set; }

        private StorageFolder _folder;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".flac");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                await CorePlayer.Current.OpenFileAsync(file);
            }

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            folderPicker.FileTypeFilter.Add(".mp3");
            folderPicker.FileTypeFilter.Add(".flac");
            _folder = await folderPicker.PickSingleFolderAsync();
            if (_folder != null)
            {
                var openPicker = new FileOpenPicker();
                openPicker.FileTypeFilter.Add(".mp3");
                openPicker.FileTypeFilter.Add(".flac");
                StorageFile file = await openPicker.PickSingleFileAsync();
                if (file != null)
                {
                    var lyricSourcesAvailable = new Dictionary<string, Lyrics>();
                    var lrcFromFile = await Lyrics.GetLyricsFromMusicFileAsync(file);
                    if (lrcFromFile != null)
                    {
                        lyricSourcesAvailable.Add("Music file tag", lrcFromFile);
                    }
                    if (_folder != null)
                    {
                        var lrcFile = await Lyrics.GetLyricsFromLrcFolderAsync(file, _folder);
                        if (lrcFile != null)
                            lyricSourcesAvailable.Add("File " + file.DisplayName + ".lrc", lrcFile);
                    }
                    var musicLrcFile = await Lyrics.GetLyricsFromLrcFolderAsync(file, KnownFolders.MusicLibrary);
                    if (musicLrcFile != null)
                    {
                        lyricSourcesAvailable.Add("File " + file.DisplayName + ".lrc from media folder", musicLrcFile);
                    }

                    if (lyricSourcesAvailable.Count > 1)
                    {
                        //ask user which source to open
                        Messages = new ObservableCollection<string>(lyricSourcesAvailable.Keys);
                        sourceSelector.ItemsSource = Messages;
                        await popupMultipleSources.ShowAsync();
                        if (sourceSelector.SelectedIndex > -1)
                        {
                            var lrc = lyricSourcesAvailable[sourceSelector.SelectedItem.ToString()];
                            await CorePlayer.Current.OpenFileAsync(file, lrc);
                            MainPage.Current.NavigateToPage(MainPage.Pages.Player);
                        }
                     }
                    else if (lyricSourcesAvailable.Count == 1)
                    {
                        //guaranteed to be one, so take it
                        foreach (var lrc in lyricSourcesAvailable.Values)
                            await CorePlayer.Current.OpenFileAsync(file, lrc);
                        MainPage.Current.NavigateToPage(MainPage.Pages.Player);
                    }
                    else if (lyricSourcesAvailable.Count == 0)
                    {
                        //open without lyrics
                        await CorePlayer.Current.OpenFileAsync(file);
                        MainPage.Current.NavigateToPage(MainPage.Pages.Player);
                    }

                }
            }
        }

        //select media source
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            popupMultipleSources.IsOpen = false;
        }

        //cancel media source selection
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            sourceSelector.SelectedIndex = -1;
            popupMultipleSources.IsOpen = false;
        }
    }

    static class Extensions
    {

        public static Task ShowAsync(this Popup popup)
        {

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            EventHandler<object> onClosed = null;
            onClosed = (s, e) => {
                popup.Closed -= onClosed;
                tcs.SetResult(true);
            };
            popup.Closed += onClosed;
            popup.IsOpen = true;

            return tcs.Task;
        }

    }
}
