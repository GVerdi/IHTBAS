﻿using UnityEngine;
using System.Collections;

public class FallingBlock : MonoBehaviour {

    private Rigidbody2D rb;
	public float delay;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    IEnumerator startFall()
    {
		yield return new WaitForSeconds(delay);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = 10;
        rb.gravityScale = 0.2f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")// || other.tag == "PhysBlock")
        {
            StartCoroutine(startFall());
        }
    }


}
