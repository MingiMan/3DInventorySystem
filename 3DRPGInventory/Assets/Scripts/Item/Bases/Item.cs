using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item
{
    public ItemData Data { get; private set; }
    // 클래스 내부에서만 변경이 가능

    // 생성자는 클래스의 인스턴스가 생성될 때 호출되며, 클래스의 초기화 작업을 수행합니다
    public Item(ItemData data) => Data = data;
}
