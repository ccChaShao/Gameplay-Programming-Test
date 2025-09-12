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
        [TitleGroup("基本信息")]
        [LabelText("唯一ID")] public uint id;
    
        [LabelText("武器伤害")] public float damage;

        [LabelText("射击间隔")] public float shotGap;
        
        [LabelText("上单时长")] public float reloadTime;
        
        [LabelText("弹匣尺寸")] public uint bulletCount;
    
        [LabelText("默认扳机类型"), EnumToggleButtons] public WeaponTriggerType defaultTrigger;
        
        [LabelText("扳机支持类型"), EnumToggleButtons] public WeaponTriggerType trigger;

        [LabelText("枪口名称")] public string muzzleName = "Muzzle";
        
        [LabelText("预制体资源")] public GameObject weaponPrefab;
        
        [FormerlySerializedAs("BulletConfig")]
        [TitleGroup("额外信息")]
        
        [LabelText("子弹")] public BulletConfig bulletConfig = new ();
    }
}
