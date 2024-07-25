using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardSystem 
{

    public SkillCardData FindSkillCardData(int skillidx)
    {
        return GameRoot.Instance.UserData.CurMode.SkillCardDatas.Find(x => x.SkillIdx == skillidx);
    }



}
