using Google.Protobuf;
using Logic;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataTool {
    //LitJson
    //若包含Dictionary结构，则key的类型必须是string，而不能是int类型（如需表示id等），否则无法正确解析！
    //若需要小数，要使用double类型，而不能使用float，可后期在代码里再显式转换为float类型。

    public static string SaveAsJSON<T>(T obj)
    {
        return LitJson.JsonMapper.ToJson(obj);
    }

    public static T LoadFromJSON<T>(string json)
    {
        return LitJson.JsonMapper.ToObject<T>(json);
    }

    public static void SavePlayer(int player_id, PlayerStruct player_struct)
    {
        //FileStream fs = File.Create(PathTool.GetPlayerSavePath(player_id));
        //BinaryFormatter bf = new BinaryFormatter();
        //bf.Serialize(fs, player_struct);
        //file.Close();


        //string json = SaveAsJSON(player_struct);
        //LogTool.Tip("player_id:" + player_id + " SavePlayer JSON: " + json);

        //FileStream fs = File.Create(PathTool.GetPlayerSavePath(player_id));
        //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        //fs.Write(bytes, 0, bytes.Length);
        //fs.Close();

        LogTool.Tip(PathTool.GetPlayerSavePath(player_id));
        Common.Protobuf.PlayerStruct mPlayerStruct = new Common.Protobuf.PlayerStruct();

        mPlayerStruct.ItemData.Grid = player_struct.item_data.grid;
        foreach (KeyValuePair<string, int> pair in player_struct.item_data.item_list)
        {
            mPlayerStruct.ItemData.ItemList[pair.Key] = pair.Value;
        }
        FileStream file = File.Create(PathTool.GetPlayerSavePath(player_id));

        mPlayerStruct.WriteTo(file);
    }

    public static PlayerStruct LoadPlayer(int player_id)
    {
        string save_path = PathTool.GetPlayerSavePath(player_id);
        if (PathTool.IsExistFile(save_path))
        {
            //FileStream fs = File.Open(save_path, FileMode.Open);
            //BinaryFormatter bf = new BinaryFormatter();
            //PlayerStruct player_struct = (PlayerStruct)bf.Deserialize(fs);
            //file.Close();


            //FileStream fs = File.Open(save_path, FileMode.Open);
            //StreamReader sr = new StreamReader(fs);
            //string json = sr.ReadToEnd();
            //LogTool.Tip("player_id:" + player_id + " LoadPlayer JSON: " + json);

            //PlayerStruct player_struct = LoadFromJSON<PlayerStruct>(json);


            FileStream file = File.Open(save_path, FileMode.Open);
            Common.Protobuf.PlayerStruct mPlayerStruct = Common.Protobuf.PlayerStruct.Parser.ParseFrom(file);
            PlayerStruct player_struct = new PlayerStruct();
            player_struct.item_data.grid = mPlayerStruct.ItemData.Grid;
            foreach (KeyValuePair<string, int> pair in mPlayerStruct.ItemData.ItemList)
            {
                player_struct.item_data.item_list[pair.Key] = pair.Value;
            }

            return player_struct;
        }
        else
        {
            return null;
        }
    }
}
