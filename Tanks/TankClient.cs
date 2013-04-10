using UnityEngine;
using System.Collections;

//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//Important make sure any Function code you create is done inside of the 
//enum of NetworkController.cs and that you make sure to add that function
//code to the PopulateFunctionCodeDict() function so the controller will be
//able to reconize that function code and send it off to people.

public class TankClient : MonoBehaviour , IntrestedObj {

    private User m_clientUser = new User();
    private TankRoom m_currentRoom = new TankRoom();
    //public GameObject m_clientPlayer;
    //public GameObject m_networkPlayer1;
	// Use this for initialization
    private ClientController m_controller = new ClientController();
	void Start() {
        m_controller.m_liveController.GetClientUserData(ref m_clientUser);

        Room t_room = new Room();
        string t_name = "";
        ushort t_type = 0;
        m_controller.m_liveController.GetRoomStateInfo(ref t_room);
        t_name = t_room.GetRoomName();
        t_type = t_room.GetRoomType();
        m_currentRoom.SetRoomInfo(ref t_name, ref t_type);

        m_controller.m_liveController.AddRPCIntrest(this,
                                   (ushort)Function_Codes.FUNCTION_USER_NEWROOM);
	
	}
    public void ProcessMessage(BinaryMessage t_recvdMsg)
    {
        GameMessageReader t_reader = new GameMessageReader();
        t_reader.InitAdapter(ref t_recvdMsg);

        switch(t_reader.GetFunctionCode())
        {
            case (ushort)Function_Codes.FUNCTION_USER_NEWROOM:
                {
                    string t_name = "";
                    ushort t_roomType = 0;
                    t_reader.Decode_FunctionUserNewRoom(ref t_roomType,
                                                         ref t_name);

                    if(t_roomType == (ushort)Function_Codes.ROOM_TYPE_CHAT)
                    {
                        Debug.Log("NEW ROOM CHAT!");
                        Room t_room = new Room();
                        t_room.SetRoomInfo(ref t_name, ref t_roomType);
                        m_controller.m_liveController.SendRoomStateData(t_room);
                        if(!m_controller.m_liveController.CheckDataSaved())
                        {

                            m_controller.m_liveController.SendClientUserData(m_clientUser);
                        }
                        Application.LoadLevel(0);
                    }
                    break;
                }
        }
    }
	// Update is called once per frame
	void Update () {
		if(m_controller.m_liveController != null)
			m_controller.m_liveController.ProcessController();

        if (Input.GetKey("p"))
        {
            BinaryMessage t_msg = new BinaryMessage();
            t_msg.Init(1024);
            GameMessageWriter t_writer = new GameMessageWriter();
            t_writer.InitAdapter(ref t_msg);
            t_writer.Encode_FunctionRequestLeaveGame();
            m_controller.m_liveController.QueueMessage(t_msg);
        }
	}
}  
