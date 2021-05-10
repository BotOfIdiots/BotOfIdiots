using System;

namespace DiscordBot.Models
{
    public class TwitchApiToken
    {
        public string ClientId;
        public string Token;
        public string TokenType;
        public DateTime ExpirationDateTime;

        public TwitchApiToken(string clientId, string token, string tokenType, DateTime expirationDateTime)
        {
            ClientId = clientId;
            Token = token;
            TokenType = tokenType;
            ExpirationDateTime = expirationDateTime;
        }
    }
}