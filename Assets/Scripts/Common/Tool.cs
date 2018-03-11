using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogTool {
    public static void Tip(object tip)
    {
        Debug.Log(tip);
    }

    public static void Log(string file_name, string str)
    {
        Debug.Log("file_name:" + file_name + " tip:" + str);
    }
}
