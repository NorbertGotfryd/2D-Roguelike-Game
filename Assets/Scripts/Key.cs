using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().hasKey = true;
            UI.instance.ToggleKeyIcon(true);
            Destroy(gameObject);
        }
    }
}
