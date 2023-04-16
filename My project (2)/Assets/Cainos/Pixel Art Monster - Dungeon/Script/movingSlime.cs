using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingSlime : MonoBehaviour
{
    Rigidbody2D rigidbody;
    // Start is called before the first frame update1
    Animator anim;
    public int nextMove;
    CircleCollider2D cCollider;
    public float speed;
    Transform target;
    float chaseX;
    float chaseY;
    Vector2 frontVec;
    Vector2 enemyVec;
    float xposChase;
    float xpos;
    float attackable;
    public int hp;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        cCollider = GetComponent<CircleCollider2D>();
        anim.fireEvents = false;
        speed = 1;
        target = null;
        Invoke("Think", 5);
        InvokeRepeating("UpdateTarget", 0f, 0.25f);
        xposChase = 0f;
        xpos = 0f;
        attackable = 0;
        hp = 2;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //move
        if (attackable != 1)
        {
            if (target != null)
            {
                xposChase = target.position.x;
                xpos = rigidbody.position.x;
                if (xpos <= xposChase)
                {
                    nextMove = 1;
                    //Debug.Log("오른쪽에적");
                }
                else if (xpos > xposChase)
                {
                    nextMove = -1;
                    //Debug.Log("왼쪽에적");
                }

            }
            anim.SetInteger("WalkSpeed", nextMove);
            rigidbody.velocity = new Vector2(nextMove * speed, rigidbody.velocity.y);

            //check road
            frontVec = new Vector2(rigidbody.position.x + nextMove * 0.7f, rigidbody.position.y);
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayhit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));


            if (rayhit.collider == null)
            {
                nextMove *= -1;
                CancelInvoke();
                Invoke("Think", 5);

            }

            //좌우 반전
            if (nextMove != 0)
            {
                if (nextMove == -1)
                { transform.localScale = new Vector3(-1, 1, 1); }
                else if (nextMove == 1)
                { transform.localScale = new Vector3(1, 1, 1); }
            }
        }
        else if (attackable == 1)
        {
            speed = 0;
            anim.SetFloat("speed", speed);
        }

    }

    void Think()
    {
        if (target == null)
        {
            nextMove = Random.Range(-1, 2);

            Invoke("Think", 5);
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "saveZone"))
        {
            Ondamage();
            resetState();
        }

        if ((collision.gameObject.tag == "weapon"))
        {
            attackByPlayer(collision);
            speed = 3;
        }


    }

    public void attackByPlayer(Collision2D collision)
    {
        if (collision.gameObject.transform.position.x > rigidbody.position.x)
        {
            nextMove = -1;
            anim.SetInteger("hit", 1);
            rigidbody.AddForce(new Vector2(nextMove, 1) * 8, ForceMode2D.Impulse);
            Invoke("offDamage", 1);
            speed = 0;
            new WaitForSeconds(0.5f);

        }
        else if (collision.gameObject.transform.position.x > rigidbody.position.x)
        {
            nextMove = 1;
            anim.SetInteger("hit", 1);
            rigidbody.AddForce(new Vector2(nextMove, 1) * 8, ForceMode2D.Impulse);
            Invoke("offDamage", 1);
            speed = 0;
            new WaitForSeconds(0.5f);
        }
        else
        {
            Ondamage();
            speed = 0;
            new WaitForSeconds(0.5f);
        }
        hp--;
        if(hp == 0)
        {
            Destroy(gameObject);
        }
     

    }

    void Ondamage()
    {
        nextMove *= -1;
        anim.SetInteger("hit", 1);
        rigidbody.AddForce(new Vector2(nextMove, 1) * 8, ForceMode2D.Impulse);
        Invoke("offDamage", 1);
    }
    void offDamage()
    {
        anim.SetInteger("hit", 0);
    }

 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.tag == "Player"))
        {
            speed = 3;
            anim.SetFloat("WalkSpeed", speed);
            cCollider.offset = new Vector2(0, 0);
            cCollider.radius = 12;
            target = collision.gameObject.transform;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        xposChase = collision.transform.position.x;
        xpos = rigidbody.position.x;
        float distance = xposChase - xpos;
        if (distance < 12)
        {
            if (distance < -12)
            {
                resetState();
            }

        }
        else
        {
            resetState();
        }
    }


    private void UpdateTarget()
    {
        if (target != null)
        {
            Vector2 chase = target.position;
            chaseX = target.position.x;
            chaseY = target.position.y;
        }
    }

    void resetState()
    {
        cCollider.offset = new Vector2(2, 0);
        cCollider.radius = 3;
        speed = 1;
        anim.SetFloat("speed", speed);
        target = null;
    }

}
