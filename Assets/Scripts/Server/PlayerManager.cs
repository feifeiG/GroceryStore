using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager {
    public static PlayerManager instance = null;
    private Dictionary<int, Player> player_list = new Dictionary<int, Player>();

    public static PlayerManager Getinstance()
    {
        if (instance == null)
        {
            instance = new PlayerManager();
        }
        return instance;
    }

    public void AddPlayer(Player player)
    {
        player_list[player.GetId()] = player;
    }

    public Player GetPlayer(int player_id)
    {
        return player_list[player_id];
    }

    public void Save()
    {
        foreach (KeyValuePair<int, Player> pair in player_list)
        {
            pair.Value.Save();
        }
    }
}
