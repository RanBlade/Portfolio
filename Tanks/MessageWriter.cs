using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class MessageWriter
{
    protected BinaryMessage m_adapterMessage;
    public void InitAdapter(ref BinaryMessage t_bmsg)
    {
        m_adapterMessage = t_bmsg;
    }

    public void AddInt2(short t_val)
    {
        byte[] t_intBytes = BitConverter.GetBytes(t_val);
        m_adapterMessage.AddToMessage(ref t_intBytes, sizeof(short));
    }
    public void AddInt2u(ushort t_val)
    {
        byte[] t_intBytes = BitConverter.GetBytes(t_val);
        m_adapterMessage.AddToMessage(ref t_intBytes, sizeof(ushort));
    }
    public void AddInt4(int t_val)
    {
        byte[] t_intBytes = BitConverter.GetBytes(t_val);
        m_adapterMessage.AddToMessage(ref t_intBytes, sizeof(int));
    }
    public void AddInt4u(uint t_val)
    {
        byte[] t_intBytes = BitConverter.GetBytes(t_val);
        m_adapterMessage.AddToMessage(ref t_intBytes, sizeof(uint));
    }
    public void AddInt8(long t_val)
    {
        byte[] t_intBytes = BitConverter.GetBytes(t_val);
        m_adapterMessage.AddToMessage(ref t_intBytes, sizeof(long));
    }
    public void AddInt8u(ulong t_val)
    {
        byte[] t_intBytes = BitConverter.GetBytes(t_val);
        m_adapterMessage.AddToMessage(ref t_intBytes, sizeof(ulong));
    }
    public void AddFloat(float t_val) 
    {
        byte[] t_floatBytes = BitConverter.GetBytes(t_val);
        m_adapterMessage.AddToMessage(ref t_floatBytes, sizeof(float));
    }
    public void AddDouble(double t_val)
    {
        byte[] t_doubleBytes = BitConverter.GetBytes(t_val);
        m_adapterMessage.AddToMessage(ref t_doubleBytes, sizeof(double));
    }
    public void AddStr(string t_val)
    {

        char[] stringArray = t_val.ToCharArray();
        byte[] t_lenBytes = BitConverter.GetBytes(stringArray.Length);
        byte[] t_stringBytes = new byte[stringArray.Length];
        for (int i = 0; i < t_stringBytes.Length; i++)
        {
            t_stringBytes[i] = (byte)stringArray[i];
        }
        m_adapterMessage.AddToMessage(ref t_lenBytes, sizeof(ushort));
        m_adapterMessage.AddToMessage(ref t_stringBytes, t_stringBytes.Length);
        //byte[] t_stringBytes = BitConverter.GetBytes( t_val );
        //m_adapterMessage.AddToMessage( t_stringBytes , t_stringBytes.Length );

        //Encoding ascii = Encoding.ASCII;
        //Encoding unicode = Encoding.Unicode;
        //byte[] t_stringBytes = unicode.GetBytes(t_val);
        //byte[] t_asciiStringBytes = Encoding.Convert(unicode, ascii, t_stringBytes);
        //byte[] t_lenBytes = BitConverter.GetBytes(t_asciiStringBytes.Length);
    }

    //public void AddMSG( ushort t_msg );
    //public void AddSender( ushort t_sender );	
};
