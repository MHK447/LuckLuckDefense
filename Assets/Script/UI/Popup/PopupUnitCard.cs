using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;

public enum UpgradeComponentType
{
    NormalRare,
    Unique,
    LegendEpic,
    SpawnPercent,
}

public class PopupUnitCard : MonoBehaviour
{

    [SerializeField]
    private List<UnitUpgradeComponent> UpgradeComponentList = new List<UnitUpgradeComponent>(); 



    public void Init()
    {
        for(int i = 0; i < UpgradeComponentList.Count; ++i)
        {
            UpgradeComponentList[i].Set((UpgradeComponentType)i);
        }
    }
}
