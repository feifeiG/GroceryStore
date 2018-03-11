using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ClientSocket
{
    private static ClientSocket instance = null;
    private Socket socket;
    private ByteCache byte_cache = new ByteCache();
    private ProtocolDispatcher protocol_dispatcher = new ProtocolDispatcher();

    public static ClientSocket Getinstance()
    {
        if (instance == null)
        {
            instance = new ClientSocket();
        }
        return instance;
    }

    public bool IsActive()
    {
        return this.socket != null;
    }

    public void Init()
    {
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.socket.Connect(new IPEndPoint(IPAddress.Parse(Const.IpStr), Const.Port));

        Thread thread = new Thread(RecieveMessage);
        thread.Start();
    }

    public void Release()
    {
        if (this.socket != null) this.socket.Close();
    }

    public void AddHandler(Type type, Handler handler)
    {
        this.protocol_dispatcher.AddHandler(type, handler);
    }

    private void RecieveMessage()
    {
        while (true)
        {
            try
            {
                byte[] data = new byte[Const.BUFFSIZE];
                int length = this.socket.Receive(data);
                LogTool.Tip("客户端接收消息:" + this.socket.RemoteEndPoint.ToString() + "长度为:" + length);

                if (length > 0)
                {
                    byte_cache.AddBytes(data, length);

                    while (true)
                    {
                        byte[] msg_bytes = byte_cache.DivideMessage();
                        if (msg_bytes == null)
                        {
                            break;
                        }

                        IMessage protocol = Protocol.Decode(msg_bytes);
                        if (protocol != null)
                        {
                            this.DispatchProtocol(protocol);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogTool.Tip(e.Message);
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();
                break;
            }
        }
    }

    public void DispatchProtocol(IMessage protocol)
    {
        this.protocol_dispatcher.Dispatch(protocol, Const.UNDEFINED_CONNECT_ID);
    }

    public void SendMessage(IMessage protocol)
    {
        if (!this.IsActive())
        {
            ServerSocket.Getinstance().DispatchProtocol(protocol, Const.LOCAL_CONNECT_ID);
        }
        else
        {
            byte[] msg_bytes = Protocol.Encode(protocol);
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteInt(msg_bytes.Length);
            buffer.WriteBytes(msg_bytes);

            byte[] data = buffer.ToBytes();
            LogTool.Tip("客户端发送消息类型:" + protocol.GetType() + " 长度为:" + data.Length);
            this.socket.Send(data);
        }
    }
}
