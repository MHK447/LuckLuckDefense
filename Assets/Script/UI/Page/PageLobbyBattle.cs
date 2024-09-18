using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;

[UIPath("UI/Page/PageLobbyBattle")]
public class PageLobbyBattle : UIBase
{

    [SerializeField]
    private Button StartBtn;

    protected override void Awake()
    {
        base.Awake();
        StartBtn.onClick.AddListener(OnClickStart);
    }


    public void OnClickStart()
    {
        GameRoot.Instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage.StartBattle(1);
        GameRoot.Instance.InGameSystem.DeadCount.Value = 0;
        GameRoot.Instance.InGameSystem.LevelProperty.Value = 0;
    }



    public override void CustomSortingOrder()
    {
        base.CustomSortingOrder();

        transform.GetComponent<Canvas>().sortingOrder = (int)UIBase.HUDTypeTopSorting.POPUPTOP;
    }


    public void SortingRollBack()
    {
        transform.GetComponent<Canvas>().sortingOrder = UISystem.START_PAGE_SORTING_NUMBER;
    }


}
