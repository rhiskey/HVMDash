using SpotifyAPI.Web;
using System.Collections.Generic;


namespace HVMDash.Shared
{
    public class SpotifyAPIPlaylist
    {
        public string Name { get; set; }
        public List<Image> Images { get; set; }
        public Followers Followers { get; set; }
    }
}
