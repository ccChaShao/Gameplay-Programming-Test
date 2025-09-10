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
        public WeaponConfig weaponConfig;
        
        public GameObject gameObject;
    }
    
    public partial class PlayerController
    {
        // 武器挂载点
        public Transform gunHolder;
        
        // 武器栏
        public WeaponData weapon02;
        public WeaponData weapon03;
        
        // 状态参数
        private WeaponIndex weaponIndex = WeaponIndex.First;            // 当前武器下标（物理序号）

        public void ExcuteAttack()
        {
            switch (currentState)
            {
                case CharacterState.NormalState:
                {
                    // 普通模式只能进行普通攻击
                    if (weaponIndex == WeaponIndex.First)
                    {
                        TryExcuteNoramlAttack();
                    }
                    break;
                }
                case CharacterState.ShotState:
                {
                    if (weaponIndex == WeaponIndex.First)
                    {
                        TryExcuteNoramlAttack();
                    }
                    else
                    {
                        TryExcuteShotAttack();
                    }
                    break;
                }
            }
        }

        private void TryExcuteNoramlAttack()
        {
            bool isInAttack = m_Input.Attack && canAttack;
            if (isInAttack)
            {
                m_Animator.SetTrigger(m_HashMeleeAttack);
            }
        }

        private void TryExcuteShotAttack()
        {
            Debug.Log("charsiew : [ExcuteAttack] : ------------------- 射击。");
        }

        private WeaponData EnsureWeaponConfig(WeaponIndex index, bool objActive = false)
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
                if (!weaponData.gameObject)
                {
                    GameObject gobj = Instantiate(weaponData.weaponConfig.weaponPrefab, gunHolder);
                    gobj.transform.localPosition = Vector3.zero;
                    gobj.transform.localRotation = Quaternion.identity;
                    weaponData.gameObject = gobj;
                }
                weaponData.gameObject.SetActive(objActive);
            }

            return weaponData;
        }

        private void ChangeWeaponIndex(WeaponIndex newIndex)
        {
            var pre = weaponIndex;
            weaponIndex = newIndex;
            
            // 旧武器退出
            EnsureWeaponConfig(pre, false);

            // 新武器进入
            EnsureWeaponConfig(newIndex, true);

            // 武器动画更新
            m_Animator.SetLayerWeight(1, newIndex > WeaponIndex.First ? 1 : 0);
        }
        
        private void OnWeaponSwitcherEnable()
        {
            m_Input.onWeaponButtonDown.AddListener(OnWeaponButtonDown);
        }
        
        private void OnWeaponSwitcherDisable()
        {
            m_Input.onWeaponButtonDown.RemoveListener(OnWeaponButtonDown);
        }

        private void OnWeaponButtonDown(WeaponIndex newIndex)
        {
            ChangeWeaponIndex(newIndex);
        }
    }
}
