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


    private int SkillIdx = 0;

    public void Set(int skillidx)
    {
        SkillIdx = skillidx;

        var td = Tables.Instance.GetTable<SkillCardInfo>().GetData(SkillIdx);

        if(td != null)
        {
            //SkillImg.sprite = Config.Instance.GetSkillAtlas()

        }
    }
}
