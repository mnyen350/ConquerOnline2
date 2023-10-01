using ConquerServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConquerServer.Client
{
    public partial class GameClient 
    {
        [Network(PacketType.Register)]
        private async Task RegisterPacketHandler(Packet p)
        {
            p.Skip(20);
            var name = p.ReadCString(40);
            p.Skip(8);
            var lookface = p.ReadUInt16();
            var job = p.ReadInt16();
            var accId = p.ReadInt32();

            //----SUMMARY OF PROFESSIONNAME.INI FILE----//

            //within each group of 10, set to the lowest class
            // trojans: 10-15, set player to 10
            // warrior: 20-25
            // archer: 40-45 
            // ninja: 50-55 -> not allowed in game, throw error lol
            // monk: 60-65
            // pirate: 70-75 -> not allowed in game, throw error lol
            // taoist:  100-101
            //                  RIP metal/wood/earth taoists, earth could be a summoner 
            // water taoist: 132/133/134/135
            // fire taoist: 142-143-144-145

            //handles disconnecting player if bad input/data
            SanityHelper.ValidateBeginnerJob(job);

            // /10 for the 10's place,
            //force the registration/CREATECHAR to use the 10/20/30.... value instead of potential
            //bad info: 14/22/35 etc via (job/10)*10;
            short chkedJob = (short)(job / 10);

            // create the character
            await Database.CreateCharacter(Username, name, lookface, (short)(chkedJob * 10));

            // continue with regular login sequence (ANSWER_OK and onwards)
            await StartLoginSequence();
        }
    }
}
