using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
//WARNING!!
//All classes must be initiated with the Init() functions
//The constructors do not seem to play nicely so make sure 
//to use init whenever you use any of the buffer classes
//Ill look into it in the future to see if its a flaw -EHR

//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//Important make sure any Function code you create is done inside of the 
//enum of NetworkController.cs and that you make sure to add that function
//code to the PopulateFunctionCodeDict() function so the controller will be
//able to reconize that function code and send it off to people.



//Buffer/Message Code


enum BuildMode : int
{
    MODE_DEBUG = 0,
    MODE_RELEASE
};
enum ChatState : int
{
    LOGON = 10000 ,
    CHAT          ,

};


             
public class ChatClient : MonoBehaviour , IntrestedObj
{

    //Globbal objects of the Game Client	
    private ClientController m_controller = new ClientController();
    private ChatRoom m_currentRoom;
    private int m_chatClientState;

    private User ClientUser = new User();

    //chat window stuffs / logon window stuff
    public Vector2 chatWindow = new Vector2(0,1);
    public Vector2 userWindow = new Vector2(0, 1);
    public string innerText = "";
    public string textfield = "";
    private bool m_chatBarPinned = true;
    private int m_lastFrameNumChatLines = 0;

    private string t_userName = "";
    private string t_password = "";
    private bool loginButton = false;
    private bool sendButton = false;


    //debug crap
    private int m_buildMode;

    public Texture2D GUITexture;
    public Texture2D LoginTexture;

    //Helper functions for the client class and interfacing into the GUI
    void KeyUpdater()
    {

        if (m_chatClientState == (int)ChatState.CHAT)
        {
            if ((Input.GetKey("enter") || sendButton || Input.GetKey("return")) && textfield != "")
            {
                //do something with textfieldstring here... send it off in chat message
                //ProcessChatMessage( textfield) ????
                m_currentRoom.AddMessageToChatWindow(ClientUser.m_username + ": " + textfield);
                BinaryMessage t_msg = new BinaryMessage();
                t_msg.Init();
                GameMessageWriter t_writer = new GameMessageWriter();
                t_writer.InitAdapter(ref t_msg);

                t_writer.Encode_FunctionUserChat(ref ClientUser.m_username, ref textfield,
                                                  ref ClientUser.m_id);

                m_controller.m_liveController.QueueMessage(t_msg);
                sendButton = false;

                textfield = "";
            }
        }
        else if( m_chatClientState == (int)ChatState.LOGON)
        {
            if ((Input.GetKey("enter") || loginButton || Input.GetKey("return")) && t_userName != "" && t_password != "")
            {
                BinaryMessage t_msg = new BinaryMessage();
                t_msg.Init();
                GameMessageWriter t_writer = new GameMessageWriter();
                t_writer.InitAdapter(ref t_msg);
                t_writer.Encode_FunctionUserLogin(ref t_userName, ref t_password);

                m_controller.m_liveController.QueueMessage(t_msg);

                m_chatClientState = (int)ChatState.CHAT;
            }
        }
        
    }
    int StartClient()
    {
        if(m_controller.m_liveController.CheckDataSaved ())
        {
            m_controller.m_liveController.GetClientUserData(ref ClientUser);
            Room t_room = new Room();
            m_controller.m_liveController.GetRoomStateInfo(ref t_room);
            string t_roomName = t_room.GetRoomName ();
            ushort t_roomType = t_room.GetRoomType ();
            m_currentRoom = new ChatRoom();
            m_currentRoom.StartNewRoom (ref t_roomName, ref t_roomType);
            m_currentRoom.AddUserToWindow(ref ClientUser.m_username);
            m_chatClientState = (int)ChatState.CHAT;
            return 0;
            
        }
        ClientUser.m_id = 0;
        ClientUser.m_username = "";
        m_chatClientState = (int)ChatState.LOGON;
        m_currentRoom = new ChatRoom();
        string name = "No Room";
        ushort type = 0;
        m_currentRoom.StartNewRoom(ref name, ref type);

        //debug crap

        return 0;

    }
    //---------------------------------------------------------------------
    //Client message specific process functions
    bool ProcessFunctionUserNewRoom( ref GameMessageReader inMsgReader)
    {
        string t_roomName = "";
        UInt16 t_roomType = 0;

            inMsgReader.Decode_FunctionUserNewRoom(ref t_roomType,
                                                    ref t_roomName);

            m_currentRoom.SetRoomInfo(ref t_roomName, ref t_roomType);

            if (t_roomType == (ushort)Function_Codes.ROOM_TYPE_TANKS)
            {
                Debug.Log("NEW ROOM TANKS!");
                m_controller.m_liveController.SendClientUserData(ClientUser);
                Room t_room = new Room();
                t_room.SetRoomInfo(ref t_roomName, ref t_roomType);
                m_controller.m_liveController.SendRoomStateData(t_room);
                m_controller.m_liveController.RemoveAllIntrest(this);
                Application.LoadLevel(1);
            }
        

        return true;
    }
    bool ProcessClientUserInRoom(ref GameMessageReader inMsgReader)
    {
        //print("FUNCTION_USER_IN_ROOM");
        List<User> t_list = new List<User>();
        ushort listSize = 0;
        inMsgReader.Decode_FunctionUserInRoom(ref t_list, ref listSize);

        if (listSize == 1 && t_list[0].m_id != ClientUser.m_id)
        {
            m_currentRoom.AddMessageToChatWindow (t_list[0].m_username + 
                                                  " has joined the channel");
            m_currentRoom.AddUserToWindow(ref t_list[0].m_username);
            m_currentRoom.CopyList(ref t_list);
        }
        else if (listSize == 0)
        {
            return false;
        }
        else
        {
            m_currentRoom.CopyList(ref t_list);
            foreach( User t_user in t_list)
            {
                m_currentRoom.AddUserToWindow(ref t_user.m_username);
            }
        }

        return true;
    }
    bool ProcessUserOutRoom(ref GameMessageReader inMsgReader)
    {
        ulong t_userID = 0;
        User  t_user   = new User();
        inMsgReader.Decode_FunctionUserOutRoom(ref t_userID);
        print(t_userID);
        m_currentRoom.FindUserByID(t_userID , ref t_user );

        m_currentRoom.RemoveUserFromWindow(ref t_user.m_username);

        m_currentRoom.AddMessageToChatWindow(t_user.m_username +
                                              ": has left the channel");
        return true;
    }
    bool ProcessClientChatMsg( ref GameMessageReader inMsgReader )
    {
        string userName = "";
        string chatMsg = "";
        UInt64 userID = 0;
        inMsgReader.Decode_FunctionUserChat(ref chatMsg , ref userName ,
                                            ref userID);

        m_currentRoom.AddMessageToChatWindow(userName + ": " + chatMsg);
        return true;
    }

//End message specific process functions-----------------------------------

    public void ProcessMessage(BinaryMessage t_recvdMsg)
    {
        GameMessageReader msgReader = new GameMessageReader();
        msgReader.InitAdapter(ref t_recvdMsg);
        print("FunctionCode " + msgReader.GetFunctionCode());
        switch (msgReader.GetFunctionCode())
        {
            
            case (ushort)Function_Codes.FUNCTION_USER_LOGIN_RESP:
                {
                    int return_code = 0;
                    msgReader.Decode_FunctionUserLoginResp(ref return_code,
                                                               ref ClientUser.m_id,
                                                               ref ClientUser.m_username);
                    m_currentRoom.AddUserToWindow(ref ClientUser.m_username);
                    break;
                }
            case (ushort)Function_Codes.FUNCTION_USER_NEWROOM:
                {
                    ProcessFunctionUserNewRoom(ref msgReader);
                    break;
                }
            case (ushort)Function_Codes.FUNCTION_USER_IN_ROOM:
                {
                    ProcessClientUserInRoom(ref msgReader);
                    break;
                }
            case (ushort)Function_Codes.FUNCTION_USER_OUT_ROOM:
                {
                    ProcessUserOutRoom(ref msgReader);
                    break;
                }
            case (ushort)Function_Codes.FUNCTION_USER_CHAT:
                {
                    ProcessClientChatMsg(ref msgReader);
                    break;
                }
        }
    }

    // Use this for initialization
    void Start()
    {
        print("Starting Game Client");
        StartClient();
            if(!m_controller.m_liveController.TestConnectivity())   
                m_controller.m_liveController.InitConnection("72.52.169.178", 6666);
            
            m_controller.m_liveController.AddRPCIntrest(this,
                              (ushort)Function_Codes.FUNCTION_USER_LOGIN_RESP);
            m_controller.m_liveController.AddRPCIntrest(this,
                              (ushort)Function_Codes.FUNCTION_USER_CHAT);
            m_controller.m_liveController.AddRPCIntrest(this,
                              (ushort)Function_Codes.FUNCTION_USER_IN_ROOM);
            m_controller.m_liveController.AddRPCIntrest(this,
                              (ushort)Function_Codes.FUNCTION_USER_OUT_ROOM);
            m_controller.m_liveController.AddRPCIntrest(this,
                              (ushort)Function_Codes.FUNCTION_USER_NEWROOM);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_controller.m_liveController != null)
            m_controller.m_liveController.ProcessController();
        KeyUpdater();
    }


    void OnGUI()
    {
        if(m_chatClientState == (int)ChatState.LOGON)
        {
            GUI.Box(new Rect(((Screen.width / 2) - 100), 200, 200, 108),
                     "Accts. disabled, enter any info."); 
            GUI.Label(new Rect((Screen.width / 2) - 90, 225, 70, 20), "Username:");
            t_userName = GUI.TextField(new Rect((Screen.width / 2) - 20, 225, 100, 20) , t_userName );
            GUI.Label(new Rect((Screen.width / 2) - 90 , 255 , 70 , 20), "Password:");

            t_password = GUI.PasswordField(new Rect((Screen.width / 2) - 20, 255, 100, 20), t_password, '*'); //new Rect((Screen.width / 2) - 20, 255, 100, 20), t_password);


            GUI.Label(new Rect((Screen.width / 2) - 256, 50, (Screen.width / 2) + 256, 178), LoginTexture);
            if(GUI.Button(new Rect((Screen.width / 2) - 93, 282, 185, 20), "Login"))
            {
                loginButton = true;
            }
        } 
        if (m_chatClientState == (int)ChatState.CHAT)  
        {

            if (GUI.Button(new Rect(GetPercent(.785F, Screen.width),
                                    GetPercent(.857F, Screen.height),
                                    GetPercent(.205F, Screen.width),
                                    GetPercent(.07F, Screen.height)),
                           "Play Game"))
            {
                BinaryMessage t_msg = new BinaryMessage();
                t_msg.Init();
                GameMessageWriter t_writer = new GameMessageWriter();
                t_writer.InitAdapter(ref t_msg);
                t_writer.Encode_FunctionRequestGame();
                m_controller.m_liveController.QueueMessage(t_msg);
            }
            if (GUI.Button(new Rect(GetPercent(.785F, Screen.width),
                                   GetPercent(.928F, Screen.height),
                                   GetPercent(.205F, Screen.width),
                                   GetPercent(.07F, Screen.height)),
                          "Exit."))
            {

            }

            GUI.Box(new Rect(GetPercent(.015F, Screen.width),
                             GetPercent(.19F, Screen.height),
                             GetPercent(.749F, Screen.width),
                             GetPercent(.61F, Screen.height)), "");

            GUI.Box(new Rect(GetPercent(.785F, Screen.width),
                             GetPercent(.066F, Screen.height),
                             GetPercent(.2F, Screen.width),
                             GetPercent(.773F, Screen.height)), "");

            //GUI.Label(new Rect(GetPercent(.1F, Screen.width),
            //                   GetPercent(.02F, Screen.height),
            //                   300,
            //                   25), "Vesmar Entertainment - Chat Client Alpha");

            // Begin the ScrollView ----- ChatWindow
            int numChatMsg = m_currentRoom.GetChatWindowLen();
            float extraLines = (float)(((numChatMsg + 1) - 28) * 13.1F);
            if (numChatMsg < 28)
            {

                chatWindow = GUI.BeginScrollView(new Rect(GetPercent(.015F, Screen.width),
                                                          GetPercent(.19F, Screen.height),
                                                          GetPercent(.749F, Screen.width),
                                                          GetPercent(.61F, Screen.height))
                                                , chatWindow,
                                                new Rect(0,
                                                         0,
                                                         GetPercent(.55F, Screen.width),
                                                         GetPercent(.61F, Screen.height)));
                //, false, true);

                GUI.Label(new Rect(5,
                             0,
                             GetPercent(.72F, Screen.width),
                             GetPercent(.65F, Screen.height)),
                    //GetPercent(.8F, Screen.height)), 
                    m_currentRoom.GetChatWindowString());
            }
            else
            {
                //Debug.Log(chatWindow);
                float size = GetPercent(.61F, Screen.height) + extraLines;
                //print(m_chatBarPriorFramePos.ToString() + System.Convert.ToInt32(chatWindow.y).ToString());
                //if (m_lastFrameNumChatLines != System.Convert.ToInt32(chatWindow.y))
                //{
                //    //Debug.Log("Oh noes");
                //}

                chatWindow = GUI.BeginScrollView(new Rect(GetPercent(.015F, Screen.width),
                                                          GetPercent(.19F, Screen.height),
                                                          GetPercent(.749F, Screen.width),
                                                          GetPercent(.6F, Screen.height))
                                                , chatWindow,
                                                new Rect(0,
                                                         0,
                                                         GetPercent(.55F, Screen.width),
                                                         GetPercent(.61F, Screen.height) + extraLines));
                if (GUI.changed && (numChatMsg == m_lastFrameNumChatLines))
                {
                    m_chatBarPinned = false;

                    if (extraLines - chatWindow.y <= 0.5)
                    {
                        m_chatBarPinned = true;
                    }
                    //print("Pls work");
                }
                

                if (m_chatBarPinned)
                {
                    chatWindow.y = GetPercent(.61F, Screen.height) + extraLines; 
                }
                m_lastFrameNumChatLines = m_currentRoom.GetChatWindowLen();
                GUI.Label(new Rect(5,
                             0,
                             GetPercent(.72F, Screen.width),
                             5000),
                    //GetPercent(.8F, Screen.height)), 
                    m_currentRoom.GetChatWindowString());
            }

            // Put something inside the ScrollView
            //innerText = m_currentRoom.GetChatWindowString();
          
            //innerText = "";
            // End the ScrollView    
            GUI.EndScrollView();
            int numUsers = m_currentRoom.GetUserWindowLen();
            //int x = 50;
            float extraLines2 = (float)(((numUsers + 1) - 36) * 13.1F);
            if (numUsers < 36)
            {

                // Begin the ScrollView  -- UserWindow
                userWindow = GUI.BeginScrollView(new Rect(GetPercent(.79F, Screen.width),
                                                          GetPercent(.066F, Screen.height),
                                                          GetPercent(.198F, Screen.width),
                                                          GetPercent(.775F, Screen.height))
                                                , userWindow,
                                                 new Rect(0,
                                                          0,
                                                          GetPercent(.09F, Screen.width),
                                                          GetPercent(.775F, Screen.height)));//m_currentRoom.GetUserWindowLen())
                //,false, true);
                //string testString = "1\n2\n3\n4\n5\n6\n7\n8\n9\n10\n11\n12\n13\n14\n15\n16\n17\n18\n19\n20\n21\n22\n23\n24\n25\n26\n27\n28\n29\n30\n31\n32\n33\n34\n35\n36\n37\n38\n39\n40\n41\n42\n43\n44\n45\n46\n47\n48\n49\n50\n";
                // Put something inside the ScrollView --- UserWindow
                //innerText = m_currentRoom.GetChatWindowString();
                GUI.Label(new Rect(0,
                                   0,
                                   GetPercent(.9F, Screen.width),
                                   GetPercent(.765F, Screen.height)),
                                   m_currentRoom.GetUserWindowString());
                //GetPercent(.8F, Screen.height)),
                //m_currentRoom.GetUserWindowString());
                //innerText = "";

            }
            else
            {
                userWindow = GUI.BeginScrollView(new Rect(GetPercent(.79F, Screen.width),
                                                         GetPercent(.066F, Screen.height),
                                                         GetPercent(.1955F, Screen.width),
                                                         GetPercent(.772F, Screen.height))
                                               , userWindow,
                                                new Rect(0,
                                                         0,
                                                         GetPercent(.09F, Screen.width),
                                                         GetPercent(.765F, Screen.height) + extraLines2));//m_currentRoom.GetUserWindowLen())
                //,false, true);
                //string testString = "1\n2\n3\n4\n5\n6\n7\n8\n9\n10\n11\n12\n13\n14\n15\n16\n17\n18\n19\n20\n21\n22\n23\n24\n25\n26\n27\n28\n29\n30\n31\n32\n33\n34\n35\n36\n37\n38\n39\n40\n41\n42\n43\n44\n45\n46\n47\n48\n49\n50\n";
                // Put something inside the ScrollView --- UserWindow
                //innerText = m_currentRoom.GetChatWindowString();
                GUI.Label(new Rect(0,
                                   0,
                                   GetPercent(.9F, Screen.width),
                                   5000),
                                   //testString);
                                   m_currentRoom.GetUserWindowString());
            }
            // End the ScrollView    
            GUI.EndScrollView();





            GUI.Box(new Rect(0,
                             GetPercent(.85F, Screen.height),
                             GetPercent(.78F, Screen.width),
                             GetPercent(.61F, Screen.height)), "$Banner Advertisement$");
            GUI.Label(new Rect(0, -3, Screen.width + 8, Screen.height + 9), GUITexture);



            textfield = GUI.TextField(new Rect(GetPercent(.115F, Screen.width),
                                               GetPercent(.807F, Screen.height),
                                               GetPercent(.649F, Screen.width),
                                               18)
                                   , textfield);

            if(GUI.Button(new Rect(GetPercent(.0155F, Screen.width),
                                GetPercent(.805F, Screen.height),
                                GetPercent(.096F, Screen.width),
                                19), "Send"))
            {
                sendButton = true;
            }

            GUI.Label(new Rect(GetPercent(.82F, Screen.width),
                               GetPercent(.008F, Screen.height),
                               300,
                               25), m_currentRoom.GetRoomName());


            


            m_currentRoom.UpdateChatWindows();
        }
    }

    float GetPercent(float percent, int size)
    {
        return percent * size;
    }

};


//DEBUG STATEMENTS THAT MIGHT BE USEFULL AGAIN SOMETIME...
// GUI.Box(new Rect(10, 10, 300, 90), "Chat Debug");
//  string readerbufferdebug = String.Format("m_charReader.AmountBuffered() = {0}   , m_charReader.AmountAvailable() = {1}",
//                                                            m_clientReader.AmountBuffered(), m_clientReader.BufferAvail());
// string writerbufferdebug = String.Format("m_charWriter.AmountBuffered() = {0} , m_charWriter.AmountAvailable() = {1}",
//                                                           m_clientWriter.AmountBuffered(), m_clientWriter.BufferAvail());
//GUI.Label(new Rect(15, 30, 500, 20), readerbufferdebug);
//GUI.Label(new Rect(15, 60, 500, 20), writerbufferdebug);