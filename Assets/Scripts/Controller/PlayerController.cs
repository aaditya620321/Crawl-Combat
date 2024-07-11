using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class handles the movement of the player with given input from the input manager
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The Speed at which player moves")]
    public float movespeed = 2f;
    [Tooltip("The Speed at which player look left and right calculated in degrees")]
    public float lookSpeed = 60f;
    [Tooltip("The power with which player jumps")]
    public float jumpPower = 8f;
    [Tooltip("The Strength of Gravity")]
    public float gravity = 9.81f;

    [Header("Jump timimg")]
    public float jumpTimeLeniency = 0.1f;
    float timeToStopBiengLenient = 0;

    [Header("Required References")]
    [Tooltip("The Player shooter script that fires projectiles")]
    public Shooter playerShooter;
    public Health playerHealth;
    public List<GameObject> disableWhileDead;
    bool doubleJumpAvailable = false;

    //The Character contoller component on the player
    private CharacterController controller;
    private InputManager inputManager;

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update call
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Start()
    {
        SetUpCharacterController();
        SetUpInputManager();
    }

    private void SetUpCharacterController()
    {
        controller = GetComponent<CharacterController>();  
        if(controller==null)
        {
            Debug.LogError("The player controller script does not have a character controller on the same game object!");
        }
    }

    void SetUpInputManager()
    {
        inputManager = InputManager.instance;
    }
    /// <summary>
    /// Description:
    /// Standard Unity function called once every frame
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Update()
    {
        if(playerHealth.currentHealth <= 0)
        {
            foreach(GameObject inGameObject in disableWhileDead)
            {
                inGameObject.SetActive(false);
            }
            return;
        }
        else
        {
            foreach (GameObject inGameObject in disableWhileDead)
            {
                inGameObject.SetActive(true);
            }
        }
        ProcessMovement();
        ProcessRotation();
    }

    Vector3 moveDirection;

    void ProcessMovement()
    {
        // Get the input from input manager
        float leftRightInput = inputManager.horizontalMoveAxis;
        //Debug.Log("Leftright input value is " + leftRightInput);
        float forwardBackwardInput = inputManager.verticalMoveAxis;
        bool jumpPressed = inputManager.jumpPressed;

        //Handle the controll of the player when it is on the ground
        if(controller.isGrounded)
        {
            doubleJumpAvailable = true;
            timeToStopBiengLenient = Time.time + jumpTimeLeniency;
            // Set the movement directtion to be recieved input, set y =0 since player is grounded
            moveDirection = new Vector3(leftRightInput, 0, forwardBackwardInput);
            //set the move direction in relation to the transfrom
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection = moveDirection * movespeed;

            if(jumpPressed)
            {
                moveDirection.y = jumpPower;
            }
        }
        else
        {
            moveDirection = new Vector3(leftRightInput * movespeed, moveDirection.y, forwardBackwardInput*movespeed);
            moveDirection = transform.TransformDirection(moveDirection);
            if(jumpPressed && Time.time < timeToStopBiengLenient)
            {
                moveDirection.y = jumpPower;
            }
            else if(jumpPressed && doubleJumpAvailable) 
            {
                moveDirection.y = jumpPower;
                doubleJumpAvailable = false;
            }
        }

        if(controller.isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -0.3f;
        }

        moveDirection.y -= gravity* Time.deltaTime;

        controller.Move(moveDirection* Time.deltaTime);
    }

    void ProcessRotation()
    {
        float horizontalLookInput = inputManager.horizontalLookAxis;
        Vector3 playerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(playerRotation.x, playerRotation.y + horizontalLookInput * lookSpeed * Time.deltaTime, playerRotation.z));
    }    
}
