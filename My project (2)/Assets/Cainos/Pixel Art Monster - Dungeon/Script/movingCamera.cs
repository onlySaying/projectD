using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingCamera : MonoBehaviour
{
    public GameObject A;  //A라는 GameObject변수 선언
    Transform AT;

    //public Transform target;

    public Vector2 center;
    public Vector2 size;
    float height;
    float width;



    void Start()
    {
        AT = A.transform;
        //height = Camera.main.orthographicSize;
        //width = height * Screen.width / Screen.height;
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, size);
    }
    */
    void LateUpdate()
    {
        transform.position = new Vector3(AT.position.x, AT.position.y, transform.position.z);

        
        //transform.position = new Vector3(AT.position.x, AT.position.y, AT.position.z);
        //transform.position = new Vector3(target.position.x, target.position.y, target.position.z);

        //transform.position = new Vector3(target.position.x, target.position.y, -10f);
        /*
        float lx = size.x * 0.5f - width;
        float clampX = Mathf.Clamp(transform.position.x, -lx + center.x, lx + center.x);

        float ly = size.y * 0.5f - height;
        float clampY = Mathf.Clamp(transform.position.y, -ly + center.x, ly + center.y);

        transform.position = new Vector3(clampX, clampY, -10f);
        */
        
    }


}