using HVMDash.Server.Context;
using HVMDash.Server.Filters;
using HVMDash.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rollbar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using vkaudioposter_ef.parser;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace HVMDash.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VKController : ControllerBase
    {
        private readonly PostedTracksContext _context;
        private readonly ConfigurationContext _configContext;
        private readonly VKAccountsContext _vKAccountsContext;
        public VKController(PostedTracksContext context, ConfigurationContext configContext, VKAccountsContext vKAccountsContext)
        {
            _context = context;
            _configContext = configContext;
            _vKAccountsContext = vKAccountsContext;
        }

        // GET: api/VK?name=123456

        [HttpGet()]
        [RequestRateLimit(Name = "Limit Request Number", Seconds = 3)]
        public async Task<ActionResult<string>> GetVKAudioIdByName(string name)
        {
            string jsonString;
            if (String.IsNullOrWhiteSpace(name))
            {
                return NotFound();
            }

            List<PostedTrack> postedTracksList = new();
            postedTracksList = await _context.PostedTracks.OrderBy(t => t.Trackname).ToListAsync();
            var isExist = postedTracksList.Exists(x => x.Trackname.Trim().ToLower() == name.Trim().ToLower());
            PostedTrack postedTrack = new();

            if (isExist)
            {
                postedTrack = postedTracksList.Find(x => x.Trackname.Trim().ToLower().Contains(name.Trim().ToLower()));
                jsonString = JsonSerializer.Serialize(postedTrack);
                return CreatedAtAction("GetVKAudioIdByName", new { Name = name, MediaId = postedTrack.MediaId, OwnerId = postedTrack.OwnerId }, jsonString);
            }
            else
            {
                var cfg = await _configContext.Configurations.FirstOrDefaultAsync();
                var accsList = await _vKAccountsContext.VKAccounts.Where(a=>a.Status == true).ToListAsync();
                TrackSearching tr = await SearchVK( name,  cfg,  accsList);
                if (tr != null)
                {
                    jsonString = JsonSerializer.Serialize(tr);
                    return CreatedAtAction("GetVKAudioIdByName", new { Name = tr.Trackname, MediaId = tr.MediaId, OwnerId = tr.OwnerId }, jsonString);
                }
                else return NotFound(); //json with status code
            }
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
                }
                catch (UserAuthorizationFailException userAuthEx)
                {
                    randAcc.Status = false;
                    _context.Entry(randAcc).State = EntityState.Modified;
                    await _vKAccountsContext.SaveChangesAsync();
                    Logging.ErrorLogging(userAuthEx, configuration.RollbarDashToken);
                }
                catch (System.InvalidOperationException twoFaError)
                {
                    randAcc.Status = false;
                    _context.Entry(randAcc).State = EntityState.Modified;
                    await _vKAccountsContext.SaveChangesAsync();
                    Logging.ErrorLogging(twoFaError, configuration.RollbarDashToken);
                }
                catch (AccessTokenInvalidException wrongToken)
                {
                    randAcc.Status = false;
                    _context.Entry(randAcc).State = EntityState.Modified;
                    await _vKAccountsContext.SaveChangesAsync();
                    Logging.ErrorLogging(wrongToken, configuration.RollbarDashToken);
                }

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
