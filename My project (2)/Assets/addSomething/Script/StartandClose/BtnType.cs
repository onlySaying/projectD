using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BtnType : MonoBehaviour
{
    public CurrentType currentType;
    public void OnBtnClick()
    {
        switch(currentType)
        {
            case CurrentType.New:
                Debug.Log("새게임");
                break;
            case CurrentType.Continue:
                Debug.Log("이어하기");
                break;
        }
    }
}
