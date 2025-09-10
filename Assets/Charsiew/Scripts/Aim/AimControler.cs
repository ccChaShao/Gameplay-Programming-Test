using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Charsiew
{
    public class AimControler : StateBase
    {
        [TitleGroup("瞄准配置")]
        [LabelText("武器握持点")] public Transform weaponHolder;

        [LabelText("武器列表")] public List<WeaponConfig> weaponConfigs = new();
    }
}
