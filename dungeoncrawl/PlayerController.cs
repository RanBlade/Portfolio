using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

    float RotationSpeed = 1.0f;
    float WalkSpeed = 0.25f;
    public Vector3 PlayerPos;

    bool enemy_Clicked;

    [SerializeField]
    Animator aPlayerAnimator;
    [SerializeField]
    NavMeshAgent PlayerAgent;
    [SerializeField]
    Stats PlayerStats;
    [SerializeField]
    Inventory PlayerInventory;
    [SerializeField]
    QuestLog PlayerLog;

   

    float DistanceSQToTarget = 0.0f;
    bool PlayerOnRoute = false;

    //Attack fields
    float TimeOfLastAttack;
    float WeaponCoolDown = 1.0f;
    bool WeaponOnCoolDown;

    bool Alive = true;

	// Use this for initialization
	void Start () {
        gameObject.name = "Player";
        
       DontDestroyOnLoad(this);
        aPlayerAnimator = GetComponent<Animator>();
        PlayerAgent = GetComponent<NavMeshAgent>();
        PlayerStats = GetComponent<Stats>();
        PlayerInventory = GetComponent<Inventory>();
        PlayerLog = GetComponent<QuestLog>();

        PlayerStats.LoadStats();
        

        //Create inital loadout of player  
        Item WarAxe = new Item
        {
            Name = "War Axe",
            Damage = 80,
            Type = ITEM_TYPE.WEAPON
        };
        Item PlateArmor = new Item
        {
            Name = "Plate Armor",
            Armor = 150,
            Type = ITEM_TYPE.ARMOR
        };
        Item Gold = new Item
        {
            Name = "Gold",
            count = 100,
            Type = ITEM_TYPE.GOLD,
        };
        Item Potion = new Item
        {
            Name = "Potion of Healing +25",
            count = 2,
            Type = ITEM_TYPE.POTION,
        };
        PlayerInventory.Weapon = WarAxe;
        PlayerInventory.Armor = PlateArmor;
        PlayerInventory.Gold = Gold;
        PlayerInventory.Potion = Potion;

        //GameObject.Find("CmeraController").GetComponent<CameraControllerISOFollow>().FollowObject = gameObject;
        //GameObject.Find("CmeraController").GetComponent<CameraControllerISOFollow>().Setup();
        //PlayerLog.CurrentQuest = new Quest();
        //PlayerLog.CurrentQuest.Name = "You currently have no quests";
        //PlayerLog.CurrentQuest.Description = "";


    }
	
	// Update is called once per frame
	void Update () {
        //TimeOfLastAttack -= Time.deltaTime;
        if (Alive)
        {
            TimeOfLastAttack -= Time.deltaTime;
     
            if (Input.GetMouseButtonDown(0))
            {
                Ray MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(MouseRay, out hit, 100))
                {

                    float distance = 0.0f;
                    distance = Vector3.Distance(transform.position, hit.transform.position);
                    Debug.Log("CLICKED - Name: " + hit.transform.name + " Distance: " + distance);
                    if (distance < 5.0f)
                    {
                        if (hit.collider.tag == "Enemy")
                        {
                            Vector3 heading = (hit.transform.position - transform.position).normalized;
                            float dot = Vector3.Dot(transform.forward, heading);                           
                            Debug.Log("The dot is: " + dot);
                            
                            // Debug.Log("The distance is: " + distance);
                            if (distance <= 5.0f)
                            {
                                //if ((dot < -0.75f && dot > -1.0f) || (dot > 0.75f && dot < 1.0f))
                                if(dot > 0.79f && dot < 2.36f)
                                {
                                    // Debug.Log("Enemy in Range.. Attacking");


                                    if (WeaponOnCoolDown == false)
                                    {
                                        
                                        TimeOfLastAttack = Time.deltaTime + WeaponCoolDown;
                                        hit.collider.gameObject.GetComponent<EnemyControllerBase>().ApplyIncomingDamage(PlayerStats.WeaponDamage);
                                        hit.collider.gameObject.GetComponent<EnemyControllerBase>().TargetedBy = gameObject;
                                        WeaponOnCoolDown = true;
                                        aPlayerAnimator.StopPlayback();
                                        aPlayerAnimator.Play("attack01");
                                        gameObject.GetComponent<AudioSource>().Play();
                                    }
                                    else
                                    {
                                        //TimeOfLastAttack -= Time.deltaTime;
                                        if (TimeOfLastAttack < 0)
                                        {
                                            WeaponOnCoolDown = false;
                                        }
                                    }
                                }
                                else
                                {
                                    // Debug.Log("Turning to look at enemy");
                                    transform.LookAt(hit.point);
                                }
                            }
                        }
                        else if(hit.collider.tag == "Quest")
                        {
                            if(hit.collider.gameObject.GetComponent<QuestObjective>().GiveQuestObjective(PlayerLog.CurrentQuest.ID))
                            {
                                PlayerLog.CurrentQuest.ObjectiveCollected = true;
                            }
                        }
                        else if(hit.collider.tag == "QuestGiver")
                        {
                            if (PlayerLog.CurrentQuest == null)
                            {
                                hit.collider.gameObject.GetComponent<QuestGiver>().QuestRequest(gameObject);
                            }
                            else if(PlayerLog.CurrentQuest != null)
                            {
                                if(PlayerLog.CurrentQuest.ObjectiveCollected == true)
                                {
                                    hit.collider.gameObject.GetComponent<QuestGiver>().CompleteQuest(PlayerLog.CurrentQuest);
                                    if(PlayerLog.CurrentQuest.QuestCompleted)
                                    {
                                        PlayerLog.CurrentQuest = null;
                                        PlayerLog.onQuest = false;
                                    }
                                }
                                else if (PlayerLog.CurrentQuest.ObjectiveCollected == false)
                                {
                                    Debug.Log("You have not found the item yet");
                                }
                            
                            }
                        }
                        
                    }
                    else
                    {
                        // Debug.Log("Starting to Walk to location");
                        PlayerAgent.destination = hit.point;
                        // Debug.Log("Destination set to: " + PlayerAgent.destination.x + " y: " + PlayerAgent.destination.y + " z: " + PlayerAgent.destination.z);
                        DistanceSQToTarget = Vector3.Distance(transform.position, hit.point);
                        PlayerOnRoute = true;
                        PlayerAgent.isStopped = false;
                        aPlayerAnimator.SetBool("Walking", true);
                    }
                }

            }
            if(Input.GetKeyDown(KeyCode.Q))
            {
                if (PlayerInventory.Potion.count > 0)
                {
                    if (PlayerStats.currentHitPoints == PlayerStats.MaxHitPoints)
                    {
                        Debug.Log("You are at full health");
                    }
                    else if ((PlayerStats.currentHitPoints + 25) > PlayerStats.MaxHitPoints)
                    {
                        PlayerStats.currentHitPoints = PlayerStats.MaxHitPoints;
                        PlayerInventory.Potion.count--;
                    }
                    
                    else
                    {
                        PlayerStats.currentHitPoints += 25;
                        PlayerInventory.Potion.count--;
                    }
                }
            }
            UpdateWeaponDamageAndArmor();
            CheckForDeath();

        }
    }
    private void FixedUpdate()
    {

        DistanceSQToTarget = Vector3.SqrMagnitude(transform.position - PlayerAgent.destination);
        if (DistanceSQToTarget <= 5.0f)
        {
            aPlayerAnimator.SetBool("Walking", false);

        }
    }

    void UpdateWeaponDamageAndArmor()
    {        
        PlayerStats.ArmorValue = PlayerInventory.Armor.Armor;
        PlayerStats.WeaponDamage = PlayerInventory.Weapon.Damage;
        PlayerStats.CalculateWeaponDamage();
    }
    void CheckForDeath()
    {
        if(PlayerStats.currentHitPoints <= 0)
        {
            PlayerAgent.isStopped = true;
            aPlayerAnimator.SetBool("Death", true);
            Alive = false;
        }
    }
    public bool ReportDeath()
    {
        if (Alive)
            return true;
        else
            return false;
    }
    public void ApplyIncomingDamage(int Damage)
    {
        PlayerStats.ApplyDamage(Damage);
    }

    public void TakeLoot(ITEM_TYPE type)
    {
        PlayerInventory.UpdateLoot(type);
    }

    public bool OnQuest()
    {
        Debug.Log("Player is OnQuest: " + PlayerLog.onQuest);
        return PlayerLog.onQuest;
    }

    public void ReceiveQuest(Quest temp)
    {
        PlayerLog.CurrentQuest = temp;
        Debug.Log("New Current Quest is: " + PlayerLog.CurrentQuest.Name);
        if (PlayerLog.CurrentQuest != null)
        {
            PlayerLog.onQuest = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if(!PlayerAgent.isStopped)
        {
            PlayerAgent.isStopped = true;
            aPlayerAnimator.SetBool("Walking", false);
        }
    }
    void TransitionSceneOut()
    {
        Debug.Log("Disabling NavMeshAgent");
        PlayerAgent.enabled = false;
        aPlayerAnimator.SetBool("Walking", false);
    }
    public void TransitionSceneIn()
    {
        Debug.Log("Player loaded into new scene");
        PlayerAgent.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Transition")
        {
            // PlayerAgent.isStopped = true;
            TransitionSceneOut();
            other.gameObject.GetComponent<TransitionScript>().TransitionScene();
        }
    }
}
