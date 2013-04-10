using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class ChatRoom : Room
{
    protected List<string> m_chatWindowBuffer;
    protected string m_userChatWindowString;
    public bool m_userChatWindowStringHasChanged;
    protected List<string> m_userWindow;
    protected string m_userWindowString;
    public bool m_userWindowStringHasChanged;

    public void StartNewRoom(ref string t_name,
                          ref ushort t_type)
    {
        m_RoomName = t_name;
        m_RoomType = t_type;
        m_users = new List<User>();
        m_chatWindowBuffer = new List<string>();
        m_userWindow = new List<string>();
        m_userChatWindowString = "";
        m_userWindowString = "";
        m_userChatWindowStringHasChanged = false;
        m_userWindowStringHasChanged = false;
    }

    public void GenerateUserWindowString()
    {

        foreach (string t_user in m_userWindow)
        {
            m_userWindowString = m_userWindowString + "\n" + t_user;
        }
        m_userWindowStringHasChanged = false;

    }

    public string GetUserWindowString() { return m_userWindowString; }
    public string GetChatWindowString() { return m_userChatWindowString; }
    public int GetChatWindowLen() { return m_chatWindowBuffer.Count;  }
    public int GetUserWindowLen() { return m_userWindow.Count; }
    public void AddUserToWindow(ref string t_userString)
    {
        m_userWindow.Add(t_userString);
        GenerateUserWindowString();
        m_userWindowStringHasChanged = true;
    }
    public void RemoveUserFromWindow(ref string t_userString)
    {
        m_userWindow.Remove(t_userString);
        GenerateUserWindowString();
        m_userWindowStringHasChanged = true;
    }
    public void GenerateChatWindowString()
    {
        while (m_chatWindowBuffer.Count > 50)
        {
            m_chatWindowBuffer.RemoveAt(0);
        }
        m_userChatWindowString = "";
        foreach (string t_msg in m_chatWindowBuffer)
        {
            m_userChatWindowString = m_userChatWindowString + "\n" + t_msg;
        }
        m_userChatWindowStringHasChanged = false;
    }
    public void AddMessageToChatWindow(string t_msg)
    {
        m_chatWindowBuffer.Add(t_msg);
        m_userChatWindowStringHasChanged = true;
    }

    public void UpdateChatWindows()
    {
        if (m_userChatWindowStringHasChanged == true)
        {
            m_userChatWindowString = "";
            GenerateChatWindowString();
        }
        if (m_userWindowStringHasChanged == true)
        {
            m_userWindowString = "";
            GenerateUserWindowString();
        }
    }

}
