using System;
using Discord;
using Discord.Commands;

namespace DiscordBot.Modules

{
    public static class ViolationManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="violationType"> Violation type. 1 = Ban, 2 = Kick, 3 = mute, 4 = warn </param>
        /// <param name="user">User that committed the violation</param>
        /// <param name="reason">Reason for the violation</param>
        /// <param name="context">Command Context</param>
        /// <returns>Embed</returns>
        public static Embed NewViolation(IGuildUser user, string reason, SocketCommandContext context, int violationType = 0)
        {
            try{
                int violationID = 0;
                
                //TODO Ban registration in database
                
                Embed violaitionEmbed = Embed(user, reason, violationID, context, violationType);
                return violaitionEmbed;
            }
            catch (NullReferenceException e)
            {
                Embed error = new EmbedBuilder 
                    {
                        Title = "NullReferenceException"
                    }
                    .WithDescription(e.ToString())
                    .AddField("Time", DateTime.Now)
                    .Build();
                
                return error;
            }
            catch (IndexOutOfRangeException)
            { 
                Embed error = new EmbedBuilder
                    {
                        Title = "Te weinig argumenten"
                    }
                    .WithDescription("Commando is uitgevoerd met te weinig argumenten")
                    .AddField("Example", Program.Config["CommandPrefix"]+"ban [user] {reason}")
                    .Build();
                
                return error;
            }
            catch (Exception e)
            {
                Embed error = new EmbedBuilder
                    {
                        Title = "Exception"
                    }
                    .WithDescription(e.ToString())
                    .AddField("Time", DateTime.Now)
                    .Build();

                return error;
            }
        }

//        private static Embed GetViolation(int violationID)
//        {
//            return Embed(user, reason, violationID, context, violationType);
//        }

        private static Embed Embed(IGuildUser user, string reason, int violationId, SocketCommandContext context, int violationType)
        {
            String violationTitle;
            switch (violationType)
            {
                case 1:
                    violationTitle = "Banned";
                    break;
                case 2:
                    violationTitle = "Kicked";
                    break;
                case 3:
                    violationTitle = "Muted";
                    break;
                default:
                    violationTitle = "Warned";
                    break;
            }

            Embed embed = new EmbedBuilder
                {
                    
                    Title = violationTitle,
                    Color = Color.Red
                }
                .WithAuthor(context.Client.CurrentUser)
                .AddField("User:", user.Mention, true)
                .AddField("Date:", DateTime.Now, true)
                .AddField("Moderator:", context.User.Mention)
                .AddField("Reason:", reason)
                .AddField("Violation ID:", violationId, true)
                .WithCurrentTimestamp()
                .WithFooter("UserID: " + user.Id)
                .Build();

            return embed;
        }
    }
}