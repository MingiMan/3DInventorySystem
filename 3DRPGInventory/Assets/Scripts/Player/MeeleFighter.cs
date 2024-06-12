using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AttackState
{
    Idle,
    WindUp,
    Impact,
    CoolDown
}
public class MeeleFighter : MonoBehaviour
{
    [Header("AttackData")]
    List<AttackData> attacks = new List<AttackData>();
    Animator animor;
    AttackState attackState;
    int comboCount = 0;
    bool doCombo;

    [Header("Anchor")]
    [SerializeField] Transform leftHandAnchor;
    [SerializeField] Transform rightHandAnchor;

    GameObject currentLeftHandWeapon;
    GameObject currentRightHandWeapon;
    BoxCollider weaponCollider;

    private void Awake()
    {
        animor = GetComponent<Animator>();
    }

    public bool InAction { get; private set; } = true;

    public void TryToAttack()
    {
        if (!InAction)
        {
            StartCoroutine(Attack());
        }

        else if (attackState == AttackState.Impact || attackState == AttackState.CoolDown)
        {
            doCombo = true;
        }
    }


    IEnumerator Attack()
    {
        InAction = true;

        attackState = AttackState.WindUp;

        animor.CrossFade(attacks[comboCount].AnimationName, 0.2f);

        yield return null;

        var animatorState = animor.GetCurrentAnimatorStateInfo(1);

        float timer = 0f;

        while (timer <= animatorState.length)
        {
            timer += Time.deltaTime;

            float normalizedTime = timer / animatorState.length;

            if (attackState == AttackState.WindUp)
            {
                if (normalizedTime >= attacks[comboCount].impactStartTime)
                {
                    attackState = AttackState.Impact;
                    EnableHitBox(attacks[comboCount]);
                }
            }

            else if (attackState == AttackState.Impact)
            {
                if (normalizedTime >= attacks[comboCount].impactEndTime)
                {
                    attackState = AttackState.CoolDown;
                    DisableAllHitBoxes();
                }
            }
            else if (attackState == AttackState.CoolDown)
            {
                if (doCombo)
                {
                    doCombo = false;
                    comboCount = (comboCount + 1) % attacks.Count;
                    StartCoroutine(Attack());
                    yield break;
                }
            }
            yield return null;
        }
        attackState = AttackState.Idle;
        comboCount = 0;
        InAction = false;
    }


    void EnableHitBox(AttackData attack)
    {
        switch (attack.attackHitBox)
        {
            case AttackHitBox.Sword:
                weaponCollider.enabled = true;
                break;

            case AttackHitBox.Axe:
                weaponCollider.enabled = true;
                break;
            default:
                break;
        }
    }

    void DisableAllHitBoxes()
    {
        weaponCollider.enabled = false;
    }

    public void GetAttackData(List<AttackData> atkDataList)
    {
        attacks.Clear();
        for (int i = 0; i < atkDataList.Count; i++)
        {
            attacks.Add(atkDataList[i]);
        }
    }

    public void GetLeftWeapon(GameObject leftHandObj)
    {
        if (currentLeftHandWeapon != null)
            Destroy(currentLeftHandWeapon);

        var leftWeapon = Instantiate(leftHandObj, leftHandAnchor);
        leftWeapon.transform.localPosition = leftHandObj.transform.localPosition;
        leftWeapon.transform.localRotation = leftHandObj.transform.localRotation;
        currentRightHandWeapon = leftWeapon;
    }

    public void GetRightWeapon(GameObject rightHandObj)
    {
        if (currentRightHandWeapon != null)
            Destroy(currentRightHandWeapon);

        var rightWeapon = Instantiate(rightHandObj, rightHandAnchor);
        rightWeapon.transform.localPosition = rightHandObj.transform.localPosition;
        rightWeapon.transform.localRotation = rightHandObj.transform.localRotation;
        currentRightHandWeapon = rightWeapon;
        weaponCollider = currentRightHandWeapon.GetComponent<BoxCollider>();
        if (currentRightHandWeapon != null)
        {
            InAction = false; // 웨폰을 들면 공격 시작
        }
    }
}
