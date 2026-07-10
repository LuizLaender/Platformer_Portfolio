using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myRigidbody;
    Rigidbody2D playerRigidbody;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        playerRigidbody = FindObjectOfType<Rigidbody2D>();
    }

    void Update()
    {
        myRigidbody.linearVelocity = new Vector2 (moveSpeed, 0f);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        moveSpeed = -moveSpeed;
        FlipEnemyFacing();
    }

    void FlipEnemyFacing()
    {
        transform.localScale = new Vector2 (-transform.localScale.x, transform.localScale.y);
    }
}
