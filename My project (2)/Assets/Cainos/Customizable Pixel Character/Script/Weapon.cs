using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee,Range }
    // Start is called before the first frame update

    public Type type;
    public int dmg;
    public float rate;
    public PolygonCollider2D MeleeArea;
    public TrailRenderer trailEffect;

    public void Use()
    {
        StopCoroutine("swing");
        StartCoroutine("swing");
    }

    IEnumerator swing()
    {
        //1
        yield return null; // 1frame wait
        MeleeArea.enabled = true;
        trailEffect.enabled = true;
        //2
        yield return new WaitForSeconds(0.3f); // 1frame wait
        MeleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;


    }
}
