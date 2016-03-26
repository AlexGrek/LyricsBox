using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace LyricsBox
{
    class CorePlayer
    {
        static CorePlayer _current;
        public static CorePlayer Current
        {
            get
            {
                if (_current != null)
                    return _current;
                else
                {
                    _current = new CorePlayer();
                    return _current;
                }
            }
        }

        public MusicInformation Information { get; private set; } = new MusicInformation();

        public event Action<Exception> ExceptionHandler;

        public void CreateNewLyrics(string val)
        {
            this.Lyrics = new Lyrics(val);
        }

        private bool _playing;
        public StorageFile MusicSource { get; private set; }
        public Lyrics Lyrics { get; private set; }

        public StorageItemThumbnail Thumb { get; set; }

        public bool IsPlaying
        {
            get
            {
                if (!_playing)
                    return false;
                else
                {
                    if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
                        return true;
                    _playing = false;
                    return _playing;
                }
            }
        }

        private async Task<bool> LoadFileAsync(StorageFile music)
        {
            Lyrics = null;
            MusicSource = music;

            try
            {
                BackgroundMediaPlayer.Current.AutoPlay = false;
                BackgroundMediaPlayer.Current.SetFileSource(MusicSource);

                Information = new MusicInformation();

                var props = await music.Properties.GetMusicPropertiesAsync();

                Information.Name = props.Title;
                Information.Artist = props.Artist;
                Information.Album = props.Album;

                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
                return false;
            }
            
        }

        public string ShortName()
        {
            if (MusicSource == null)
                return "No music loaded";
            var std = Information.Artist + " - " + Information.Name;
            if (std.Length < 120)
                return std;
            else
            {
                if (Information.Artist.Length > 60)
                    std = Information.Artist.Substring(0, 57) + "... - ";
                else
                    std = Information.Artist + " - ";
                var std1 = std + Information.Name;
                if (std1.Length < 120)
                    return std1;
                else
                {
                    var diff = 120 - std.Length;
                    std = std + Information.Artist.Substring(0, diff - 4) + "...";
                    return std;
                }

            }
        }

        public async Task<bool> OpenFileAsync(StorageFile music, Lyrics lrc)
        {
            var ans = await LoadFileAsync(music);
            Lyrics = lrc;
            return ans;
        }

        public async Task<bool> OpenFileAsync(StorageFile music)
        {
            var ans = await LoadFileAsync(music);
            return ans;
        }

        public void Play()
        {
            if (IsPlaying)
                return;

            try
            {
                BackgroundMediaPlayer.Current.Play();
                _playing = true;
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        public void Pause()
        {
            if (MusicSource == null)
            {
                if (BackgroundMediaPlayer.Current.CanPause)
                    BackgroundMediaPlayer.Current.Pause();
                return;
            }

            try
            {
                if (BackgroundMediaPlayer.Current.CanPause)
                    BackgroundMediaPlayer.Current.Pause();
                _playing = false;
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        public void Seek(TimeSpan t)
        {
            try
            {
                BackgroundMediaPlayer.Current.Position = t;
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        public void Seek(double seconds)
        {
            try
            { 
                BackgroundMediaPlayer.Current.Position = TimeSpan.FromSeconds(seconds);
            }
            catch (Exception ex)
            {
                ExceptionHandler(ex);
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (MusicSource == null)
                    return TimeSpan.Zero;
                else
                    return BackgroundMediaPlayer.Current.NaturalDuration;
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (MusicSource == null)
                    return TimeSpan.Zero;
                else
                    return BackgroundMediaPlayer.Current.Position;
            }
            set { Seek(value); }
        }
    }
}
