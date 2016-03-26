using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Playback;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace LyricsBox.Models
{
    public class PlayerPageViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<LyricString> _normalized;
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
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private double sliderPosition;
        private DispatcherTimer _timer = new DispatcherTimer();

        public PlayerPageViewModel()
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
            if (_normalized == null)
                return;

            if (_currentString < _normalized.Count - 1 && _normalized[_currentString + 1].Tags[0].Time < CorePlayer.Current.Position)
            {
                _currentString++;
                OnPropertyChanged("CurrentString");
            }
        }

        public void Dispose()
        {
            _timer.Stop();
        }

        public ICollection<LyricString> LyricsList { get
            {
                if (_normalized == null && CorePlayer.Current.Lyrics != null)
                    _normalized = CorePlayer.Current.Lyrics.Normalize();
                if (_normalized == null)
                    return new List<LyricString>();
                
                else return _normalized;
            }
        }
    }


    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
