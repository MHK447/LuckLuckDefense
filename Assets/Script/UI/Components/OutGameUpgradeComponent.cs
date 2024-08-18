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


            if (finddata != null)
            {
                var unitleveltd = Tables.Instance.GetTable<OutGameUnitLevelInfo>().GetData(finddata.UnitLevel);

                if(unitleveltd != null)
                {
                    slidevalue = (float)finddata.UnitLevel / (float)unitleveltd.cardcount;
                }   
            }



            UnitCountSlider.value = slidevalue;
        }
    }


    private void OnClickUnitInfo()
    {

    }
}
