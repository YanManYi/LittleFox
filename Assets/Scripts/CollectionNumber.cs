using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionNumber : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (gameObject.CompareTag("Cherry")) { ++PlayerController.Instance.cherry; SoundManager.Instance.CherrySound(); }
            else if (gameObject.CompareTag("Gem")) { ++PlayerController.Instance.gem; SoundManager.Instance.CherrySound(); }


            Destroy(gameObject);
        }

    }
}
