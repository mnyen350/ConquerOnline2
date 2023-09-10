using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ConquerServer.Network;

namespace ConquerServer.Client
{
    public class Inventory : IEnumerable<Item>
    {
        public const int MaxSize = 40;

        private List<Item> _items;

        public GameClient Owner { get; private set; }
        public IReadOnlyList<Item> Items { get { return _items; } }
        
        public Inventory(GameClient client) 
        {
            Owner = client;
            _items = new List<Item>(MaxSize);
        }

        public IEnumerator<Item> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public bool TryAdd(Item item)
        {
            if (Items.Count >= 40) return false; 
            Add(item); 
            return true; 
        }


        private void Add(Item item)
        {
            /*
             * picking things up
             * buying items
             * trading items
             */

            //duplicate item id, throw error + disconnect player
            SanityHelper.Validate(() => !_items.Any(i => i.Id == item.Id), "A duplicate item id is being added to the inventory.");

            //add item to inventory
            _items.Add(item);

            //change item owner
            item.Owner = this.Owner;
            item.Position = ItemPosition.Inventory;

            //update player via packet
            Owner.SendItemInfo(item, ItemInfoAction.AddItem);

        }

        public bool TryRemove(int itemId)
        {
            Item? item = Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null) return false;
            _items.Remove(item);
            Owner.SendItemUse(ItemAction.Drop, itemId);
            return true;
        }
    }
}
