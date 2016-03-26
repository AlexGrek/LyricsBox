using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources.Core;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace LyricsBox.Models
{
    public class EditorPageViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        int _currentString = -1;

        public int CurrentString
        {
            get { return _currentString; }
        }

        public string SongTitle
        {
            get
            {
                if (CorePlayer.Current.MusicSource != null)
                    return CorePlayer.Current.ShortName();
                else
                    return "No music loaded";
            }
            set
            {
                OnPropertyChanged("SongTitle");
                return;
            }
        }

        public string LyricsText { get
            {
                if (CorePlayer.Current.Lyrics == null)
                {
                    var resource1 = ResourceManager.Current.MainResourceMap.GetValue("Resources/PasteLyricsPlease", ResourceContext.GetForCurrentView());
                    return resource1.ValueAsString;
                }
                else
                    return CorePlayer.Current.Lyrics.RawLyrics;
            }

            set
            {
                if (CorePlayer.Current.Lyrics != null)
                    CorePlayer.Current.Lyrics.UpdateRaw(value);
                else
                    CorePlayer.Current.CreateNewLyrics(value);
                return;
            }
        }

        public void ItemClicked(int i)
        {
            CorePlayer.Current.Lyrics.AddTimeTagTo(i, CorePlayer.Current.Position);
            OnPropertyChanged("LyricsText");
            OnPropertyChanged("LyricsList");
        }

        public string CurrentTime { get
            {
                var moment = CorePlayer.Current.Position;
                return $"{moment.Minutes:D2}:{moment.Seconds:D2}.{moment.Milliseconds:D2}";
            } }

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private double sliderPosition;
        private DispatcherTimer _timer = new DispatcherTimer();

        public EditorPageViewModel()
        {
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += TimerOnTick;
            _timer.Start();
            _canExecute = true;
        }

        private ICommand _playCommand;
        public ICommand PlayCommand
        {
            get
            {
                return _playCommand ?? (_playCommand = new CommandHandler(() => Play(), _canExecute));
            }
        }

        private bool _canExecute;
        public void Play()
        {
            if (CorePlayer.Current.IsPlaying)
                CorePlayer.Current.Pause();
            else
                CorePlayer.Current.Play();
        }

        public double SliderPosition
        {
            get { return sliderPosition; }
            set
            {
                if (sliderPosition != value)
                {
                    sliderPosition = value;
                        var pos = TimeSpan.FromSeconds(CorePlayer.Current.Duration.TotalSeconds * sliderPosition / 100);
                        CorePlayer.Current.Seek(pos);
                    OnPropertyChanged("SliderPosition");
                }
            }
        }

        private void TimerOnTick(object sender, object eventArgs)
        {
                sliderPosition = Math.Min(100, CorePlayer.Current.Position.TotalSeconds * 100 / CorePlayer.Current.Duration.TotalSeconds);
                OnPropertyChanged("SliderPosition");
            if (CorePlayer.Current.Lyrics == null)
                return;

            OnPropertyChanged("CurrentTime");
        }

        public void Dispose()
        {
            _timer.Stop();
        }

        public ICollection<string> LyricsList { get
            {
                if (CorePlayer.Current.Lyrics != null)
                    return CorePlayer.Current.Lyrics.RawStrings;
                else
                    return new List<string>();
            }
        }
    }

}
