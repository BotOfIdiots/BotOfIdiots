using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using TwitchLib.Client.Events;

namespace DiscordBot.Models
{
    public class TwitchOauth
    {
        private static readonly HttpClient client = new HttpClient();
        private FormUrlEncodedContent RequestContent;

        private string _clientId;
        private string _accessToken;
        private string _tokenType;
        private DateTime _expirationDate;
        public string TokenType;

        public TwitchOauth(string clientId, string secret, string scope = "channel:manage:videos")
        {
            _clientId = clientId;

            var values = new Dictionary<string, string>
            {
                {"client_id", _clientId},
                {"client_secret", secret},
                {"grant_type", "client_credentials"},
                {"scope", scope}
            };

            RequestContent = new FormUrlEncodedContent(values);
        }

        public TwitchApiToken Authenticate()
        {
            PostRequest();
            return new TwitchApiToken(_clientId, _accessToken, _tokenType, _expirationDate);
        }

        private async void PostRequest()
        {
            var httpResponse =
                await client.PostAsync("https://id.twitch.tv/oauth2/token", RequestContent);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseString = await httpResponse.Content.ReadAsStringAsync();

                JObject jSonData = JObject.Parse(responseString);

                _accessToken = jSonData["access_token"].ToString();
                _tokenType = jSonData["token_type"].ToString();
                _expirationDate = SetExpirationDate(Convert.ToInt64(jSonData["expires_in"]));
            }
        }

        private DateTime SetExpirationDate(long expires_in)
        {
            var now = DateTime.Now;
            return new DateTime(now.Ticks + expires_in*1000);
        }
    }
}