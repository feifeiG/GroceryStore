using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public enum LogicIdx:int
    {
        Item = 1,
        Stop = 2,
    }

    [System.Serializable]
    public class DataBase
    {

    }

    public class LogicBase
    {
        protected Module module;
        protected Player player;

        public LogicBase(Module module, Player player){
            this.module = module;
            this.player = player;
        }

        public virtual void Init(PlayerStruct player_struct) { }
        public virtual void Save(PlayerStruct player_struct) { }

        protected void Log(string func_name, string desc)
        {
            this.player.Log(this.module, func_name, desc);
        }
    }
}
