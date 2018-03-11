using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private void Awake()
    {
        PathTool.SetRootPath(Application.persistentDataPath);
    }

    private void StartGame()
    {
        Server.Getinstance().Init();
        Client.Getinstance().Init();
    }

    private void StartOnlineGame()
    {
        Server.Getinstance().Init();
        Server.Getinstance().InitSocket();

        Client.Getinstance().Init();
        Client.Getinstance().InitSocket();
    }

    private void OnApplicationQuit()
    {
        Client.Getinstance().Release();
        Server.Getinstance().Release();
    }
}
