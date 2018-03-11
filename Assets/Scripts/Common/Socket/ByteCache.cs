﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class ByteCache
{
    private ByteBuffer buffer = new ByteBuffer();

    public void AddBytes(byte[] bytes, int len)
    {
        this.buffer.Seek(0, SeekOrigin.End);
        this.buffer.WriteBytes(bytes, len);
        this.buffer.Flush();
    }

    public byte[] DivideMessage()
    {
        byte[] msg_bytes = null;
        this.buffer.Seek(0, SeekOrigin.Begin);
        if(this.buffer.RemainingBytes() > Const.MSG_LEN_BYTES)
        {
            //LogTool.Tip("字节流的长度为:" + this.buffer.RemainingBytes());
            int msg_len = this.buffer.ReadInt();
            if(this.buffer.RemainingBytes() >= msg_len)
            {
                //LogTool.Tip("协议长度为:" + this.buffer.RemainingBytes());
                msg_bytes = this.buffer.ReadBytes(msg_len);
                byte[] left_bytes = this.buffer.ReadBytes((int)this.buffer.RemainingBytes());
                this.buffer.Clear();
                this.buffer.WriteBytes(left_bytes);
                this.buffer.Flush();
            }
            else
            {
                this.buffer.Seek(-Const.MSG_LEN_BYTES, SeekOrigin.Current);
            }
        }
        return msg_bytes;
    }
}
