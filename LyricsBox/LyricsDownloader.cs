using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net.Http;

namespace LyricsBox
{
    public class LyricsDownloader
    {
        public string Artist { get; private set; }
        public string Title { get; private set; }
        public HtmlDocument Parsed;

        public LyricsDownloader(string artist, string title)
        {
            Artist = artist;
            Title = title;
        }

        public async Task<List<string>> GetLyricsAsync()
        {
            var url = new UriBuilder("http", "search.azlyrics.com");
            var input = Artist.Replace(' ', '+') + "+" + Title.Replace(' ', '+');
            url.Path = "search.php";
            url.Query = "q=" + input;
            var htmlWeb = new HtmlWeb();
            Parsed = await htmlWeb.LoadFromWebAsync(url.Uri.ToString());
            var ans = new List<string>();
            
            return ans;
        }
    }
}
