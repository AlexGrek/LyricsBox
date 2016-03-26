using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LyricsBox
{
    class FilePrefs: IDictionary<string, string>
    {
        Dictionary<string, string> _dict = new Dictionary<string, string>();

        #region IDictionary
        public string this[string key]
        {
            get
            {
                return ((IDictionary<string, string>)_dict)[key];
            }

            set
            {
                ((IDictionary<string, string>)_dict)[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return ((IDictionary<string, string>)_dict).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<string, string>)_dict).IsReadOnly;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return ((IDictionary<string, string>)_dict).Keys;
            }
        }

        public ICollection<string> Values
        {
            get
            {
                return ((IDictionary<string, string>)_dict).Values;
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            ((IDictionary<string, string>)_dict).Add(item);
        }

        public void Add(string key, string value)
        {
            ((IDictionary<string, string>)_dict).Add(key, value);
        }

        public void Clear()
        {
            ((IDictionary<string, string>)_dict).Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)_dict).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, string>)_dict).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((IDictionary<string, string>)_dict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return ((IDictionary<string, string>)_dict).GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)_dict).Remove(item);
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, string>)_dict).Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return ((IDictionary<string, string>)_dict).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, string>)_dict).GetEnumerator();
        }

        #endregion IDictionary

        private void Load()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            ApplicationDataContainer filePrefsContainer;
            var fprefsExists = localSettings.Containers.TryGetValue("filePrefs", out filePrefsContainer);
            if (!fprefsExists)
                localSettings.CreateContainer("filePrefs", ApplicationDataCreateDisposition.Always);
            else
            {
                //read the data from filePrefs container
                //TODO: DO IT!
                //filePrefsContainer...
            }
        }
    }
}
