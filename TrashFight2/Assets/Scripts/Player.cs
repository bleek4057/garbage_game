using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public enum ATTACK { NONE, PUNCH, KICK, THROW, DASH}
    public enum MOVE { IDLE, RIGHT, LEFT }

    private ATTACK attackState;
    private MOVE moveState;

    public float moveSpeed;
    public float maxMoveSpeed;
    private float baseMoveSpeed;

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

    public float health;
    public float maxHealth;

    [Range(0,1)]
    public int playerID;

    public Player opponent;
    public Transform centerPoint;

    private float lastAttackTime;

    public int myAbilityVolumeLayer;

    private bool knockingBack;
    private int knockBackDir;

    //+ or -
    public int movementDir;

    public bool alive = true;
    private float startKnockbackTime;
    public float knockbackTime;
    public float knockbackAmount;

    public float runningAwayDampen;

    public ParticleSystem hitParticle;

    public Camera cam;
    private Animator anim;

    //Note from John. This is all bad. Don't do this in a real game

    void Awake() {
        health = maxHealth;
        baseMoveSpeed = moveSpeed;
    }

    void Start () {

	}
	
	void Update () {
        if(alive) {
            if(!CheckKnockback()) {
                CheckAbilities();
                CheckMove();
            }
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
    private bool CheckKnockback() {
        if(knockingBack) {
            if(Time.time < startKnockbackTime + knockbackTime) {
                //also kind of stuns you
                centerPoint.eulerAngles = new Vector3(0, centerPoint.eulerAngles.y + knockBackDir * knockbackAmount, 0);
                return true;
            } else {
                knockingBack = false;
                return false;
            }
        }

        return false;
    }
    private void CheckAbilities() {
        //If there's no attack active, we can attack
        if(attackState == ATTACK.NONE) {
            if (playerID == 0) {
                if (Input.GetKeyDown(KeyCode.F)) {
                    Punch();
                } else if (Input.GetKeyDown(KeyCode.G)) {
                    Kick();
                } else if (Input.GetKeyDown(KeyCode.H)) {
                    Throw();
                }else if(Input.GetKeyDown(KeyCode.Space)) {
                    Dash();
                }

            } else {
                if (Input.GetKey(KeyCode.J)) {
                    Punch();
                } else if (Input.GetKey(KeyCode.K)) {
                    Kick();
                } else if (Input.GetKey(KeyCode.L)) {
                    Throw();
                } else if(Input.GetKeyDown(KeyCode.RightShift)) {
                    Dash();
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
                        moveSpeed = baseMoveSpeed;
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
    private void StartWalk() {
        anim.SetTrigger("Walk");
    }
    private void CheckMove() {
        float moveAmount = 0;

        if(playerID == 0) {
            if (Mathf.Abs(Input.GetAxis("Player0Horiz")) > 0) { moveAmount = -moveSpeed * (Input.GetAxis("Player0Horiz") > 0 ? 1 : -1); }
        } else {
            if (Mathf.Abs(Input.GetAxis("Player1Horiz")) > 0) { moveAmount = -moveSpeed * (Input.GetAxis("Player1Horiz") > 0 ? 1 : -1);  }
        }

        if(moveAmount > 0) {
            //Turn right
            movementDir = -1;
            transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
        } else if(moveAmount < 0) {
            //Turn left
            movementDir = 1;
            transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }

        //If we're moving counter clockwise and the opponent is a head of us, clockwise 
        if(moveAmount < 0 && opponent.transform.parent.eulerAngles.y > transform.parent.eulerAngles.y) {
            moveAmount *= runningAwayDampen;

        //If we're moving clockwise and the oppenent is a head of us counter clockwise
        }else if (moveAmount > 0 && opponent.transform.parent.eulerAngles.y < transform.parent.eulerAngles.y) {
            moveAmount *= runningAwayDampen;
        }

        if (attackState != ATTACK.DASH) {
            if(Mathf.Abs(moveAmount) > maxMoveSpeed){
                moveAmount = moveAmount > 0 ? maxMoveSpeed : -maxMoveSpeed;
            }
        }

        centerPoint.eulerAngles = new Vector3(0, centerPoint.eulerAngles.y + moveAmount, 0);
    }

    private void Dash() {
        attackState = ATTACK.DASH;
        lastAttackTime = Time.time;
        moveSpeed *= dashSpeed;
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
        if(!alive) {
            return;
        }

        cam.GetComponent<CameraShake>().CamShake(0.2f, 0.2f);
        hitParticle.Emit(70);
        Knockback(opponent.movementDir);

        health -= _damage;
        if(health <= 0) {
            Die();
        }
    }
    private void Die() {
        print("Player " + playerID  + " died");
        alive = false;
    }
    private void Knockback(int _dir) {
        knockingBack = true;
        knockBackDir = -_dir;
        startKnockbackTime = Time.time;
    }
    private void OnTriggerEnter(Collider _other) {
        //print("Triggered!!");
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
