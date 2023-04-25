using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
 
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float jumpForce, smoothTime;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    Rigidbody rb;
    PhotonView PV;

    const float maxHealth = 100;
    float currentHealth = maxHealth;
    PlayerManager playerManager;


    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;

    //

    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] float gravity = -13.0f;


    [SerializeField] bool lockCursor = true;

    //

    [SerializeField] private float runSpeed = 5.0f;
    [SerializeField] private float runBuildUpSpeed;
    [SerializeField] private KeyCode runKey;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;


    //


    public float JumpSpeed = 8.0f;
    private Vector3 moveDirection = Vector3.zero;
    private bool isRunning;
    private float normalSpeed = 6.0f;


    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    public int team = 0;
   
    


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();

    }

    void Start()
    {
        
        if (PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
            Destroy(controller);
        }
    }

    void Update()
    {
        Jump();
        UpdateMouseLook();
        UpdateMovement();
        if (!PV.IsMine)

            return;

        for(int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if(itemIndex >= items.Length - 1)
            {
                EquipItem(0);
            }
            EquipItem(itemIndex + 1);
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if(itemIndex <= 0)
            {
                EquipItem(items.Length - 1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }


        if(Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }

       
        
    }


    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }


    }



    void UpdateMouseLook()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;

        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement()
    {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if (controller.isGrounded)
        {
            velocityY = 0.0f;
            if (Input.GetButton("Jump"))
            {
                velocityY = JumpSpeed;

            }
        }

        velocityY += gravity * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKey(KeyCode.W) && Input.GetButton("Fire3"))
        {
            walkSpeed = runSpeed;
            isRunning = true;
        }
        else
        {
            isRunning = false;
            walkSpeed = normalSpeed;
        }
    }




   public void SetGroundedState(bool _grounded)
   {
       grounded = _grounded;
   }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        if (!PV.IsMine)
        {
            return;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }
    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
            return;


        if (_index == previousItemIndex)
            return;

        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if(PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        }
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC] 
    void RPC_TakeDamage(float damage)
    {
        if (!PV.IsMine)
            return;

        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        if(currentHealth <= 0)
        {
            if (PV.IsMine)
            {
                PV.RPC("RPC_PlayerKilled", RpcTarget.All, team);
            }
            Die();
        }
    }


    //[PunRPC]
    //void RPC_PlayerKilled()
    //{
    //    if (playerManager.playerOne)
    //    {
    //        FindObjectOfType<GameManager>().playerTwoScore++;
    //    }
    //    if (playerManager.playerTwo)
    //    {
    //        FindObjectOfType<GameManager>().playerOneScore++;
    //    }

    //}



    void Die()
    {
        playerManager.Die();
    }

}