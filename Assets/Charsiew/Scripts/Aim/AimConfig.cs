using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Charsiew
{
    /// <summary>
    /// 瞄准状态配置 —— 后续接入xls
    /// </summary>
    [CreateAssetMenu(menuName = "Charsiew Assets/SO Assets/Create Aim Config", fileName = "New_Aim_Config")]
    public class AimConfig : ScriptableObject
    {
        [LabelText("鼠标灵敏度")] public float mouseSensitivity;

        [LabelText("最大移动速度")] public float maxForwardSpeed;
    }
}

