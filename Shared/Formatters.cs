using System;
using System.Text.RegularExpressions;

namespace HVMDash.Shared
{
    public class Formatters
    {
        public static string FormatDateTime(DateTime dt)
        {
            return dt.ToString("dd.MM.yyyy HH:mm");
        }

        public static string ConvertSpotifyLinkToUri(string link)
        {
            string pattern2 = @"https://open.spotify.com/playlist/\w*";
            string formattedUri = "";
            if (Regex.IsMatch(link, pattern2, RegexOptions.IgnoreCase))
            {
                int iof = link.IndexOf(@"playlist/");
                int start = iof + 9;
                string first = link.Remove(0, start);
                int iof2 = first.IndexOf(@"?si");
                string uri = first.Substring(0, iof2);
                formattedUri = @"spotify:playlist:" + uri;
            }
            return formattedUri;
        }

        public static string GetIdFromSpotifyLink(string link)
        {
            string pattern2 = @"https://open.spotify.com/playlist/\w*";
            string formattedUri = "";
            if (Regex.IsMatch(link, pattern2, RegexOptions.IgnoreCase))
            {
                int iof = link.IndexOf(@"playlist/");
                int start = iof + 9;
                string first = link.Remove(0, start);
                int iof2 = first.IndexOf(@"?si");
                string uri = first.Substring(0, iof2);
                formattedUri = uri;
            }
            return formattedUri;
        }
        public static string GetIdFromSpotifyUri(string uri)
        {
            string spotyUrl = @"spotify:playlist:12345678";
            int index1 = uri.IndexOf("playlist:");
            string newUri = "";
            if (uri.Length != 0)
                newUri = uri.Remove(0, index1 + 9);
            return newUri;
        }
        public static string CreateSpotifyLinkFromUri(string uri)
        {
            string spotyUrl = @"https://open.spotify.com/playlist/";
            int index1 = uri.IndexOf("playlist:");
            string newUri = "";
            if (uri.Length != 0)
                newUri = uri.Remove(0, index1 + 9);
            return spotyUrl + newUri;
        }



        public static string GetPostLink(long? ownId, long id)
        {
            string url = @"https://vk.com/hvmlabel?w=wall";
            return url + $"{ownId}_{id}";

        }

        public static string GetHasTagLink(string msg)
        {
            string url = "https://vk.com/hvmlabel/";
            int dogInd = msg.IndexOf('@');
            msg = msg[1..dogInd];
            return url + msg;
        }
    }
}
