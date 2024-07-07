using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BanpoFri;

public class UnitUpgradeComponent : MonoBehaviour
{
    [SerializeField]
    private Text LevelText;

    [SerializeField]
    private Text UpgradeCostText;

    [SerializeField]
    private Button UpgradeBtn;

    private UpgradeComponentType CurType;

    private InGameUnitUpgradeData UnitUpgradeData;

    private int BaseCostValue = 0;
    private int CurPriceValue = 0;

    private void Awake()
    {
        UpgradeBtn.onClick.AddListener(OnClickUpgrade);
    }

    public void Set(UpgradeComponentType type)
    {
        CurType = type;

        UnitUpgradeData = GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas.Find(x => x.UpgradeTypeIdx == (int)type);


        var td = Tables.Instance.GetTable<UnitUpgradeInfo>().GetData((int)CurType);

        if(td != null)
        {
            BaseCostValue = td.cost_value;
            SetCostText();
        }
    }

    private void SetCostText()
    {
        CurPriceValue = BaseCostValue * UnitUpgradeData.Level;

        UpgradeCostText.text = CurPriceValue.ToString();

        LevelText.text = $"Lv.{UnitUpgradeData.Level}";
    }

    public void OnClickUpgrade()
    {
        var td = Tables.Instance.GetTable<UnitUpgradeInfo>().GetData((int)CurType);

        if (td != null)
        {
            switch (td.cost_idx)
            {
                case (int)Config.CurrencyID.EnergyMoney:
                    {
                        if (GameRoot.Instance.UserData.CurMode.EnergyMoney.Value >= CurPriceValue)
                        {
                            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.EnergyMoney, -CurPriceValue);
                            UnitUpgradeData.LevelProperty.Value += 1;
                            SetCostText();
                        }
                    }
                    break;
                case (int)Config.CurrencyID.GachaCoin:
                    {
                        if (GameRoot.Instance.UserData.CurMode.GachaCoin.Value >= CurPriceValue)
                        {
                            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.GachaCoin, -CurPriceValue);
                            UnitUpgradeData.LevelProperty.Value += 1;
                            SetCostText();
                        }
                    }
                    break;
            }
        }

    }
}
