using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vkaudioposter_ef.parser;

namespace WAAuth.Server.Data
{
    public delegate void PostedTracksDelegate(object sender, PostedTracksChangeEventArgs args);

    public class PostedTracksChangeEventArgs : EventArgs
    {
        public PostedTrack NewPostedTracks { get; }
        public PostedTrack OldPostedTracks { get; }

        public PostedTracksChangeEventArgs(PostedTrack newPostedTracks, PostedTrack oldPostedTracks)
        {
            this.NewPostedTracks = newPostedTracks;
            this.OldPostedTracks = oldPostedTracks;
        }
    }
    public interface IPostedTracksService
    {
        public event PostedTracksDelegate OnPostedTracksChanged;
        IList<PostedTrack> GetPostedTracks();
    }
}
