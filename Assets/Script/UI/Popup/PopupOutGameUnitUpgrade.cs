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



    public void Init()
    {
        var tdlist = Tables.Instance.GetTable<PlayerUnitInfo>().DataList.ToList();

        foreach(var td in tdlist)
        {
            var getobj = GetCachedObject().GetComponent<OutGameUpgradeComponent>();

            if(getobj != null)
            {

            }
        }
        
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
