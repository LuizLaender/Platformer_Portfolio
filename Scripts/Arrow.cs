using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] AudioClip arrowHitEnemySFX;
    [SerializeField] float arrowSpeed = 12f;

    float horizontalSpeed;
    int enemyKillReward = 25;

    Rigidbody2D myRigidbody2d;
    PlayerMovement player;

    void Awake()
    {
        myRigidbody2d = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        horizontalSpeed = player.transform.localScale.x * arrowSpeed;
    }

    void Update()
    {
        myRigidbody2d.linearVelocity = new Vector2(horizontalSpeed,0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy")
        {
            FindObjectOfType<GameSession>().AddToScore(enemyKillReward);
            AudioSource.PlayClipAtPoint(arrowHitEnemySFX, Camera.main.transform.position);
            Destroy(other.gameObject);
        }
        Destroy(gameObject); //destroys arrow when colliding with enemy
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}
