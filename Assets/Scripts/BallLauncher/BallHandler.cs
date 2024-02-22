using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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

    // Update is called once per frame
    void Update()
    {
        if (currentBallRb2d == null) return;

        if (!Touchscreen.current.primaryTouch.press.isPressed)
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

        //acquire touch screen position
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

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
