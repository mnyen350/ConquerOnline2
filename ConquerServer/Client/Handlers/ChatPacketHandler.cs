using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public partial class GameClient
    {
        [Network(PacketType.Chat)]

        private void ChatPacketHandler(Packet p)
        {
            var timestamp = p.ReadUInt32(); // 5735
            var color = p.ReadInt32();
            var mode = (ChatMode)p.ReadInt16();
            var style = p.ReadInt16();
            var time = p.ReadInt32();
            var playerId = p.ReadInt32();
            var lookface = p.ReadInt32();
            var messages = p.ReadStrings();

            //string[] 
            //char name, mode/whisp target name, ???, message itself ...

            string messageContents = messages[3];

            if (messageContents.StartsWith("/"))
            {
                

                try
                {
                    //if message starts with / call the S.C.dispatcher
                    if (!DispatchSlash(messageContents.Split(' ')))
                    {
                        //if unable to dispatch command..
                        Console.WriteLine($"Unknown Slash Command");
                    }
                }
                catch (Exception e) 
                {
                    string error = (e.Message.Length <= 255) ? e.Message : (e.Message.Substring(0, 255 - 3) + "...");
                    SendSystemMessage($"{error}");
                }
            }
            else if (mode == ChatMode.Normal)
            {
                //if mode== all chat
                //send to talk packet to everyone in FOV
                using (var talkPacket = new ChatPacket(ChatMode.Normal, Name, string.Empty, messages[3]))
                {
                    FieldOfView.Send(talkPacket);
                }
            }
            else if (mode == ChatMode.Whisper)
            {
                //if mode==whisper
                //find whisp target in world.playrers
                GameClient? target;
                if (!World.TryGetPlayer(messages[1], out target))
                {
                    //if not found, return "player not online to player"
                    SendSystemMessage("Target player is offline");
                }
                else if (target != null) // this should always be true if the "else" hits
                {
                    //else/found, send targetwhisp the talk/whisp packet
                    using (var whisperPacket = new ChatPacket(ChatMode.Whisper, Name, target.Name, messages[3]))
                    {
                        target.Send(whisperPacket);
                    }
                }


            }
            else
            {
                Console.WriteLine(p.Dump("Unknown Talk - " + mode));
            }
        }
    }
}
