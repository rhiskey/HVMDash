using HVMDash.Server.Context;
using HVMDash.Server.Service;
using HVMDash.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vkaudioposter_ef.Model;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace HVMDash.Server.VK
{
    public class LongPollHandler
    {
        public static VkApi api = new VkApi();
        private static ConfigurationContext _configContext;
        private static string vkapiToken;
        private static string communityToken;
        private static ulong groupID;
        private static string rollbarToken;

        private static ulong ts;
        private static ulong? pts;

        static KeyboardButtonColor agree = KeyboardButtonColor.Positive;
        public static KeyboardButtonColor decine = KeyboardButtonColor.Negative;
        public static KeyboardButtonColor def = KeyboardButtonColor.Default;
        public static KeyboardButtonColor prim = KeyboardButtonColor.Primary;
        static LongPollServerResponse longPoolServerResponse;


        public static async Task LongPollListenerAsync(Configuration _cfg)
        {
            var cfg = _cfg;
            vkapiToken = cfg.KateMobileToken;
            communityToken = cfg.VKCommunityAccessTokenProd;
            rollbarToken = cfg.RollbarDashToken;
            groupID = cfg.GroupIdSpotyShare;

            KeyboardBuilder key = new KeyboardBuilder();

            key.AddButton("Начать", null, agree);
            key.AddButton("Start", null, def);

            MessageKeyboard keyboard = key.Build();

        Start:
            try
            {
            Restart:
                while (true) 
                {
                                       
                    var api = new VkApi();

                    try
                    {
                        await api.AuthorizeAsync(new ApiAuthParams()
                        {
                            AccessToken = vkapiToken, 
                        });
                    }
                    catch (Exception apiAuth) {
                        Logging.ErrorLogging(apiAuth, rollbarToken);
                        goto Restart;
                    }
                    longPoolServerResponse = new LongPollServerResponse();
                    longPoolServerResponse = await api.Groups.GetLongPollServerAsync(groupID); 
          
                    BotsLongPollHistoryResponse poll = null;
                    try
                    {
                        poll = await api.Groups.GetBotsLongPollHistoryAsync(
                           new BotsLongPollHistoryParams()
                           { Server = longPoolServerResponse.Server, Ts = longPoolServerResponse.Ts, Key = longPoolServerResponse.Key, Wait = 25 });
                        pts = longPoolServerResponse.Pts;
                        if (poll?.Updates == null) continue; 
                    }
                    catch (Exception ex)
                    {
                        Logging.ErrorLogging(ex, rollbarToken);
                        goto Restart;
                    }
                    foreach (var a in poll.Updates)
                    {
                        if (a.Type == GroupUpdateType.MessageNew)
                        {
                            string userMessage = a.Message.Text.ToLower();
                            long? userID = a.Message.FromId;
                            string payload = a.Message.Payload;
                            //long? chatID = a.Message.ChatId;
                            //long? msgID = a.Message.Id;
                            //var markAsRead = a.Message.ReadState;
                            
                            //Console.WriteLine(userMessage);
                            //byte[] bytes = Encoding.Default.GetBytes(userMessage);
                            //userMessage = Encoding.UTF8.GetString(bytes);


                            // извлекает первый при сообщении, а нужно все получить, прогнаться по всем
                          
                            //Обработка  входящих сообщений
                            if (payload != null)//если пришло нажатие кнопки
                            {
                                SendMessageAsync("Теперь вы можете импортировать треки из Spotify! https://play.google.com/store/apps/details?id=com.rhiskey.spoty2vkshare", userID, null);

                            }
                            else //Если кнопку не нажимали, написали любое сообщение, отправляем клавиатуру?!
                            {
                                //сделать таймер повторной отправки для конкретного юзера = АНТИСПАМ
                                // Как вариант запоминать предыдущее сообщение, сравнить с новым, если оно повторяется - АТАТА
                                switch (userMessage)
                                {

                                    case "рѕр°с‡р°с‚сњ":
                                        SendMessageAsync("Теперь вы можете импортировать треки из Spotify! https://play.google.com/store/apps/details?id=com.rhiskey.spoty2vkshare", userID, keyboard);
                                        break;
                                    case "рќр°с‡р°с‚сњ": //shit
                                        SendMessageAsync("Теперь вы можете импортировать треки из Spotify! https://play.google.com/store/apps/details?id=com.rhiskey.spoty2vkshare", userID, keyboard);
                                        break;
                                    case "начать": //shit
                                        SendMessageAsync("Теперь вы можете импортировать треки из Spotify! https://play.google.com/store/apps/details?id=com.rhiskey.spoty2vkshare", userID, keyboard);
                                        break;
                                    case "start":
                                        SendMessageAsync("Теперь вы можете импортировать треки из Spotify! https://play.google.com/store/apps/details?id=com.rhiskey.spoty2vkshare", userID, keyboard);
                                        break;
   
                                }
                            } 

                        }
                    }
                }
            } catch (Exception ex) {
                Logging.ErrorLogging(ex, rollbarToken);
            }
            return;
        }

        private static async void SendMessageAsync(string v, long? userID, MessageKeyboard keyboard)
        {
            var api = new VkApi();

            try
            {
                await api.AuthorizeAsync(new ApiAuthParams()
                {
                    AccessToken = communityToken,
                });
            }
            catch (Exception apiAuth) {
                Logging.ErrorLogging(apiAuth, rollbarToken);
            }

            await api.Messages.SendAsync(new MessagesSendParams
            {
                UserId = userID,
                Message = v,
                Keyboard = keyboard,
                RandomId = DateTime.Now.Ticks

            });
        }
    }
}
