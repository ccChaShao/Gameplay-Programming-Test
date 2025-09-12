using System;
using System.Collections;
using System.Collections.Generic;
using Gamekit3D.Message;
using Sirenix.OdinInspector;
using UnityEngine;
using Charsiew;
using System;
using UnityEngine.Serialization;

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
        public bool isBlockAttack = false;          // 攻击🔒
        public bool isBlockReload = false;          // 上弹🔒
        public uint bulletCount = 0;            
        public WeaponTriggerType trigger;
        
        // private data
        private GameObject m_GameObject;
        private GameObject m_MuzzleGameObject;
        private Coroutine m_AttackGapCoroutine;
        private Coroutine m_ReloadGapCoroutine;

        public GameObject gameObject
        {
            get => m_GameObject;
            set => m_GameObject = value;
        }

        public GameObject muzzleGameObject
        {
            get => m_MuzzleGameObject;
            set => m_MuzzleGameObject = value;
        }

        public Coroutine attackGapCoroutine
        {
            get => m_AttackGapCoroutine;
            set => m_AttackGapCoroutine = value;
        }

        public Coroutine reloadGapCoroutine
        {
            get => m_ReloadGapCoroutine;
            set => m_ReloadGapCoroutine = value;
        }

        public IEnumerator IEAttackGap()
        {
            isBlockAttack = true;
            yield return new WaitForSeconds(weaponConfig.shotGap);
            isBlockAttack = false;
        }

        public IEnumerator IERealoadGap()
        {
            bulletCount = 0;
            isBlockReload = true;
            yield return new WaitForSeconds(weaponConfig.reloadTime);
            bulletCount = weaponConfig.bulletCount;
            isBlockReload = false;
        }
    }
    
    public partial class PlayerController
    {
        // 武器挂载点
        public Transform gunHolder;
        
        // 近战攻击
        public float normalAttackDuring = 2.0f;
        public bool isBlockNormalAttack = false;
        
        // 二三号武器槽
        public WeaponData weapon02 = new();
        public WeaponData weapon03 = new();
        
        // 状态参数
        private WeaponIndex m_WeaponIndex = WeaponIndex.First;          // 当前武器下标（物理序号）
        private Coroutine m_normalAttackGapCoroutine;
        
        private Vector2 screenCenter = new (Screen.width / 2, Screen.height / 2);

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
            if (m_Input.Attack && !isBlockNormalAttack)
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
            WeaponData data = EnsureWeaponData(m_WeaponIndex);
            if (data == null)
                return;
            // 基础间隔判断
            if (!m_Input.Attack || data.isBlockAttack)
                return;
            
            // 子弹数量判断
            if (data.bulletCount <= 0)
            {
                TryStartReloadWeapon(m_WeaponIndex);        // 尝试上弹
                return;
            }
            
            // 数据更新
            data.bulletCount -= 1;
            
            // 射击执行
            DoShoot();

            // 间隔处理
            switch (data.trigger)
            {
                case WeaponTriggerType.Auto:                // 自动
                {
                    TryStartWeaponAttackGap(m_WeaponIndex);
                    break;
                }
                case WeaponTriggerType.HalfAuto:            // 半自动
                {
                    data.isBlockAttack = true;
                    break;
                }
            }
        }

        private WeaponData EnsureWeaponData(WeaponIndex index)
        {
            WeaponData data = null; 
            
            if (index == WeaponIndex.Second)
                data = weapon02;
            else if(index == WeaponIndex.Third)
                data = weapon03;

            if (data != null)
            {
                // 实例初始化
                if (!data.gameObject)
                {
                    GameObject gobj = Instantiate(data.weaponConfig.weaponPrefab, gunHolder);
                    gobj.transform.localPosition = Vector3.zero;
                    gobj.transform.localRotation = Quaternion.identity;
                    gobj.SetActive(false);
                    data.gameObject = gobj;
                }
                // 枪口对象
                if (!data.muzzleGameObject&& data.gameObject)
                {
                    data.muzzleGameObject = data.gameObject.transform.Find(data.weaponConfig.muzzleName).gameObject;
                }
                // 运行时数据初始化
                if (!data.isInited)
                {
                    data.bulletCount = data.weaponConfig.bulletCount;
                    data.trigger = data.weaponConfig.defaultTrigger;
                    data.isInited = true;
                    data.isBlockAttack = false;
                    data.isBlockReload = false;
                }
            }

            return data;
        }

        private void ChangeWeaponIndex(WeaponIndex newIndex)
        {
            var preIndex = m_WeaponIndex;
            m_WeaponIndex = newIndex;
            
            // 旧武器退出
            var preData = EnsureWeaponData(preIndex);
            if (preData != null)
            {
                preData.gameObject.SetActive(false);
                TryStopWeaponReloadGap(preIndex);
                TryStopWeaponAttackGap(preIndex);
            }

            // 新武器进入
            var newData = EnsureWeaponData(newIndex);
            if (newData != null)
            {
                newData.gameObject.SetActive(true);
            }
            
            // 武器动画更新
            m_Animator.SetLayerWeight(1, newIndex > WeaponIndex.First ? 1 : 0);
        }

        // ======== 普通攻击 ========

        private void ClearNormalAttackGapCoroutine()
        {
            if (m_normalAttackGapCoroutine != null)
                StopCoroutine(m_normalAttackGapCoroutine);
        }

        private IEnumerator IENormalAttackGap(float gap)
        {
            isBlockNormalAttack = true;
            yield return new WaitForSeconds(gap); // 等待指定间隔
            isBlockNormalAttack = false;
        }
        
        // ======== 二三号武器槽位 ========

        private void TryStartReloadWeapon(WeaponIndex index)
        {
            var data = EnsureWeaponData(index);
            if (data == null)
                return;
            if (data.isBlockReload)
            {
                return;
            }
            // 清理
            if (data.reloadGapCoroutine != null)
                StopCoroutine(data.reloadGapCoroutine);
            // 执行
            data.reloadGapCoroutine = StartCoroutine(data.IERealoadGap());
        }

        private void TryStopWeaponReloadGap(WeaponIndex index)
        {
            var data = EnsureWeaponData(index);
            if (data == null)
                return;
            if (data.reloadGapCoroutine != null)
                StopCoroutine(data.reloadGapCoroutine);
            data.isBlockReload = false;
        }

        private void TryStartWeaponAttackGap(WeaponIndex index)
        {
            var data = EnsureWeaponData(index);
            if (data == null)
                return;
            if (data.attackGapCoroutine != null) 
                StopCoroutine(data.attackGapCoroutine);
            data.attackGapCoroutine = StartCoroutine(data.IEAttackGap());
        }

        private void TryStopWeaponAttackGap(WeaponIndex index)
        {
            var data = EnsureWeaponData(index);
            if (data == null)
                return;
            if (data.attackGapCoroutine != null)
                StopCoroutine(data.attackGapCoroutine);
            data.isBlockAttack = false;
        }

        private void DoShoot()
        {
            var data = EnsureWeaponData(m_WeaponIndex);
            if (data == null)
                return;
            Ray ray = cameraSettings.mainCamera.ScreenPointToRay(screenCenter);
            
            Vector3 targetPoint;
            
            if (Physics.Raycast(ray, out RaycastHit raycastHit, shootConfig.maxShootDistance, shootConfig.shootLayerMask))
                // 击中点为目标点
                targetPoint = raycastHit.point;         
            else
                // 远处一点为目标点
                targetPoint = ray.GetPoint(shootConfig.maxShootDistance);
            
            var muzzle = data.muzzleGameObject.transform;
            
            var shootDirection = (targetPoint - muzzle.position).normalized;
            var weaponConfig = data.weaponConfig;
            
            // 执行射击
            GameObject bullet = Instantiate(weaponConfig.bulletPrefab);
            bullet.transform.position = muzzle.position;
            bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
            BulletControler bulletControler = bullet.AddComponent<BulletControler>();
            bulletControler.DataInit(
                weaponConfig.bulletSpeed,
                weaponConfig.damge,
                weaponConfig.bulletAlive,
                weaponConfig.colliderLayer
            );

            // 可选：可视化调试射线
            Debug.DrawRay(muzzle.position, shootDirection * shootConfig.maxShootDistance, Color.red, 1f);
        }
        
        /// <summary>
        /// Mono Awake
        /// </summary>
        private void OnWeaponControlerAwake()
        {
            EnsureWeaponData(WeaponIndex.Third);
            EnsureWeaponData(WeaponIndex.Second);
        }
        
        /// <summary>
        /// Mono OnEnable
        /// </summary>
        private void OnWeaponControlerEnable()
        {
            m_Input.onWeaponButtonDown.AddListener(OnWeaponButtonDown);
            m_Input.onAttackButtonDown.AddListener(OnAttackButtonDown);
            m_Input.onAttackButtonUp.AddListener(OnAttackButtonUp);
            m_Input.onReloadButtonDown.AddListener(OnReloadButtonDown);
            m_Input.onReloadButtonUp.AddListener(OnReloadButtonUp);
            m_Input.onChangeTriggerButtonDown.AddListener(OnChangeTriggerButtonDown);
            m_Input.OnChangeTriggerButtonUp.AddListener(OnChangeTriggerButtonUp);
        }
        
        /// <summary>
        /// Mono OnDisable
        /// </summary>
        private void OnWeaponControlerDisable()
        {
            // 数据清理
            weapon02.isInited = false;
            weapon03.isInited = false;
            StopAllCoroutines();
            // 监听清理
            m_Input.onWeaponButtonDown.RemoveListener(OnWeaponButtonDown);
            m_Input.onAttackButtonDown.RemoveListener(OnAttackButtonDown);
            m_Input.onAttackButtonUp.RemoveListener(OnAttackButtonUp);
            m_Input.onReloadButtonDown.RemoveListener(OnReloadButtonDown);
            m_Input.onReloadButtonUp.RemoveListener(OnReloadButtonUp);
            m_Input.onChangeTriggerButtonDown.RemoveListener(OnChangeTriggerButtonDown);
            m_Input.OnChangeTriggerButtonUp.RemoveListener(OnChangeTriggerButtonUp);
        }

        private void OnAttackEnter() { }

        private void OnAttackExit() { 
            TryStopWeaponAttackGap(m_WeaponIndex);
        }

        private void OnWeaponButtonDown(WeaponIndex index)
        {
            ChangeWeaponIndex(index);
        }

        private void OnAttackButtonDown()
        {
            OnAttackEnter();
        }

        private void OnAttackButtonUp()
        {
            OnAttackExit();
        }

        private void OnReloadButtonDown()
        {
            TryStartReloadWeapon(m_WeaponIndex);
        }

        private void OnReloadButtonUp() { }

        private void OnChangeTriggerButtonDown() { }

        private void OnChangeTriggerButtonUp() { }
    }
}
