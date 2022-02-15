using System;

namespace Discord.Interactions
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum CommandTargetType
    {
        None = 0x00,
        SlashCommand = 0x1,
        MessageCommand = 0x2,
        UserCommand = 0x4,
        ButtonInteraction = 0x8,
        SelectMenuInteraction = 0x10,
        ModalInteraction = 0x20,
        AutocompleteInteraction = 0x40
    }
}
