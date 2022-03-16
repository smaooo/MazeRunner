using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    private Animator controller;
    
    void Start()
    {
        controller = this.transform.GetChild(0).GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();   
        if (Input.GetKeyDown(KeyCode.Space))
        {
            controller.SetTrigger("Jump");
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            controller.ResetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            controller.SetTrigger("Pick");
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            controller.ResetTrigger("Pick");
        }
    }

    private void Move()
    {


        controller.SetFloat("Move", Input.GetAxisRaw("Vertical"));
        controller.SetBool("Walking", Input.GetAxisRaw("Vertical") < 0.1f ? false : true);
        
        

    }
}
