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

    private int UnitIdx = 0;

    public void Set(int unitidx)
    {
        UnitIdx = unitidx;

        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(UnitIdx);

        if(td != null)
        {
            UnitNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.name);

        }
    }
}
