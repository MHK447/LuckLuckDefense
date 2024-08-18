using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using System.Linq;

[UIPath("UI/Popup/PopupOutGameUnitUpgrade")]
public class PopupOutGameUnitUpgrade : UIBase
{
    [SerializeField]
    private List<OutGameUpgradeComponent> CachedComponents = new List<OutGameUpgradeComponent>();

    [SerializeField]
    private Button CardSpawnBtnTen;

    [SerializeField]
    private Button CardSpawnBtnTweenty;


    protected override void Awake()
    {
        base.Awake();

        CardSpawnBtnTen.onClick.AddListener(OnclickTenBtn);
        CardSpawnBtnTweenty.onClick.AddListener(OnClickTweenty);
    }


    public void Init()
    {
        foreach(var cachedobj in CachedComponents)
        {
            cachedobj.Init();
        }
    }


    private void OnclickTenBtn()
    {
            
    }


    private void OnClickTweenty()
    {

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
