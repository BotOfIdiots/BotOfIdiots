using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace DiscordBot.Twitch.Models
{
    public class TwitchOauth
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly FormUrlEncodedContent _requestContent;

        private string _clientId;
        private string _accessToken;
        private string _tokenType;
        private DateTime _expirationDate;

        public TwitchOauth(string clientId, string secret, string scope = "channel:manage:videos")
        {
            _clientId = clientId;
            
            var values = new Dictionary<string, string>
            {
                {"client_id", clientId},
                {"client_secret", secret},
                {"grant_type", "client_credentials"},
                {"scope", scope}
            };

            _requestContent = new FormUrlEncodedContent(values);
        }

        public TwitchApiToken Authenticate()
        {
            PostRequest();
            return new TwitchApiToken(_accessToken, _tokenType, _expirationDate, _clientId);
        }

        private async void PostRequest()
        {
            var httpResponse =
                await Client.PostAsync("https://id.twitch.tv/oauth2/token", _requestContent);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseString = await httpResponse.Content.ReadAsStringAsync();

                JObject jSonData = JObject.Parse(responseString);

                _accessToken = jSonData["access_token"].ToString();
                _tokenType = jSonData["token_type"].ToString();
                _expirationDate = SetExpirationDate(Convert.ToInt64(jSonData["expires_in"]));
            }
        }

        private DateTime SetExpirationDate(long expiresIn)
        {
            var now = DateTime.Now;
            return new DateTime(now.Ticks + expiresIn*1000);
        }
    }
}