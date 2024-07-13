using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;


public class UnitSkillSystem
{

    public enum PassiveSkillType
    {
        DefenseReduction = 4,
        AttackPowerIncrease = 5,
        MoveSpeedReduction = 6,
        CoinGainSummon = 8,
    }


    public enum ActiveSkillType
    {
        AttackSkill = 1,
        Sturn = 2,
        Slow = 3,
        AttackCoin = 7,
    }

    public int[] PassiveSkillIdxList = {(int)PassiveSkillType.DefenseReduction , (int)PassiveSkillType.AttackPowerIncrease ,
        (int)PassiveSkillType.MoveSpeedReduction , (int)PassiveSkillType.CoinGainSummon };

    public void AddPassiveSkill(int unitidx)
    {
        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if(td != null)
        {
            foreach(var skillidx in td.unit_skill)
            {
                var skilltd = Tables.Instance.GetTable<PlayerSkillInfo>().GetData(skillidx);

                if(skilltd != null && PassiveSkillIdxList.Contains(skillidx))
                {
                    GameRoot.Instance.UserData.CurMode.PassiveSkillDatas.Add(new PassiveSkillData(skillidx, td.unit_skil_value, unitidx));
                }
            }
        }
    }


    public void RemovePassiveSkill(int unitidx)
    {
        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if (td != null)
        {
            foreach (var skillidx in td.unit_skill)
            {
                var findskill = GameRoot.Instance.UserData.CurMode.PassiveSkillDatas.Find(x => x.SkillIdx == skillidx);

                if (findskill != null)
                {
                    GameRoot.Instance.UserData.CurMode.PassiveSkillDatas.Remove(findskill);
                }

            }
        }
    }

    public int FindPassiveSkillValue(int skillidx)
    {
        int skillvalue = 0;


        var findallskill = GameRoot.Instance.UserData.CurMode.PassiveSkillDatas.FindAll(x => x.SkillIdx == skillidx);


        foreach(var findskill in findallskill)
        {
            skillvalue += findskill.SkillValue;
        }

        return skillvalue;
    }
}
