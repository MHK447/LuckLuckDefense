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
    }


}
