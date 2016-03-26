using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LyricsBox
{
    class Lyrics: IList<LyricString>
    {
        List<LyricString> _lyrics = new List<LyricString>();
        public string RawLyrics { get; private set; }
        public ObservableCollection<string> RawStrings { get; private set; } = new ObservableCollection<string>();

        public StorageFile Source { get; set; }

        LyricString IList<LyricString>.this[int index]
        {
            get
            {
                return _lyrics[index];
            }

            set
            {
                _lyrics[index] = value;
            }
        }

        public Lyrics(string str)
        {
            RawLyrics = str;
            var strs = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            
            foreach (var s in strs)
            {
                RawStrings.Add(s);
                _lyrics.Add(new LyricString(s));
            }
        }

        public Lyrics(IEnumerable<string> strs)
        {
            foreach (var s in strs)
            {
                RawStrings.Add(s);
                _lyrics.Add(new LyricString(s));
                RawLyrics = RawLyrics + Environment.NewLine + s;
            }
        }

        public void AddTimeTagTo(int index, TimeSpan moment)
        {
            if (index >= RawStrings.Count || index < 0)
                return;
            var tag = new TimeTag(moment).ToString();
            

            _lyrics[index].Tags.Add(new TimeTag(moment));
            RawStrings[index] = _lyrics[index].ToString();

            RawLyrics = "";
            foreach (var s in RawStrings)
            {
                RawLyrics = RawLyrics + s + Environment.NewLine;
            }
        }

        public void UpdateRaw(string str)
        {
            RawLyrics = str;
            var strs = str.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            RawStrings.Clear();
            _lyrics.Clear();
            foreach (var s in strs)
            {
                RawStrings.Add(s);
                _lyrics.Add(new LyricString(s));
            }
        }

        #region IList realization
        int ICollection<LyricString>.Count
        {
            get
            {
                return _lyrics.Count;
            }
        }

        bool ICollection<LyricString>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<LyricString>.Add(LyricString item)
        {
            _lyrics.Add(item);
        }

        void ICollection<LyricString>.Clear()
        {
            _lyrics.Clear();
        }

        bool ICollection<LyricString>.Contains(LyricString item)
        {
            return _lyrics.Contains(item);
        }

        void ICollection<LyricString>.CopyTo(LyricString[] array, int arrayIndex)
        {
            _lyrics.CopyTo(array, arrayIndex);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _lyrics.GetEnumerator();
        }

        IEnumerator<LyricString> IEnumerable<LyricString>.GetEnumerator()
        {
            return _lyrics.GetEnumerator();
        }

        int IList<LyricString>.IndexOf(LyricString item)
        {
            return _lyrics.IndexOf(item);
        }

        void IList<LyricString>.Insert(int index, LyricString item)
        {
            _lyrics.Insert(index, item);
        }

        bool ICollection<LyricString>.Remove(LyricString item)
        {
            return _lyrics.Remove(item);
        }

        void IList<LyricString>.RemoveAt(int index)
        {
            _lyrics.RemoveAt(index);
        }

        #endregion

        public static async Task<Lyrics> GetLyricsFromMusicFileAsync(StorageFile file)
        {
            var props = await file.Properties.GetMusicPropertiesAsync();
            var dict = await props.RetrievePropertiesAsync(new string[] { "System.Music.Lyrics" });
            var retrievedData = dict["System.Music.Lyrics"];
            if (retrievedData != null)
            {
                var mod = new Lyrics(retrievedData.ToString());
                mod.Source = file;
                return mod;
            }
            return null;
        }

        public static async Task<Lyrics> GetLyricsFromLrcFolderAsync(StorageFile file, StorageFolder folder)
        {
            var lyricsFile = (await folder.TryGetItemAsync(file.DisplayName + ".lrc")) as StorageFile;
            if (lyricsFile == null)
                return null;
            string fileContent = null;
            try {
                fileContent = await FileIO.ReadTextAsync(lyricsFile);
            } catch (Exception)
            {
                //invalidate the source, if we cannot read it
                return null;
            }
            var mod = new Lyrics(fileContent);
            mod.Source = await folder.GetFileAsync(file.DisplayName + ".lrc");
            return mod;
        }

        public static async Task<Lyrics> GetLyricsFromFileAsync(StorageFile file)
        {
            string fileContent = null;
            try
            {
                fileContent = await FileIO.ReadTextAsync(file);
            }
            catch (Exception)
            {
                //make another code deal with that
                throw;
            }
            var mod = new Lyrics(fileContent);
            mod.Source = file;
            return mod;
        }

        public static ObservableCollection<LyricString> Normalize(IEnumerable<LyricString> strs)
        {
            var list = new List<LyricString>();
            foreach (var str in strs)
            {
                foreach (var tag in str.Tags)
                    list.Add(new LyricString(tag, str.Text));
            }
            list.Sort();
            return new ObservableCollection<LyricString>(list);
        }

        public ObservableCollection<LyricString> Normalize()
        {
            return Normalize(this);
        }

        public async Task<bool> UpdateSourceAsync()
        {
            if (Source == CorePlayer.Current.MusicSource)
            {
                CorePlayer.Current.Pause();
                var props = await Source.Properties.GetMusicPropertiesAsync();
                var kvp = new KeyValuePair<string, object>("System.Music.Lyrics", RawLyrics);
                await props.SavePropertiesAsync(new KeyValuePair<string, object>[] { kvp });
                return true;
            }
            if (Source != null && Source.FileType == ".lrc")
            {
                await FileIO.WriteTextAsync(Source, RawLyrics);
                return true;
            }
            return false;
        }

        public bool IsSourceAvailable()
        {
            return Source != null;
        }
    }
}
