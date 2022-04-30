using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel = Discord.API.ApplicationCommandInteractionData;
using Model = Discord.API.Interaction;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based slash command.
    /// </summary>
    public class RestSlashCommand : RestCommandBase, ISlashCommandInteraction, IDiscordInteraction
    {
        /// <summary>
        ///     Gets the data associated with this interaction.
        /// </summary>
        public new RestSlashCommandData Data { get; private set; }

        internal RestSlashCommand(DiscordRestClient client, Model model)
            : base(client, model)
        {   
        }

        internal new static RestSlashCommand Create(DiscordRestClient client, Model model)
        {
            var entity = new RestSlashCommand(client, model);
            entity.Update(client, model);
            return entity;
        }

        internal override void Update(DiscordRestClient client, Model model)
        {
            base.Update(client, model);

            var dataModel = model.Data.IsSpecified
                ? (DataModel)model.Data.Value
                : null;

            Data = RestSlashCommandData.Create(client, dataModel, Guild, Channel);
        }

        //ISlashCommandInteraction
        /// <inheritdoc/>
        IApplicationCommandInteractionData ISlashCommandInteraction.Data => Data;

        //IApplicationCommandInteraction
        /// <inheritdoc/>
        IApplicationCommandInteractionData IApplicationCommandInteraction.Data => Data;
    }
}
