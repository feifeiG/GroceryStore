using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerSocket
{
    private static ServerSocket instance = null;
    private Socket socket;
    private Dictionary<Socket, ByteCache> cache_map = new Dictionary<Socket, ByteCache>();
    private CustomDictionary<int, Socket> socket_map = new CustomDictionary<int, Socket>();
    private CustomDictionary<int, Socket> temp_socket_map = new CustomDictionary<int, Socket>();
    private ProtocolDispatcher protocol_dispatcher = new ProtocolDispatcher();
    private int max_connect_id = 0;

    public static ServerSocket Getinstance()
    {
        if (instance == null)
        {
            instance = new ServerSocket();
        }
        return instance;
    }

    public void Init()
    {
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.socket.Bind(new IPEndPoint(IPAddress.Parse(Const.IpStr), Const.Port));
        this.socket.Listen(10);
        LogTool.Tip("服务端Socket创建成功:" + this.socket.LocalEndPoint.ToString());

        Thread thread = new Thread(ConnectListen);
        thread.Start();
    }

    public void Release()
    {
        if(this.socket != null) this.socket.Close();
    }

    public void AddHandler(Type type, Handler handler)
    {
        this.protocol_dispatcher.AddHandler(type, handler);
    }

    public void RecordSocket(int connect_id, int player_id)
    {
        this.socket_map.Add(player_id, this.temp_socket_map.GetValue(connect_id));
        this.temp_socket_map.Remove(connect_id);
    }

    private Socket GetSocket(int connect_id)
    {
        Socket socket = null;
        if (this.socket_map.HaveKey(connect_id))
        {
            socket = this.socket_map.GetValue(connect_id);
        }
        else if (this.temp_socket_map.HaveKey(connect_id))
        {
            socket = this.temp_socket_map.GetValue(connect_id);
        }
        return socket;
    }

    private int CreateConnectId()
    {
        if(++this.max_connect_id > Const.MAX_CONNECT_ID)
        {
            this.max_connect_id = 1;
        }
        return this.max_connect_id;
    }

    private void ConnectListen()
    {
        while (true)
        {
            Socket client_socket = socket.Accept();
            client_socket.ReceiveBufferSize = Const.BUFFSIZE;
            client_socket.SendBufferSize = Const.BUFFSIZE;
            this.temp_socket_map.Add(this.CreateConnectId(), client_socket);
            LogTool.Tip("客户端Socket连接成功:" + client_socket.RemoteEndPoint.ToString());

            Thread thread = new Thread(RecieveMessage);
            thread.Start(client_socket);
        }
    }

    private void RecieveMessage(object client)
    {
        Socket client_socket = (Socket)client;
        while (true)
        {
            try
            {
                byte[] data = new byte[Const.BUFFSIZE];
                int length = client_socket.Receive(data);
                LogTool.Tip("服务端接收消息:" + client_socket.RemoteEndPoint.ToString() + "长度为:" + length);

                if(length > 0)
                {
                    if (!cache_map.ContainsKey(client_socket))
                    {
                        cache_map[client_socket] = new ByteCache();
                    }
                    ByteCache cache = cache_map[client_socket];
                    cache.AddBytes(data, length);

                    while (true)
                    {
                        byte[] msg_bytes = cache.DivideMessage();
                        if (msg_bytes == null)
                        {
                            break;
                        }

                        IMessage protocol = Protocol.Decode(msg_bytes);
                        if (protocol != null)
                        {
                            int connect_id;
                            if(this.socket_map.HaveValue(client_socket)){
                                connect_id = this.socket_map.GetKey(client_socket);
                            }
                            else if(this.temp_socket_map.HaveValue(client_socket)){
                                connect_id = this.temp_socket_map.GetKey(client_socket);
                            }
                            else
                            {
                                Exception e = new Exception("Socket对应的连接ID不存在!!!");
                                throw e;
                            }

                            this.DispatchProtocol(protocol, connect_id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogTool.Tip(e.StackTrace);
                client_socket.Shutdown(SocketShutdown.Both);
                client_socket.Close();
                break;
            }
        }
    }

    public void DispatchProtocol(IMessage protocol, int connect_id)
    {
        this.protocol_dispatcher.Dispatch(protocol, connect_id);
    }

    public void SendMessage(IMessage protocol, int connect_id)
    {
        if(connect_id == Const.LOCAL_CONNECT_ID)
        {
            ClientSocket.Getinstance().DispatchProtocol(protocol);
        }
        else
        {
            byte[] msg_bytes = Protocol.Encode(protocol);
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(msg_bytes.Length);
            buffer.WriteBytes(msg_bytes);

            Socket socket = this.GetSocket(connect_id);
            if (socket == null)
            {
                LogTool.Tip("玩家Socket不存在:" + connect_id);
                return;
            }
            socket.Send(buffer.ToBytes());
        }
    }
}
