using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;


public class UnitInfoSkillComponent : MonoBehaviour
{
    [System.Serializable]
    public class SkillBtn
    {
        public Image SkillImg;
        public Button SKillBtn;
    }

    [SerializeField]
    private Text SkillNameText;

    [SerializeField]
    private Text SkillDescText;

    [SerializeField]
    private List<SkillBtn> SkillBtnList = new List<SkillBtn>();

    private int Unitidx  = 0;

    private int CurSelectSkillIdx = 0;


    private void Awake()
    {
        int iter = 1;

        foreach(var skill in SkillBtnList)
        {
            skill.SKillBtn.onClick.AddListener(()=> { SelectSkill(iter); });
            iter += 1;
        }
    }

    public void Set(int unitidx)
    {
        Unitidx = unitidx;

        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(Unitidx);

        foreach(var skill in SkillBtnList)
        {
            ProjectUtility.SetActiveCheck(skill.SKillBtn.gameObject, false);
        }


        for(int i = 0; i < td.unit_skill.Count; ++i)
        {
            ProjectUtility.SetActiveCheck(SkillBtnList[i].SKillBtn.gameObject, true);
            //skillimage set
        }



        if(td != null)
        {
            SelectSkill(td.unit_skill[0]);
        }
    }


    public void SelectSkill(int skillidx)
    {
        CurSelectSkillIdx = skillidx;

        var td = Tables.Instance.GetTable<PlayerSkillInfo>().GetData(CurSelectSkillIdx);

        var unitinfotd = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(Unitidx);

        if (td != null)
        {
            SkillNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.desc_name);

            var indexs = unitinfotd.unit_skill.IndexOf(CurSelectSkillIdx);

            switch (td.skill_idx)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    {
                        SkillDescText.text = Tables.Instance.GetTable<Localize>().GetFormat($"skill_{td.skill_idx}_desc",unitinfotd.unit_skil_percent[indexs], unitinfotd.unit_skil_value[indexs]);
                    }
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    {
                        SkillDescText.text = Tables.Instance.GetTable<Localize>().GetFormat($"skill_{td.skill_idx}_desc", unitinfotd.unit_skil_value[indexs]);
                    }
                    break;
            }
        }
    }

}
