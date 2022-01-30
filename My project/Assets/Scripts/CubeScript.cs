using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScript : MonoBehaviour
{

    Rigidbody rb;
    float horizontalInput;
    float verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = (Input.GetAxis("Horizontal"));
        verticalInput = (Input.GetAxis("Vertical"));
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(horizontalInput * 3, rb.velocity.y, verticalInput * 3);
    }

}
