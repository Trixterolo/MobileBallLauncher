using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
public class BallHandler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] float detachDelayDuration;
    [SerializeField] float respawnDelayDuration;

    private Rigidbody2D currentBallRb2d;
    private SpringJoint2D currentBallSpringJoint;
   
    private Camera mainCamera;
    private bool isDragging;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRb2d == null) return;

        if (Touch.activeTouches.Count == 0)
        {
            if(isDragging)
            {
                LaunchBall();
            }
            isDragging = false;
            return;
        }

        isDragging = true;
        //enables rigidbody kinematic
        currentBallRb2d.isKinematic = true;


        Vector2 touchPosition = new Vector2(); //Right here, it is just (0,0) because of zero touches.
         
        int effectiveTouchCount = 0; //needed to calculate touch points.
        foreach (Touch touch in Touch.activeTouches)
        {
            if (new Rect(1, 1, Screen.width - 1, Screen.height - 1).Contains(touch.screenPosition)) //needed to calculate touch points.
            {
                touchPosition += touch.screenPosition;
                effectiveTouchCount++;
            }
        }
        touchPosition /= effectiveTouchCount; //This get the centrepoints of all the touches.

        ////acquire touch screen position
        //Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        //converts screen to world position
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        //converts currentball position to world position that was pressed.
        currentBallRb2d.position = worldPosition;
        ////print(worldPosition);

    }
    private void SpawnNewBall()
    {
        GameObject newBall = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRb2d = newBall.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = newBall.GetComponent<SpringJoint2D>();

        //attached springjoint to pivot
        currentBallSpringJoint.connectedBody = pivot;


    }
    private void LaunchBall()
    {
        //disable kinematic to enable physics like gravity and spring joint.
        currentBallRb2d.isKinematic = false;
        //clears our ball reference
        currentBallRb2d = null;

        //Legit a great use for string phrases for failsafe with invokes
        Invoke(nameof(DetachBall), detachDelayDuration);

    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelayDuration);

    }


}
