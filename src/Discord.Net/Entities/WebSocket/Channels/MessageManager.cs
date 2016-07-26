﻿using Discord.API.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord
{
    internal class MessageManager
    {
        private readonly DiscordSocketClient _discord;
        private readonly ISocketMessageChannel _channel;

        public virtual IReadOnlyCollection<SocketMessage> Messages 
            => ImmutableArray.Create<SocketMessage>();

        public MessageManager(DiscordSocketClient discord, ISocketMessageChannel channel)
        {
            _discord = discord;
            _channel = channel;
        }

        public virtual void Add(SocketMessage message) { }
        public virtual SocketMessage Remove(ulong id) => null;
        public virtual SocketMessage Get(ulong id) => null;

        public virtual IImmutableList<SocketMessage> GetMany(ulong? fromMessageId, Direction dir, int limit = DiscordRestConfig.MaxMessagesPerBatch)
            => ImmutableArray.Create<SocketMessage>();

        public virtual async Task<SocketMessage> DownloadAsync(ulong id)
        {
            var model = await _discord.ApiClient.GetChannelMessageAsync(_channel.Id, id).ConfigureAwait(false);
            if (model != null)
                return new SocketMessage(_channel, new User(model.Author.Value), model);
            return null;
        }
        public async Task<IReadOnlyCollection<SocketMessage>> DownloadAsync(ulong? fromId, Direction dir, int limit)
        {
            //TODO: Test heavily, especially the ordering of messages
            if (limit < 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (limit == 0) return ImmutableArray<SocketMessage>.Empty;

            var cachedMessages = GetMany(fromId, dir, limit);
            if (cachedMessages.Count == limit)
                return cachedMessages;
            else if (cachedMessages.Count > limit)
                return cachedMessages.Skip(cachedMessages.Count - limit).ToImmutableArray();
            else
            {
                Optional<ulong> relativeId;
                if (cachedMessages.Count == 0)
                    relativeId = fromId ?? new Optional<ulong>();
                else
                    relativeId = dir == Direction.Before ? cachedMessages[0].Id : cachedMessages[cachedMessages.Count - 1].Id;
                var args = new GetChannelMessagesParams
                {
                    Limit = limit - cachedMessages.Count,
                    RelativeDirection = dir,
                    RelativeMessageId = relativeId
                };
                var downloadedMessages = await _discord.ApiClient.GetChannelMessagesAsync(_channel.Id, args).ConfigureAwait(false);

                var guild = (_channel as ISocketGuildChannel)?.Guild;
                return cachedMessages.Concat(downloadedMessages.Select(x =>
                {
                    IUser user = _channel.GetUser(x.Author.Value.Id, true);
                    if (user == null)
                    {
                        var newUser = new User(x.Author.Value);
                        if (guild != null)
                            user = new GuildUser(guild, newUser);
                        else
                            user = newUser;
                    }
                    return new SocketMessage(_channel, user, x);
                })).ToImmutableArray();
            }
        }
    }
}
