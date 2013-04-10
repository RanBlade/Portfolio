using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


public class BinaryMessage
{
    private ushort m_len;
    public byte[] m_buffer;
    private int m_readPos;
    private int m_writePos;
    //private helper functions ++++++++++++++++++++++++++++++++++++++++
    private void AddMessageLen()
    {
        byte[] t_lenBytes = BitConverter.GetBytes(m_len);
        for (int i = 0; i < sizeof(ushort); i++)
        {
            m_buffer[i] = t_lenBytes[i];
        }
    }
    public void PrintDebugBuffer()
    {
        int count = 0;
        foreach (byte b in m_buffer)
        {
            Debug.Log("MessagePos: " + count.ToString() + " contains: " + b.ToString());
            count++;
        }
    }
    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    //Init function overloads ----------------------------------
    public void Init(int t_size)
    {
        m_len = 0;
        m_buffer = new byte[t_size];
        m_readPos = 2;
        m_writePos = 2;
        AddMessageLen();
    }
    public void Init()
    {
        m_len = 0;
        m_buffer = new byte[1024];
        m_readPos = 2;
        m_writePos = 2;
        AddMessageLen();
    }
    public void Init(ref byte[] t_buffer, ushort t_len)
    {
        m_len = t_len;
        m_buffer = new byte[t_len];
        m_readPos = 2;
        m_writePos = 2;
        int count = 0;
        for (int i = m_writePos; i < t_len; i++)
        {
            m_buffer[i] = t_buffer[count];
            count++;
        }
        m_writePos += t_len;
        AddMessageLen();
    }
    //------------------------------------------------------------
    public void ResetRead()
    {
        m_readPos = 2;
    }
    public int GetMessageLen_wLen()
    {
        return (int)(m_len + sizeof(ushort));
    }
    public ushort GetMessageLen()
    {
        return m_len;
    }
    public void BuildMessage(ref byte[] t_buffer)
    {
    }
    public void AddToMessage(ref byte[] t_buffer, int t_len)
    {
        int count = 0;
        for (int i = m_writePos; i < (m_writePos + t_len); i++)
        {
            m_buffer[i] = t_buffer[count];
            count++;
        }
        m_writePos += t_len;
        m_len += (ushort)t_len;
        AddMessageLen();
    }
    public void GetMessageChunk(ref byte[] t_buffer, int t_len)
    {
        int count = 0;
        for (int i = m_readPos; i < (m_readPos + t_len); i++)
        {
            t_buffer[count] = m_buffer[i];
            count++;
        }
        m_readPos += t_len;
    }
    public void GetMessageDataWithLen(ref byte[] t_buffer)
    {
        t_buffer = m_buffer;
    }
    public byte[] GetMessageDataWithLen()
    {
        return m_buffer;
    }
    public void GetMessageData(ref byte[] t_buffer)
    {
        byte[] t_msgBuff = new byte[m_len];
        int count = 0;
        for (int i = 2; i < m_len; i++)
        {
            t_msgBuff[count] = m_buffer[i];
            count++;
        }
        t_buffer = t_msgBuff;
    }

};