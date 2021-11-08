using System;

namespace DiscordBot.Twitch.Models
{
    public class TwitchApiToken
    {
        public readonly string Token;
        public readonly string ClientId;
        public readonly string TokenType;
        private readonly DateTime _expirationDateTime;

        public TwitchApiToken(string token, string tokenType, DateTime expirationDateTime, string clientId)
        {
            Token = token;
            TokenType = tokenType;
            _expirationDateTime = expirationDateTime;
            ClientId = clientId;
        }

        public bool IsValid()
        {
            return _expirationDateTime > DateTime.Now;
        }
    }
}