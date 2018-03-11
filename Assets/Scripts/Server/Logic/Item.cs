using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic{

    [System.Serializable]
    public class ItemData: DataBase
    {
        public int grid = 100;
        public Dictionary<string, int> item_list = new Dictionary<string, int>();
    }

    public class Item : LogicBase
    {
        private ItemData item_data = new ItemData();

        public Item(Module module, Player player) : base(module, player) {}

        public override void Init(PlayerStruct player_struct)
        {
            item_data = player_struct.item_data ?? item_data;
        }

        public override void Save(PlayerStruct player_struct)
        {
            player_struct.item_data = item_data;
        }

        public bool PutIn(int item_id, int num)
        {
            if (num <= 0) return false;

            string key = Convert.ToString(item_id);
            if (!item_data.item_list.ContainsKey(key))
            {
                item_data.item_list[key] = 0;
            }
            int have_num = item_data.item_list[key];
            int new_num = have_num + num;
            item_data.item_list[key] = new_num;

            this.Log(System.Reflection.MethodBase.GetCurrentMethod().Name,
                string.Format("item_id:{0} have_num:{1} new_num:{2}", key, have_num, new_num));

            return true;
        }

        public bool ConsumeItem(int item_id, int num)
        {
            if(num <= 0) return false;

            string key = Convert.ToString(item_id);
            if (!item_data.item_list.ContainsKey(key))
            {
                return false;
            }

            int have_num = item_data.item_list[key];
            if(have_num < num)
            {
                return false;
            }

            int new_num = have_num - num;
            item_data.item_list[key] = new_num;

            this.Log(System.Reflection.MethodBase.GetCurrentMethod().Name, 
                string.Format("item_id:{0} have:{1} new:{2}", key, have_num, new_num));

            return true;
        }
    }
}
