using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class MessageReader
{
    public BinaryMessage m_readAdapterMsg;
    public void InitAdapter(ref BinaryMessage t_bmsg)
    {
        m_readAdapterMsg = t_bmsg;
    }


    public void GetInt2(ref short t_val)
    {
        byte[] t_bytes = new byte[sizeof(short)];
        m_readAdapterMsg.GetMessageChunk(ref t_bytes, sizeof(short));
        t_val = BitConverter.ToInt16(t_bytes, 0);
    }
    public void GetInt2u(ref ushort t_val)
    {
        byte[] t_bytes = new byte[sizeof(ushort)];
        m_readAdapterMsg.GetMessageChunk(ref t_bytes, sizeof(ushort));
        t_val = BitConverter.ToUInt16(t_bytes, 0);
    }
    public void GetInt4(ref int t_val)
    {
        byte[] t_bytes = new byte[sizeof(int)];
        m_readAdapterMsg.GetMessageChunk(ref t_bytes, sizeof(int));
        t_val = BitConverter.ToInt32(t_bytes, 0);
    }
    public void GetInt4u(ref uint t_val)
    {
        byte[] t_bytes = new byte[sizeof(uint)];
        m_readAdapterMsg.GetMessageChunk(ref t_bytes, sizeof(uint));
        t_val = BitConverter.ToUInt32(t_bytes, 0);
    }
    public void GetInt8(ref long t_val)
    {
        byte[] t_bytes = new byte[sizeof(long)];
        m_readAdapterMsg.GetMessageChunk(ref t_bytes, sizeof(long));
        t_val = BitConverter.ToInt64(t_bytes, 0);
    }
    public void GetInt8u(ref ulong t_val)
    {
        byte[] t_bytes = new byte[sizeof(ulong)];
        m_readAdapterMsg.GetMessageChunk(ref t_bytes, sizeof(ulong));
        t_val = BitConverter.ToUInt64(t_bytes, 0);
    }
    public void GetFloat( ref float t_val)
    {
        byte[] t_bytes = new byte[sizeof(float)];
        m_readAdapterMsg.GetMessageChunk(ref t_bytes, sizeof(float));
        t_val = BitConverter.ToSingle(t_bytes, 0);
    }
    public void GetDouble( ref double t_val )
    {
        byte[] t_bytes = new byte[sizeof(double)];
        m_readAdapterMsg.GetMessageChunk(ref t_bytes, sizeof(double));
        t_val = BitConverter.ToDouble(t_bytes, 0);
    }
    public void GetStr(ref string t_val)
    {
        byte[] t_stringLenBytes = new byte[sizeof(ushort)];
        m_readAdapterMsg.GetMessageChunk(ref t_stringLenBytes, sizeof(ushort));
        int t_stringLen = BitConverter.ToUInt16(t_stringLenBytes, 0);

        byte[] t_bytes = new byte[t_stringLen];
        char[] t_charToStringBytes = new char[t_stringLen];
        m_readAdapterMsg.GetMessageChunk(ref t_bytes, t_stringLen);

        for (int i = 0; i < t_stringLen; i++)
        {
            t_charToStringBytes[i] = (char)t_bytes[i];
        }
        t_val = new string(t_charToStringBytes);
        //byte[] t_bytes;
        //m_readAdapterMsg.GetMessageChunk( t_bytes , sizeof(short) );
        //t_val = BitConverter.ToInt16( t_bytes );
    }

    //public void GetMSG( ref ushort t_msg );
    //public void GetSender( ref ushort t_sender );
};