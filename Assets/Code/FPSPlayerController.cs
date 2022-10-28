
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FPSPlayerController : MonoBehaviour
{

    float m_Yaw;
    float m_Pitch;
    public float m_Life = 5f;

    public float m_YawRotationSpeed;
    public float m_PitchRotationSpeed;

    public float m_MinPitch;
    public float m_MaxPitch;

    public float m_PlayerMass;

    public Transform pitchController;
    

    public bool yawInverted;
    public bool pitchInverted;

    public bool m_moving;


    

    public CharacterController m_CharacterController;
    public float m_Speed;
    [Header("Inputs")]
    public KeyCode m_LeftKey;
    public KeyCode m_RightKey;
    public KeyCode m_UpKey;
    public KeyCode m_DownKey;
    public KeyCode m_ReloadKey;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKey = KeyCode.Space;

 
    [Header(" ")]
    float m_VerticalSpeed = 0.0f;
    bool m_OnGround = true;

    public float m_JumpSpeed = 10.0f;

    public float m_FastSpeedMultiplier = 1.5f;

    public Camera m_Camera;
    public float m_NormalSpeedFOV;
    public float m_FastSpeedFOV;
    public float m_IncreaseSpeedFOV;
    private float m_FOV;

    private float m_TimeOnAir;
    public float m_CoyoteTime = 0.0f;

    private Vector3 m_mov;


    [Header("Shoot")]
    public float m_MaxShootDistance = 50.0f;
    public LayerMask m_ShootingLayerMask;
    public Transform m_FirePoint;
    public GameObject m_ShootCanonEffect;
    public static Action OnShoot;

    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;

    public float m_AimingFOV;
    public float m_NotAimingFOV;
    bool m_Aiming;

    [Header("Animations")]
    public Animation m_MyAnimation;
    public AnimationClip m_IdleAnimation;
    public AnimationClip m_ShootAnimation;
    public AnimationClip m_ReloadAnimation;
    public AnimationClip m_RunAnimation;

    bool m_IsRunning = false;



    public bool m_HaveKey;

    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    void Start()
    {
        m_Yaw = transform.rotation.y;
        m_Pitch = pitchController.localRotation.x;
        m_FOV = m_NormalSpeedFOV;
        //SetIdleWeaponAnimation();
        //GameController.GetGameController().SetPlayer(this);
        m_StartRotation = transform.rotation;
        m_StartPosition = transform.position;
       
       
    }

#if UNITY_EDITOR
    void Shortcuts()
    {
        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
    }

#endif

    void Update()
    {
#if UNITY_EDITOR

        Shortcuts();
#endif

        Vector3 l_RightDirection = transform.right;
        Vector3 l_ForwardDirection = transform.forward;
        Vector3 l_Direction = Vector3.zero;
        float l_Speed = m_Speed;

        
        //Inputs
        if (Input.GetKey(m_UpKey))
        {
            l_Direction = l_ForwardDirection;
        }
        if (Input.GetKey(m_DownKey))
        {
            l_Direction = -l_ForwardDirection;
        }
        if (Input.GetKey(m_RightKey))
        {
            l_Direction += l_RightDirection;
        }
        if (Input.GetKey(m_LeftKey))
        {
            l_Direction -= l_RightDirection;
        }
        if (Input.GetKeyDown(m_JumpKey) && m_OnGround)
        {
            m_VerticalSpeed = m_JumpSpeed;
        }
        if (Input.GetKey(m_RunKeyCode))
        {
           l_Speed = m_Speed * m_FastSpeedMultiplier;
           m_IsRunning = true;
            if (m_FOV < m_FastSpeedFOV)
            {
                m_FOV += m_IncreaseSpeedFOV * Time.deltaTime;
            }


        }
        else
        {
            m_IsRunning = false;
            if (m_FOV > m_NormalSpeedFOV && !m_Aiming)
            {
                m_FOV -= m_IncreaseSpeedFOV * Time.deltaTime;
            }
        }
        


        CheckIfMoving(l_Direction);

      
        


        l_Direction.Normalize();

        PlayerRotation();

        //Move
        m_mov = l_Direction * l_Speed * Time.deltaTime;



        SetGravity();

        CollisionFlags l_collisionFlags = m_CharacterController.Move(m_mov);

        CheckCollision(l_collisionFlags);


        if (Input.GetMouseButton(0))
        {
            //m_CurrentWeapon.Shoot();

        }


        if (Input.GetMouseButton(1))
        {
            m_Aiming = true;
            if (m_FOV > m_AimingFOV)
            {
                m_FOV -= m_IncreaseSpeedFOV * Time.deltaTime;
            }
            
        }
        else
        {
            m_Aiming = false;
            if (m_FOV < m_NotAimingFOV)
            {
                m_FOV += m_IncreaseSpeedFOV * Time.deltaTime;
            }
           
        }
        m_Camera.fieldOfView = m_FOV;

   


    }

   

    void SetIdleWeaponAnimation()
    {
        m_MyAnimation.CrossFade(m_IdleAnimation.name);
    }


    void SetRunAnimation()
    {
        
        m_MyAnimation.CrossFade(m_RunAnimation.name);
        m_MyAnimation.CrossFadeQueued(m_IdleAnimation.name);
            
        
       
    }

    

  
    void SetGravity()
    {
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime - m_PlayerMass;
        m_mov.y = m_VerticalSpeed * Time.deltaTime;
    }

    void PlayerRotation()
    {
        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");
#if UNITY_EDITOR
        if (m_AngleLocked)
        {
            l_MouseX = 0.0f;
            l_MouseY = 0.0f;
        }
#endif

        m_Yaw += m_YawRotationSpeed * l_MouseX * Time.fixedDeltaTime * (yawInverted ? -1f : 1f);
        m_Pitch += m_PitchRotationSpeed * l_MouseY * Time.fixedDeltaTime * (pitchInverted ? -1f : 1f);
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);



        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        pitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);
    }



    void CheckCollision(CollisionFlags collisionFlag)
    {
        if ((collisionFlag & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }

        if ((collisionFlag & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = 0.0f;
            m_TimeOnAir = 0.0f;
            m_OnGround = true;
        }
        else
        {
            m_TimeOnAir += Time.deltaTime;
            if (m_TimeOnAir > m_CoyoteTime)
                m_OnGround = false;
        }
    }

    void CheckIfMoving(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            m_moving = true;
        }
        else
        {
            m_moving = false;
        }
    }

   


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            //other.GetComponent<Item>().Pick(this);
        }
        else if(other.tag == "DeadZone")
        {
            //m_PlayerHealth.OnDie();
        }
        else if (other.tag == "MovingPlatform")
        {
            transform.parent = other.transform;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "MovingPlatform")
        {
            transform.parent = null;
        }
    }

    public void RestartGame()
    {
       
        m_CharacterController.enabled = false;
        transform.rotation = m_StartRotation;
        //transform.position = GameController.GetGameController().GetLevel().GetLastCheckPoint().position;
        m_CharacterController.enabled = true;
    }

}



