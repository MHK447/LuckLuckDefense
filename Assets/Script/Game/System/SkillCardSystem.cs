using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;

public class SkillCardSystem 
{

    public SkillCardData FindSkillCardData(int skillidx)
    {
        return GameRoot.Instance.UserData.CurMode.SkillCardDatas.Find(x => x.SkillIdx == skillidx);
    }


    public int GachaUnitCard()
    {
        var tdlist = Tables.Instance.GetTable<SkillCardInfo>().DataList.ToList();

        return Random.Range(0, tdlist.Count);
    }


    public void SkillCardLevelUp(int skillidx)
    {
        var finddata = FindSkillCardData(skillidx);

        if(finddata != null)
        {
            finddata.Level += 1;
        }
        else
        {
            var skillcardata = new SkillCardData(skillidx, 1);

            GameRoot.Instance.UserData.CurMode.SkillCardDatas.Add(skillcardata);
        }
    }

}
