using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDestroyerScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gold1") || collision.CompareTag("Gold2"))
        {
            Destroy(collision.gameObject);
        }
    }
}
