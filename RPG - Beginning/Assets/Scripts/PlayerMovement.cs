using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    walk, attack, interact, stagger, idle
}

public class PlayerMovement : MonoBehaviour
{
    public PlayerState currentState;
    public float speed;
    private Rigidbody2D Rigidbody;
    private Vector3 change;
    private Animator animator;
    public FloatValue currentHealth;
    public Signal playerHealthSignal;

    void Start()
    {
        currentState = PlayerState.walk;
        animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody2D>();
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
    }

    void Update()
    {
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        if(Input.GetButtonDown("meleeAttack") && currentState != PlayerState.attack && currentState != PlayerState.stagger)
        {
            StartCoroutine(AttackCo());
        }
        else if(currentState == PlayerState.walk || currentState == PlayerState.idle)
        {
            UpdateAnimationAndMove();
        }
    }

    private IEnumerator AttackCo()
    {
        animator.SetBool("attacking", true);
        currentState = PlayerState.attack;
        yield return null;
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(0.3f);
        currentState = PlayerState.walk;
    }

    void UpdateAnimationAndMove()
    {
        if (change != Vector3.zero)
        {
            MoveCharacter();
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);
            animator.SetBool("moving", true);
        }
        else
        {
            animator.SetBool("moving", false);
        }
    }

    void MoveCharacter()
    {
        change.Normalize();
        Rigidbody.MovePosition(transform.position + change * speed * Time.deltaTime);
    }

    public void Knock(float knockTime, float damage)
    {
        currentHealth.initialValue -= damage;

        if (currentHealth.initialValue > 0 )
        {
            playerHealthSignal.Raise();
            StartCoroutine(KnockCo(knockTime));
        }
    }

    private IEnumerator KnockCo(float knockTime)
    {
        if (Rigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            Rigidbody.velocity = Vector2.zero;
            currentState = PlayerState.idle;
            Rigidbody.velocity = Vector2.zero;
        }
    }
}
