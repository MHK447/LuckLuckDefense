using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using UnityEngine.UI;

[UIPath("UI/Popup/PopupSelectGacha")]
public class PopupSelectGacha : UIBase
{
    [SerializeField]
    private List<GameObject> SelectComponentList = new List<GameObject>();

    [SerializeField]
    private GameObject SelectComponentPrefab;

    [SerializeField]
    private Transform SelectRoot;

    [SerializeField]
    private Button AdResetBtn;

    private List<int> SelectCountList = new List<int>();



    protected override void Awake()
    {
        base.Awake();
        AdResetBtn.onClick.AddListener(OnClickAdReward);
    }

    public void Init()
    {
        GameRoot.Instance.InGameSystem.LevelProperty.Value += 1;
        SelectCountList.Clear();

        List<SelectWeaponGachaSkilInfoData> tdlist = new List<SelectWeaponGachaSkilInfoData>();




        if(GameRoot.Instance.UserData.CurMode.StageData.SelectSkill >= GameRoot.Instance.InGameBattleSystem.MaximumSkillSelect)
        {
            tdlist = Tables.Instance.GetTable<SelectWeaponGachaSkilInfo>().DataList.ToList().FindAll(x=> x.select_type == 2);
        }
        else
        {
            tdlist = Tables.Instance.GetTable<SelectWeaponGachaSkilInfo>().DataList.ToList();
        }




        foreach(var select in SelectComponentList)
        {
            ProjectUtility.SetActiveCheck(select, false);
        }

        for(int i = 0; i < 3; ++i)
        {
            if (tdlist.Count <= 0) break;

            var randvalue = Random.Range(0, tdlist.Count);

            SelectCountList.Add(tdlist[randvalue].skill_idx);
            tdlist.Remove(tdlist[randvalue]);
        }

        foreach(var weaponidx in SelectCountList)
        {
            var getobj = GetCachedObject().GetComponent<SelectComponent>();

            ProjectUtility.SetActiveCheck(getobj.gameObject, true);

            getobj.Set(weaponidx, Select);
        }

    }

    public override void OnShowAfter()
    {
        base.OnShowAfter();

        Time.timeScale = 0f;    
    }


    public void Select(int weaponidx)
    {
        GameRoot.Instance.GachaSkillSystem.AddWeaponSkillBuff((SelectGachaWeaponSkillSystem.GachaWeaponSkillType)weaponidx);
        Hide();
    }   

    public override void Hide()
    {
        base.Hide();
        Time.timeScale = 1f;
    }


    public void OnClickAdReward()
    {
        GameRoot.Instance.GetAdManager.ShowRewardedAd(() => {

            List<SelectWeaponGachaSkilInfoData> tdlist = new List<SelectWeaponGachaSkilInfoData>();

            SelectCountList.Clear();

            if (GameRoot.Instance.UserData.CurMode.StageData.SelectSkill >= GameRoot.Instance.InGameBattleSystem.MaximumSkillSelect)
            {
                tdlist = Tables.Instance.GetTable<SelectWeaponGachaSkilInfo>().DataList.ToList().FindAll(x => x.select_type == 2);
            }
            else
            {
                tdlist = Tables.Instance.GetTable<SelectWeaponGachaSkilInfo>().DataList.ToList();
            }




            foreach (var select in SelectComponentList)
            {
                ProjectUtility.SetActiveCheck(select, false);
            }

            for (int i = 0; i < 3; ++i)
            {
                if (tdlist.Count <= 0) break;

                var randvalue = Random.Range(0, tdlist.Count);

                SelectCountList.Add(tdlist[randvalue].skill_idx);
                tdlist.Remove(tdlist[randvalue]);
            }

            foreach (var weaponidx in SelectCountList)
            {
                var getobj = GetCachedObject().GetComponent<SelectComponent>();

                ProjectUtility.SetActiveCheck(getobj.gameObject, true);

                getobj.Set(weaponidx, Select);
            }

        });

    }

    public GameObject GetCachedObject()
    {
        var inst = SelectComponentList.Find(x => !x.activeSelf);
        if (inst == null)
        {
            inst = GameObject.Instantiate(SelectComponentPrefab);
            inst.transform.SetParent(SelectRoot);
            inst.transform.localScale = Vector3.one;
            SelectComponentList.Add(inst);
        }

        return inst;
    }

}
