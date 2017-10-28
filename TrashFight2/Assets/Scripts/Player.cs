using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public enum ATTACK { NONE, PUNCH, KICK, THROW, DASH}
    public enum MOVE { IDLE, RIGHT, LEFT }

    private ATTACK attackState;
    private MOVE moveState;

    public float moveSpeed;

    public float dashSpeed;
    public float dashDuration;
    private bool dashActive;

    public float punchDamage;
    public float punchDuration;
    public GameObject punchVolume;
    private bool punchActive;

    public float kickDamage;
    public float kickDuration;
    private bool kickActive;
    public GameObject kickVolume;

    public float throwDamage;
    public float throwDuration;
    private bool throwActive;
    public GameObject projectilePrefab;

    private float health;
    public float maxHealth;

    [Range(0,1)]
    public int playerID;

    public Player opponent;
    public Transform centerPoint;

    private float lastAttackTime;

    public int myAbilityVolumeLayer;

    public bool alive = true;
	
    void Awake() {
        health = maxHealth;
    }

    void Start () {
		
	}
	
	void Update () {
        if(alive) {
            CheckAbilities();
            CheckMove();
        }
        CheckDead();
    }
    private void CheckDead() {
        if(!alive) {
            foreach(MeshRenderer rend in transform.GetComponentsInChildren<MeshRenderer>()) {
                rend.enabled = false;
            }
        }
    }
    private void CheckAbilities() {
        //If there's no attack active, we can attack
        if(attackState == ATTACK.NONE) {
            if (playerID == 0) {
                if (Input.GetKey(KeyCode.F)) {
                    Punch();
                } else if (Input.GetKey(KeyCode.G)) {
                    Kick();
                } else if (Input.GetKey(KeyCode.H)) {
                    Throw();
                }

            } else {
                if (Input.GetKey(KeyCode.J)) {
                    Punch();
                } else if (Input.GetKey(KeyCode.K)) {
                    Kick();
                } else if (Input.GetKey(KeyCode.L)) {
                    Throw();
                }
            }

        } else {
        //If there is an attacka active, we need to check its active timer
            switch (attackState) {
                case ATTACK.PUNCH:
                    if(TimerComplete(lastAttackTime, punchDuration)){
                        attackState = ATTACK.NONE;
                    }
                    break;
                case ATTACK.KICK:
                    if (TimerComplete(lastAttackTime, kickDuration)) {
                        attackState = ATTACK.NONE;
                    }
                    break;
                case ATTACK.THROW:
                    if (TimerComplete(lastAttackTime, throwDuration)) {
                        attackState = ATTACK.NONE;
                    }
                    break;
                case ATTACK.DASH:
                    if (TimerComplete(lastAttackTime, dashDuration)) {
                        attackState = ATTACK.NONE;
                    }
                    break;
            }

            if(attackState == ATTACK.NONE) {
                punchVolume.SetActive(false);
                kickVolume.SetActive(false);
            }
        }
    }
    private bool TimerComplete(float _lastTime, float _duration) {
        if (Time.time > _lastTime + _duration) {
            return true;
        }
        return false;
    }
    private void CheckMove() {
        float moveAmount = 0;

        if(playerID == 0) {
            if (Mathf.Abs(Input.GetAxis("Player0Horiz")) > 0) { moveAmount = -moveSpeed * Input.GetAxis("Player0Horiz"); }
        } else {
            if (Mathf.Abs(Input.GetAxis("Player1Horiz")) > 0) { moveAmount = -moveSpeed * Input.GetAxis("Player1Horiz"); }
        }

        if(moveAmount > 0) {
            //Turn right
            transform.localRotation = Quaternion.Euler(new Vector3(0,-90,0));
        } else if(moveAmount < 0) {
            //Turn left
            transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }

        centerPoint.rotation = Quaternion.Euler(new Vector3(0, centerPoint.eulerAngles.y + moveAmount, 0));
    }

    private void Dash() {
        attackState = ATTACK.DASH;
        lastAttackTime = Time.time;
    }
    private void Punch() {
        attackState = ATTACK.PUNCH;
        lastAttackTime = Time.time;
        punchVolume.SetActive(true);
    }
    private void Kick() {
        attackState = ATTACK.KICK;
        lastAttackTime = Time.time;
        kickVolume.SetActive(true);
    }
    private void Throw() {
        attackState = ATTACK.THROW;
        lastAttackTime = Time.time;
    }
    private void TakeDamage(float _damage) {
        health -= _damage;
        if(health <= 0) {
            Die();
        }
    }
    private void Die() {
        print("Player " + playerID  + " died");
        alive = false;
    }
    private void OnTriggerEnter(Collider _other) {
        print("Triggered!!");
        if(_other.gameObject.layer != myAbilityVolumeLayer) {
            switch (_other.tag) {
                case "PunchVolume":
                    TakeDamage(punchDamage);
                    break;
                case "KickVolume":
                    TakeDamage(kickDamage);
                    break;
            }
        }
    }
}
