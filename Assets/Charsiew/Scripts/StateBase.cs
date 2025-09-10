using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase : MonoBehaviour
{
    private bool m_IsEnable = false;

    public bool IsEnable
    {
        get { return m_IsEnable; }
        set { m_IsEnable = value; }
    }

    public void EnterState()
    {
        m_IsEnable = true;
        OnEnterAimState();
    }

    public void ExitState()
    {
        m_IsEnable = false;
        OnExitAimState();
    }

    protected virtual void OnEnterAimState() { }
    protected virtual void OnExitAimState() { }
}
