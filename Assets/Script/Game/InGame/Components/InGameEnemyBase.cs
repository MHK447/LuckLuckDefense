using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;

public class InGameEnemyBase : MonoBehaviour
{
    public enum State
    {
        Done,
        Move,
        Dead,
    }


    [SerializeField]
    protected Animator Anim;

    [SerializeField]
    private Transform HpTr;

    private InGameHpUI HpUI;

    private InGameTimeUI TimeUI;

    private int UnitIdx = 0;

    private InGameBattle.Direction CurDirection;

    private State CurState;

    public Transform GetHpTr { get { return HpTr; } }

    public bool IsDeath { get { return CurState == State.Dead; } }

    public int GetUnitIdx { get { return UnitIdx; } }

    public int Hp = 0;

    private float MoveSpeed = 0;

    private int MoveSpawnCount = 1;

    private int MoveEndSpawnCount = 4;

    private float ticketdeltime = 0f;

    [HideInInspector]
    public InGameBattle Battle;

    private InGameBattle.SpawnTrPos Target;

    public void Set(int unitidx , InGameBattle battle)
    {
        UnitIdx = unitidx;

        var td = Tables.Instance.GetTable<EnemyInfo>().GetData(UnitIdx);

        if(td != null)
        {
            var curwaveidx = GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value;

            var basehp = GameRoot.Instance.InGameBattleSystem.enemy_normal_base_hp;

            var increasehp = GameRoot.Instance.InGameBattleSystem.enemy_wave_increase_hp / 100;

            var value = basehp  * (increasehp);

            var increase = curwaveidx < 10 ? 1 : (curwaveidx / 10) * 2;

            Hp = (int)value * increase;
            MoveSpeed = (float)td.movespeed / 100f;
            MoveSpawnCount = 1;
            Battle = battle;

            Target = Battle.GetSpawnTr(MoveSpawnCount);
        }

        SetState(State.Move);
        SetDirection(InGameBattle.Direction.TOP);
        ticketdeltime = 0f;

        if (UnitIdx == GameRoot.Instance.InGameSystem.TicketEnemyIdx)
        {
            if (TimeUI == null)
            {
                GameRoot.Instance.UISystem.LoadFloatingUI<InGameTimeUI>((timeui) =>
                {
                    TimeUI = timeui;
                    timeui.Init(HpTr);
                    ProjectUtility.SetActiveCheck(TimeUI.gameObject, true);
                });
            }
            else
            {
                ProjectUtility.SetActiveCheck(TimeUI.gameObject, true);
            }
        }

        if(HpUI == null)
        {
            GameRoot.Instance.UISystem.LoadFloatingUI<InGameHpUI>((hpui) => {
                ProjectUtility.SetActiveCheck(hpui.gameObject, true);
                HpUI = hpui;
                hpui.Set(Hp);
                hpui.Init(HpTr);
            });
        }
        else
        {
            ProjectUtility.SetActiveCheck(HpUI.gameObject, true);
            HpUI.Set(Hp);
        }
    }

    public void Clear()
    {
        if (HpUI != null)
            ProjectUtility.SetActiveCheck(HpUI.gameObject, false);
    }
    public void SetDirection(InGameBattle.Direction direction)
    {
        switch(direction)
        {
            case InGameBattle.Direction.LEFT:
                Anim.Play("Left_Move", 0, 0f);
                break;
            case InGameBattle.Direction.RIGHT:
                Anim.Play("Right_Move", 0, 0f);
                break;
            case InGameBattle.Direction.BOTTOM:
                Anim.Play("Bottom_Move", 0, 0f);
                break;
            case InGameBattle.Direction.TOP:
                Anim.Play("Top_Move", 0, 0f);
                break;
        }
    }

    public void SetState(State state)
    {
        CurState = state;
    }

    public void Damage(int damage)
    {
        if (Hp <= 0) return;
        if (IsDeath) return;

        Hp -= damage;


        if(HpUI != null)
        {
            HpUI.SetSliderValue(damage);
        }

        if(Hp <= 0)
        {
            Dead();
        }
    }

    

    public void Dead()
    {
        if (HpUI != null)
            ProjectUtility.SetActiveCheck(HpUI.gameObject, false);

        if (TimeUI != null)
            ProjectUtility.SetActiveCheck(TimeUI.gameObject, false);

        SetState(State.Dead);
        GameRoot.Instance.UserData.CurMode.StageData.UnitCountPropety.Value -= 1;

        if (UnitIdx == GameRoot.Instance.InGameSystem.TicketEnemyIdx)
            GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.GachaCoin, 2);
        else
            GameRoot.Instance.UserData.CurMode.EnergyMoney.Value += 1;


        //GameRoot.Instance.EffectSystem.MultiPlay<IconTextEffect>(HpTr.position, effect =>
        //{
        //    effect.SetText(1.ToString(), (int)Config.CurrencyID.EnergyMoney);
        //    effect.SetAutoRemove(true, 1f);
        //    effect.transform.SetParent(GameRoot.Instance.UISystem.WorldCanvas.transform);
        //});

        if (GameRoot.Instance.UserData.CurMode.StageData.IsBossProperty.Value == true && UnitIdx > 1000)
        {
            GameRoot.Instance.UserData.CurMode.StageData.IsBossProperty.Value = false;
        }


        if(UnitIdx > 1000)
        {
            GetGachaCoin();
        }
    }



    public void GetGachaCoin()
    {
        var waveidx = GameRoot.Instance.UserData.CurMode.StageData.WaveIdxProperty.Value;

        var increase = waveidx < 10 ? 1 : (waveidx / 10) * 2;

        var rewardvalue = GameRoot.Instance.InGameBattleSystem.boss_gacha_coin_base * increase;

        GameRoot.Instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.GachaCoin,
            rewardvalue);

        //GameRoot.Instance.EffectSystem.MultiPlay<IconTextEffect>(HpTr.position, effect =>
        //{
        //    effect.SetText(rewardvalue.ToString(), (int)Config.CurrencyID.GachaCoin);
        //    effect.SetAutoRemove(true, 1f);
        //    effect.transform.SetParent(HpTr.transform);
        //});
    }

    public void DeadAnimEnd()
    {
        ProjectUtility.SetActiveCheck(this.gameObject, false);
    }

    void Update()
    {
        if(UnitIdx == GameRoot.Instance.InGameSystem.TicketEnemyIdx && CurState != State.Dead)
        {
            if(TimeUI != null)
            {
                ticketdeltime += Time.deltaTime;


                var time = GameRoot.Instance.InGameBattleSystem.ticket_unit_time - ticketdeltime;

                TimeUI.SetTime((int)time);

                if(time <= 0)
                {
                    //end
                    Dead();
                }
            }
        }

        if (Target != null)
        {
            // 현재 위치에서 목표 위치까지의 방향 벡터 계산
            Vector3 direction = Target.SpawnTr.position - transform.position;
            direction.Normalize();

            // 유닛을 목표 방향으로 이동
            transform.position += direction * MoveSpeed * Time.deltaTime;

            float distance = Vector3.Distance(transform.position, Target.SpawnTr.position);

            if(distance < 0.1f)
            {
                MoveSpawnCount += 1;
                if (MoveSpawnCount >= MoveEndSpawnCount)
                {
                    MoveSpawnCount = 0;
                    Target = Battle.GetSpawnTr(MoveSpawnCount);
                    SetDirection(Target.SpawnDirection);
                }
                Target = Battle.GetSpawnTr(MoveSpawnCount);
                SetDirection(Target.SpawnDirection);
            }
        }
    }
}
