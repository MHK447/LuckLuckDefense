using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EffectPath("Effect/BlueFireEffect", false, true)]
public class BlueFireEffect : Effect
{
    public void Set(int damage, InGameEnemyBase enemy)
    {
        enemy.Damage(damage);
    }
}
