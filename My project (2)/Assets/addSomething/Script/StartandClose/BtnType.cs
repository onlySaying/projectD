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
                Debug.Log("������");
                break;
            case CurrentType.Continue:
                Debug.Log("�̾��ϱ�");
                break;
        }
    }
}
