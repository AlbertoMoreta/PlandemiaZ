using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private CharacterController controller;
    private Vector3 direction;
    public float forwardSpeed = 1;
    public float lateralSpeed = 2;
    public float jumpForce = 10;
    public float gravity = 20;

    private int desiredLane = 1;
    private int numLanes = 3;
    public float laneDistance = 4; //Distance between running lanes

    public GameObject sprite;
    public LevelManager level;
    public GameObject dropBleach;

    private float leftPosition;


    const int DIR_LEFT = -1;
    const int DIR_RIGHT = 1;

    private bool alive;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        direction.z = forwardSpeed;
        leftPosition = transform.position.x - laneDistance * Mathf.FloorToInt(numLanes/2);
        alive = true;
    }

    private void Update()
    {
        if (controller.isGrounded)
        {
            
            if (alive && (Input.GetKeyDown(KeyCode.UpArrow) || SwipeController.swipeUp)) { Jump(); }
        }
        else
        {
            direction.y -= gravity * Time.deltaTime;
        }

        if (alive)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || SwipeController.swipeRight) { ChangeLane(DIR_LEFT); }
            if (Input.GetKeyDown(KeyCode.LeftArrow) || SwipeController.swipeLeft) { ChangeLane(DIR_RIGHT); }
            float targetPosition = leftPosition + desiredLane * laneDistance;
            if ((direction.x > 0 && targetPosition < transform.position.x)
                || (direction.x < 0 && targetPosition > transform.position.x))
            {
                direction.x = 0;
                transform.position = new Vector3(targetPosition, transform.position.y, transform.position.z);
            }
        }
        
        
    }

    private void ChangeLane(int dir)
    {
        int targetLane = Mathf.Clamp(desiredLane + dir, 0, numLanes - 1);
        if (targetLane != desiredLane)
        {
            desiredLane = targetLane;
            direction.x = dir * lateralSpeed;
        }
    }

    private void Jump()
    {
        direction.y = jumpForce;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacles")
        {
            if (hit.gameObject.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rigid = hit.gameObject.GetComponent<Rigidbody>();
                Vector3 hitVector = (hit.transform.position - transform.position).normalized;
                rigid.AddForce(hitVector * 1200);
            }
            Die();
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if (!alive) return;

        if (other.gameObject.tag == "Bleach")
        {
            //hit.gameObject.SetActive(false);
            Animator spriteAnimator = other.gameObject.GetComponent<Animator>();
            if (spriteAnimator != null)
            {
                spriteAnimator.SetTrigger("Collected");
            }
            LevelManager.bleach++;
        }

        if (other.gameObject.tag == "GatesStreet")
        {
            direction.z = 0;
            level.toogleUI(false, false);
            animator.SetTrigger("Gates");
        }

        if (other.gameObject.tag == "Laser")
        {
            Animator spriteAnimator = sprite.GetComponent<Animator>();
            if (spriteAnimator != null)
            {
                spriteAnimator.SetTrigger("LaserHit");
                LevelManager.bleach -= 5;
                if (LevelManager.bleach < 0)
                {
                    LevelManager.bleach = 0;
                    Die();
                } else {
                    DropBleach();
                }
            }
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");

        Animator spriteAnimator = sprite.GetComponent<Animator>();
        if (spriteAnimator != null)
        {
            spriteAnimator.SetTrigger("Die");
        }
        level.toogleUI(false, true);

        direction.z = 0;
        direction.x = 0;
        alive = false;
    }

    public void OnDyingAnimationEnd()
    {
        LevelManager.gameEnded = true;
    }

    public void onGatesAnimationEnd()
    {
        direction.z = forwardSpeed;
        level.toogleUI(true, false);
        level.awakeGates();

    }

    public void DropBleach(){ 
        var bleachInstance = Instantiate(dropBleach, sprite.transform.position, Quaternion.identity) as GameObject;
        foreach(Transform bleachBottle in bleachInstance.transform){
            var rigidBody = bleachBottle.gameObject.GetComponent<Rigidbody>();
            rigidBody.velocity = direction;
            var xForce = Random.Range(-1f,1f);
            var zForce = Random.Range(-1f,1f);
            rigidBody.AddForce(xForce, 3.0f, zForce, ForceMode.Impulse);
        }
        Destroy(bleachInstance, 5f);
    }
}
