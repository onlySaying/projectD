using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectByDash : MonoBehaviour
{
   // public TrailRenderer trailEffect;

    public void Use()
    {
       
        StopCoroutine("Dash");
        StartCoroutine("Dash");
    }
    public void Stopping()
    {
   
        StopCoroutine("Walking");
        StartCoroutine("Walking");
      
    }

    IEnumerator Dash()
    {
        //1
        yield return null; // 1frame wait
     
        //2
        yield return new WaitForSeconds(12f); // 1frame wait
       // trailEffect.enabled = false;


    }

    IEnumerator Walking()
    {
        yield return new WaitForSeconds(0.3f); // 1frame wait
       // trailEffect.enabled = false;
    }
}
