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

    private void Awake()
    {
        UpgradeBtn.onClick.AddListener(OnClickUpgrade);
    }

    public void Set(UpgradeComponentType type)
    {
        CurType = type;

        var finddata = GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas.Find(x => x.UpgradeTypeIdx == (int)type);

        if(finddata != null)
        {

        }
    }

    public void OnClickUpgrade()
    {

    }
}
