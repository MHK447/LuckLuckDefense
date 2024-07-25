using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;


public class PassiveCardComponent : MonoBehaviour
{
    [SerializeField]
    private Image SkillImg;

    [SerializeField]
    private Text SkillNameText;

    [SerializeField]
    private Text LevelText;

    [SerializeField]
    private GameObject LockObj;

    private int SkillIdx = 0;

    public void Set(int skillidx)
    {
        SkillIdx = skillidx;

        var td = Tables.Instance.GetTable<SkillCardInfo>().GetData(SkillIdx);

        if(td != null)
        {
            SkillImg.sprite = Config.Instance.GetSkillAtlas(td.image);
            SkillNameText.text = Tables.Instance.GetTable<Localize>().GetString(td.desc_name);
            var finddata = GameRoot.Instance.SkillCardSystem.FindSkillCardData(skillidx);

            ProjectUtility.SetActiveCheck(LockObj, finddata == null);

            LevelText.text = finddata == null ? "1" : finddata.Level.ToString();

        }
    }
}
