using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;


[UIPath("UI/Popup/PopupPassiveCardUpgrade")]
public class PopupPassiveCardUpgrade : UIBase
{
    [SerializeField]
    private List<PassiveCardComponent> PassiveCardComponentList = new List<PassiveCardComponent>();

    public void Init()
    {
        var tdlist = Tables.Instance.GetTable<SkillCardInfo>().DataList;

        for(int i = 0; i < tdlist.Count; ++i)
        {
            PassiveCardComponentList[i].Set(tdlist[i].skill_idx);
        }

    }


}
