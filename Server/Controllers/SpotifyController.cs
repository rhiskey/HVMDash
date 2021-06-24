using HVMDash.Shared;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace HVMDash.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        // GET: api/Spotify/name?id=123456
        [HttpGet("name")]
        public async Task<ActionResult<string>> GetSpotify(string id)
        {
            string name = "";
            if (id == null || id == "")
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

            if (name == null)
            {
                return NotFound();
            }
            SpotifyAPIPlaylist pl2Return = new SpotifyAPIPlaylist { Name = name, Images = images, Followers = flwrs };

            var jsonString = JsonSerializer.Serialize(pl2Return);
            return CreatedAtAction("GetSpotify", new { Name = name, Images = images, Followers = flwrs }, jsonString);
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
