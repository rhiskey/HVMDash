using HVMDash.Server.Context;
using HVMDash.Shared;
using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVMDash.Server.Service
{
    static class UpdatePlaylists
    {
        public static async Task UpdatePlaylistsTask(PlaylistContext context)
        {
            var allPlaylists = context.Playlists.ToList();
            foreach(var pl in allPlaylists.ToList())
            {
                var uri = pl.PlaylistId;
                var id = Formatters.GetIdFromSpotifyUri(uri);

                //Get Playlist Info from api
                string name = "";
                FullPlaylist playlist = null;
                Followers flwrs = null;
                List<Image> images = null;

                var config = SpotifyClientConfig
                            .CreateDefault()
                            .WithAuthenticator(new ClientCredentialsAuthenticator(Program.SPOTIFY_CLIENT_ID, Program.SPOTIFY_CLIENT_SECRET));

                var api = new SpotifyClient(config);

                try
                {
                    playlist = await api.Playlists.Get(id);
                    name = playlist.Name;
                    images = playlist.Images;
                    flwrs = playlist.Followers;

                    pl.ImageUrl = images.First().Url;
                    pl.FollowersTotal = flwrs.Total;
                    pl.UpdateDate = DateTime.Now;

                    context.Entry(pl).State = EntityState.Modified;
                    await context.SaveChangesAsync();

                }
                catch (SpotifyAPI.Web.APIException webApiEx) //Something with playlist (NOT FOUND, deleted)
                {
                    var dbPlId = String.Format("spotify:playlist:{0}", id);
                    var playlistInDBbyID = context.Playlists.Where(b => b.PlaylistId == dbPlId).FirstOrDefault();

                    context.Playlists.Remove(playlistInDBbyID);
                    await context.SaveChangesAsync();
                }



            }

        }


    }
}
