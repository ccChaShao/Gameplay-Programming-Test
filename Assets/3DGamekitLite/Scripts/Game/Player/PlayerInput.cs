using UnityEngine;
using System;
using System.Collections;
using Gamekit3D;
using UnityEngine.Events;
using Charsiew;


public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance
    {
        get { return s_Instance; }
    }

    protected static PlayerInput s_Instance;

    [HideInInspector]
    public bool playerControllerInputBlocked;

    protected Vector2 m_Movement;
    protected Vector2 m_Camera;
    protected bool m_Jump;
    protected bool m_Attack;
    protected bool m_Pause;
    protected bool m_ExternalInputBlocked;

    #region Charsiew

    protected const string m_AimButtonKey = "Aim"; 
    
    protected UnityEvent m_OnAimButtonDown = new ();
    public UnityEvent onAimButtonDown => m_OnAimButtonDown;
    
    protected UnityEvent m_OnAimButtonUp = new ();
    public UnityEvent onAimButtonUp => m_OnAimButtonUp;
    
    protected string m_FirstWeaponButtonKey = "FirstWeapon";
    protected string m_SecondWeaponButtonKey = "SecondWeapon";
    protected string m_ThirdWeaponButtonKey = "ThirdWeapon";
    
    protected UnityEvent<WeaponIndex> m_OnWeaponButtonDown = new ();
    public UnityEvent<WeaponIndex> onWeaponButtonDown => m_OnWeaponButtonDown;

    #endregion

    public Vector2 MoveInput
    {
        get
        {
            if(playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Movement;
        }
    }

    public Vector2 CameraInput
    {
        get
        {
            if(playerControllerInputBlocked || m_ExternalInputBlocked)
                return Vector2.zero;
            return m_Camera;
        }
    }

    public bool JumpInput
    {
        get { return m_Jump && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Attack
    {
        get { return m_Attack && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
    }

    public bool Pause
    {
        get { return m_Pause; }
    }

    WaitForSeconds m_AttackInputWait;
    Coroutine m_AttackWaitCoroutine;

    const float k_AttackInputDuration = 0.03f;

    void Awake()
    {
        m_AttackInputWait = new WaitForSeconds(k_AttackInputDuration);
    
        if (s_Instance == null)
            s_Instance = this;
        else if (s_Instance != this)
            throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + s_Instance.name + " and " + name + ".");
    }


    void Update()
    {
        m_Movement.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_Camera.Set(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        m_Jump = Input.GetButton("Jump");

        if (Input.GetButtonDown("Fire1"))
        {
            if (m_AttackWaitCoroutine != null)
                StopCoroutine(m_AttackWaitCoroutine);

            m_AttackWaitCoroutine = StartCoroutine(AttackWait());
        }

        if (Input.GetButtonDown(m_AimButtonKey))
        {
            m_OnAimButtonDown?.Invoke();
        }

        if (Input.GetButtonUp(m_AimButtonKey))
        {
            onAimButtonUp?.Invoke();
        }

        if (Input.GetButtonDown(m_FirstWeaponButtonKey))
        {
            m_OnWeaponButtonDown?.Invoke(WeaponIndex.First);
        }

        if (Input.GetButtonDown(m_SecondWeaponButtonKey))
        {
            m_OnWeaponButtonDown?.Invoke(WeaponIndex.Second);
        }

        if (Input.GetButtonDown(m_ThirdWeaponButtonKey))
        {
            m_OnWeaponButtonDown?.Invoke(WeaponIndex.Third);
        }

        m_Pause = Input.GetButtonDown ("Pause");
    }

    IEnumerator AttackWait()
    {
        m_Attack = true;

        yield return m_AttackInputWait;

        m_Attack = false;
    }

    public bool HaveControl()
    {
        return !m_ExternalInputBlocked;
    }

    public void ReleaseControl()
    {
        m_ExternalInputBlocked = true;
    }

    public void GainControl()
    {
        m_ExternalInputBlocked = false;
    }
}
