using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Character : MonoBehaviour
{

    private Animator controller;
    
    private Rigidbody rb;
    [SerializeField]
    private float speed = 10f;

    private float rotation = 0;
    [SerializeField]
    private float RotationSense = 100;

    private GameObject currentCell;
    [SerializeField]
    private GameObject Axe;

    private int score;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private Slider healthBar;
    private bool usingAxe = false;
    private bool canUpdateHealth = true;

    void Start()
    {
        controller = this.transform.GetChild(0).GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateHealth()
    {
        if (canUpdateHealth)
        {
            healthBar.value -= 0.05f;
            canUpdateHealth = false;
            Invoke("HealthCondition", 2);
        }
    }

    private void HealthCondition()
    {
        canUpdateHealth = true;
    }
    public void UpdateScore(int s)
    {
        score += s;
        scoreText.text = score.ToString();
    }
    private void FixedUpdate()
    {
     
        if (currentCell != null) 
            Move();
        if (Input.GetAxis("Mouse X") != 0)
        {
            Rotate();

        }
        if (Input.GetAxisRaw("Vertical") < 0.1f)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                controller.SetTrigger("Jump");
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                controller.ResetTrigger("Jump");
            }

        }
       
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            usingAxe = true;
            controller.SetTrigger("Pick");
            Axe.GetComponent<CapsuleCollider>().enabled = true;
            Axe.GetComponent<BoxCollider>().enabled = true;
            Invoke("ResetAxe", 1);
        }
        
       
    }

    private void ResetAxe()
    {
        controller.ResetTrigger("Pick");
        Axe.GetComponent<CapsuleCollider>().enabled = false;
        Axe.GetComponent<BoxCollider>().enabled = false;
        usingAxe = false;
    }
    private void Move()
    {
        
        //controller.SetFloat("Move", Input.GetKey(KeyCode.LeftShift) ? Input.GetAxisRaw("Vertical") * 2f : Input.GetAxisRaw("Vertical"));
        controller.SetBool("Walking", Input.GetAxisRaw("Vertical") < 0.1f ? false : true);

        
        var tmpSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2 : speed;


        if (Input.GetAxisRaw("Vertical") > 0.1f)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(JumpForward());
            }
            
            StartCoroutine(LerpToRun(Input.GetKey(KeyCode.LeftShift) ? 1 : 0));
            rb.velocity = Input.GetAxisRaw("Vertical") * this.transform.forward * tmpSpeed;
            //controller.SetFloat("JumpForward", 0);
            
        }
        else
        {
            StopAllCoroutines();
            controller.SetFloat("Move", 0);
            rb.velocity = Vector3.zero;
        }




    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !usingAxe)
        {
            UpdateHealth();
        }
    }

   
    private IEnumerator JumpForward()
    {
        float timer = 0;
        while (controller.GetFloat("JumpForward") < 1f)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / 2;
            controller.SetFloat("JumpForward", Mathf.Lerp(controller.GetFloat("JumpForward"), 1, timer));
        }

        //yield return new WaitForSeconds(0.1f);
        timer = 0;
        while (controller.GetFloat("JumpForward") > 0.1f)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime / 2f;
            controller.SetFloat("JumpForward", Mathf.Lerp(controller.GetFloat("JumpForward"), 0, timer));
        }
    }
    private IEnumerator LerpToRun(int state)
    {

        float move = 0;
        switch (state)
        {
            case 0:
                move = 1f;
                break;

            case 1:
                move = 2f;
                break;
        }

        float timer = 0;
        while (controller.GetFloat("Move") != move)
        {
            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime / 2;

            controller.SetFloat("Move", Mathf.Lerp(controller.GetFloat("Move"), move, timer));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            currentCell = other.gameObject;
        }
        else if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            UpdateScore(10);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            currentCell = other.gameObject;
        }
    }
    private void Rotate()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        rotation = Input.GetAxis("Mouse X") * RotationSense;
        //rotation = ClampAngle(rotation, -270f, 270f);
        this.transform.Rotate(new Vector3(0, rotation, 0));
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
