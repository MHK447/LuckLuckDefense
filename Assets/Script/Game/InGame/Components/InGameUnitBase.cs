using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using UnityEngine.UI;
using UniRx;

public class InGameUnitBase : MonoBehaviour
{
    public enum State
    {
        Idle,
        Attack,
        Move,
    }


    [System.Serializable]
    public class UnitActiveSkillInfo
    {
        public int UnitSkillIdx;
        public int UnitSkillRatio;
        public int UnitSkillValue;


        public UnitActiveSkillInfo(int unitskillidx , int unitskillratio , int unitskillvalue)
        {
            UnitSkillIdx = unitskillidx;
            UnitSkillRatio = unitskillratio;
            UnitSkillValue = unitskillvalue;
        }
    }


    [System.Serializable]
    public class Info
    {
        public float AttackRange;
        public double Attack;
        public float criticalChance;
        public float AttackSpeed;
        public List<UnitActiveSkillInfo> UnitSkillInfoList = new List<UnitActiveSkillInfo>();
    }

    [SerializeField]
    private Animator Anim;

    [SerializeField]
    private AnimFunction AttackAnimAction;

    public SpriteRenderer attackRangeIndicator;

    [SerializeField]
    protected Info info = new Info();

    private float radioussize = 1.82f;

    private int UnitIdx = 0;

    public int GetUnitIdx { get { return UnitIdx; } }

    private float lastattacktime = 0f;

    private InGameBattle Battle;

    private State CurState = State.Idle;

    private string AnimStr;

    private InGameEnemyBase Target;

    private UnitTileComponent CurTileComponent;

    private Transform MoveTr;

    [HideInInspector]
    public Transform AttackRangeTr;

    private bool facingRight = false;

    private CompositeDisposable disposables = new CompositeDisposable();

    private int UpgradeIdx = 0;

    public void Set(int unitidx , InGameBattle battle)
    {
        Target = null;
        UnitIdx = unitidx;
        Battle = battle;
        AttackAnimAction.FuncAction = StartAttack;

        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(unitidx);

        if(td != null)
        {
            SetInfo();

            ChangeState(State.Idle);


            ProjectUtility.SetActiveCheck(attackRangeIndicator.gameObject, false);

            attackRangeIndicator.transform.localScale = new Vector2(info.AttackRange * radioussize, info.AttackRange * radioussize);

            if (td.grade == 1 || td.grade == 2)
            {
                UpgradeIdx = 1;
            }
            else if (td.grade == 3)
            {
                UpgradeIdx = 2;
            }
            else if (td.grade == 4 || td.grade == 5)
            {
                UpgradeIdx = 3;
            }

            var finddata = GameRoot.Instance.UserData.CurMode.UnitUpgradeDatas.Find(x => x.UpgradeTypeIdx == UpgradeIdx);

            finddata.LevelProperty.Subscribe(x => { SetInfo(); }).AddTo(disposables);
        }
    }

    private void OnDestroy()
    {
        disposables.Clear();
    }

    private void OnDisable()
    {
        disposables.Clear();
    }


    public void SetInfo()
    {
        var td = Tables.Instance.GetTable<PlayerUnitInfo>().GetData(UnitIdx);

        if (td != null)
        {

            info.AttackRange = td.attackrange / 100f;

            info.Attack = td.attack * GameRoot.Instance.UnitUpgradeSystem.GetUgpradeValue(UpgradeIdx) * (float)GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.DAMAGEINCREASE);

            var criticalbuffvalue = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.CRITICALPERECENT);

            info.criticalChance = (float)td.criticalchance * (float)criticalbuffvalue * (float)GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.SKILLPERCENT);
            
            var buffvalue = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.ATTACKSPEED);

            var attackspeedvalue = td.attackspeed / 100f;

            var speedbuffvalue = (attackspeedvalue * buffvalue) / 100f;

            info.AttackSpeed = attackspeedvalue + (float)speedbuffvalue;
           


            info.UnitSkillInfoList.Clear();


            for(int i = 0; i < td.unit_skill.Count; ++i)
            {
                switch(td.unit_skill[i])
                {
                    case (int)UnitSkillSystem.SkillType.Slow:
                    case (int)UnitSkillSystem.SkillType.Sturn:
                    case (int)UnitSkillSystem.SkillType.AttackSkill:
                        {
                            var newskill = new UnitActiveSkillInfo(td.unit_skill[i], td.unit_skil_percent[i], td.unit_skil_value[i]);
                            info.UnitSkillInfoList.Add(newskill);
                        }
                        break;
                    case (int)UnitSkillSystem.SkillType.DefenseReduction:
                    case (int)UnitSkillSystem.SkillType.AttackPowerIncrease:
                    case (int)UnitSkillSystem.SkillType.MoveSpeedReduction:
                    case (int)UnitSkillSystem.SkillType.AttackCoin:
                    case (int)UnitSkillSystem.SkillType.CoinGainSummon:
                        {
                            GameRoot.Instance.UnitSkillSystem.AddPassiveSkill(td.unit_idx, td.unit_skill[i], td.unit_skil_value[i]);
                        }
                        break;
                }
            }
        }
        }

    private bool IsCriticalHit()
    {
        return Random.value < info.criticalChance;
    }


    public void RemoveUnit()
    {
        ProjectUtility.SetActiveCheck(this.gameObject, false);
        GameRoot.Instance.UnitSkillSystem.RemovePassiveSkill(UnitIdx);
    }

    public void SetTileComponent(UnitTileComponent tilecomponent)
    {
        CurTileComponent = tilecomponent;
    }

    public void TileAttackRangeActive(bool isactive)
    {
        ProjectUtility.SetActiveCheck(attackRangeIndicator.gameObject, isactive);
    }

    public void Attack(InGameEnemyBase enemy , double damage)
    {
        ChangeState(State.Attack);

        for(int i = 0; i < info.UnitSkillInfoList.Count; ++i)
        {
            var isskillon = SkillProbability(info.UnitSkillInfoList[i].UnitSkillRatio);

            if(isskillon)
            {
                enemy.DebuffDamage((InGameEnemyBase.DebuffType)info.UnitSkillInfoList[i].UnitSkillIdx, info.UnitSkillInfoList[i].UnitSkillValue, info.UnitSkillInfoList[i].UnitSkillValue / 10);
            }
        }


        if(IsCriticalHit())
        {
            var criticaldamagebuff = GameRoot.Instance.SkillCardSystem.GetBuffValue((int)SKillCardIdx.CRITICALDAMAGE);

            var crticialdamage = criticaldamagebuff * SkillCardSystem.CrtiticalDamage;

            damage = damage * crticialdamage;
        }


        enemy.Damage((int)damage);
    }

    public void DeathUnit()
    {
        ProjectUtility.SetActiveCheck(this.gameObject, false);
    }

    public void ChangeState(State state)
    {
        CurState = state;

        switch(state)
        {
            case State.Idle:
                {
                    StartAnim("0_idle");
                }
                break;
            case State.Attack:
                {
                    StartAnim("2_Attack_Normal");
                }
                break;
            case State.Move:
                {
                    Target = null;
                    StartAnim("1_Run");
                }
                break;
        }
    }

    private void StartAnim(string anim)
    {
        if (AnimStr == anim) return;

        if(CurState == State.Attack)
            Anim.SetFloat("AttackSpeed", info.AttackSpeed);

        AnimStr = anim;
        Anim.Play(anim, 0, 0f);
    }

    public void MoveToTile(UnitTileComponent tile , Transform movetr)
    {
        MoveTr = movetr;  
        ChangeState(State.Move);
        CurTileComponent = tile;
        SetTileComponent(tile);
    }

     
    public void StartAttack()
    {
        if(Target != null)
        {
            float distanceToEnemy = Vector3.Distance(AttackRangeTr.position, Target.transform.position);
            if (distanceToEnemy < info.AttackRange)
            {
                Attack(Target, info.Attack);

                if (Target.IsDeath)
                    Target = null;  
            }
            else
            {
                Target = null;
            }
        }
    }



    public bool SkillProbability(int probability)
    {

        int randomValue = Random.Range(0, 100);
        return randomValue < probability;
    }


    void OnDrawGizmos()
    {
        if (CurTileComponent == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackRangeTr.position, info.AttackRange); // 공격 범위는 여기서 설정
    }

        void Update()
    {
        if (CurState == State.Move && MoveTr != null)
        {
            Vector3 direction = MoveTr.position - transform.position;
            direction.Normalize();

            // 좌우 방향 판단
            if (direction.x > 0 && !facingRight)
            {
                Flip();
            }
            else if (direction.x < 0 && facingRight)
            {
                Flip();
            }

            // 유닛을 목표 방향으로 이동
            transform.position += direction * 10f * Time.deltaTime;

            float distance = Vector3.Distance(MoveTr.position, this.transform.position);

            if(distance < 0.1f)
            {
                //이동이후
                ChangeState(State.Idle);
            }


            return;
        }

        if (Target == null)
        {
            Target = Battle.FindClosestEnemy(info.AttackRange, AttackRangeTr);

            if (Target != null)
            {
                Vector3 direction = Target.transform.position - transform.position;
                direction.Normalize();

                // 좌우 방향 판단
                if (direction.x > 0 && !facingRight)
                {
                    Flip();
                }
                else if (direction.x < 0 && facingRight)
                {
                    Flip();
                }

                ChangeState(State.Attack);
            }
            else
            {
                ChangeState(State.Idle);
            }
        }
    }



    void Flip()
    {
        // 유닛의 방향 전환
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
}
