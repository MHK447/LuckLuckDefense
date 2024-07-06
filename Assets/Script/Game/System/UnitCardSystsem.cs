using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;



public class UnitCardSystsem
{

    public void Create()
    {
        var findunit = FindLegendCard(1001);

        if(findunit == null)
        {
            AddUnitCard(1001, 1);
        }
    }

    public UnitCardData FindLegendCard(int unitidx)
    {
        return GameRoot.Instance.UserData.CurMode.UnitCardDatas.Find(x => x.UnitIdx == unitidx);
    }

    public  void AddUnitCard(int unitidx , int cardcount)
    {
        var finddata = GameRoot.Instance.UserData.CurMode.UnitCardDatas.Find(x => x.UnitIdx == unitidx);

        if(finddata != null)
        {
            finddata.CardCount += cardcount;
        }
        else
        {
            var unitcarddata = new UnitCardData(unitidx, 1, 1);

            GameRoot.Instance.UserData.CurMode.UnitCardDatas.Add(unitcarddata);
        }
    }

}
