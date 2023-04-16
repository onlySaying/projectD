using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageMonster : MonoBehaviour
{
    public int health;
    Rigidbody2D rigidbody;
    // Start is called before the first frame update1
    Animator anim;
    // Start is called before the first frame update
    BoxCollider2D collider;
    public GameObject mm;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        mm.GetComponent<MovingMonster>();

    }
    void FixedUpdate()
    {
       
    }
    void Onhit(int dmg)
    {
        health -= dmg;
        Invoke("returnSprites", 0.1f);
        Debug.Log("dmg");
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger");
        if (other.tag == "saveZone")
        {
            Debug.Log("saveZon!!");
        }
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        /*if (collision.gameObject.name == "statue")
        {
            Debug.Log("statue!!!");
        }
        if ((collision.gameObject.layer == 7))
        {
            
            Debug.Log("7");
            
        }*/
        if ((collision.gameObject.tag == "saveZone"))
        {
            MovingMonster mm = collision.gameObject.GetComponent<MovingMonster>();
            Debug.Log("saveZone");
            Ondamage(mm.nextMove);
        }
    }

    void Ondamage(int vec)
    {
        vec *= -1;
        rigidbody.AddForce(new Vector2(0, 2)*6, ForceMode2D.Impulse);
    }



 }
