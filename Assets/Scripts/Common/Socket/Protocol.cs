using Common.Protobuf;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Protocol
{
    private static Dictionary<short, MessageParser> ProtocolParser = new Dictionary<short, MessageParser>() {
        {EMsgCode.CS_Login, CSLogin.Parser},
        {EMsgCode.CS_CreatePlayer, CSCreatePlayer.Parser},
        {EMsgCode.CS_LoadPlayer, CSLoadPlayer.Parser},
        {EMsgCode.SC_Login, SCLogin.Parser},
        {EMsgCode.SC_PlayerList, SCPlayerList.Parser},
        {EMsgCode.SC_PlayerInfo, SCPlayerInfo.Parser},


        {EMsgCode.CS_ItemInfo, CSItemInfo.Parser},
        {EMsgCode.CS_ItemUse, CSItemInfo.Parser},
        {EMsgCode.SC_ItemInfo, SCItemInfo.Parser},
    };

    private static Dictionary<Type, short> ProtocolMap = new Dictionary<Type, short>()
    {
        {typeof(CSLogin), EMsgCode.CS_Login},
        {typeof(CSCreatePlayer), EMsgCode.CS_CreatePlayer},
        {typeof(CSLoadPlayer), EMsgCode.CS_LoadPlayer},
        {typeof(SCLogin), EMsgCode.SC_Login},
        {typeof(SCPlayerList), EMsgCode.SC_PlayerList},
        {typeof(SCPlayerInfo), EMsgCode.SC_PlayerInfo},


        {typeof(CSItemInfo), EMsgCode.CS_ItemInfo},
        {typeof(CSItemUse), EMsgCode.CS_ItemUse},
        {typeof(SCItemInfo), EMsgCode.SC_ItemInfo},
    };

    public static void LogProtocolMap(){
        foreach(KeyValuePair<Type, short> pair in ProtocolMap){
            LogTool.Tip("Key:" + pair.Key + " Value:" + pair.Value);
        }
    }

    public static byte[] Encode(IMessage protocol)
    {
        Type type = protocol.GetType();
        if (!ProtocolMap.ContainsKey(type))
        {
            LogTool.Tip("协议类型没有赋予协议号:" + type.ToString());
            return null;
        }
        short msg_code = ProtocolMap[type];

        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteShort(msg_code);
        buffer.WriteBytes(protocol.ToByteArray());
        return buffer.ToBytes();
    }

    public static IMessage Decode(byte[] msg_bytes)
    {
        ByteBuffer buffer = new ByteBuffer(msg_bytes);
        short msg_code = buffer.ReadShort();
        byte[] msg = buffer.ReadBytes((int)buffer.RemainingBytes());

        if (!ProtocolParser.ContainsKey(msg_code))
        {
            LogTool.Tip("未知类型的协议号:" + msg_code);
            return null;
        }
        MessageParser parser = ProtocolParser[msg_code];
        return parser.ParseFrom(msg);
    }
}
