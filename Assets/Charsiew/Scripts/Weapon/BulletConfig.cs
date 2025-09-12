using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Charsiew
{
    /// <summary>
    /// 子弹配置 —— 后续接入xls
    /// </summary>
    [CreateAssetMenu(menuName = "Charsiew Assets/SO Assets/Create Bullet Config", fileName = "New_Bullet_Config")]
    public class BulletConfig : ScriptableObject
    {
        [LabelText("子弹伤害")] public float bulletDamge;
        
        [LabelText("子弹速度")] public float bulletSpeed;
        
        [LabelText("子弹存活")] public float bulletAlive;
        
        [LabelText("碰撞层级")] public LayerMask colliderLayer;
        
        [LabelText("预制体资源")] public GameObject bulletPrefab;
    }
}
