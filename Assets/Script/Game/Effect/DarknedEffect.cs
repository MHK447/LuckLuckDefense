using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EffectPath("Effect/DarknedEffect", false, true)]
public class DarknedEffect : Effect
{
    public void Set(int damage, InGameEnemyBase enemy)
    {
        enemy.Damage(damage);
    }
}
