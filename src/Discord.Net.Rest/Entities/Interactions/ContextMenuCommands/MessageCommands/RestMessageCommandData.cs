using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents the data for a <see cref="RestMessageCommand"/>.
    /// </summary>
    public class RestMessageCommandData : RestCommandBaseData, IMessageCommandInteractionData, IDiscordInteractionData
    {
        /// <summary>
        ///     Gets the message associated with this message command.
        /// </summary>
        public RestMessage Message
            => ResolvableData?.Messages.FirstOrDefault().Value;

        /// <inheritdoc/>
        /// <remarks>
        ///     <b>Note</b> Not implemented for <see cref="RestMessageCommandData"/>
        /// </remarks>
        public override IReadOnlyCollection<IApplicationCommandInteractionDataOption> Options
            => throw new System.NotImplementedException();

        internal RestMessageCommandData(DiscordRestClient client, Model model)
            : base(client, model) { }

        internal new static RestMessageCommandData Create(DiscordRestClient client, Model model, RestGuild guild, IRestMessageChannel channel)
        {
            var entity = new RestMessageCommandData(client, model);
            entity.Update(client, model, guild, channel);
            return entity;
        }

        //IMessageCommandInteractionData
        /// <inheritdoc/>
        IMessage IMessageCommandInteractionData.Message => Message;
    }
}
