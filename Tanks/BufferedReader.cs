using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class BufferedReader : RingBuffer
{
    public int PumpMessageReader(ref Socket t_sock, ref BinaryMessage t_msg)
    {
        int messageRead = -1;

        if (AmountBuffered() > 0)
        {
            ushort t_len = BitConverter.ToUInt16(m_buffer, m_startPos);
            if (AmountBuffered() > t_len)
            {
                byte[] t_msgBuff = new byte[t_len];
                Get(ref t_msgBuff);
                t_msg.AddToMessage(ref t_msgBuff, t_len);
                messageRead = 1;

                return messageRead;
            }
        }
        if (t_sock.Available > 0)
        {
            //Debug.Log("Im inside!");
            byte[] t_buffer = new byte[1024];
            int recvdBytes = t_sock.Receive(t_buffer);
            Put(ref t_buffer, recvdBytes);

            messageRead = 2;
            return messageRead;
        }
        return messageRead;
    }
};