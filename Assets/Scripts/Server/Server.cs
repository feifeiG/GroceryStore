using Common.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server {
    private static Server instance = null;
    private CommonData common_data = new CommonData();

    public static Server Getinstance()
    {
        if (instance == null)
        {
            instance = new Server();
        }
        return instance;
    }

    public void Init()
    {
        ServerSocket.Getinstance().AddHandler(typeof(CSLogin), this.CSLogin);
        ServerSocket.Getinstance().AddHandler(typeof(CSCreatePlayer), this.CSCreatePlayer);
        ServerSocket.Getinstance().AddHandler(typeof(CSLoadPlayer), this.CSLoadPlayer);
        LogTool.Tip("Server Init");
    }

    public void InitSocket()
    {
        ServerSocket.Getinstance().Init();
        LogTool.Tip("ServerSocket Init");
    }

    public void Release()
    {
        ServerSocket.Getinstance().Release();
        LogTool.Tip("Server Release");
    }

    private void CSLogin(object data, int connect_id)
    {
        CSLogin protocol = data as CSLogin;
        string uid = protocol.Uid;

        if (this.common_data.UidToPlayerlist.ContainsKey(uid))
        {
            SCPlayerList message = new SCPlayerList();
            message.PlayerList = this.common_data.UidToPlayerlist[uid];
            ServerSocket.Getinstance().SendMessage(message, connect_id);
        }
        else
        {
            SCLogin message = new SCLogin();
            message.Code = NoticeCode.NotExistPlayer;
            ServerSocket.Getinstance().SendMessage(message, connect_id);
        }
    }

    private void CSCreatePlayer(object data, int connect_id)
    {
        CSCreatePlayer protocol = data as CSCreatePlayer;
        string uid = protocol.Uid;
        string name = protocol.Name;

        int player_id = ++this.common_data.MaxPlayerId;
        PlayerList player_list = this.common_data.UidToPlayerlist[uid] ?? new PlayerList();
        this.common_data.UidToPlayerlist[uid] = player_list;
        player_list.IdList.Add(player_id);

        Player player = new Player(player_id);
        PlayerStruct player_struct = new PlayerStruct();
        player_struct.player_data.name = name;
        player.Init(player_struct);

        ServerSocket.Getinstance().RecordSocket(connect_id, player_id);
        PlayerManager.Getinstance().AddPlayer(player);
    }

    private void CSLoadPlayer(object data, int connect_id)
    {
        CSLoadPlayer protocol = data as CSLoadPlayer;
        string uid = protocol.Uid;
        int player_id = protocol.PlayerId;

        PlayerList player_list = this.common_data.UidToPlayerlist[uid];
        if(player_list != null && player_list.IdList.Contains(player_id))
        {
            Player player = new Player(player_id);
            PlayerStruct player_struct = DataTool.LoadPlayer(player_id);
            player.Init(player_struct ?? new PlayerStruct());

            ServerSocket.Getinstance().RecordSocket(connect_id, player_id);
            PlayerManager.Getinstance().AddPlayer(player);
        }
    }

    private void UnLoadPlayer()
    {

    }
}
