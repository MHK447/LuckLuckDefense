using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

public class OutGameUnitUpgradeInfoComponent : MonoBehaviour
{
    [SerializeField]
    private Text LevelText;

    [SerializeField]
    private Text DescText;



    public void Set(int skillidx , int level)
    {
        var td = Tables.Instance.GetTable<UnitActiveSkill>().GetData(skillidx);

        if(td != null)
        {
            LevelText.text = level.ToString();
            DescText.text = Tables.Instance.GetTable<Localize>().GetString(td.name);
        }
    }
}
