using UnityEngine;
using System.Collections;

//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//Important make sure any Function code you create is done inside of the 
//enum of NetworkController.cs and that you make sure to add that function
//code to the PopulateFunctionCodeDict() function so the controller will be
//able to reconize that function code and send it off to people.

public class RemoteTankMovement : MonoBehaviour , IntrestedObj {

    private ClientController m_controller = new ClientController();
    public GameObject TurretSceneNode;

    public Transform bottomNode;
    public Transform spawn1;
    // Use this for initialization
    void Start()
    {
        m_controller.m_liveController.AddRPCIntrest(this, 
                                                    (ushort)Function_Codes.FUNCTION_TANKROOM_USER_MOVE_TANK);
        m_controller.m_liveController.AddRPCIntrest(this,
                                                    (ushort)Function_Codes.FUNCTION_TANKROOM_USER_ROTATE_TANK);

        transform.position = spawn1.position;

    }
    public void ProcessMessage(BinaryMessage t_recvdMsg)
    {
        GameMessageReader t_reader = new GameMessageReader();
        t_reader.InitAdapter(ref t_recvdMsg);
        Debug.Log(t_reader.GetFunctionCode().ToString() + " tank movement recd\n");
        switch( t_reader.GetFunctionCode ())
        {
            
            case (ushort)Function_Codes.FUNCTION_TANKROOM_USER_MOVE_TANK:
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    float h = 0;
                    float p = 0;
                    float r = 0;
                    t_reader.Decode_FunctionTankUpdatePOSHPR(ref x ,
                                                           ref y ,
                                                           ref z ,
                                                           ref h ,
                                                           ref p ,
                                                           ref r );
                    transform.position = new Vector3(x, y, z);
                    print(h.ToString() + p.ToString() + r.ToString());
                    Quaternion newOrientation = Quaternion.Euler(new Vector3(h,p,r));
                    transform.rotation = newOrientation;
                    break;
                }
            //case (ushort)Function_Codes.FUNCTION_TANKROOM_USER_ROTATE_TANK:
            //    {
            //        short direction = 0;
            //        t_reader.Decode_FunctionTankUpdateRotation(ref direction);

            //        if(direction == 1 )
            //        {
            //            transform.RotateAroundLocal(Vector3.up, -.02F);
            //        }
            //        if(direction == -1)
            //        {
            //            transform.RotateAroundLocal(Vector3.up, .02F);
            //        }

            //        break;
            //    }
        }
    }
    // Update is called once per frame
    void Update()
    {
        

    }
}
