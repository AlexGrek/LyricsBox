using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LyricsBox
{
    public struct TimeTag: IComparable<TimeTag>
    {
        public enum TagType {ExtendedTag, MiniTag };

        public TimeSpan Time { get; internal set; }

        public TimeTag(TimeSpan value)
        {
            Time = value;
        }

        public TimeTag(string input, TagType type)
        {
            var min = input.Substring(1, 2);
            var sec = input.Substring(4, 2);
            int mins = int.Parse(min);
            int secs = int.Parse(sec);
            int ms = 0;
            if (type == TagType.ExtendedTag)
            {
                var mss = input.Substring(7, 2);
                ms = int.Parse(mss);
            }
            Time = new TimeSpan(0, 0, mins, secs, ms);
        }

        public override string ToString()
        {
            var min = Time.Minutes;
            var sec = Time.Seconds;
            var ms = Time.Milliseconds;
            var mstring = string.Format("{0:D2}", ms);
            if (mstring.Length > 2)
                mstring = mstring.Substring(0, 2);
            return $"[{min:D2}:{sec:D2}.{mstring}]";
        }

        public int CompareTo(TimeTag other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}
