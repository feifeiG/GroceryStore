using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathTool {
    //数据目录
    //根目录，需要初始化时赋值，因各平台有所不同
    private static string RootPath;

    //角色目录
    private static string PlayerSavePath = "Data/Player";

    public static void SetRootPath(string path)
    {
        RootPath = path;
    }

    public static string GetPlayerSavePath(int player_id)
    {
        string path = string.Format("{0}/{1}/{2}.Data", RootPath, PlayerSavePath, player_id);
        return InitPath(path);
    }

    public static string InitPath(string file_path)
    {
        string path = file_path.Substring(0, file_path.LastIndexOf("/"));
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            LogTool.Tip("Create Path" + path);
        }
        return file_path;
    }

    public static bool IsExistFile(string file_path)
    {
        string path = file_path.Substring(0, file_path.LastIndexOf("/"));
        return Directory.Exists(path) && File.Exists(file_path);
    }
}
