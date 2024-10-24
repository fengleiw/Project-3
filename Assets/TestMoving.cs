using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoving : MonoBehaviour
{
    Rigidbody2D rb;
    float speed = 5;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.instance.transform.position, Time.deltaTime * speed));
    }
}
