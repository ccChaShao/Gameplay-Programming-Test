using System;
using System.Collections;
using System.Collections.Generic;
using Gamekit3D.Message;
using Sirenix.OdinInspector;
using UnityEngine;
using Charsiew;
using System;

namespace Gamekit3D
{
    [Serializable]
    public class WeaponData
    {
        // config
        public WeaponConfig weaponConfig;
        
        // run time state
        [HideInInspector] public bool isInited = false;
        
        // run time data
        [HideInInspector] public uint bulletCount = 0;            
        [HideInInspector] public WeaponTriggerType trigger;
        [HideInInspector] public GameObject gameObject;
    }
    
    public partial class PlayerController
    {
        // 武器挂载点
        public Transform gunHolder;
        
        // 近战攻击
        public float normalAttackDuring = 0.03f;
        
        // 武器栏
        public WeaponData weapon02 = new();
        public WeaponData weapon03 = new();
        
        // 状态参数
        private WeaponIndex m_WeaponIndex = WeaponIndex.First;            // 当前武器下标（物理序号）
        private bool m_IsBlockAttack = false;
        private Coroutine m_AttackWaitCoroutine;

        private void TryAttack()
        {
            switch (currentState)
            {
                case CharacterState.NormalState:
                {
                    if (m_WeaponIndex == WeaponIndex.First)         // 普通模式只能进行普通攻击
                        TryExcuteNoramlAttack();
                    break;
                }
                case CharacterState.ShotState:
                {
                    if (m_WeaponIndex == WeaponIndex.First)
                        TryExcuteNoramlAttack();
                    else
                        TryExcuteShotAttack();
                    break;
                }
            }
        }

        /// <summary>
        /// 进入普通攻击
        /// </summary>
        private void TryExcuteNoramlAttack()
        {
            if (m_Input.Attack)
            {
                m_Animator.SetTrigger(m_HashMeleeAttack);
            }
        }

        /// <summary>
        /// 进入射击攻击
        /// </summary>
        private void TryExcuteShotAttack()
        {
            WeaponData currentWeaponData = EnsureWeaponData(m_WeaponIndex);
            switch (currentWeaponData.trigger)
            {
                case WeaponTriggerType.Auto:                // 自动
                {
                    break;
                }
                case WeaponTriggerType.HalfAuto:            // 半自动
                {
                    break;
                }
            }
        }

        private WeaponData EnsureWeaponData(WeaponIndex index, bool objActive = false)
        {
            WeaponData weaponData = null; 
            
            if (index == WeaponIndex.Second)
            {
                weaponData = weapon02;
            }
            else if(index == WeaponIndex.Third)
            {
                weaponData = weapon03;
            }

            if (weaponData != null)
            {
                // 实例初始化
                if (!weaponData.gameObject)
                {
                    GameObject gobj = Instantiate(weaponData.weaponConfig.weaponPrefab, gunHolder);
                    gobj.transform.localPosition = Vector3.zero;
                    gobj.transform.localRotation = Quaternion.identity;
                    weaponData.gameObject = gobj;
                }
                // 运行时数据初始化
                if (!weaponData.isInited)
                {
                    weaponData.bulletCount = weaponData.weaponConfig.bulletCount;
                    weaponData.trigger = weaponData.weaponConfig.defaultTrigger;
                    weaponData.isInited = true;
                }
                // 显示初始化
                weaponData.gameObject.SetActive(objActive);
            }

            return weaponData;
        }

        private void ChangeWeaponIndex(WeaponIndex newIndex)
        {
            var pre = m_WeaponIndex;
            m_WeaponIndex = newIndex;
            
            // 旧武器退出
            EnsureWeaponData(pre);

            // 新武器进入
            EnsureWeaponData(newIndex, true);

            // 武器动画更新
            m_Animator.SetLayerWeight(1, newIndex > WeaponIndex.First ? 1 : 0);
        }

        private IEnumerator AttackWait(float wait)
        {
            Debug.Log("charsiew : [AttackWait] : -------------------- 进入。"+wait);
            m_IsBlockAttack = true;

            yield return new WaitForSeconds(wait);

            Debug.Log("charsiew : [AttackWait] : -------------------- 退出。"+wait);
            m_IsBlockAttack = false;
            m_AttackWaitCoroutine = StartCoroutine(AttackWait(wait));
        }

        /// <summary>
        /// 攻击进入
        /// </summary>
        private void OnAttackEnter()
        {
            float waitTime = normalAttackDuring;
            if (m_WeaponIndex >= WeaponIndex.Second)
            {
                WeaponData currentWeaponData = EnsureWeaponData(m_WeaponIndex);
                waitTime = currentWeaponData.weaponConfig.shotGap;
            }
            
            // 携程重置
            if (m_AttackWaitCoroutine != null)
            {
                StopCoroutine(m_AttackWaitCoroutine);
            }
            m_AttackWaitCoroutine = StartCoroutine(AttackWait(waitTime));
        }

        /// <summary>
        /// 攻击退出
        /// </summary>
        private void OnAttackExit()
        {
            // 携程清空
            if (m_AttackWaitCoroutine != null)
            {
                StopCoroutine(m_AttackWaitCoroutine);
            }
        }

        private void OnWeaponSwitcherAwake()
        {
            EnsureWeaponData(WeaponIndex.Third);
            EnsureWeaponData(WeaponIndex.Second);
        }
        
        private void OnWeaponSwitcherEnable()
        {
            m_Input.onWeaponButtonDown.AddListener(OnWeaponButtonDown);
            m_Input.onAttackButtonDown.AddListener(OnAttackButtonDown);
            m_Input.onAttackButtonUp.AddListener(OnAttackButtonUp);
        }
        
        private void OnWeaponSwitcherDisable()
        {
            m_Input.onWeaponButtonDown.RemoveListener(OnWeaponButtonDown);
            m_Input.onAttackButtonDown.RemoveListener(OnAttackButtonDown);
            m_Input.onAttackButtonUp.RemoveListener(OnAttackButtonUp);
        }

        private void OnWeaponButtonDown(WeaponIndex newIndex)
        {
            ChangeWeaponIndex(newIndex);
        }

        private void OnAttackButtonDown()
        {
            OnAttackEnter();
        }

        private void OnAttackButtonUp()
        {
            OnAttackExit();
        }
    }
}
