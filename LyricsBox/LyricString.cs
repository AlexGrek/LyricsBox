using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LyricsBox
{
    public class LyricString : IComparable<LyricString>
    {
        public static Regex ExtTimeTag = new Regex(@"\[\d\d:\d\d.\d\d\]");
        public static Regex MiniTimeTag = new Regex(@"\[\d\d:\d\d]");

        public string Text {get; set;}
        public List<TimeTag> Tags { get; private set; } = new List<TimeTag>();

        public LyricString(string str)
        {
            var exttags = ExtTimeTag.Matches(str);
            foreach (Match t in exttags)
            {
                Tags.Add(new TimeTag(t.Value, TimeTag.TagType.ExtendedTag));
                str = str.Replace(t.Value, "");
            }
            var mtags = MiniTimeTag.Matches(str);
            foreach (Match t in mtags)
            {
                Tags.Add(new TimeTag(t.Value, TimeTag.TagType.MiniTag));
                str = str.Replace(t.Value, "");
            }
            Text = str;
        }

        public LyricString(TimeTag t, string text)
        {
            Text = text;
            Tags.Add(t);
        }

        public override string ToString()
        {
            var s = "";
            foreach (var tag in Tags)
            {
                s += tag.ToString();
            }
            return s + Text;
        }

        public int CompareTo(LyricString other)
        {
            if (this.Tags.Count == 0 || other.Tags.Count == 0)
                return -1;
            return this.Tags[0].CompareTo(other.Tags[0]);
        }
    }
}
