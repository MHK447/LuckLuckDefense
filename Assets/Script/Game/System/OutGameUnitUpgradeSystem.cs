using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OutGameUnitUpgradeSystem 
{


    public void AddUnitData(int unitidx , int unitcount)
    {
        var finddata = FindOutGameUnit(unitidx);

        if (finddata != null)
        {
            finddata.UnitCount += unitcount;
        }
        else
        {
            var newdata = new OutGameUnitUpgradeData(unitidx, 1, unitcount);
            GameRoot.Instance.UserData.CurMode.OutGameUnitUpgradeDatas.Add(newdata);
        }
    }
    


    public OutGameUnitUpgradeData FindOutGameUnit(int unitidx)
    {
        var finddata = GameRoot.Instance.UserData.CurMode.OutGameUnitUpgradeDatas.ToList().Find(x => x.UnitIdx == unitidx);

        return finddata;
    }
}
