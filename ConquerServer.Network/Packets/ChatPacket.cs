using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Network.Packets
{
    [Flags]
    public enum ChatStyleFlags
    {
        None = 0
    }
    public enum ChatMode
    {
        Normal = 2000,
        Whisper = 2001,
        Action = 2002,
        Team = 2003,
        Guild = 2004,
        System = 2005,
        Family = 2006,
        Talk = 2007,
        Yell = 2008,
        Friend = 2009,
        Global = 2010,
        GM = 2011,
        Ghost = 2013,
        Service = 2014,
        Tips = 2015,

        World = 2021,
        Athletic = 2022, // arena qualifier

        GuildAlly = 2025,

        Register = 2100,
        Entrance = 2101,
        Shop = 2102,
        PetTalk = 2103,
        Cryout = 2104,
        Webpage = 2105,
        NewMessage = 2106,
        Task = 2107,

        FeedbackFirst = 2108,
        Feedback = 2109,

        LeaveWord = 2110,
        GuildAnnounceG = 2111,
        GulldAnnounceL = 2114,

        MessageBox = 2112,
        Reject = 2113,

        MsgBoardTrade = 2201,
        MsgBoardFriend = 2202,
        MsgBoardTeam = 2203,
        MsgBoardGuild = 2204,
        MsgBoardOther = 2205,

        Broadcast = 2500,

        Unknown = 2600 // uses [Talk] prefix
    }

    public class ChatPacket : Packet
    {
        public ChatPacket(ChatMode mode, string message)
            : this(mode, 0, 0, "SYSTEM", "ALLUSERS", message)
        {
        }

        public ChatPacket(ChatMode mode, string source, string target, string message)
            : this(mode, 0, 0, source, target, message)
        {
        }

        public ChatPacket(
            ChatMode mode,
            uint timestamp,
            int color,
            string source,
            string target,
            string message,
            ChatStyleFlags style = ChatStyleFlags.None)
            : base(7 * 32 + source.Length + target.Length + message.Length)
        {
            WriteUInt32(TimeStamp.GetTime()); // 5735
            WriteInt32(color); // 8
            WriteInt16((short)mode); // 12
            WriteInt16((short)style); // 14
            WriteUInt32(timestamp); // 16
            WriteInt32(0); // 20 PlayerID
            WriteInt32(0); // 24 ModelID
            WriteStrings(source, target, string.Empty, message, string.Empty, string.Empty, string.Empty);
            Build(PacketType.Chat);
        }
    }
}
