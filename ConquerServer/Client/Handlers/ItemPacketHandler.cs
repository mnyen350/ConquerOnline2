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
        [Network(PacketType.Item)]
        private void ItemPacketHandler(Packet p)
        {
            /*
             * this apparently has a million things it could be
             * how thrilling
             */
            uint timestamp = p.ReadUInt32(); // 5735
            int id = p.ReadInt32(); //item's id
            int data = p.ReadInt32();
            var action = (ItemAction)p.ReadInt32(); //equiping is a ITEMACTION.USE


            if (action == ItemAction.Use)
            {
                //also applies to non-equipment like consumables (potions/scroll)
                //data = "3" -> what slot to equip to -> to be ignored 


                //find in inv using id -> grab item.attributeid
                Item? item = this.Inventory.Items.FirstOrDefault(i => i.Id == id);
                if (item == null)
                {
                    SendSystemMessage("Unable to equip item, item not in inventory");
                    return;
                }
                //try equiping
                this.Equipment.EquipFromInventory(item.Id);

            }
            else if(action == ItemAction.Unequip) 
            {
                if (!this.Equipment.UnequipToInventory((ItemPosition)data))
                {
                    SendSystemMessage("Unable to unequip item, inventory may be full.");
                }
            }
            /*
            //equip the item
            this.SendItemUse(ItemAction.Equip, test.Id, 0, (int)ItemPosition.Set1Garment);
        */
        }    
    }
}
