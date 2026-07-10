using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Sound effects:")]
    [SerializeField] AudioClip shootArrowSFX;
    [SerializeField] AudioClip playerJumpSFX;
    [SerializeField] AudioClip playerHurtSFX;

    [Header("Objects:")]
    [SerializeField] GameObject arrow;
    [SerializeField] Transform arrowSpawnPosition;

    [Header("Variables:")]
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float playerVelocityMultiplier = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float deathDelay = 1;

    float horizontalSpeedThreshold = 0.1f;
    float verticalSpeedThreshold = 0.1f;
    float playerStartingGravityScale = 4f;
    bool isPlayerAlive = true;

    Vector2 moveInput;
    Rigidbody2D myRigidbody2D;
    Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider2D; // player's body
    BoxCollider2D myBoxCollider2D; // player's feet

    void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody2D = GetComponent<Rigidbody2D>();
        myCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        myBoxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        myRigidbody2D.gravityScale = playerStartingGravityScale;
    }

    void Update()
    {
        if(!isPlayerAlive) {return;}

        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    public bool GetPlayerHasHorizontalSpeed()
    {
        return Mathf.Abs(myRigidbody2D.linearVelocity.x) > horizontalSpeedThreshold;
    }

    public bool GetPlayerHasVerticalSpeed()
    {
        return Mathf.Abs(myRigidbody2D.linearVelocity.y) > verticalSpeedThreshold;
    }

    public void ShootArrowThroughAnimationEvent()
    {
        PlayAudioShootingArrow();

        GameObject instantiatedArrow = Instantiate(arrow, arrowSpawnPosition.position, transform.rotation); 
        
        // The following flips the arrow sprite acording to the direction that the player is facing
        if (transform.localScale.x > 0)
        {instantiatedArrow.transform.localScale = new Vector2(1,1);}

        else if (transform.localScale.x < 0)
        {instantiatedArrow.transform.localScale = new Vector2(-1,1);}
    }

    public void SetPlayerControlls(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        moveInput = new Vector2(moveInput.x, moveInput.y);
    }

    void PlayAudioShootingArrow()
    {
        AudioSource.PlayClipAtPoint(shootArrowSFX, Camera.main.transform.position);
    }

    void OnMove(InputValue value)
    {
        if(!isPlayerAlive){return;}

        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if(!isPlayerAlive){return;}

        int groundLayer = LayerMask.GetMask("Ground");

        if(value.isPressed && myBoxCollider2D.IsTouchingLayers(groundLayer))
        {
            myRigidbody2D.linearVelocity += new Vector2 (0f, jumpSpeed);
            AudioSource.PlayClipAtPoint(playerJumpSFX, Camera.main.transform.position);
        }
    }

    void OnFire(InputValue value)
    {
        if(!isPlayerAlive){return;}

        if(value.isPressed)
        {
            myAnimator.SetTrigger("shootArrow");
        }
    }

    void ClimbLadder()
    {
        bool playerHasVerticalSpeed = GetPlayerHasVerticalSpeed();
        bool isCollidingWithLadder = myCapsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing"));

        if(!isCollidingWithLadder) 
        {
            myRigidbody2D.gravityScale = playerStartingGravityScale;
            myAnimator.SetBool("isClimbing", false);
            myAnimator.speed = 0.75f;
            return;
        }

        Vector2 climbVelocity = new Vector2(moveInput.x * playerVelocityMultiplier, moveInput.y * climbSpeed);
        myRigidbody2D.linearVelocity = climbVelocity;
        myRigidbody2D.gravityScale = 0f;

        if(isCollidingWithLadder)
        {
            myAnimator.SetBool("isClimbing", true);
        }

        if(!playerHasVerticalSpeed) 
        {
            myAnimator.SetBool("isClimbing", true);
            myAnimator.speed = 0;
        }
        else
        {
            myAnimator.speed = 0.75f;
        }
    }

    void Run()
    {
        bool playerHasHorizontalSpeed = GetPlayerHasHorizontalSpeed();

        Vector2 playerVelocity = new Vector2(moveInput.x * playerVelocityMultiplier, myRigidbody2D.linearVelocity.y);
        myRigidbody2D.linearVelocity = playerVelocity;

        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void Die()
    {
        if(myRigidbody2D.IsTouchingLayers(LayerMask.GetMask("Enemies", "Spikes", "Water")))
        {
            AudioSource.PlayClipAtPoint(playerHurtSFX, Camera.main.transform.position);
            isPlayerAlive = false;
            myAnimator.SetTrigger("dying");
            myRigidbody2D.linearVelocity = new Vector2(-myRigidbody2D.linearVelocity.x,-myRigidbody2D.linearVelocity.y);

            StartCoroutine(DieAfterDelay(deathDelay));
        }
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = GetPlayerHasHorizontalSpeed();

        if(playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody2D.linearVelocity.x), 1f);
        }
    }

    IEnumerator DieAfterDelay(float waitTimeEnum)
    {
        yield return new WaitForSecondsRealtime(waitTimeEnum);

        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }
}
