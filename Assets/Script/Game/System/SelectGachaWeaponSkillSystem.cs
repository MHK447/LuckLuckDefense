using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BanpoFri;

public class SelectGachaWeaponSkillSystem
{

    public enum GachaWeaponSkillType
    {
        AttackDamageIncrease = 1,
        AttackSpeedIncrease = 2,
        BlueFlame = 3,
        CriticalDamageIncrease = 4,
        CriticalHitRateIncrease = 5,
        DarknessSphere = 6,
        LightSphere = 7,
        MonsterKillMoneyBonus = 8,
        PierceSpear = 9,
        RandomEpicUnitAdd = 10,
        RandomRareUnitAdd = 11,
        WaterRise = 12,
        QuickEnergy = 13,
    }

    public void AddWeaponSkillBuff(GachaWeaponSkillType skilltype)
    {
        var finddata = GameRoot.Instance.UserData.CurMode.SelectGachaWeaponSkillDatas.ToList().Find(x => x.SkillTypeIdx == (int)skilltype);

        if(finddata != null)
        {
            finddata.Level += 1;
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
                case GachaWeaponSkillType.RandomEpicUnitAdd:
                    {
                        var findlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.ToList();

                        var unitlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.FindAll(x => x.grade == 3);

                        var randidx = Random.Range(0, unitlist.Count);

                        GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetBattle.AddUnit(unitlist[randidx].unit_idx);
                    }
                    break;
                case GachaWeaponSkillType.RandomRareUnitAdd:
                    {
                        var findlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.ToList();

                        var unitlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.FindAll(x => x.grade == 2);

                        var randidx = Random.Range(0, unitlist.Count);

                        GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.GetBattle.AddUnit(unitlist[randidx].unit_idx);
                    }
                    break;
                case GachaWeaponSkillType.QuickEnergy:
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
                    case GachaWeaponSkillType.AttackDamageIncrease:
                    case GachaWeaponSkillType.AttackSpeedIncrease:
                    case GachaWeaponSkillType.CriticalDamageIncrease:
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

}
