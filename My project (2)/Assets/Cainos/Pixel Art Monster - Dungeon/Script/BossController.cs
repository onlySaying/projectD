using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Timers;

namespace Cainos.PixelArtMonster_Dungeon
{
    public class BossController : MonoBehaviour
    {
        //MOVEMENT PARAMETERS
        public MovementType defaultMovement = MovementType.Walk;

        public float walkSpeedMax = 2.5f;                           // max walk speed
        public float walkAcc = 10.0f;                               // walking acceleration

        public float runSpeedMax = 5.0f;                            // max run speed
        public float runAcc = 10.0f;                                // running acceleration

        public float airSpeedMax = 2.0f;                            // max move speed while in air
        public float airAcc = 8.0f;                                 // air acceleration

        public float groundBrakeAcc = 6.0f;                         // braking acceleration (from movement to still) while on ground
        public float airBrakeAcc = 1.0f;                            // braking acceleration (from movement to still) while in air

        public float jumpSpeed = 5.0f;                              // speed applied to character when jump
        public float jumpCooldown = 0.55f;                          // time to be able to jump again after landing
        public float jumpDelay = 0.5f;                              // time for the character to actually jump after jump key is pressed down, to make time for the jump prepare animation to be played
        public float jumpGravityMutiplier = 0.6f;                   // gravity multiplier when character is jumping, should be within [0.0,1.0], set it to lower value so that the longer you press the jump button, the higher the character can jump    
        public float fallGravityMutiplier = 1.3f;                   // gravity multiplier when character is falling, should be equal or greater than 1.0

        public Vector2 groundCheckSize = new Vector2(0.1f, 0.1f);    // size of the box at the character's bottom to determine whether the character is on ground
        public float movingBlendTransitionSpeed = 2.0f;             // the transition speed of moving blend value
        public bool canAttackInAir = true;                          // can the character attack while in air?
        public bool canAttackWhenMoving = true;                     // can the character attack when moving? When turned off, it also forbit the character to move while in attack animation

        //RUNTIME INPUT PARAMETERS
        public Vector2 inputMove = Vector2.zero;                    // movement input, x for horizontal, y for vertical, x and y should be in [-1.0, 1.0]                 
        public bool inputMoveModifier = false;                      // switch between walk and run
        public bool inputJump = false;                              // jump input
        public bool inputAttack = false;                            // attack input



        [ExposeProperty]                                            // is the character dead? if dead, plays dead animation and disable control
        public bool IsDead
        {
            get { return isDead; }
            set
            {
                isDead = value;
                pm.IsDead = isDead;
            }
        }
        private bool isDead;

        private PixelMonster pm;                                    // the PixelMonster script attached the character
        private Collider2D collider2d;                              // Collider compoent on the character
        private Rigidbody2D rb2d;                                   // Rigidbody2D component on the character
        private CircleCollider2D circleCollider;


        private bool isGrounded;                                    // is the character on ground?
        private bool isMoving;                                      // is the character moving?
        private Vector2 curVel;                                     // current velocity
        private float jumpCdTimer;                                  // timer for jump cooldown
        private float jumpDelayTimer;                               // timer for jump delay ( jump prepare animation)
        private bool isInJumpPrepare;                               // is the character in jump prepare animation

        private Collider2D[] groundCheckResult = new Collider2D[2];
        private ContactFilter2D contactFilter2D = new ContactFilter2D();
        private float xposChase;
        private float xpos;
        private Timer timer;
        private bool inputPermit;
        private Transform target;

        private void Awake()
        {
            pm = GetComponent<PixelMonster>();
            collider2d = GetComponent<Collider2D>();
            rb2d = GetComponent<Rigidbody2D>();
            circleCollider = GetComponent<CircleCollider2D>();
            target = null;
            contactFilter2D.NoFilter();
            contactFilter2D.useTriggers = false;
            timer = new Timer();
            timer.Interval = 1000;
            inputPermit = true;
            
        }

        private void Update()
        {
            if (target != null)
            {
                if (rb2d.position.x > target.position.x)
                {
                    inputMove.x = -1;
                }
                else if (rb2d.position.x < target.position.x)
                {
                    inputMove.x = 1;
                }
            }
            //set should the character walk or run base on input and default movement type
            bool shouldRun = false;
            if (defaultMovement == MovementType.Walk && inputMoveModifier) shouldRun = true;
            if (defaultMovement == MovementType.Run && !inputMoveModifier) shouldRun = true;
           
            //set moving blend
            float movingBlend = 0.0f;
            if (Mathf.Abs(inputMove.x) > 0.01f)
            {
                movingBlend = 0.5f;
                if (shouldRun) movingBlend = 1.0f;
            }
            pm.MovingBlend = Mathf.MoveTowards(pm.MovingBlend, movingBlend, Time.deltaTime * movingBlendTransitionSpeed);

            //perform move and attack
            Move(inputMove.x, shouldRun, inputJump);
            Attack(inputAttack);

            //CHECK IF THE CHARACTER IS ON GROUND
            isGrounded = false;
            for (int i = 0; i < groundCheckResult.Length; i++) groundCheckResult[i] = null;
            Physics2D.OverlapBox(transform.position, groundCheckSize, 0, contactFilter2D, groundCheckResult);
            foreach (var c in groundCheckResult)
            {
                if (c && c.gameObject != gameObject) isGrounded = true;
            }

            //jump cooldown
            if (isGrounded)
            {
                if (jumpCdTimer < jumpCooldown) jumpCdTimer += Time.deltaTime;
            }
        }

        public void Attack(bool inputAttack)
        {
            if (isInJumpPrepare) return;
            if (!canAttackInAir && !isGrounded) return;
            if (!canAttackWhenMoving && isMoving) return;

            if (inputAttack)
            {
                pm.Attack();
                inputAttack = false;
            }
        }

        public void Move(float inputH, bool inputRunning, bool inputJump)
        {
            if (isDead) inputH = 0.0f;
            if (!canAttackWhenMoving && pm.IsAttacking) inputH = 0.0f;
            if (isInJumpPrepare) inputH = 0.0f;

            //get current velocity from rigidbody
            curVel = rb2d.velocity;

            //set acceleration and max speed base on condition
            float acc = 0.0f;
            float max = 0.0f;
            float brakeAcc = 0.0f;

            if (isGrounded)
            {
                acc = inputRunning ? runAcc : walkAcc;
                max = inputRunning ? runSpeedMax : walkSpeedMax;
                brakeAcc = groundBrakeAcc;

                if (isInJumpPrepare) acc = 0;
            }
            else
            {
                acc = airAcc;
                max = airSpeedMax;
                brakeAcc = airBrakeAcc;
            }

            //horizontal movement
            //has horizontal movement input
            if (Mathf.Abs(inputH) > 0.01f)
            {
                isMoving = true;

                //if current horizontal speed is out of allowed range, let it fall to the allowed range
                bool shouldMove = true;
                bool vect = true;
                
                //direction ¹æÇâ
                if (inputH > 0 && curVel.x > max) // right
                {  
                    curVel.x = Mathf.MoveTowards(curVel.x, max, brakeAcc * Time.deltaTime);
                    shouldMove = false;
                }
                if (inputH < 0 && curVel.x < -max) //left
                {
                    curVel.x = Mathf.MoveTowards(curVel.x, -max, brakeAcc * Time.deltaTime);
                    shouldMove = false;
                }
               

                //otherwise, add movement acceleration to cureent velocity
                if (shouldMove)
                {
                    curVel.x += acc * Time.deltaTime * inputH;
                    curVel.x = Mathf.Clamp(curVel.x, -max, max);
                }
            }
            //no horizontal movement input, brake to speed zero
            else
            {
                isMoving = false;
                curVel.x = Mathf.MoveTowards(curVel.x, 0.0f, brakeAcc * Time.deltaTime);
            }

           checkUse(inputH, inputPermit);

            //jump
            //jumpCdTimer >= 0.05f to prevent errors when the jumpCooldown is too small
            if (isGrounded && inputJump && jumpCdTimer >= jumpCooldown && jumpCdTimer >= 0.05f)
            {
                isInJumpPrepare = true;
                pm.IsInJumpPrepare = true;

                jumpDelayTimer += Time.deltaTime;
                if (jumpDelayTimer > jumpDelay)
                {
                    jumpDelayTimer = 0.0f;
                    isGrounded = false;
                    jumpCdTimer = 0.0f;
                    curVel.y += jumpSpeed+3;
                }
                inputJump = false;

            }
            else
            {
                jumpDelayTimer = 0.0f;
                isInJumpPrepare = false;
                pm.IsInJumpPrepare = false;
                inputJump = false;
               

            }

            if (inputJump && curVel.y > 0)
            {
                curVel.y += Physics.gravity.y * (jumpGravityMutiplier - 1.0f) * Time.deltaTime;
            }
            else if (curVel.y > 0)
            {
                curVel.y += Physics.gravity.y * (fallGravityMutiplier - 1.0f) * Time.deltaTime;
            }

            //set modified velocity back to rigidbody
            rb2d.velocity = curVel;

            //set parameters on PixelMonster script to change appearance accordingly
            pm.SpeedVertical = curVel.y;
            //pm.Facing = Mathf.RoundToInt(inputH);
            pm.IsGrounded = isGrounded;
            if (inputH !=0) // positive right negative left
            {
                if (inputH > 0)
                {
                    transform.localScale = new Vector3(1.3f, 1.4f, 1f);
                }
                else if (inputH < 0)
                {
                    transform.localScale = new Vector3(-1.3f, 1.4f, 1);
                }
            }

         
        }

        public enum MovementType
        {
            Walk,
            Run
        }

        private void OnDrawGizmosSelected()
        {
            //Draw the ground check box
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, groundCheckSize);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((collision.gameObject.tag == "Player"))
            {
                inputMoveModifier = true;
                circleCollider.offset = new Vector2(0, 0);
                circleCollider.radius = 10;
                target = collision.gameObject.transform;
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {

            xposChase = collision.transform.position.x;
            xpos = rb2d.position.x;
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

        public void resetState()
        {
            circleCollider.offset = new Vector2(2, 1);
            circleCollider.radius = 5;
            target = null;
        }

        public void checkUse(float input, bool permit)
        {
            Transform temp = rb2d.transform;
            StartCoroutine(jumpChecking(input, permit,temp));
        }

        IEnumerator jumpChecking(float inputH, bool permit, Transform temp)
        {
            if (permit)
            {
                inputPermit = false;
                if (inputH != 0)
                {
                    yield return new WaitForSeconds(2f);
                    if (Mathf.Abs(temp.position.x - rb2d.position.x) == 0)
                    {
                      
                        inputJump = true;
                    }
                    

                }
                else if (inputH == 0)
                {
                    inputJump = false;
                }
                inputPermit = true;

            }
            else
            {
                
            }
        }


    }
}
