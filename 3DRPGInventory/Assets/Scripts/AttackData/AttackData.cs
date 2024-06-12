using UnityEngine;


[CreateAssetMenu(fileName = "AttackData", menuName = "ComboSystem/AttackData")]
public class AttackData : ScriptableObject
{
    [SerializeField] public string AnimationName;
    [SerializeField] public float impactStartTime;
    [SerializeField] public float impactEndTime;
    [SerializeField] public AttackHitBox attackHitBox;

}
public enum AttackHitBox
{
    Sword,
    Axe
}
