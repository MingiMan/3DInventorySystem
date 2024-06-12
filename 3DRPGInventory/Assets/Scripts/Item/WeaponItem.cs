using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : EquipmentItem
{
    public WeaponItem(WeaponItemData data) : base(data) { }
    // WeaponItemData  형식의 데이터를 매개변수로 받습니다.
    // base 키워드를 사용하여 기본 클래스인 EquipmentItem의 생성자를 호출합니다

}
