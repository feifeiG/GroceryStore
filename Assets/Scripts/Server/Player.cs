using Logic;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum Module
{
    Player,
    Item,
}

[System.Serializable]
public class PlayerStruct{
    public PlayerData player_data = new PlayerData();
    public ItemData item_data = new ItemData();
}

[System.Serializable]
public class PlayerData
{
    public int id = 0;
    public string name = "";
    public int money = 0;
    public int praise = 0;
}

public class Player {
    private PlayerData player_data = new PlayerData();
    private Dictionary<LogicIdx, LogicBase> logic_list = new Dictionary<LogicIdx, LogicBase>();

    public Player(int id){
        this.player_data.id = id;

        logic_list[LogicIdx.Item] = new Item(Module.Item, this);

        AddProtocolListener();
    }

    private void AddProtocolListener()
    {

    }

    public void Init(PlayerStruct player_struct)
    {
        this.player_data = player_struct.player_data ?? this.player_data;
        foreach (KeyValuePair<LogicIdx, LogicBase> pair in logic_list)
        {
            pair.Value.Init(player_struct);
        }
    }

    public void Save()
    {
        PlayerStruct player_struct = new PlayerStruct();
        player_struct.player_data = this.player_data;
        foreach (KeyValuePair<LogicIdx, LogicBase> pair in logic_list)
        {
            pair.Value.Save(player_struct);
        }

        DataTool.SavePlayer(this.GetId(), player_struct);
    }

    public int GetId()
    {
        return this.player_data.id;
    }

    public string GetName()
    {
        return this.player_data.name;
    }

    public int GetMoney()
    {
        return this.player_data.money;
    }

    public int GetPraise()
    {
        return this.player_data.praise;
    }

    public bool ChangeMoney(int value)
    {
        int have_money = this.player_data.money;
        int new_money = have_money + value;
        if (new_money < 0) return false;

        this.player_data.money = new_money;
        Log(Module.Player, System.Reflection.MethodBase.GetCurrentMethod().Name,
            string.Format("have:{1} new:{2}", have_money, new_money));

        return true;
    }

    public bool ChangePraise(int value)
    {
        int have_praise = this.player_data.praise;
        int new_praise = have_praise + value;
        if (new_praise < 0) return false;

        this.player_data.praise = new_praise;
        Log(Module.Player, System.Reflection.MethodBase.GetCurrentMethod().Name,
            string.Format("have:{1} new:{2}", have_praise, new_praise));

        return true;
    }

    public LogicBase GetLogic(LogicIdx idx)
    {
        return logic_list[idx];
    }

    //格式[模块名:方法名][player[玩家ID 玩家名] 详情]
    public void Log(Module module, string func_name, string desc)
    {
        string str = string.Format("[{0}:{1}][player[{2} {3}] {4}]", module.ToString(), func_name, this.GetId(), this.GetName(), desc);
        LogTool.Log(module.ToString(), str);
    }

    public void Notice(int notice_code)
    {

    }

    public void Login(object obj)
    {

    }
}
