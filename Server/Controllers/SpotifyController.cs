using HVMDash.Server.Context;
using HVMDash.Shared;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HVMDash.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly PlaylistContext _context;
        public SpotifyController(PlaylistContext context)
        {
            _context = context;
        }


        // GET: api/Spotify/name?id=123456
        [HttpGet("name")]
        public async Task<ActionResult<string>> GetSpotify(string id)
        {
            string name = "";
            FullPlaylist playlist = null;
            Followers flwrs = null;
            List<Image> images = null;

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

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
            } catch (SpotifyAPI.Web.APIException webApiEx) //Something with playlist (NOT FOUND, deleted)
            {
                var dbPlId = String.Format("spotify:playlist:{0}", id);
                var playlistInDBbyID = _context.Playlists.Where(b => b.PlaylistId == dbPlId).FirstOrDefault();

                ////var playlistInDB = await _context.Playlists.FindAsync(id);
                if (playlistInDBbyID == null)
                {
                    return NotFound();
                }

                _context.Playlists.Remove(playlistInDBbyID);
                await _context.SaveChangesAsync();

                //return NotFound();
            }

            if (name == null)
            {
                return NotFound();
            }
            SpotifyAPIPlaylist pl2Return = new() { Name = name, Images = images, Followers = flwrs };

            var jsonString = JsonSerializer.Serialize(pl2Return);
            return CreatedAtAction("GetSpotify", new { Name = name, Images = images, Followers = flwrs }, jsonString);
        }

        // GET: api/Spotify/searchname?uri=asdavs1
        [HttpGet("searchname")]
        public async Task<ActionResult<string>> GetSearchName(string uri)
        {
            string name = "";
            if (string.IsNullOrEmpty(uri))
            {
                return NotFound();
            }

            var config = SpotifyClientConfig
                        .CreateDefault()
                        .WithAuthenticator(new ClientCredentialsAuthenticator(Program.SPOTIFY_CLIENT_ID, Program.SPOTIFY_CLIENT_SECRET));

            var api = new SpotifyClient(config);

            var track = await api.Tracks.Get(uri);
            StringBuilder sb = new StringBuilder("", 250);

            foreach (var artist in track.Artists)
            {
                sb.AppendFormat("{0} ", artist.Name);
            }
            sb.AppendJoin(separator: ' ', track.Name);

            name = sb.ToString();

            if (string.IsNullOrEmpty(name))
            {
                return NotFound();
            }
            //var photo = track.
            object toReturn = new { Name = name};
            
            var jsonString = JsonSerializer.Serialize(toReturn);
            return CreatedAtAction("GetSpotify", new { Name = name }, jsonString);
        }

        [HttpGet("oneimgflwrs")]
        public async Task<ActionResult<string>> GetSpotifyImageFlrs(string id)
        {
            string name = "";
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var config = SpotifyClientConfig
                        .CreateDefault()
                        .WithAuthenticator(new ClientCredentialsAuthenticator(Program.SPOTIFY_CLIENT_ID, Program.SPOTIFY_CLIENT_SECRET));

            var api = new SpotifyClient(config);

            var playlist = await api.Playlists.Get(id);
            name = playlist.Name;
            var images = playlist.Images;
            var flwrs = playlist.Followers;

            string imgUrl = "";
            int followers = flwrs.Total;

            foreach(var img in images)
            {
                if(!string.IsNullOrEmpty(img.Url))
                {
                    imgUrl = img.Url;
                }
            }
    
            if (name == null)
            {
                return NotFound();
            }
            SpotifyAPIPlaylistSimple pl2Return = new() { Name = name, Image = imgUrl, Followers = followers };

            var jsonString = JsonSerializer.Serialize(pl2Return);
            return CreatedAtAction("GetSpotify", new { Name = name, Image = imgUrl, Followers = followers }, jsonString);
        }

        //// GET: api/Spotify/name?id=123456
        //[HttpGet("image")]
        //public async Task<ActionResult<string>> GetSpotifyImage(string? id)
        //{
        //    string name = "";
        //    if (id == null || id == "")
        //    {
        //        return NotFound();
        //    }

        //    var config = SpotifyClientConfig
        //                .CreateDefault()
        //                .WithAuthenticator(new ClientCredentialsAuthenticator("9db2bd4bb704465aaf147ad19c1b3ca5", "635b926e660c42219f826647455a00d1"));
        //    //.WithAuthenticator(new ClientCredentialsAuthenticator("CLIENT_ID", "CLIENT_SECRET"));

        //    var api = new SpotifyClient(config);

        //    var playlist = await api.Playlists.Get(id);
        //    name = playlist.Name;
        //    var images = playlist.Images;
        //    var flwrs = playlist.Followers;

        //    //var v = new { Name = name, Images = images, Followers = flwrs };


        //    if (name == null)
        //    {
        //        return NotFound();
        //    }
        //    SpotifyAPIPlaylist pl2Return = new SpotifyAPIPlaylist { Name = name, Images = images, Followers = flwrs };

        //    var jsonString = JsonSerializer.Serialize(pl2Return);
        //    return CreatedAtAction("GetSpotify", jsonString);
        //    ///*Content(v, "application/json", encod);
        //}
    }
}
