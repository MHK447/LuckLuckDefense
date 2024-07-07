using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;

public enum UpgradeComponentType
{
    NormalRare = 1,
    Unique,
    LegendEpic,
    SpawnPercent,
}

[UIPath("UI/Popup/PopupUnitUpgrade")]
public class PopupUnitUpgrade : UIBase
{

    [SerializeField]
    private List<UnitUpgradeComponent> UpgradeComponentList = new List<UnitUpgradeComponent>();

    [SerializeField]
    private Text MoneyText;

    [SerializeField]
    private Text GachaCoinText;


    protected override void Awake()
    {
        base.Awake();

        GameRoot.Instance.UserData.CurMode.EnergyMoney.Subscribe(x => {
            MoneyText.text = x.ToString();
        }).AddTo(this);

        GameRoot.Instance.UserData.CurMode.GachaCoin.Subscribe(x => {
            GachaCoinText.text = x.ToString();
        }).AddTo(this);

    }

    public void Init()
    {
        for (int i = 0; i < UpgradeComponentList.Count; ++i)
        {
            UpgradeComponentList[i].Set((UpgradeComponentType)i + 1);
        }
    }
}
