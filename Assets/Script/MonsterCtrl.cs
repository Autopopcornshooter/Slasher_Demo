using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCtrl : MonoBehaviour
{
    [Header("Monster Specifications")]
    [SerializeField]
    private float MonsterSpeed = 2.0f;
    [SerializeField]
    private float monsterHp = 100.0f;
    [SerializeField]
    private float monsterATK = 10.0f;
    [SerializeField]
    private float monsterAPS = 1.0f;
    [SerializeField]
    private float monsterAttackRange = 1.0f;
    [SerializeField]
    private float monsterEXP = 1.0f;
    [Header("Reference")]
    [SerializeField]
    private Animator monsterAnimator;
    [SerializeField]
    private GameObject monsterModel;
    [SerializeField]
    private GameObject damageText_prefab;

    private Rigidbody monsterRigidbody;
    private IEnumerator MainAttackRoutine;
    private bool isDead = false;
    private bool isAttack = false;

    private void Awake()
    {
        InitiateMonsterReference();
    }
    private void InitiateMonsterReference()
    {
        monsterRigidbody = GetComponent<Rigidbody>();
        MainAttackRoutine = MonsterAttackRoutine();
    }
    private void Start()
    {
        StartCoroutine(MainAttackRoutine);
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            if (!isAttack)
            {
                ChasePlayer();
            }
            CheckDeath();
            LookTarget();
        }
    }
    private void CheckDeath()
    {
        if (monsterHp <= 0 && !isDead)
        {
            isDead = true;
            StopAllCoroutines();
            MonsterSpawnCtrl.Instance().DeleteMonsterOnList(gameObject);
            PlayerCtrl.Instance().PlayerGetEXP(monsterEXP);
            StartCoroutine(MonsterDead());
        }
    }
    public void MonsterDamaged(float damage)
    {
        monsterHp -= damage;
        GameObject temp = Instantiate(damageText_prefab, GameObject.Find("DamageTexts").transform);
        temp.GetComponent<DamageText>().ShootText(gameObject, damage);
    }
    IEnumerator MonsterDead()
    {
        monsterAnimator.SetTrigger("Dead");
        yield return new WaitForSecondsRealtime(2.0f);
        Destroy(gameObject);
        yield return null;
    }
    IEnumerator MonsterAttackRoutine()
    {
        while (true)
        {
            if (Seeker())
            {

                monsterRigidbody.velocity = Vector3.zero;
                isAttack = true;
                monsterAnimator.SetBool("Move", false);
                monsterAnimator.SetTrigger("Attack");
                yield return new WaitForSecondsRealtime(0.12f);
                PlayerCtrl.Instance().PlayerDamaged(monsterATK);
            }
            else
            {
                isAttack = false;
            }
            yield return new WaitForSecondsRealtime(monsterAPS);
        }
    }
    private bool Seeker()
    {
        float currDistance = (PlayerCtrl.Instance().transform.position - transform.position).magnitude;
        if (currDistance <= monsterAttackRange)
        {
            return true;
        }
        return false;
    }
    private void LookTarget()
    {
        Vector3 directionToTarget = PlayerCtrl.Instance().transform.position - transform.position;
        monsterModel.transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
    }
    private void ChasePlayer()
    {
        Vector3 ChaseDir = (PlayerCtrl.Instance().transform.position - transform.position).normalized;
        monsterRigidbody.velocity = ChaseDir * MonsterSpeed;
        monsterAnimator.SetBool("Move", true);
    }
}
