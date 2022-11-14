
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class FPSPlayerController : MonoBehaviour
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

    Vector3 m_Direction = Vector3.zero;
    [Range(0, 90)] public float m_AngleToEnterPortalInDegrees;


    [Header("Attach")]
    public Transform m_AttachPosition;
    Rigidbody m_ObjectAttached;
    bool m_AttachedObject = false;
    public float m_AttachingObjectSpeed = 80.0f;
    public float m_MaxDistanceAttachObject = 10.0f;
    public LayerMask m_AttachMask;
    Quaternion m_AttachingObjectStartRotation;
    public float m_AttachedObjectThrowForce = 20.0f;
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
    public KeyCode m_AttachObjectKeyCode = KeyCode.E;

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
    bool m_ClickedButton = false;
    bool m_DummyState = true;

    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;


    public Portal m_BluePortal;
    public Portal m_OrangePortal;

    private bool m_BluePortalActive;
    private bool m_OrangePortalActive;

    public DummyPortal m_DummyPortal;
    public float m_IncreasePortalSpeed = 10.0f;
    [Header("Animations")]
    public Animation m_MyAnimation;
    public AnimationClip m_IdleAnimation;
    public AnimationClip m_ShootAnimation;
    public AnimationClip m_ReloadAnimation;
    public AnimationClip m_RunAnimation;

    public bool m_HaveKey;

    Quaternion m_StartRotation;
    public Transform m_CheckPoint;

    public float m_OffsetPortalTeleport;


    void Start()
    {
        m_BluePortal.gameObject.SetActive(false);
        m_OrangePortal.gameObject.SetActive(false);
        m_DummyPortal.gameObject.SetActive(false);
        m_Yaw = transform.rotation.y;
        m_Pitch = pitchController.localRotation.x;
        m_FOV = m_NormalSpeedFOV;
        GameController.GetGameController().SetPlayer(this);
        m_StartRotation = transform.rotation;

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
        m_Direction = Vector3.zero;
        float l_Speed = m_Speed;


        //Inputs
        if (Input.GetKey(m_UpKey))
        {
            m_Direction = l_ForwardDirection;
        }
        if (Input.GetKey(m_DownKey))
        {
            m_Direction = -l_ForwardDirection;
        }
        if (Input.GetKey(m_RightKey))
        {
            m_Direction += l_RightDirection;
        }
        if (Input.GetKey(m_LeftKey))
        {
            m_Direction -= l_RightDirection;
        }
        if (Input.GetKeyDown(m_JumpKey) && m_OnGround)
        {
            m_VerticalSpeed = m_JumpSpeed;
        }
        if (Input.GetKey(m_RunKeyCode))
        {
            l_Speed = m_Speed * m_FastSpeedMultiplier;

            if (m_FOV < m_FastSpeedFOV)
            {
                m_FOV += m_IncreaseSpeedFOV * Time.deltaTime;
            }


        }

        CheckIfMoving(m_Direction);

        m_Direction.Normalize();

        PlayerRotation();

        m_mov = m_Direction * l_Speed * Time.deltaTime;
        SetGravity();

        CollisionFlags l_collisionFlags = m_CharacterController.Move(m_mov);

        CheckCollision(l_collisionFlags);

       
        if (m_ObjectAttached != null && m_AttachedObject)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ThrowAttachedObject(m_AttachedObjectThrowForce);
            }
            if (Input.GetMouseButtonDown(1))
            {
                ThrowAttachedObject(0);
            }
        }


        if(!m_AttachedObject)
        {
            if (Input.GetMouseButton(0))
            {
                UpdateDummyPortal(m_DummyPortal, m_BluePortal);

            }
            else
            {
                m_DummyState = false;
            }
            if (Input.GetMouseButton(1))
            {
                UpdateDummyPortal(m_DummyPortal, m_OrangePortal);

            }
            else
            {
                m_DummyState = false;
            }
            if (Input.GetMouseButtonUp(0))
            {
                m_ClickedButton = false;
                m_DummyState = false;
                
                Shoot(m_BluePortal);
                UpdateCrosshairState();
                m_BluePortalActive = true;
                m_DummyPortal.transform.localScale = Vector3.one;
            }
            if (Input.GetMouseButtonUp(1))
            {
                m_ClickedButton = false;
                m_DummyState = false;
               
                Shoot(m_OrangePortal);
                UpdateCrosshairState();
                m_OrangePortalActive = true;
                m_DummyPortal.transform.localScale = Vector3.one;
            }

        }

        if (Input.GetKeyDown(m_AttachObjectKeyCode) && CanAttachObject())
        {
            AttachObject();
        }

        if (m_AttachedObject)
        {
            UpdateAttachObject();
        }
        


    }

    public void UpdateCrosshairState()
    {
        m_BluePortalActive = m_BluePortal.gameObject.activeSelf;
        m_OrangePortalActive = m_OrangePortal.gameObject.activeSelf;

        if(m_BluePortalActive && m_OrangePortalActive)
        {
            GameController.GetGameController().GetInterface().ChangeCrosshairState(CROSSHAIR_STATES.Full);
        }
        else if (m_BluePortalActive)
        {
            GameController.GetGameController().GetInterface().ChangeCrosshairState(CROSSHAIR_STATES.Blue);

        }
        else if (m_OrangePortal)
        {
            GameController.GetGameController().GetInterface().ChangeCrosshairState(CROSSHAIR_STATES.Orange);

        }
    }

    void UpdateDummyPortal(DummyPortal _DummyPortal, Portal _CurrentPortal)
    {
        Vector3 l_Position;
        Vector3 l_Normal;
        
        Resizing();

        if (_DummyPortal.IsValidPosition(m_Camera.transform.position, m_Camera.transform.forward, m_MaxShootDistance, m_ShootingLayerMask, out l_Position, out l_Normal))
        {
            m_DummyState = true;

        }
        else
        {
            m_DummyState = false;
            _DummyPortal.IsValidPosition(m_Camera.transform.position, m_Camera.transform.forward, m_MaxShootDistance, m_ShootingLayerMask, out l_Position, out l_Normal);

        }
        UpdateCrosshairState();
        _CurrentPortal.gameObject.SetActive(false);
        m_DummyPortal.gameObject.SetActive(m_DummyState);
    }

    public float l_ScaleIncrease;

    void Resizing()
    {
        
        l_ScaleIncrease = 0.0f;
        if (Input.mouseScrollDelta.y > 0 && m_DummyPortal.transform.localScale.x <= 2.0f && m_DummyPortal.transform.localScale.y <= 2.0f)
        {

            l_ScaleIncrease += Time.deltaTime * m_IncreasePortalSpeed;
            m_DummyPortal.transform.localScale = new Vector3(m_DummyPortal.transform.localScale.x + l_ScaleIncrease, m_DummyPortal.transform.localScale.y + l_ScaleIncrease, m_DummyPortal.transform.localScale.z + l_ScaleIncrease);
           
        }

        if (Input.mouseScrollDelta.y < 0 && m_DummyPortal.transform.localScale.x >= 0.5f && m_DummyPortal.transform.localScale.y >= 0.5f)
        {
            l_ScaleIncrease -= Time.deltaTime * m_IncreasePortalSpeed;
            m_DummyPortal.transform.localScale = new Vector3(m_DummyPortal.transform.localScale.x + l_ScaleIncrease, m_DummyPortal.transform.localScale.y + l_ScaleIncrease, m_DummyPortal.transform.localScale.z + l_ScaleIncrease);
            
        }  

    }

   
    bool CanAttachObject()
    {
        return m_ObjectAttached == null;
    }

    void AttachObject()
    {
        Ray l_Ray = m_Camera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_Ray, out l_RaycastHit, m_MaxDistanceAttachObject, m_AttachMask.value))
        {
            if (l_RaycastHit.collider.GetComponent<Companion>() != null)
            {
                m_AttachedObject = true;
                m_ObjectAttached = l_RaycastHit.collider.GetComponent<Rigidbody>();
                m_ObjectAttached.isKinematic = true;
                m_ObjectAttached.GetComponent<Companion>().SetAttach(true);
                m_AttachingObjectStartRotation = l_RaycastHit.collider.transform.rotation;
            }
        }
    }
    void UpdateAttachObject()
    {
        if (m_ObjectAttached != null)
        {
            Vector3 l_EulerAngles = m_AttachPosition.rotation.eulerAngles;
            Vector3 l_Direction = m_AttachPosition.transform.position - m_ObjectAttached.transform.position;
            float l_Distance = l_Direction.magnitude;
            float l_Movement = m_AttachingObjectSpeed * Time.deltaTime;
            if (l_Movement >= l_Distance)
            {
                m_AttachedObject = true;
                m_ObjectAttached.transform.SetParent(m_AttachPosition);
                m_ObjectAttached.transform.localPosition = Vector3.zero;
                m_ObjectAttached.transform.localRotation = Quaternion.identity;

            }
            else
            {
                l_Direction /= l_Distance;
                m_ObjectAttached.MovePosition(m_ObjectAttached.transform.position + l_Direction * l_Movement);
                m_ObjectAttached.MoveRotation(Quaternion.Lerp(m_AttachingObjectStartRotation, Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z), 1.0f - Mathf.Min(l_Distance / 1.5f, 1.0f)));

            }
        }

    }

    void ThrowAttachedObject(float _Force)
    {
        if (m_ObjectAttached != null)
        {
           
            m_ObjectAttached.transform.SetParent(null);
            m_ObjectAttached.isKinematic = false;
            m_ObjectAttached.AddForce(pitchController.forward * _Force);
            m_ObjectAttached.GetComponent<Companion>().SetAttach(false);
            m_ObjectAttached = null;
            StartCoroutine(SetAttachFalse());
            
        }
    }

    IEnumerator SetAttachFalse()
    {
        yield return new WaitForSeconds(0.25f);
        m_AttachedObject = false;
    }

    void Shoot(Portal _Portal)
    {
        m_DummyPortal.gameObject.SetActive(false);
        Vector3 l_Position;
        Vector3 l_Normal;
        _Portal.transform.localScale = m_DummyPortal.transform.localScale;
        if (_Portal.IsValidPosition(m_Camera.transform.position, m_Camera.transform.forward, m_MaxShootDistance, m_ShootingLayerMask, out l_Position, out l_Normal))
            _Portal.gameObject.SetActive(true);
        else
            _Portal.gameObject.SetActive(false);
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
        if (other.tag == "Portal")
        {
            Portal l_Portal = other.GetComponent<Portal>();
            if (Vector3.Dot(l_Portal.transform.forward, -m_Direction) > Mathf.Cos(m_AngleToEnterPortalInDegrees) * Mathf.Deg2Rad)
                Teleport(l_Portal);
        }
    }

    void Teleport(Portal _Portal)
    {
        Vector3 l_LocalPosition = _Portal.m_OtherPortalTransform.InverseTransformPoint(transform.position);
        Vector3 l_Direction = _Portal.m_OtherPortalTransform.transform.InverseTransformDirection(transform.forward);

        Vector3 l_LocalDirectionMovement = _Portal.m_OtherPortalTransform.transform.InverseTransformDirection(m_Direction);
        Vector3 l_WorldDirectionMovement = _Portal.m_MirrorPortal.transform.TransformDirection(l_LocalDirectionMovement);



        m_CharacterController.enabled = false;
        transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_Direction);
        m_Yaw = transform.rotation.eulerAngles.y;
        transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition) + l_WorldDirectionMovement * m_OffsetPortalTeleport;
        m_CharacterController.enabled = true;
    }

    public void OnDie()
    {
        m_CharacterController.enabled = false;
        m_AngleLocked = true;
        GameController.GetGameController().GetInterface().SetDieInterface();
        
    }

    public void RestartGame()
    {

        m_CharacterController.enabled = false;
        transform.position = m_CheckPoint.position;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
        m_AngleLocked = false;
    }
}



