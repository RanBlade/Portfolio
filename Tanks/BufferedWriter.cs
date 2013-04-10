using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class BufferedWriter : RingBuffer
{
    public bool FlushWrite(ref Socket t_sock)
    {
        //Debug.Log(" FLUSH WRITE:" + AmountBuffered().ToString() + " START -- " + m_startPos.ToString() + "  END  -- " + m_endPos.ToString());
        if (AmountBuffered() > 0)
        {
            int amtBuff = AmountBuffered();
            byte[] sendBuffer = new byte[amtBuff];
            if (m_endPos >= m_startPos)
            {
                Array.Copy(m_buffer, m_startPos, sendBuffer, 0, amtBuff);
                int sent = t_sock.Send(sendBuffer, amtBuff, 0);
                m_startPos = GetNextValidIndex(m_startPos, sent);
                return true;
            }
            else
            {
                int lenToEnd = (m_bufferSize) - m_startPos;
                Array.Copy(m_buffer, m_startPos, sendBuffer, 0, lenToEnd);
                Array.Copy(m_buffer, 0, sendBuffer, lenToEnd, m_endPos);
                int sent = t_sock.Send(sendBuffer, amtBuff, 0);
                m_startPos = GetNextValidIndex(m_startPos, sent);
                return true;

            }


        }//send the buffer.... 
        return false;
    }
    public bool AddMessage(ref BinaryMessage t_msg)
    {
        bool messageAdded = false;
        int count = 0;
        byte[] t_buff = t_msg.GetMessageDataWithLen();

        int i = m_endPos;
        int finIndex = GetNextValidIndex(m_endPos, t_msg.GetMessageLen_wLen());

        if (finIndex < m_endPos)
        {
            for (; i < m_bufferSize; i++)
            {
                m_buffer[i] = t_buff[count];
                count++;
            }
            i = GetNextValidIndex(i);
            for (; i < finIndex; i = GetNextValidIndex(i))
            {
                m_buffer[i] = t_buff[count];
                count++;
            }
        }
        else
        {
            for (; i < finIndex; i = GetNextValidIndex(i))
            {
                m_buffer[i] = t_buff[count];
                count++;
            }
        }
        m_endPos = GetNextValidIndex(m_endPos, t_msg.GetMessageLen_wLen());
        messageAdded = true;
        return messageAdded;
    }
};