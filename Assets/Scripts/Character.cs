using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using MazeObjects;

public class Character : MonoBehaviour
{

    private Animator controller;
    
    private Rigidbody rb;
    [SerializeField]
    private float speed = 10f;

    private float rotation = 0;
    [SerializeField]
    private float RotationSense = 100;

    public Cell currentCell;
    [SerializeField]
    private GameObject Axe;

    private int score;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private Slider healthBar;
    private bool usingAxe = false;
    private bool canUpdateHealth = true;
    private bool isGrounded = true;

    
    void Start()
    {
        controller = this.transform.GetChild(0).GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        //print(currentCell);
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
        //if (Input.GetAxisRaw("Vertical") < 0.1f)
        //{

        //    if (Input.GetKeyDown(KeyCode.Space))
        //    {
        //        controller.SetTrigger("Jump");
        //    }
        //    if (Input.GetKeyUp(KeyCode.Space))
        //    {
        //        controller.ResetTrigger("Jump");
        //    }

        //}
       
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

        //print(isGrounded);
        if (Input.GetAxisRaw("Vertical") > 0.1f && isGrounded)
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    var jumpAx = Quaternion.Euler(0, 0, 45f) * this.transform.up;
            //    rb.AddForce(Quaternion.Euler(0,0,45) * Vector3.up * 200, ForceMode.Impulse);
            //    isGrounded = false;
            //    StartCoroutine(JumpForward());
            //}
            
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

    //private bool isGrounded()
    //{
    //    var sp = this.GetComponent<SphereCollider>();
    //    return Physics.CheckCapsule(this.transform.position, new Vector3(sp.bounds.center.x,sp.bounds.min.y,sp.bounds.center.z),
    //        sp.radius, LayerMask.GetMask("Cell"));
    //}


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !usingAxe)
        {
            UpdateHealth();
        }

        else if (collision.gameObject.CompareTag("Cell"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cell"))
        {
            isGrounded = true;
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
            currentCell = other.GetComponent<CellProp>().properties.cell;
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
            currentCell = other.GetComponent<CellProp>().properties.cell;
        }
    }
    private void Rotate()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rotation = Input.GetAxis("Mouse X") * RotationSense;
        //rotation = ClampAngle(rotation, -270f, 270f);
        this.transform.Rotate(new Vector3(0, rotation, 0));
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
