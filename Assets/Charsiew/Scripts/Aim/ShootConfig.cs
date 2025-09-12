using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Charsiew
{
    [CreateAssetMenu(menuName = "Charsiew Assets/SO Assets/Create Shoot Config", fileName = "New_Shoot_Config")]
    public class ShootConfig : ScriptableObject
    {
        [LabelText("最大射击距离")] public float maxShootDistance;

        [LabelText("射击碰撞图层")] public LayerMask shootLayerMask;
        
        [LabelText("鼠标灵敏度")] public float mouseSensitivity;

        [LabelText("最大移动速度")] public float maxForwardSpeed;
    }
}

