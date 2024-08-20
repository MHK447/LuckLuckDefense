using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

public class OutGameUpgradeComponent : MonoBehaviour
{
    [SerializeField]
    private Text UnitNameText;

    [SerializeField]
    private Slider UnitCountSlider;

    [SerializeField]
    private Text UnitCountText;

    [SerializeField]
    private Text LevelText;

    [SerializeField]
    private Image UnitImg;

    [SerializeField]
    private GameObject LockObj;

    [SerializeField]
    private Button UnitInfoBtn;

    [SerializeField]
    private int UnitIdx = 0;


    private void Awake()
    {
        UnitInfoBtn.onClick.AddListener(OnClickUnitInfo);
    }

    public void Init()
    {
        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(UnitIdx);

        if(td != null)
        {
            UnitNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.name);


            var finddata = GameRoot.Instance.OutGameUnitUpgradeSystem.FindOutGameUnit(UnitIdx);


            Utility.SetActiveCheck(LockObj, finddata == null);

            float slidevalue = 0f;

            UnitImg.sprite = Config.Instance.GetUnitImg(td.icon);

            if (finddata != null)
            {
                var unitleveltd = Tables.Instance.GetTable<OutGameUnitLevelInfo>().GetData(finddata.UnitLevel);

                if(unitleveltd != null)
                {
                    slidevalue = (float)finddata.UnitLevel / (float)unitleveltd.cardcount;
                }   
            }

            int curunitcount = finddata == null ? 0 : finddata.UnitCount;

            int curlevel = finddata == null ? 1 : finddata.UnitLevel;

            var unitupgradetd = Tables.Instance.GetTable<UnitUpgradeLevelInfo>().GetData(curlevel);

            if(unitupgradetd != null)
            {
                UnitCountText.text = $"{curunitcount}/{unitupgradetd.need_card}";   
            }
            UnitCountSlider.value = slidevalue;
        }
    }


    private void OnClickUnitInfo()
    {
        GameRoot.Instance.UISystem.OpenUI<PopupOutGameUnitUpgradeInfo>(popup => popup.Set(UnitIdx));
    }
}
