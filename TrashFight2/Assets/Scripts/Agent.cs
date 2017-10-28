using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
    /*Any object in the game that moves or needs to be effected by physics or ablilities must
    have an attatched Agent component. It is used to move objects. That's pretty much it*/


    public bool debugAcceleration, debugVelocity, moveable;

    public bool clampVelocity;
    public bool clampAcceleration;

    public float dragForce;
    public float mass;

    public float maxVelocity;
    public float maxAcceleration;

    private Vector3 velocity;
    private Vector3 drag;
    private Vector3 acceleration;

    private float origYPos;
    public bool lockYPos;
    public bool horizRot;

    /*The actual numbers required to move agents using forces when couple with 
    delta time, are very large, so for the sake of clean, easy to digest numbers, the
    values we input will be small, then will be multiplied by this modifier whenever they are used.*/
    public static float globalForcesModifier = 500;

    /*Is set to true when a player enters a collision. this value is sent to various
    abilities that need to know but cannot detect on their own*/
    private bool enterCollisionFlag;

    #region Properties

    public Vector3 Velocity {
        get {
            return velocity;
        }

        set {
            velocity = value;
        }
    }

    public Vector3 Drag {
        get {
            return drag;
        }

        set {
            drag = value;
        }
    }

    public Vector3 Acceleration {
        get {
            return acceleration;
        }

        set {
            acceleration = value;
        }
    }

    public bool EnterCollisionFlag {
        get {
            return enterCollisionFlag;
        }

        set {
            enterCollisionFlag = value;
        }
    }
    #endregion

    void Awake() {
        dragForce *= globalForcesModifier;
        mass *= globalForcesModifier;

        maxVelocity *= globalForcesModifier;
        maxAcceleration *= globalForcesModifier;
    }
    void Start() {
        origYPos = transform.position.y;
    }

    /*Update method specific to this class. 
    Is called by various Ability Objects manually. Don't forget to do this*/
    public void AgentUpdate() {
        if (!moveable) { return; }

        ShowDebugLines();
        ResetVectors();

        if (clampAcceleration) { ClampAcceleration(); }

        CalcVelocity();

        if (clampVelocity) { ClampVelocity(); }
    }

    /*Handles applying calculated forces to the object's position.
    Should always be called after AgentUpdate. This basically exists
    so that objects can apply forces to themselves after other things have 
    been calculated. It's useful, alright.*/
    public void AgentLateUpdate() {
        if (moveable) {
            AddVelocity();
        }

        GetComponent<Rigidbody>().velocity = Vector3.zero;

        LockYPos();
        LockYRot();

        enterCollisionFlag = false;
    }
    void LockYRot() {
        if (horizRot) transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }
    void LockYPos() {
        if (lockYPos) {
            if (transform.position.y != origYPos) {
                transform.position = new Vector3(transform.position.x, origYPos, transform.position.z);
            }
        }
    }
    void ShowDebugLines() {
        if (debugVelocity) Debug.DrawLine(transform.position, velocity, Color.green);
        if (debugAcceleration) Debug.DrawLine(transform.position, acceleration, Color.yellow);
    }

    //Addes net acceleration forces to velocity and smooths based on framrate
    void CalcVelocity() {
        CalcDrag();
        Velocity += Acceleration * Time.deltaTime;
    }

    //Applies movement to the object
    void AddVelocity() {
        transform.position += Velocity;
    }

    public void ResetVectors() {
        Velocity = Vector3.zero;
        Drag = Vector3.zero;
    }
    void CalcDrag() {
        Acceleration = Acceleration * dragForce;
    }
    void ClampVelocity() {
        if (clampVelocity) Velocity = Vector3.ClampMagnitude(Velocity, maxVelocity);
    }
    void ClampAcceleration() {
        if (clampAcceleration) Acceleration = Vector3.ClampMagnitude(Acceleration, maxAcceleration);
    }

    public void AddRandomForce(float _forceScale) {
        Vector3 randomForce = _forceScale * (new Vector3(Random.Range(0, 1), 0, Random.Range(0, 1)));
        acceleration += randomForce;
    }
    #region Collisions
    void OnCollisionEnter() {
        enterCollisionFlag = true;
    }
    #endregion
}
