using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTurn : MonoBehaviour
{
    public Rigidbody player;
    public Transform rotator;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var joystickAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

        if(joystickAxis.x >= .8f){ //-(left) +(right) -1 ~ +1 0.8이면 최대한 오른쪽으로
            player.transform.RotateAround(rotator.position, rotator.up, speed * .1f);
        }
        if(joystickAxis.x <= -.8f){ //almost all the way to the left
            player.transform.RotateAround(rotator.position, rotator.up, speed * -.1f);
        }
    }
}
