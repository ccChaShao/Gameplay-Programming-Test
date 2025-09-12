using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Charsiew
{
    /// <summary>
    /// 武器配置 —— 后续接入xls
    /// </summary>
    [CreateAssetMenu(menuName = "Charsiew Assets/SO Assets/Create Weapon Config", fileName = "New_Weapon_Config")]
    public class WeaponConfig : ScriptableObject
    {
        [TitleGroup("武器信息")]
        [LabelText("唯一ID")] public uint id;
    
        [LabelText("武器伤害")] public float damge;

        [LabelText("射击间隔")] public float shotGap;
        
        [LabelText("上单时长")] public float reloadTime;
        
        [LabelText("弹匣尺寸")] public uint bulletCount;
    
        [LabelText("默认扳机类型"), EnumToggleButtons] public WeaponTriggerType defaultTrigger;
        
        [LabelText("扳机支持类型"), EnumToggleButtons] public WeaponTriggerType trigger;

        [LabelText("枪口名称")] public string muzzleName = "Muzzle";
        
        [LabelText("武器预制体")] public GameObject weaponPrefab;
        
        [TitleGroup("子弹信息")]
        
        [LabelText("子弹速度")] public float bulletSpeed;
        
        [LabelText("子弹存活")] public float bulletAlive;
        
        [LabelText("碰撞层级")] public LayerMask colliderLayer;
        
        [LabelText("预制体资源")] public GameObject bulletPrefab;
    }
}
