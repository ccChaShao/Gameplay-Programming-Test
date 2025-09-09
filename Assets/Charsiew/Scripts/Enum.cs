using System;

namespace Charsiew
{
    /// <summary>
    /// 武器射击触发类型
    /// </summary>
    [Flags]
    public enum WeaponTriggerType
    {
        HalfAuto = 1 << 0,      // 半自动
        Auto = 1 << 1,          // 自动
        Charge = 1 << 2,        // 蓄力武器（未实现）
    }

    /// <summary>
    /// 玩家状态
    /// </summary>
    public enum CharacterState
    {
        NormalState,
        ShotState,
    }
}
