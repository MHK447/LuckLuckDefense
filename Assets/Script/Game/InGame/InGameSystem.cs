using System.Collections.Generic;
using System.Linq;
using BanpoFri;
using UnityEngine;
using UniRx;
public enum InGameType
{
    Main,
    Event,
}

public class InGameSystem
{
    public SceneSystem SceneSystem { get; private set; } = new SceneSystem();
    public InGameMode CurInGame {get; private set;} = null;

    private bool firstInit = false;
	private bool IsActiveEventTuto = false;
	private bool IsTitle = false;

	private System.Action NextStageAction = null;
	private System.Action NextStageCloseAction = null;

    public bool nextStage = false;

    public System.Action NextActionClear = null;


    public int TicketEnemyIdx = 10001;

    CompositeDisposable disposables = new CompositeDisposable();

    public T GetInGame<T>() where T : InGameMode
    {
        return CurInGame as T;
    }

    public void RegisteInGame(InGameMode mode)
	{
		CurInGame = mode;
	}
    public void ChangeMode(InGameType type)
    {
       // Tables.Instance.GetTable<FacilityUpgrade>().CalculateUpgradeTable(GameRoot.Instance.UserData.CurMode.StageData.StageIdx);
        System.GC.Collect();
        StartGame(type, () => {
            firstInit = false;
        });
    }
    private void StartGame(InGameType type, System.Action loadCallback = null)
    {
        GameRoot.Instance.Loading.Show(true);
        SceneSystem.ChangeScene(type, loadCallback);
    }

    public bool inInitPopups { get; private set; } = false;

    public void InitPopups()
    {
        disposables.Clear();
        var ActionQueue = new Queue<System.Action>();
        IsActiveEventTuto = false;
        inInitPopups = true;


        NextActionClear = () =>
        {
            inInitPopups = false;
            ActionQueue.Clear();
        };

        System.Action NextAction = () =>
        {
            
            if (ActionQueue.Count < 1)
            {
                inInitPopups = false;

                return;
            }

            var action = ActionQueue.Dequeue();
            action.Invoke();
        };

        if (!firstInit)
        {
            GameRoot.Instance.Loading.Hide(true, () =>
            {
                NextAction();
            });
        }

        NextAction();
        nextStage = false;



        if (!firstInit)
        {
            firstInit = true;
        }
    }

}
