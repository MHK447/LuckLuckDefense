using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OutGameUnitUpgradeSystem 
{
        


    public OutGameUnitUpgradeData FindOutGameUnit(int unitidx)
    {
        var finddata = GameRoot.Instance.UserData.CurMode.OutGameUnitUpgradeDatas.ToList().Find(x => x.UnitIdx == unitidx);

        return finddata;
    }
}
