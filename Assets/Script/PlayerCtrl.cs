using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerCtrl : MonoBehaviour
{
    [Header("Player Specifications")]
    [SerializeField]
    private float PlayerSpeed_Defalt=4.0f;
    [SerializeField]
    private float PlayerHP_Defalt = 100.0f;
    [SerializeField]
    private float PlayerATK_Defalt = 10.0f;
    [SerializeField]
    private float PlayerAPS_Defalt = 1.0f;
    [SerializeField]
    private float PlayerSeekerRange_Defalt = 1.0f;
    [SerializeField]
    private float PlayerAttack2Range_Defalt = 2.0f;

    
    private float PlayerATK_Current;
    [Header("Reference")]
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private GameObject playerModel;
    [SerializeField]
    private GameObject HP_Indicator;
    [SerializeField]
    private GameObject damageText_prefab;
    private GameObject OnPlayerUI;




    private Rigidbody playerRigidbody;

    [HideInInspector]
    public float playerEXP = 0.0f;
    [HideInInspector]
    public int playerLV = 1;
    [HideInInspector]
    public int killPoint = 0;
    private float CurrentPlayerHP;
    private float MaxPlayerHP;
    private bool isAttacking=false;
    private bool playerDead = false;
    private IEnumerator MainAttackRoutine;
    
    private GameObject currentTarget;

    private static PlayerCtrl instance;

    public static PlayerCtrl Instance()
    {
        return instance;
    }

    private void Awake()
    {
        Singltonization();
        InitiateReference();
        InitiatePlayerStatus();
    }
    private void Start()
    {
        SetPlayerStats();
        RefreshHP_Indicator();
        StartCoroutine(MainAttackRoutine);
    }
    private void FixedUpdate()
    {
        //PlayerUI_Locating();
        //if (!playerDead)
        //{
        OnPlayerUILocate();
        PlayerMove();
        Seeker();
        
        if (currentTarget != null)
        {
            LookTarget(currentTarget);
        }
        // }
    }
    private void InitiatePlayerStatus()
    {
        MaxPlayerHP = PlayerHP_Defalt;
        CurrentPlayerHP = MaxPlayerHP;
    }
    private void InitiateReference()
    {
        OnPlayerUI = GameObject.Find("OnPlayerUI");
        playerRigidbody = GetComponent<Rigidbody>();
        MainAttackRoutine = AttackRoutine();
    }
    private void Singltonization()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void SetPlayerStats()
    {
        MaxPlayerHP = PlayerHP_Defalt+ ((playerLV - 1) * 3);
        PlayerATK_Current = PlayerATK_Defalt + ((playerLV - 1) * 2);
    }
    public float GetMaxEXP()
    {
        float MaxEXP = 10 + (playerLV - 1) * 3;
        return MaxEXP;
    }
    public void PlayerGetEXP(float exp)
    {
        killPoint++;
        playerEXP += exp;
        if (GetMaxEXP() <= playerEXP)
        {
            playerEXP = playerEXP - GetMaxEXP();
            playerLV++;
            SetPlayerStats();
        }
    }
    public GameObject GetCurrentTarget()
    {
        return currentTarget;
    }
    private void PlayerMove()   //이동 구현
    {
        Vector3 playerMoveDir;
        Vector3 keyboardInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 joystickInput = JoyStick.Instance().GetMoveDir();
        playerMoveDir = keyboardInput + joystickInput;  //조이스틱과의 조작 결합

        playerRigidbody.velocity = playerMoveDir.normalized * PlayerSpeed_Defalt;
        if (playerRigidbody.velocity.magnitude > 0)
        {
            playerAnimator.SetBool("IsMove", true);
        }
        else
        {
            playerAnimator.SetBool("IsMove", false);
        }
        float targetAngle = Mathf.Atan2(playerMoveDir.x, playerMoveDir.z) * Mathf.Rad2Deg;
        float angle = Mathf.LerpAngle(playerModel.transform.eulerAngles.y, targetAngle, 0.5f);  //각도 선형 보간
        playerModel.transform.rotation = Quaternion.Euler(0, angle, 0);
        
    }
  

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (currentTarget != null)
            {
                Attack1_trigger();
                yield return new WaitForSecondsRealtime(PlayerAPS_Defalt);
            }
            if (currentTarget != null)
            {
                Attack2_trigger();
                yield return new WaitForSecondsRealtime(PlayerAPS_Defalt);
            }
            if (currentTarget != null)
            {
                Healing();
                yield return new WaitForSecondsRealtime(PlayerAPS_Defalt);
            }
            yield return null;
        }
    }
    #region Attack1
    private void Attack1_trigger()
    {
        playerAnimator.SetTrigger("Attack1");
        StartCoroutine(Attack1_Routine());
    }
    IEnumerator Attack1_Routine()
    {
        yield return new WaitForSecondsRealtime(0.16f);
        if(currentTarget == null)
        {
            yield break;    
        }
        currentTarget.GetComponent<MonsterCtrl>().MonsterDamaged(PlayerATK_Current);
        yield return null;
    }
    #endregion
    #region Attack2
    private void Attack2_trigger()
    {
        playerAnimator.SetTrigger("Attack2");
        StartCoroutine(Attack2_Routine());
    }
    IEnumerator Attack2_Routine()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        if (currentTarget == null)
        {
            yield break;
        }
        foreach (GameObject monster in MonsterSpawnCtrl.Instance().MonsterList())
        {
            float currDistance = (monster.transform.position - this.transform.position).magnitude;
            if (currDistance <= PlayerAttack2Range_Defalt)
            {
                monster.GetComponent<MonsterCtrl>().MonsterDamaged(PlayerATK_Current);
            }
        }
        yield return null;
    }
    #endregion
    #region Heal
    private void Healing()
    {
        Debug.Log("Player Healed");
        CurrentPlayerHP += PlayerATK_Current;
        if (CurrentPlayerHP > MaxPlayerHP)
        {
            CurrentPlayerHP = MaxPlayerHP;
        }
        RefreshHP_Indicator();
    }
    #endregion


    public void PlayerDamaged(float hitPoint)
    {
        CurrentPlayerHP -= hitPoint;
        if (CurrentPlayerHP <= 0)
        {
            CurrentPlayerHP = 0.0f;
        }
        RefreshHP_Indicator();
        GameObject temp = Instantiate(damageText_prefab, GameObject.Find("DamageTexts").transform);
        temp.GetComponent<DamageText>().ShootText(gameObject, hitPoint);    //글자 랜덤 방향으로 발사
        Camera.main.GetComponent<CameraChase>().CamShake();     //카메라 흔들림
    }
    private void RefreshHP_Indicator()
    {
        HP_Indicator.GetComponent<Slider>().value = CurrentPlayerHP / MaxPlayerHP;
    }
    private void OnPlayerUILocate()
    {
        OnPlayerUI.GetComponent<RectTransform>().position = 
            Camera.main.WorldToScreenPoint(PlayerCtrl.Instance().transform.position);
    }
   

    private void Seeker()
    {
        GameObject target = null;
        if (MonsterSpawnCtrl.Instance().MonsterList().Count > 0)
        {
            foreach (GameObject monster in MonsterSpawnCtrl.Instance().MonsterList())
            {
                float currDistance = (monster.transform.position - this.transform.position).magnitude;
                if (currDistance < PlayerSeekerRange_Defalt)
                {
                    if (target != null)
                    {
                        float targetDistance = (target.transform.position - this.transform.position).magnitude;
                        if (currDistance < targetDistance)
                        {
                            target = monster;
                        }
                    }
                    else
                    {
                        target = monster;
                    }
                }
            }
        }
        currentTarget = target;
    }

    private void LookTarget(GameObject target)
    {
        Vector3 directionToTarget = currentTarget.transform.position - transform.position;
        playerModel.transform.rotation = Quaternion.LookRotation(new Vector3(directionToTarget.x,0,directionToTarget.z));
    }
}
