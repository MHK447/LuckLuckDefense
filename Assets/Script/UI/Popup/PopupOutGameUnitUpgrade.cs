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
    private List<GameObject> CachedComponents = new List<GameObject>();

    [SerializeField]
    private GameObject UpgradeComponentPrefb;

    [SerializeField]
    private Transform UpgradeComponentRoot;


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
        var tdlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.ToList();


        foreach(var cachedobj in CachedComponents)
        {
            ProjectUtility.SetActiveCheck(cachedobj, false);
        }



        foreach(var td in tdlist)
        {
            var getobj = GetCachedObject().GetComponent<OutGameUpgradeComponent>();

            if(getobj != null)
            {
                ProjectUtility.SetActiveCheck(getobj.gameObject, true);

                getobj.Set(td.unit_idx);
            }
        }
        
    }


    private void OnclickTenBtn()
    {
            
    }


    private void OnClickTweenty()
    {

    }


    public GameObject GetCachedObject()
    {
        var inst = CachedComponents.Find(x => !x.activeSelf);
        if (inst == null)
        {
            inst = GameObject.Instantiate(UpgradeComponentPrefb);
            inst.transform.SetParent(UpgradeComponentRoot);
            inst.transform.localScale = Vector3.one;
            CachedComponents.Add(inst);
        }

        return inst;
    }
}
