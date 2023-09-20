using ConquerServer.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConquerServer.Client
{
    public class Equipment :IEnumerable<Item> 
        //equipmentmanager! 
    {
        public GameClient Owner { get; private set; }
        private Dictionary<ItemPosition, Item> _equipped; 

        public Equipment(GameClient client)
        {
            Owner = client;
            _equipped = new Dictionary<ItemPosition, Item>();
        }

        public IEnumerator<Item> GetEnumerator() => _equipped.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _equipped.Values.GetEnumerator();
        /*
         // sum up the stats
            var minAttack = this.Inventory.Sum(i => i.Attributes.MinimumPhysicalAttack);


            // sum the stats up from equipment (i.e. determine min/max attack, defense, etc)
            //health -> enchant on the item
            
            int physicalAttackMax = Equipment
            int physicalAttackMin = Equipment.Sum(e => e.Attributes.MinimumPhysicalAttack);

            int magicAttack = Equipment.

            int physicalDefense
         */
        public int MinPhysicalAttack
        {
            get
            {
                return this.Sum(i => i.Attributes.MinimumPhysicalAttack);
            }
        }
        public int MaxPhysicalAttack
        {
            get
            {
                return this.Sum(e => e.Attributes.MaximumPhysicalAttack);
            }
        }
        public int MagicAttack
        {
            get
            {
                return this.Sum(e => e.Attributes.MagicAttack);
            }
        }
        public int Life
        {
            get
            {
                int enchantment = this.Sum(e => e.Enchant);
                int bonus = this.Sum(e => e.Attributes.PotionAddHP);
                return enchantment + bonus;
            }
        }
        public int Mana
        {
            get
            {
                int enchantment = this.Sum(e => e.Enchant);
                int bonus = this.Sum(e => e.Attributes.PotionAddMP);
                return enchantment + bonus;
            }
        }
        public int PhysicalDefense
        {
            get
            {
                return this.Sum(e => e.Attributes.PhysicalDefense);
            }
        }
        public int MagicDefense
        {
            get
            {
                return this.Sum(e => e.Attributes.MagicDefense);
            }
        }
        public int Dexterity
        {
            get
            {
                return this.Sum(e=> e.Attributes.Dexterity);
            }
        }
        public int Dodge
        {
            get
            {
                return this.Sum(e => e.Attributes.Dodge);
            }
        }
        public int AttackRange
        {
            get
            {
                Item? weaponR, weaponL;
                _equipped.TryGetValue(ItemPosition.Set1Weapon1, out weaponR);
                _equipped.TryGetValue(ItemPosition.Set1Weapon2, out weaponL);

                int range = 1;
                if (weaponR?.Attributes?.Range > 0 && weaponL?.Attributes?.Range > 0)
                    range = ((weaponR.Attributes.Range) + (weaponL.Attributes.Range)) / 2;
                else if (weaponR?.Attributes?.Range > 0)
                    range = weaponR.Attributes.Range;
                else if (weaponL?.Attributes?.Range > 0)
                    range = weaponL.Attributes.Range;

                return range;
            }
        }
        public int AttackRate
        {
            get 
            {
                int rate = 900; //default attack rate of 900ms

                //range for right hand weapon
                int rateR = 0;
                Item? weaponR;
                _equipped.TryGetValue(ItemPosition.Set1Weapon1, out weaponR);
                if (weaponR != null)
                    rateR = weaponR.Attributes.AttackSpeed;

                //range for left hand weapon
                int rateL = 0;
                Item? weaponL;
                _equipped.TryGetValue(ItemPosition.Set1Weapon2, out weaponL);
                if (weaponL != null)
                    rateL = weaponL.Attributes.AttackSpeed;

                if (rateR > 0 && rateL > 0)
                    rate = (rateR + rateL) / 2;
                else if (rateR > 0)
                    rate = rateR;
                else if (rateL > 0)
                    rate = rateL;

                return rate; 
            }
        }

        public Item? GetItem(ItemPosition position)
        {
            Item? equip;
            _equipped.TryGetValue(position, out equip);
            return equip;
        }

        public Item? this[ItemPosition position] //give [] operator shortcut
        {
            get
            {
                return GetItem(position);
            }
        }

        public void Equip(Item item, ItemPosition position, bool update = true)
        {
            _equipped[position] = item;
            item.Owner = Owner;
            item.Position = position;

            Owner.SendItemInfo(item, ItemInfoAction.AddItem);
            Owner.SendItemUse(ItemAction.Equip, item.Id, 0, (int)position);

            if (update)
                Update();
        }
        public void EquipFromInventory(int itemId)
        {
            Item? equipment = Owner.Inventory.FirstOrDefault(i => i.Id == itemId);
            if (equipment == null)
            {
                Owner.SendSystemMessage("Item not found, cannot equip");
                return;
            }

            ItemPosition position = equipment.EquipPosition;

            Owner.Inventory.TryRemove(itemId);

            UnequipToInventory(position);
            Equip(equipment, position);
        }

        public bool UnequipToInventory(ItemPosition position)
        {
            // is there already an item existin at this position?
            Item? existing;
            if (_equipped.TryGetValue(position, out existing))
            {
                // try to add it to the inventory
                if (Owner.Inventory.TryAdd(existing))
                {
                    // if successful in adding it to the inventory...
                    _equipped.Remove(position);
                    Owner.SendItemUse(ItemAction.Unequip, existing.Id, 0, (int)position);

                    Update();
                    return true;
                }
                else
                {
                    Owner.SendSystemMessage("Unable to unequip due to inventory being full");
                }
            }
            return false;
        }
        public void Update()
        {
            var idArray = new int[4 + 13]; // 3 zeroes and 13 equipment places
                                           // idArray[0] should be either 0 or 1 depending on the equipment mode (main/sub)
            for (int i = 1; i < idArray.Length; i++)
                idArray[i] = -1;

            Func<ItemPosition, int> id = (p) => _equipped.ContainsKey(p) ? _equipped[p].Id : -1;

            idArray[4] = id(ItemPosition.Set1Helmet);
            idArray[5] = id(ItemPosition.Set1Necklace);
            idArray[6] = id(ItemPosition.Set1Armor);
            idArray[7] = id(ItemPosition.Set1Weapon1);
            idArray[8] = id(ItemPosition.Set1Weapon2);
            idArray[9] = id(ItemPosition.Set1Ring);
            idArray[10] = id(ItemPosition.Set1Gourd);
            idArray[11] = id(ItemPosition.Set1Boots);
            idArray[12] = id(ItemPosition.Set1Garment);
            idArray[13] = id(ItemPosition.W1Accessory);
            idArray[14] = id(ItemPosition.W2Accessory);
            idArray[15] = id(ItemPosition.Steed);
            idArray[16] = id(ItemPosition.Crop);

            Owner.SendItemUse(ItemAction.Equipment, 0, 0, idArray);
            Owner.RecalculateStats();
        }
    }
}
