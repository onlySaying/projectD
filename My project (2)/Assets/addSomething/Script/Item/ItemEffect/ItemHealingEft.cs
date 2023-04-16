using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEft/Consumable/Healteft")]
public class ItemHealingEft : ItemEffect
{
    public Player player;
    public int healingPoint = 0;
    public override bool ExecuteRole()
    {
        Debug.Log("PlayerHP Add"+healingPoint);
        //player.AddHP(healingPoint);
        return true;
    }
}
