using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


public class RingBuffer
{
    //-------------------------------
    public byte[] m_buffer;
    public int m_bufferSize;
    public int m_startPos;
    public int m_endPos;
    //-------------------------------
    public void Init(int t_size)
    {
        m_buffer = new byte[t_size];
        m_bufferSize = t_size;
        m_startPos = 0;
        m_endPos = 0;
    }

    protected int GetNextValidIndex(int idxPos)
    {
        int newIdx = idxPos + 1;
        if (newIdx >= m_bufferSize)
        {
            newIdx = 0;
        }
        return newIdx;
    }
    protected int GetNextValidIndex(int idxPos, int dist)
    {
        //if(idxPos == 2048)
        // Debug.Log(idxPos.ToString() + "  " + dist.ToString() + "   " + m_bufferSize.ToString());
        int newIdx = idxPos + dist;
        if (newIdx >= m_bufferSize)
        {
            newIdx = (newIdx - m_bufferSize);
        }
        return newIdx;
    }
    //Ill implment this later if needed.
    private void GrowBuffer()
    {
    }
    private void FullCompress()
    {
        //m_startPos = 0;
        //m_endPos   = 0;
    }
    public int AmountBuffered()
    {
        //Let X == start Y == end
        // - we dont want + we want
        //
        //We want the distance from head to tail.
        //CASE: --X++++++Y--
        if (m_endPos >= m_startPos)
        {
            return (m_endPos - m_startPos);
        }
        //CASE::++Y-----X++
        else
        {
            return (m_bufferSize - m_startPos) + m_endPos;
        }
        //return m_endPos - m_startPos;
    }
    public int BufferAvail()
    {
        //Let X == start Y == end
        // - we dont want + we want
        //
        //We want the distance from tail to head
        //CASE: ++X-----Y++
        if (m_endPos >= m_startPos)
        {
            //return (m_bufferSize - m_endPos) + m_startPos;
            return (m_bufferSize - m_endPos) + m_startPos;
        }
        //CASE: --Y++++++X--
        else
        {
            return (m_startPos - m_endPos);
        }
    }
    protected byte[] GetBufferFull()
    {
        return m_buffer;
    }
    public bool Put(ref byte[] t_buffer, int t_len)
    {
        bool t_isDone = false;
        if (t_len > BufferAvail())
            return t_isDone;
        //FullCompress();

        int t_count = m_endPos;

        //Debug.Log("T_LEN" + t_len.ToString());
        for (int i = 0; i < (t_len); i++)
        {
            //Debug.Log("BUFFER[" + t_count.ToString() + "] == " + t_buffer[i].ToString());
            m_buffer[t_count] = t_buffer[i];
            t_count = GetNextValidIndex(t_count);
        }
        //m_endPos += (t_len);
        m_endPos = GetNextValidIndex(m_endPos, t_len);
        t_isDone = true;
        return t_isDone;
    }
    public bool Get(ref byte[] t_buffer)
    {
        bool done = false;
        if (AmountBuffered() > 0)
        {
            uint len = BitConverter.ToUInt16(m_buffer, (m_startPos));
            int count = 0;
            //for(int i = (m_startPos + 2) ; i < (m_startPos + 2 + len); i++)
            //Debug.Log(BitConverter.ToUInt16( m_buffer , (m_startPos )).ToString()+ "   " + "Start Pos + len == "+ m_startPos.ToString() +  "   ,  " + GetNextValidIndex(m_startPos, (int)(2 + len)).ToString());
            for (int i = GetNextValidIndex(m_startPos, 2)
                ; i < GetNextValidIndex(m_startPos, (int)(2 + len))
                ; i = GetNextValidIndex(i))
            {

                //Debug.Log("i == " + i.ToString());
                t_buffer[count] = m_buffer[i];
                count++;
            }
            //m_startPos += (int)(len + 2);
            m_startPos = GetNextValidIndex(m_startPos, (int)(2 + len));

            done = true;
        }
        return done;
    }

};