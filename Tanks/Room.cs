using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class Room
{
    protected ushort      m_RoomType;
    protected string      m_RoomName;
    protected List<User>  m_users;

    public void StartNewRoom(ref string t_name,
                              ref ushort t_type)
    {

    }
    public void SetRoomInfo(ref string t_name,
                             ref ushort t_type)
    {
        m_RoomName = t_name;
        m_RoomType = t_type;
    }
    public void LeaveRoom()
    {
        m_RoomType = 0;
        m_RoomName = "";
        m_users.Clear();
    }
    public void FindUserByID( ulong t_id , ref User t_user )
    {
        foreach( User u in m_users )
        {
            if( u.m_id == t_id )
            {
                t_user = u;
            }
        }
    }
    public void RemoveUserByID( ulong t_id )
    {
        foreach( User u in m_users )
        {
            if (u.m_id == t_id)
                m_users.Remove(u);
        }
    }
    public List<User> GetRoomUserList() { return m_users; }

    public void CopyList(ref List<User> t_list)    
    {
        foreach (User user in t_list)
        {
            m_users.Add(user);
        }
    }
    public void AddToUserList(ref User t_user)
    {

        m_users.Add(t_user);
    }
    public string GetRoomName() { return m_RoomName; }
    public ushort GetRoomType() { return m_RoomType; }


};