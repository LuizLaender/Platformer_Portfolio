using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    [SerializeField] AudioClip pickupCoinsSFX;
    [SerializeField] int valueOfEachCoin = 10;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && other.GetType().Equals(typeof(CapsuleCollider2D)))
        {
            FindObjectOfType<GameSession>().AddToScore(valueOfEachCoin);
            AudioSource.PlayClipAtPoint(pickupCoinsSFX, Camera.main.transform.position);
            Destroy(gameObject);
        }    
    }
}
