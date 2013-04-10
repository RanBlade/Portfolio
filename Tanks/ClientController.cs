using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

//Function codes to be used by the client to talk to the server.
enum Function_Codes : ushort
{
    FUNCTION_USER_LOGIN = 101, // IN
    FUNCTION_USER_LOGIN_RESP = 102, // OUT

    FUNCTION_USER_LOGOFF = 103,// IN

    FUNCTION_USER_NEWROOM = 109,// OUT 

    FUNCTION_USER_CHAT = 110,// IN-OUT
    FUNCTION_USER_IN_ROOM = 111,// OUT
    FUNCTION_USER_OUT_ROOM = 112,// OUT


    FUNCTION_CODE_GAME_SPECIFIC_LO = 10000,
    FUNCTION_CODE_GAME_SPECIFIC_HIGH = 19999,

    // function code for the main chat room

    FUNCTION_MAINROOM_REQUEST_GAME = 10100,

    // function code for the tank room
    FUNCTION_TANKROOM_REQUEST_LEAVE    = 10500 ,
    FUNCTION_TANKROOM_USER_MOVE_TANK   = 11000 ,
    FUNCTION_TANKROOM_USER_TURRET_MOVE = 11001 ,
    FUNCTION_TANKROOM_USER_ROTATE_TANK = 11002 ,

    // the type of rooms...

    ROOM_TYPE_CHAT = 1234,
    ROOM_TYPE_TANKS = 1235 ,

    FUNCTION_SYS_STORE_ROOM_DATA = 5000 ,
    FUNCTION_SYS_GET_ROOM_DATA = 5001 

};         
//NetworkController class
//This class will be implemented as a singleton so every object that needs
//access to the network communications will be using the same connection
public class ClientController {
   private static ClientController                        m_controller;
   private static Socket                                  m_sock;
   private static BufferedReader                          m_reader = new BufferedReader();
   private static BufferedWriter                          m_writer = new BufferedWriter();
   private static Dictionary<ushort, List<IntrestedObj>>  m_funcCodeIntrestList = 
                                                            new Dictionary<ushort, List<IntrestedObj> >();
   private static List<ushort>                            m_funcCodeIDList;

   private User m_clientUser = new User();
   private Room m_currentRoom = new Room();
    //probably going to deletexxxxxxxxxxxxxxxxxxxx
   private User m_storedUserInfo = new User();
   private Room m_storedRoomInfo = new Room();
   private bool m_DataSaved      = false;
    //xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    //Function: PopulateFunctionCodeDict
    //arguments: none
    //Purpose: To set up the Dictonary for use by the Network controller
    //         m_funcCodeIntrestList is a Dictonary of Function code keys
    //         with Lists of Intrested Objects as values.
   private void PopulateFunctionCodeDict()
   {
       m_funcCodeIDList = new List<ushort>();
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_USER_LOGIN);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_USER_LOGIN_RESP);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_USER_CHAT);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_USER_IN_ROOM);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_USER_NEWROOM);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_USER_LOGOFF);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_USER_OUT_ROOM);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_CODE_GAME_SPECIFIC_HIGH);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_CODE_GAME_SPECIFIC_LO);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_MAINROOM_REQUEST_GAME);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_TANKROOM_REQUEST_LEAVE);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_TANKROOM_USER_MOVE_TANK);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_TANKROOM_USER_ROTATE_TANK);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_TANKROOM_USER_TURRET_MOVE);
       m_funcCodeIDList.Add((ushort)Function_Codes.ROOM_TYPE_CHAT);
       m_funcCodeIDList.Add((ushort)Function_Codes.ROOM_TYPE_TANKS);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_SYS_GET_ROOM_DATA);
       m_funcCodeIDList.Add((ushort)Function_Codes.FUNCTION_SYS_STORE_ROOM_DATA);

       foreach (ushort t_code in m_funcCodeIDList)
       {
           List<IntrestedObj> t_obj = new List<IntrestedObj>();// new IntrestedObj();
           m_funcCodeIntrestList[t_code] = t_obj;
       }
   }
    //Function: PumpRPCQueue()
    //Argument: none
    //Purpose: Function checks the socket for a buffered message. If yes then
    //         check the code of the message through the current function
    //         codes and then send the message off to the Intrested Objects
    //         of that function code.
   private void PumpRPCQueue()
   {
       BinaryMessage t_msg = new BinaryMessage();
       t_msg.Init();
       int pumped = m_reader.PumpMessageReader(ref m_sock, ref t_msg);
       if (pumped == 1)
       {
           GameMessageReader t_readerAdapter = new GameMessageReader();
           t_readerAdapter.InitAdapter(ref t_msg);

           List<IntrestedObj> t_list;
           if (m_funcCodeIntrestList.TryGetValue(t_readerAdapter.GetFunctionCode(),
                                                out t_list))
           {
               t_readerAdapter.ResetToFuncCode();
               foreach (IntrestedObj t_obj in t_list)
               {
                   t_obj.ProcessMessage(t_msg);
               }
           }

       }
   }
    //Function: FlushController
    //Argument: none
    //Purpose: Quite simply to flush the buffered writer controlled by the 
    //         network controller class(send the stored messages).
   private void FlushController()
   {
       m_writer.FlushWrite(ref m_sock);
   }
   //Constructor
    public ClientController()
    {
        if (m_controller != null)
        {
            return;
        }
        else
        {
            //create our socket instance
            //Debug.Log("Instatiating ClientNetworkController Singleton");


            PopulateFunctionCodeDict();
            m_sock = new Socket(AddressFamily.InterNetwork,
                  SocketType.Stream,
                  ProtocolType.Tcp);
            m_reader.Init(4096);
            m_writer.Init(4096);
            m_controller = this;
          
        }
    }
    //public instance usable by the interfaces

    //Data Memeber: m_liveController
    //This is the singleton instance usable by all outside instances.
    public ClientController m_liveController
    {
        get{
            if (m_controller == null)
            {
                m_controller = new ClientController();
            }

            return m_controller;
        }
    }
    //FUNCTION: TestConnectivity
    //Arguments: none
    //Purpose:  Very simply to check if the socket is connected before 
    //          performing a operation invloving the client socket.
    public bool TestConnectivity() { return m_sock.Connected; }
   
    //Function: InitConnection()
    //Argument: string
    //Argument: int
    //Purpose:  This function initates the connection to the server.
    public int InitConnection(string t_ip, int t_port)
    {
        if (!TestConnectivity())
        {
            IPAddress serverAddr = IPAddress.Parse(t_ip);
            IPEndPoint serverEP = new IPEndPoint(serverAddr, t_port);
            //Connect to Game Server
            m_sock.Connect(serverEP);
            m_sock.Blocking = false;
            m_writer = new BufferedWriter();
            m_reader = new BufferedReader();

            m_writer.Init(4096);
            m_reader.Init(4096);
            return 0;
        }
        Debug.Log("Socket already connected.. please pay attention!\n");
        return -1;
    }
    //Function: ProcessController
    //Argument: none
    //Purpose:  This function simply processes any of the recived data from
    //          either the remote host or the intrested objects.
    public bool ProcessController() 
    {
        PumpRPCQueue();
        FlushController();
        return true;
    }
    //Function: AddRPCIntrest
    //Argument: none
    //Purpose:  Here the Intrested objects identify that they want to be 
    //          informed of any BinaryMessages that have been recived with
    //          a certian function code.
    public void AddRPCIntrest(IntrestedObj t_iOBJ, ushort t_code)
    { 
        //if key doesnt exist then print error and return
        if( !m_funcCodeIntrestList.ContainsKey( t_code ) )
        {
            Debug.Log("FUNCTION_CODE: " + t_code.ToString () + " does not exist\n Could not remove");
            return;
        }

        List<IntrestedObj> t_list;
        m_funcCodeIntrestList.TryGetValue(t_code, out t_list);
        t_list.Add(t_iOBJ);

        m_funcCodeIntrestList[t_code] = t_list;
    }
    //Function: RemoveIntrest
    //Argument: IntrestedObj
    //Argument: ushort
    //Purpose:  To remove the intrest of a IntrestedObj from a specific 
    //          function code
    public void RemoveIntrest( IntrestedObj t_iObj , ushort t_code )
    {
        if(!m_funcCodeIntrestList.ContainsKey ( t_code )){
            Debug.Log("FUNCTION CODE: " + t_code.ToString() + " does not exist \n Could not remove obj");
            return;
        }

        List<IntrestedObj> t_list;
        m_funcCodeIntrestList.TryGetValue(t_code, out t_list);
        t_list.Remove(t_iObj);

        m_funcCodeIntrestList[t_code] = t_list;
    }
    //Function: RemoveAllIntrest
    //Argument: IntrestedObj
    //Purpose:  This function removes the IntrestedObj from all the function
    //          codes it was intrested in.
    public void RemoveAllIntrest( IntrestedObj t_iObj )
    {
        foreach(ushort t_code in m_funcCodeIDList )
        {
            List<IntrestedObj> t_list;
            m_funcCodeIntrestList.TryGetValue(t_code, out t_list);

            t_list.Remove(t_iObj);
            m_funcCodeIntrestList[t_code] = t_list;
        }
    }
    //Function: QueueMessage
    //Argument: none
    //Purpose:  Simply to add a message from the Intrested Object to the 
    //          buffered writer.
    public void QueueMessage(BinaryMessage t_msg)
    {
        m_writer.AddMessage(ref t_msg);
    }
    public void SetUserName(string t_name)
    {
        m_clientUser.m_username = t_name;
    }
    public string GetUserName()
    {
        return m_clientUser.m_username;
    }
    public void SetUserID( ulong t_id)
    {
        m_clientUser.m_id = t_id;
    }
    public ulong GetUserID()
    {
        return m_clientUser.m_id;
    }
    //marked for deletion... xxxxxxxxxxxxxxxxxxxx
    public void SendClientUserData( User t_user)
    {
        m_storedUserInfo = t_user;
        m_DataSaved = true;
    }
    public void GetClientUserData( ref User t_user )
    {
        t_user = m_storedUserInfo;
    }
    public void SendRoomStateData(Room t_room)
    {
        m_storedRoomInfo = t_room;
    }
    public void GetRoomStateInfo( ref Room t_room )
    {
        t_room = m_storedRoomInfo;
        
    }
    public bool CheckDataSaved(){ return m_DataSaved;}
    //xxxxxxxxxxxxxxxxxx
};
//Idea: object registers a message List so each object can process its own data.
//Each class can register what function codes it would like to be informed of ...