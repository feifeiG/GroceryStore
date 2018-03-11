using Common.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Client {
    public static Client instance = null;

    public static Client Getinstance()
    {
        if (instance == null)
        {
            instance = new Client();
        }
        return instance;
    }

    public void Init()
    {
        ClientSocket.Getinstance().AddHandler(typeof(SCLogin), this.SCLogin);
        LogTool.Tip("Client Init");
    }

    public void InitSocket()
    {
        ClientSocket.Getinstance().Init();
        LogTool.Tip("ClientSocket Init");
    }

    public void Release()
    {
        ClientSocket.Getinstance().Release();
        LogTool.Tip("Client Release");
    }

    public void CSLogin()
    {
        CSLogin protocol = new CSLogin()
        {
            Uid = SystemInfo.deviceUniqueIdentifier,
        };

        ClientSocket.Getinstance().SendMessage(protocol);
    }

    public void SCLogin(object data, int connect_id)
    {
        SCLogin protocol = data as SCLogin;
        LogTool.Tip("Login RetCode:" + protocol.Code);
    }
}
