using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate void Handler(object protocol, int connect_id);

public class ProtocolDispatcher
{
    private Dictionary<Type, Handler> handle_map = new Dictionary<Type, Handler>();

    public void AddHandler(Type type, Handler handler)
    {
        handle_map[type] = handler;
    }

    public void Dispatch(IMessage protocol, int connect_id)
    {
        Type type = protocol.GetType();
        if (!handle_map.ContainsKey(type))
        {
            LogTool.Tip("协议类型没有注册处理函数:" + type.ToString());
            return;
        }

        Handler handler = handle_map[type];
        handler(protocol, connect_id);
    }
}

