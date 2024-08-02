using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;


[UIPath("UI/Popup/PopupPassiveCardUpgrade")]
public class PopupPassiveCardUpgrade : UIBase
{
    [SerializeField]
    private List<PassiveCardComponent> PassiveCardComponentList = new List<PassiveCardComponent>();

    [SerializeField]
    private Button CardUpgradeBtn;

    [SerializeField]
    private Text CardCostText;

    private int card_upgrade_cost = 0;

    protected override void Awake()
    {
        base.Awake();

        CardUpgradeBtn.onClick.AddListener(OnClickUpgrade);

        card_upgrade_cost = Tables.Instance.GetTable<Define>().GetData("card_upgrade_cost").value;
    }


    public void OnClickUpgrade()
    {
        if(card_upgrade_cost >= GameRoot.Instance.UserData.CurMode.EnergyMoney.Value)
        {
            var cardidx = GameRoot.Instance.SkillCardSystem.GachaUnitCard();

            GameRoot.Instance.SkillCardSystem.SkillCardLevelUp(cardidx);

            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.EnergyMoney, -GameRoot.Instance.UserData.CurMode.EnergyMoney.Value);
        }
    }


    public void SortingRollBack()
    {
        transform.GetComponent<Canvas>().sortingOrder = UISystem.START_PAGE_SORTING_NUMBER;
    }




    public override void CustomSortingOrder()
    {
        base.CustomSortingOrder();

        transform.GetComponent<Canvas>().sortingOrder = (int)UIBase.HUDTypeTopSorting.POPUPTOP;
    }


    public void Init()
    {
        var tdlist = Tables.Instance.GetTable<SkillCardInfo>().DataList;

        for(int i = 0; i < tdlist.Count; ++i)
        {
            PassiveCardComponentList[i].Set(tdlist[i].skill_idx);
        }
    }
}
