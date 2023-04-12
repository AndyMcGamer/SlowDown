using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool facingRight;
    [SerializeField] private float gravity;

    [SerializeField] private bool grounded;

    [SerializeField] private float buttonThreshold;
    [SerializeField] private float attackDelay = 0f;
    private bool falling = false;
    private bool attacking = false;
    [SerializeField] private float fallAnimDelay = 0f;
    [SerializeField] private LayerMask enemyMask;

    [SerializeField] private Vector2 groundCheckOffset;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask platformMask;

    [SerializeField] private float slowMultiplier = 0.5f;
    [SerializeField] private float maxSlowTimer = 1f;
    private float slowTimer = 0f;
    [SerializeField] private float rechargeRate = 0.5f;
    private bool recharging = false;


    [SerializeField] private int health = 3;

    [SerializeField] private float damageDelay = 0f;
    private float damageTimer = 0f;
    

    private float buttonTimer = 0f;
    private float attackBuffer = 0f;
    private float delayTimer = 0f;
    private bool invincible = false;
    private bool bouncing = false;

    private bool dead = false;

    [SerializeField] private LayerMask wallLayer; 

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Image[] heartImages;
    [SerializeField] private GameObject timeBar;
    [SerializeField] private GameObject emptyTimeBar;
    [SerializeField] private GameObject transition;
    [SerializeField] private CameraShake shake;
    [SerializeField] private ParticleSystem system;

    private void Awake()
    {
        dead = false;
        bouncing = false;
        invincible = false;
        falling = false;
        attacking = false;
        recharging = false;
        health = 3;
        UpdateHealth();
        GameLogic.instance.StartThings();
    }

    private void Update()
    {
        if (dead) return;
        animator.speed = 1 * GameLogic.instance.speedup;
        if(slowTimer >= maxSlowTimer)
        {
            timeBar.SetActive(false);
            emptyTimeBar.SetActive(false);
        }

        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
        else
        {
            GroundCheck();
            //WallCheck();
            if (grounded && rb.velocity.y < 0) bouncing = false;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (buttonTimer <= buttonThreshold)
                {
                    Attack();
                }
                buttonTimer = 0f;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                buttonTimer += Time.deltaTime;
                if (buttonTimer > buttonThreshold && slowTimer >= 0 && !recharging)
                {
                    slowTimer -= Time.deltaTime;
                    timeBar.SetActive(true);
                    emptyTimeBar.SetActive(true);
                    timeBar.transform.localScale = new Vector3(slowTimer / maxSlowTimer, 0.1f, 1f);
                    GameLogic.instance.timeMultiplier = slowMultiplier;
                }
                if (slowTimer < 0)
                {
                    GameLogic.instance.timeMultiplier = 1f;
                    recharging = true;
                }
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                GameLogic.instance.timeMultiplier = 1f;
                if (buttonTimer <= buttonThreshold)
                {
                    if (grounded) Jump();
                }
                buttonTimer = 0f;
                recharging = false;
            }
        }
        if (transform.position.x <= -5.7)
        {
            facingRight = true;
            rb.position = new Vector2(rb.position.x + 0.1f, rb.position.y);
        }
        if (transform.position.x >= 5.7)
        {
            facingRight = false;
            rb.position = new Vector2(rb.position.x - 0.1f, rb.position.y);
        }
        attackBuffer -= Time.deltaTime * GameLogic.instance.speedup;
        sprite.flipX = !facingRight;
        if(GameLogic.instance.timeMultiplier == 1f && slowTimer < maxSlowTimer)
        {
            timeBar.SetActive(true);
            emptyTimeBar.SetActive(true);
            timeBar.transform.localScale = new Vector3(slowTimer / maxSlowTimer, 0.1f, 1f);
            slowTimer += Time.deltaTime * rechargeRate * GameLogic.instance.speedup;
            //if (recharging && slowTimer >= rechargeAmount) recharging = false;
            if(slowTimer > maxSlowTimer)
            {
                slowTimer = maxSlowTimer;
            }
        }
        if (attacking)
        {
            animator.SetTrigger("Attack");
            delayTimer -= Time.deltaTime * GameLogic.instance.speedup;
            if (!invincible)
            {
                AttackCheck();
            }
            if(delayTimer <= 0)
            {
                attacking = false;
                invincible = false;
                delayTimer = fallAnimDelay;
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Falling");
            }
        }
    }

    private void AttackCheck()
    {

        Vector3 offset = new Vector3((facingRight ? 1 : -1) * 0.5f, 0.3f, 0);
        Collider2D col = Physics2D.OverlapCircle(transform.position + offset, 1.15f, enemyMask);
        if (col && !invincible)
        {
            invincible = true;
            if (col.CompareTag("Enemy"))
            {
                shake.ShakeCam(0.1f, 0.235f);
                //RaycastHit2D hit = Physics2D.Raycast(transform.position, ( - transform.position).normalized, 1.15f, enemyMask);
                system.gameObject.transform.position = col.gameObject.transform.position;
                system.Play();
                Vector3 d = col.gameObject.transform.position - transform.position;
                d = d.normalized;
                Bounce();
                float speed = 3*Mathf.Abs(Vector3.Dot(d, Vector3.down));
                col.gameObject.GetComponent<EnemyScript>().ChangeDir(d, speed);
                //attacking = false;
                invincible = true;
                bouncing = true;
                
            }
        }
    }

    public void Bounce()
    {
        invincible = true;
        float bounceFactor = Random.Range(1.3f, 1.6f);
        AudioManager.instance.Play("Jump");
        rb.AddForce(new Vector2(0, jumpForce * bounceFactor * Mathf.Max(GameLogic.instance.speedMultiplier * 0.75f, 1f)), ForceMode2D.Impulse);
        attackBuffer = attackDelay;
    }

    private void GroundCheck()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y) + groundCheckOffset, groundCheckRadius, platformMask);
        foreach (var c in colliders)
        {
            
            if (c.CompareTag("Platform"))
            {
                grounded = true;
                return;
            }
        }
        grounded = false;
    }

    //private void WallCheck()
    //{
    //    Collider2D col = Physics2D.OverlapBox(transform.position,new Vector2(0.7f,1.2f),wallLayer);
    //    if (col)
    //    {
    //        if (col.CompareTag("Wall"))
    //        {
    //            facingRight = !facingRight;
    //        }
    //    }
        
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    var col = collision.collider;
    //    if (col.CompareTag("Wall"))
    //    {
    //        facingRight = !facingRight;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && damageTimer <= 0 && !bouncing)
        {
            if (damageTimer <= 0)
            {
                if (dead) return;
                health -= 1;
                damageTimer = damageDelay;
                animator.SetTrigger("Damage");
                AudioManager.instance.Play("Hit");
                UpdateHealth();
                shake.ShakeCam(0.2f, 0.4f);
                if (health <= 0)
                {
                    Death();
                }
            }

        }
        if (collision.CompareTag("Lava"))
        {
            if (dead) return;
            AudioManager.instance.Play("Hit");
            shake.ShakeCam(0.15f, 0.5f);
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0, 5.5f), ForceMode2D.Impulse);
            Death();
        }
        if (collision.CompareTag("Heart"))
        {
            AudioManager.instance.Play("Heart");
            if(health < 3)
            {
                health += 1;
                UpdateHealth();
            }
            
            Destroy(collision.gameObject);
        }
    }

    private void UpdateHealth()
    {
        for (int i = 0; i < health; i++)
        {
            heartImages[i].fillAmount = 1;
        }
        for(int i = health; i < 3; ++i)
        {
            heartImages[i].fillAmount = 0;
        }
    }

    private void Death()
    {
        animator.SetTrigger("Dead");
        dead = true;
        
        animator.speed = 1;
        GameLogic.instance.StopEverything();
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        rb.gravityScale = 2f;
        yield return new WaitForSeconds(0.3f);
        
        // Transition
        transition.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(1f);
        GameLogic.instance.ChangeScene("MainMenu");
    }

    private void Attack()
    {
        if (!grounded)
        {
            if (attackBuffer <= 0)
            {
                AudioManager.instance.Play("Attack");
                animator.ResetTrigger("Jump");
                animator.SetTrigger("Attack");
                attacking = true;
                delayTimer = fallAnimDelay;
                rb.velocity = Vector2.zero;
                attackBuffer = attackDelay;
            }
        }
    }

    private void Jump()
    {
        if(grounded)
        {
            // Jump
            animator.SetTrigger("Jump");
            AudioManager.instance.Play("Jump");

            rb.AddForce(new Vector2(0, jumpForce * Mathf.Max(GameLogic.instance.speedMultiplier * 0.75f, 1f)), ForceMode2D.Impulse);
            falling = true;
        }
    }

    private void FixedUpdate()
    {
        if (dead) return;
        if (damageTimer > 0)
        {
            animator.ResetTrigger("Grounded");
            animator.ResetTrigger("Falling");
            animator.SetTrigger("Damage");
            rb.velocity = new Vector2(rb.velocity.x, 0);
            return;
        }
        if (!falling && !attacking)
        {
            if (!grounded)
            {
                animator.ResetTrigger("Grounded");
                if (rb.velocity.y <= 0)
                {
                    animator.SetTrigger("Falling");
                }
                else
                {
                    animator.SetTrigger("Jump");
                }
                
            }
            else
            {
                animator.ResetTrigger("Falling");
                animator.SetTrigger("Grounded");
                attackBuffer = 0;
                
            }
            
            if (grounded)
            {
                animator.SetTrigger("Grounded");
            }
            
            rb.velocity = new Vector2(GameLogic.instance.speedup * speed * (facingRight ? 1 : -1), Mathf.Max(GameLogic.instance.timeMultiplier*0.5f, 1f) * (rb.velocity.y + GameLogic.instance.speedMultiplier * gravity) * (grounded ? 0 : 1));
            
        }
        falling = false;
    }
}
