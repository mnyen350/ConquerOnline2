using ConquerServer.Network;
using ConquerServer.Shared;
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
        private async Task ItemPacketHandler(Packet p)
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
                else
                {
                    ItemPosition? overridePosition = null;

                    if (item.EquipPosition == ItemPosition.Set1Weapon1)
                    {
                        if (Profession == Profession.Warrior && item.Sort == ItemSort.Shield) // warrior shields always go into 2nd hand
                            overridePosition = ItemPosition.Set1Weapon2;
                        else if (Profession == Profession.Trojan && Equipment[ItemPosition.Set1Weapon1] != null) // trojan weapons go into 2nd hand if 1st hand used
                            overridePosition = ItemPosition.Set1Weapon2;
                    }

                    this.Equipment.EquipFromInventory(item.Id, overridePosition);

                }
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
