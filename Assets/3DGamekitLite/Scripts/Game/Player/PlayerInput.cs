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

    // 瞄准按钮
    protected const string m_AimButtonKey = "Aim"; 
    protected UnityEvent m_OnAimButtonDown = new ();
    protected UnityEvent m_OnAimButtonUp = new ();
    public UnityEvent onAimButtonDown => m_OnAimButtonDown;
    public UnityEvent onAimButtonUp => m_OnAimButtonUp;
    
    // 武器切换按钮
    protected const string m_FirstWeaponButtonKey = "FirstWeapon";
    protected const string m_SecondWeaponButtonKey = "SecondWeapon";
    protected const string m_ThirdWeaponButtonKey = "ThirdWeapon";
    protected UnityEvent<WeaponIndex> m_OnWeaponButtonDown = new ();
    public UnityEvent<WeaponIndex> onWeaponButtonDown => m_OnWeaponButtonDown;
    
    // 攻击按钮
    protected const string m_AttackButtonKey = "Fire1";
    protected UnityEvent m_OnAttackButtonDown = new ();
    protected UnityEvent m_OnAttackButtonUp = new ();
    public UnityEvent onAttackButtonDown => m_OnAttackButtonDown;
    public UnityEvent onAttackButtonUp => m_OnAttackButtonUp;
    
    // 上弹按钮
    protected const string m_ReloadButtonKey = "Reload";
    protected UnityEvent m_OnReloadButtonDown = new ();
    protected UnityEvent m_OnReloadButtonUp = new ();
    public UnityEvent onReloadButtonDown => m_OnReloadButtonDown;
    public UnityEvent onReloadButtonUp => m_OnReloadButtonUp;
    
    // 扳机切换按钮
    protected const string m_ChangeTriggerKey = "ChangeTrigger";
    protected UnityEvent m_OnChangeTriggerButtonDown = new ();
    protected UnityEvent m_OnChangeTriggerButtonUp = new ();
    public UnityEvent onChangeTriggerButtonDown => m_OnChangeTriggerButtonDown;
    public UnityEvent OnChangeTriggerButtonUp => m_OnChangeTriggerButtonUp;

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

    public bool Attack => (m_Attack && !playerControllerInputBlocked);

    public bool Pause => m_Pause;

    void Awake()
    {
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
        m_Attack = Input.GetButton(m_AttackButtonKey);
        

        if (Input.GetButtonDown(m_AttackButtonKey))
            m_OnAttackButtonDown?.Invoke();
        if (Input.GetButtonUp(m_AttackButtonKey))
            m_OnAttackButtonUp?.Invoke();
        

        if (Input.GetButtonDown(m_AimButtonKey))
            m_OnAimButtonDown?.Invoke();
        if (Input.GetButtonUp(m_AimButtonKey))
            onAimButtonUp?.Invoke();
        

        if (Input.GetButtonDown(m_FirstWeaponButtonKey))
            m_OnWeaponButtonDown?.Invoke(WeaponIndex.First);
        if (Input.GetButtonDown(m_SecondWeaponButtonKey))
            m_OnWeaponButtonDown?.Invoke(WeaponIndex.Second);
        if (Input.GetButtonDown(m_ThirdWeaponButtonKey))
            m_OnWeaponButtonDown?.Invoke(WeaponIndex.Third);
        

        if (Input.GetButtonDown(m_ReloadButtonKey))
            m_OnReloadButtonDown?.Invoke();
        if (Input.GetButtonUp(m_ReloadButtonKey))
            m_OnReloadButtonUp?.Invoke();
        

        if (Input.GetButtonDown(m_ChangeTriggerKey))
            m_OnChangeTriggerButtonDown?.Invoke();
        if (Input.GetButtonUp(m_ChangeTriggerKey))
            m_OnChangeTriggerButtonUp?.Invoke();

        m_Pause = Input.GetButtonDown ("Pause");
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
