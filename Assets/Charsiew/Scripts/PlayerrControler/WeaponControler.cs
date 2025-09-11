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
        
        private bool m_IsBlockAttack = false;
        private Coroutine m_AttackGapCoroutine;

        public Coroutine attackGapCoroutine
        {
            get => m_AttackGapCoroutine;
            set => m_AttackGapCoroutine = value;
        }

        public bool isBlockAttack
        {
            get => m_IsBlockAttack;
            set => m_IsBlockAttack = value;
        }

        public IEnumerator IENormalWeapon02Gap()
        {
            m_IsBlockAttack = true;
            yield return new WaitForSeconds(weaponConfig.shotGap);
            m_IsBlockAttack = false;
        }
    }
    
    public partial class PlayerController
    {
        // 武器挂载点
        public Transform gunHolder;
        
        // 近战攻击
        public float normalAttackDuring = 2.0f;
        private bool m_IsBlockNormalAttack = false;
        private Coroutine m_normalAttackGapCoroutine;
        
        // 武器栏
        public WeaponData weapon02 = new();
        private Coroutine m_Weapon02GapCoroutine;
        public WeaponData weapon03 = new();
        private Coroutine m_Weapon03GapCoroutine;
        
        // 状态参数
        private WeaponIndex m_WeaponIndex = WeaponIndex.First;          // 当前武器下标（物理序号）

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
            if (m_Input.Attack && !m_IsBlockNormalAttack)
            {
                // 攻击进入
                m_Animator.SetTrigger(m_HashMeleeAttack);
                // 间隔进入
                ClearNormalAttackGapCoroutine();
                m_normalAttackGapCoroutine = StartCoroutine(IENormalAttackGap(normalAttackDuring));
            }
        }

        /// <summary>
        /// 进入射击攻击
        /// </summary>
        private void TryExcuteShotAttack()
        {
            WeaponData weaponData = EnsureWeaponData(m_WeaponIndex);
            if (!m_Input.Attack || weaponData.isBlockAttack)
            {
                return;
            }

            switch (weaponData.trigger)
            {
                case WeaponTriggerType.Auto:                // 自动
                {
                    // 攻击进入
                    Debug.Log("charsiew : [TryExcuteShotAttack] : --------------------- 全自动射击。");
                    // 间隔进入
                    if (weaponData.attackGapCoroutine != null) 
                        StopCoroutine(weaponData.attackGapCoroutine);
                    weaponData.attackGapCoroutine = StartCoroutine(weaponData.IENormalWeapon02Gap());
                    break;
                }
                case WeaponTriggerType.HalfAuto:            // 半自动
                {
                    // 攻击进入
                    Debug.Log("charsiew : [TryExcuteShotAttack] : --------------------- 半自动射击。");
                    // 间隔进入（区分武器槽位，考虑下怎么统一清理）
                    weaponData.isBlockAttack = true;
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

        #region 攻击间隔协程

        // ======== 普通攻击 ========

        private void ClearNormalAttackGapCoroutine()
        {
            if (m_normalAttackGapCoroutine != null)
                StopCoroutine(m_normalAttackGapCoroutine);
        }

        private IEnumerator IENormalAttackGap(float gap)
        {
            m_IsBlockNormalAttack = true;
            yield return new WaitForSeconds(gap); // 等待指定间隔
            m_IsBlockNormalAttack = false;
        }

        #endregion

        /// <summary>
        /// 攻击进入
        /// </summary>
        private void OnAttackEnter()
        {
        }

        /// <summary>
        /// 攻击退出
        /// </summary>
        private void OnAttackExit()
        {
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
            // 数据清理
            ClearNormalAttackGapCoroutine();
            if (weapon02.attackGapCoroutine != null) 
                StopCoroutine(weapon02.attackGapCoroutine);
            if (weapon02.attackGapCoroutine != null) 
                StopCoroutine(weapon02.attackGapCoroutine);
            // 监听清理
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
