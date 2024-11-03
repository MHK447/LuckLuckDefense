using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BanpoFri;

public class SelectGachaWeaponSkillSystem
{

    public enum GachaWeaponSkillType
    {
        Meteor = 1,
        WaterRise = 2,
        Earthquake = 3,
        Tornado = 4,
        LightSphere = 5,
        AttackPowerIncrease = 6,
        AttackSpeedIncrease = 7,
        CriticalHitRateIncrease = 8,
        CriticalHitDamageIncrease = 9,
        ExtraGoldOnMonsterKill = 10,
        RandomRareUnit = 11,
        RandomEpicUnit = 12,
        InstantEnergyGain = 13,
        Explosion = 14,
        Thunder = 15,
        BlueFire = 16,
        SwordFallEffect = 17,
        FlamePillarEffect = 18,
        HandOfDarkness = 19,
        DarkExplosion = 20,

    }

    private InGameBattle Battle;

    public void Create()
    {
        GameRoot.Instance.WaitTimeAndCallback(1f, () => {
            Battle = GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetBattle;
        });
    }


    public void AddWeaponSkillBuff(GachaWeaponSkillType skilltype)
    {
        var finddata = GameRoot.Instance.UserData.CurMode.SelectGachaWeaponSkillDatas.ToList().Find(x => x.SkillTypeIdx == (int)skilltype);

        if(finddata != null)
        {
            finddata.LevelProperty.Value += 1;
        }
        else
        {
            var addweapondata = new SelectGachaWeaponSkillData((int)skilltype, 1);
            GameRoot.Instance.UserData.CurMode.SelectGachaWeaponSkillDatas.Add(addweapondata);
        }

        var td = Tables.Instance.GetTable<SelectWeaponGachaSkilInfo>().GetData((int)skilltype);

        if(td != null && td.instantuse == 1)
        {
            switch (skilltype)
            {
                case GachaWeaponSkillType.RandomEpicUnit:
                    {
                        var findlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.ToList();

                        var unitlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.FindAll(x => x.grade == 3);

                        var randidx = Random.Range(0, unitlist.Count);

                        GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetBattle.AddUnit(unitlist[randidx].unit_idx);
                    }
                    break;
                case GachaWeaponSkillType.RandomRareUnit:
                    {
                        var findlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.ToList();

                        var unitlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.FindAll(x => x.grade == 2);

                        var randidx = Random.Range(0, unitlist.Count);

                        GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetBattle.AddUnit(unitlist[randidx].unit_idx);
                    }
                    break;
                case GachaWeaponSkillType.InstantEnergyGain:
                    {
                        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.EnergyMoney, td.value_1);
                        break;
                    }
            }
        }
    }

    public float GetBuffValue(GachaWeaponSkillType type)
    {
        float buffvalue = 0f;

        var finddata = GameRoot.Instance.UserData.CurMode.SelectGachaWeaponSkillDatas.ToList().Find(x => x.SkillTypeIdx == (int)type);

        if(finddata != null)
        {
            var tdtype = Tables.Instance.GetTable<SelectWeaponGachaSkilInfo>().GetData((int)type);

            if(tdtype != null)
            {
                switch(type)
                {
                    case GachaWeaponSkillType.AttackPowerIncrease:
                    case GachaWeaponSkillType.AttackSpeedIncrease:
                    case GachaWeaponSkillType.CriticalHitDamageIncrease:
                    case GachaWeaponSkillType.CriticalHitRateIncrease:
                        {
                            buffvalue = tdtype.value_1 + (tdtype.level_buff_value * (finddata.Level - 1));
                        }
                        break;
                }
            }
        }
        return buffvalue;
    }


    public void Update()
    {
        foreach(var skill in GameRoot.Instance.UserData.CurMode.SelectGachaWeaponSkillDatas)
        {
           switch(skill.SkillTypeIdx)
            {
                case (int)GachaWeaponSkillType.BlueFire:
                case (int)GachaWeaponSkillType.SwordFallEffect:
                case (int)GachaWeaponSkillType.FlamePillarEffect:
                case (int)GachaWeaponSkillType.Thunder:
                case (int)GachaWeaponSkillType.Earthquake:
                case (int)GachaWeaponSkillType.WaterRise:
                case (int)GachaWeaponSkillType.Meteor:
                case (int)GachaWeaponSkillType.Tornado:
                case (int)GachaWeaponSkillType.Explosion:
                case (int)GachaWeaponSkillType.DarkExplosion:
                case (int)GachaWeaponSkillType.HandOfDarkness:
                    {
                        if(Time.time >= skill.NextFireTime)
                        {
                            var enemy =  Battle.GetEnemyList.Find(x => x.IsDeath == false && x.gameObject.activeSelf == true);

                            if (enemy != null)
                            {

                                var td = Tables.Instance.GetTable<SelectWeaponGachaSkilInfo>().GetData((int)skill.SkillTypeIdx);



                                var damage = td.value_1 + (td.level_buff_value * skill.Level);

                                FireSkill((GachaWeaponSkillType)skill.SkillTypeIdx, enemy , damage);

                                skill.NextFireTime = Time.time + td.value_2;
                            }
                        }
                    }
                    break;
            }
        }
    }

    public void FireSkill(GachaWeaponSkillType type , InGameEnemyBase target , int damage)
    {
        switch(type)
        {
            case GachaWeaponSkillType.Meteor:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<MeteorEffect>(target.transform.position, effect =>
                    {
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                        effect.Set(damage);
                    });
                }
                break;
            case GachaWeaponSkillType.WaterRise:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<WaterRiseEffect>(target.transform.position, effect =>
                    {
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                        effect.Set(damage, target);
                    });
                }
                break;
            case GachaWeaponSkillType.Earthquake:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<EarthQuakeEffect>(target.transform.position, effect =>
                    {
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                        effect.Set(damage);
                    });
                }
                break;
            case GachaWeaponSkillType.Explosion:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<BombEffect>(target.transform.position, effect =>
                    {
                        effect.Set(damage);
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                    });
                }
                break;
            case GachaWeaponSkillType.Tornado:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<TornadoEffect>(target.transform.position, effect =>
                    {
                        effect.Set(damage);
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                    });
                }
                break;
            case GachaWeaponSkillType.Thunder:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<ThunderEffect>(target.transform.position, effect =>
                    {
                        effect.Set(damage);
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                    });
                }
                break;
            case GachaWeaponSkillType.BlueFire:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<BlueFireEffect>(target.transform.position, effect =>
                    {
                        effect.Set(damage, target);
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                    });
                }
                break;
            case GachaWeaponSkillType.SwordFallEffect:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<SwordFallEffect>(target.transform.position, effect =>
                    {
                        effect.Set(damage);
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                    });
                }
                break;
            case GachaWeaponSkillType.FlamePillarEffect:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<FlamePillarEffect>(target.transform.position, effect =>
                    {
                        effect.Set(damage);
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                    });
                }
                break;
            case GachaWeaponSkillType.DarkExplosion:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<DarkExplosionEffect>(target.transform.position, effect =>
                    {
                        effect.Set(damage);
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                    });
                }
                break;
            case GachaWeaponSkillType.HandOfDarkness:
                {
                    GameRoot.Instance.EffectSystem.MultiPlay<DarknedEffect>(target.transform.position, effect =>
                    {
                        effect.Set(damage,target);
                        ProjectUtility.SetActiveCheck(effect.gameObject, true);
                        effect.SetAutoRemove(true, 2f);
                    });
                }
                break;
        }
    }

}
