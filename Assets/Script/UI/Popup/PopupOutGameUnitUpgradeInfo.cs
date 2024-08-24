using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;


[UIPath("UI/Page/PopupOutGameUnitUpgradeInfo")]
public class PopupOutGameUnitUpgradeInfo : UIBase
{

    [SerializeField]
    private Image UnitImg;

    [SerializeField]
    private Text UnitNameText;

    [SerializeField]
    private Text DamageText;

    [SerializeField]
    private Text SkillCoolTimeText;

    [SerializeField]
    private List<OutGameUnitUpgradeInfoComponent> InfoComponentList = new List<OutGameUnitUpgradeInfoComponent>();

    private int UnitIdx = 0;

    public void Set(int unitidx)
    {
        UnitIdx = unitidx;

        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if(td != null)
        {
            UnitImg.sprite = Config.Instance.GetUnitImg(td.icon);

            UnitNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.name);

            DamageText.text = td.attack.ToString();

            var skillcooltime = td.attackspeed / 100f;

            SkillCoolTimeText.text = skillcooltime.ToString();

            var tdlist = Tables.Instance.GetTable<OutGameUnitUpgrade>().DataList.FindAll(x => x.unit_idx == unitidx);

            foreach(var obj in InfoComponentList)
            {
                ProjectUtility.SetActiveCheck(obj.gameObject, false);
            }

            for(int i = 0; i < tdlist.Count; ++i)
            {
                ProjectUtility.SetActiveCheck(InfoComponentList[i].gameObject, true);
                InfoComponentList[i].Set(UnitIdx , tdlist[i].skill_idx, tdlist[i].level);
            }

        }

    }
}
