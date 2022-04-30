using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ApplicationCommandInteractionData;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents the base data tied with the <see cref="RestCommandBase"/> interaction.
    /// </summary>
    public class RestCommandBaseData<TOption> : RestEntity<ulong>, IApplicationCommandInteractionData where TOption : IApplicationCommandInteractionDataOption
    {
        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets a collection of <typeparamref name="TOption"/> received with this interaction.
        /// </summary>
        public virtual IReadOnlyCollection<TOption> Options { get; internal set; }

        internal RestResolvableData<Model> ResolvableData;

        internal RestCommandBaseData(BaseDiscordClient client, Model model)
            : base(client, model.Id)
        {
        }

        internal static RestCommandBaseData Create(DiscordRestClient client, Model model, RestGuild guild, IRestMessageChannel channel)
        {
            var entity = new RestCommandBaseData(client, model);
            entity.Update(client, model, guild, channel);
            return entity;
        }

        internal virtual void Update(DiscordRestClient client, Model model, RestGuild guild, IRestMessageChannel channel)
        {
            Name = model.Name;
            if (model.Resolved.IsSpecified && ResolvableData == null)
            {
                ResolvableData = new RestResolvableData<Model>();
                ResolvableData.PopulateAsync(client, guild, channel, model).ConfigureAwait(false);
            }
        }

        IReadOnlyCollection<IApplicationCommandInteractionDataOption> IApplicationCommandInteractionData.Options
            => (IReadOnlyCollection<IApplicationCommandInteractionDataOption>)Options;
    }

    /// <summary>
    ///     Represents the base data tied with the <see cref="RestCommandBase"/> interaction.
    /// </summary>
    public class RestCommandBaseData : RestCommandBaseData<IApplicationCommandInteractionDataOption>
    {
        internal RestCommandBaseData(DiscordRestClient client, Model model)
            : base(client, model) { }
    }
}
