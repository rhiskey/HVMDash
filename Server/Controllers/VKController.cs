using HVMDash.Server.Context;
using HVMDash.Server.Filters;
using HVMDash.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rollbar;
using SpotifyAPI.Web;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using vkaudioposter_ef.Model;
using vkaudioposter_ef.parser;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using HVMDash.Server.VK;

namespace HVMDash.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VKController : ControllerBase
    {
        private readonly PostedTracksContext _context;
        private readonly ConfigurationContext _configContext;
        private readonly VKAccountsContext _vKAccountsContext;
        private readonly FoundTracksContext _foundTracksContext;
        public VKController(PostedTracksContext context, ConfigurationContext configContext, VKAccountsContext vKAccountsContext, FoundTracksContext foundTracksContext)
        {
            _context = context;
            _configContext = configContext;
            _vKAccountsContext = vKAccountsContext;
            _foundTracksContext = foundTracksContext;

        }

        // GET: api/VK?name=123456

        [HttpGet()]
        [RequestRateLimit(Name = "Limit Request Number", Seconds = 1)]
        public async Task<ActionResult<string>> GetVKAudioIdByName(string name)
        {
            string jsonString;
            if (String.IsNullOrWhiteSpace(name))
            {
                return NotFound();
            }

            List<FoundTracks> foundTracksList = new();
            foundTracksList = await _foundTracksContext.FoundTracks.OrderBy(t => t.Trackname).ToListAsync();
            
           
            var isExist = foundTracksList.Exists(x => x.Trackname.Trim().ToLower() == name.Trim().ToLower());
            FoundTracks foundTrack = new();

            if (isExist)
            {
                foundTrack = foundTracksList.Find(x => x.Trackname.Trim().ToLower().Contains(name.Trim().ToLower()));
                jsonString = JsonSerializer.Serialize(foundTrack);
                return CreatedAtAction("GetVKAudioIdByName", new { Name = name, MediaId = foundTrack.MediaId, OwnerId = foundTrack.OwnerId }, jsonString);
            }
            else
            {
                var cfg = await _configContext.Configurations.FirstOrDefaultAsync();
                var accsList = await _vKAccountsContext.VKAccounts.Where(a=>a.Status == true).ToListAsync();
                TrackSearching tr = await SearchVK( name,  cfg,  accsList);
                if (tr.MediaId != null)
                {
                    jsonString = JsonSerializer.Serialize(tr);
                    return CreatedAtAction("GetVKAudioIdByName", new { Name = tr.Trackname, MediaId = tr.MediaId, OwnerId = tr.OwnerId }, jsonString);
                }
                else
                {
                    TrackSearching tr2 = await SearchSpotifyAndUploadVK(name, cfg, accsList);

                    if (tr2.MediaId == null) return NotFound();
                    else
                    {
                        jsonString = JsonSerializer.Serialize(tr2);
                        //Try to download spotify preview and upload to media attachment - return uploaded trackSearching obj
                        return CreatedAtAction("GetVKAudioIdByName", new { Name = tr2.Trackname, MediaId = tr2.MediaId, OwnerId = tr2.OwnerId }, jsonString);
                    }
                }
                //else return NotFound(); //json with status code
            }
        }

        private async Task<TrackSearching> SearchSpotifyAndUploadVK(string name, vkaudioposter_ef.Model.Configuration cfg, List<vkaudioposter_ef.Model.VKAccounts> vKAccounts)
        {
            TrackSearching tr = new();
            String timeStamp = DateTime.Now.ToString();
            var fileName = name + ".mp3"; 

            var config = SpotifyClientConfig
            .CreateDefault()
            .WithAuthenticator(new ClientCredentialsAuthenticator(cfg.SpotifyClientId, cfg.SpotifyClientSecret));

            var spotify = new SpotifyClient(config);
            var search = await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Track, name));
            var downloadUrl = "";

            await foreach (var item in spotify.Paginate(search.Tracks, (s) => s.Tracks))
            {
                downloadUrl = item.PreviewUrl;
                break;
            }

            var idx = downloadUrl.IndexOf("ew/");

            //fileName = downloadUrl.Substring(idx + 3, downloadUrl.Length-(idx+3)) + ".mp3";

            using (var client = new WebClient())
            {
                client.DownloadFile(downloadUrl, fileName);
            }

            //Upload as attach to VK
            var services = new ServiceCollection();
            services.AddAudioBypass();
            var api = new VkApi(services);

            var random = new Random();
        PickRandomAcc:
            //int index = random.Next(vKAccounts.Count);
            //var randAcc = vKAccounts[index];

            var thrustedAcc = vKAccounts.Where(acc => acc.Id == 2).FirstOrDefault();

            try
            {
                api.Authorize(new ApiAuthParams
                {
                    Login = thrustedAcc.VKLogin,
                    Password = thrustedAcc.VKPassword
                });
            }
            catch (Exception ex)
            {
                Logging.ErrorLogging(ex, cfg.RollbarDashToken);
                //goto PickRandomAcc;                 
            }

            var uploadServer = await api.Audio.GetUploadServerAsync();
            try
            {
                WebClient wc = new();
                string responseAudio = Encoding.ASCII.GetString(wc.UploadFile(uploadServer, fileName));
                responseAudio.GetHashCode();
                var track = await api.Audio.SaveAsync(responseAudio);

                tr.MediaId = track.Id;
                tr.OwnerId = track.OwnerId;
                tr.Trackname = name;

                var isEntityExist = await _foundTracksContext.FoundTracks.FirstOrDefaultAsync(e => e.Trackname == name);
                if (isEntityExist != null)
                {
                    return null;
                }
                else
                {
                    FoundTracks ft = new();
                    ft.MediaId = track.Id;
                    ft.OwnerId = track.OwnerId;
                    ft.Trackname = name;
                    //add in db found tracks
                    var created = _foundTracksContext.Add(ft).Entity;
                    var res = await _foundTracksContext.SaveChangesAsync();
                }
            }
            catch (Exception ex) { Logging.ErrorLogging(ex, cfg.RollbarDashToken); }

            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }


            return tr;
        }

        // POST: api/vk/send?MediaId=111111&OwnerId=-2222222&userid=3333333&message=text
        [HttpPost("send")]
        //[RequestRateLimit(Name = "Limit Request Number", Seconds = 1)]
        public async Task<ActionResult<string>> SendVKMessage(string message, long? userid, long? ownerid, long? mediaid)
        {
            string jsonString;
            var cfg = await _configContext.Configurations.FirstOrDefaultAsync();
            var msgId = await SendVK(ref cfg, ref message, ref userid, ref ownerid, ref mediaid);

            VkMessageSendModel msg = new();
            msg.sendMessageId = msgId;

            jsonString = JsonSerializer.Serialize(msg);

            return CreatedAtAction("SendVKMessage", new { MessageId = msgId }, jsonString);
        }

        private async Task<TrackSearching> SearchVK( string name,  vkaudioposter_ef.Model.Configuration configuration,  List<vkaudioposter_ef.Model.VKAccounts> vKAccounts)
        {
            TrackSearching newTrack = new();
            //string apiSearchToken = configuration.ApiUrl;

            var services = new ServiceCollection();
            services.AddAudioBypass();
            var api = new VkApi(services);

            var random = new Random();
        PickRandomAcc:
            int index = random.Next(vKAccounts.Count);
            var randAcc = vKAccounts[index];

            try
            {
                try
                {
                    api.Authorize(new ApiAuthParams
                    {
                        Login = randAcc.VKLogin,
                        Password = randAcc.VKPassword
                    });
                }
                catch (CaptchaNeededException ex)
                {
                    //captchaKey = GettingCaptcha?.Invoke(ex.Img);
                    //captchaSid = ex.Sid;                   
                }
                catch (VkAuthorizationException authEx)
                {
                    randAcc.Status = false;
                    _context.Entry(randAcc).State = EntityState.Modified;
                    await _vKAccountsContext.SaveChangesAsync();
                    Logging.ErrorLogging(authEx, configuration.RollbarDashToken);
                    goto PickRandomAcc;
                }
                catch (UserAuthorizationFailException userAuthEx)
                {
                    randAcc.Status = false;
                    _context.Entry(randAcc).State = EntityState.Modified;
                    await _vKAccountsContext.SaveChangesAsync();
                    Logging.ErrorLogging(userAuthEx, configuration.RollbarDashToken);
                    goto PickRandomAcc;
                }
                catch (System.InvalidOperationException twoFaError)
                {
                    randAcc.Status = false;
                    _context.Entry(randAcc).State = EntityState.Modified;
                    await _vKAccountsContext.SaveChangesAsync();
                    Logging.ErrorLogging(twoFaError, configuration.RollbarDashToken);
                    goto PickRandomAcc;
                }
                catch (AccessTokenInvalidException wrongToken)
                {
                    randAcc.Status = false;
                    _context.Entry(randAcc).State = EntityState.Modified;
                    await _vKAccountsContext.SaveChangesAsync();
                    Logging.ErrorLogging(wrongToken, configuration.RollbarDashToken);
                    goto PickRandomAcc;
                }
                catch (Exception anyEx)
                {
                    //randAcc.Status = false;
                    //_context.Entry(randAcc).State = EntityState.Modified;
                    //await _vKAccountsContext.SaveChangesAsync();
                    Logging.ErrorLogging(anyEx, configuration.RollbarDashToken);
                    goto PickRandomAcc;
                }

                if (api.IsAuthorized)
                {
                    var audios = api.Audio.Search(new AudioSearchParams
                    {
                        Autocomplete = false,
                        Query = name,
                        Count = 10,
                        SearchOwn = false,
                        Sort = AudioSort.AddedDate
                    });

                    if (audios.Count != 0)
                    {
                        string fullTrackName = null;
                        foreach (var audio in audios.ToList())
                        {
                            string allArtists = null;
                            var mp3Url = audio.Url;
                            var mainArtists = audio.MainArtists;
                            string oneArtist = audio.Artist;
                            string trackName = audio.Title;
                            string subTitle = audio.Subtitle;

                            int mainArtistsCount = 0;
                            try { mainArtistsCount = mainArtists.Count(); }
                            catch (Exception ex)
                            {
#if DEBUG
                                //Logging.ErrorLogging(ex);
#endif
                            }
                            if (mainArtistsCount > 1)
                                foreach (var artist in mainArtists)
                                {
                                    if (artist.Name != null)
                                    {
                                        allArtists += " " + artist.Name;
                                        fullTrackName = allArtists + " " + trackName + " " + subTitle;
                                    }
                                    else continue;
                                }
                            else
                            {
                                if (mainArtists != null || oneArtist != null)
                                    fullTrackName = oneArtist + " " + trackName + " " + subTitle;
                                else continue;
                            }

                            int diff = vkaudioposter_Console.Tools.Metrics.LevenshteinDistance(name, fullTrackName);
                            if (diff != -1)
                            {
                                newTrack.Trackname = fullTrackName.Trim();
                                newTrack.OwnerId = audio.OwnerId;
                                newTrack.MediaId = audio.Id;

                                break;
                            }
                            else continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorLogging(ex, configuration.RollbarDashToken);
            }
            return newTrack;
        }

        private Task<long> SendVK(ref vkaudioposter_ef.Model.Configuration configuration, ref string message, ref long? userId, ref long? ownerId, ref long? mediaId)
        {
            var services = new ServiceCollection();
            services.AddAudioBypass();
            var api = new VkApi(services);
            api.Authorize(new ApiAuthParams
            {
                AccessToken = configuration.VKCommunityAccessTokenProd
            });

            List<VkNet.Model.Attachments.MediaAttachment> attachments = new();

            attachments.Add(
               new Audio
               {
                   OwnerId = ownerId,
                   Id = mediaId,
                   AccessKey = configuration.AccessToken,
               });

            //VkNet.Enums.SafetyEnums.Intent intentType = VkNet.Enums.SafetyEnums.Intent.ConfirmedNotification;

            Task<long> res = null;
            try
            {
                res = api.Messages.SendAsync(new MessagesSendParams
                {
                    UserId = userId,
                    Attachments = attachments,
                    Message = message,
                    RandomId = new Random().Next(),
                    DontParseLinks = false,
                });
            } catch (Exception ex)
            {
                Logging.ErrorLogging(ex, configuration.RollbarDashToken);
            }
            return res;
        }



    }

}
