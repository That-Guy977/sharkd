using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

class GawrBehaviour : MonoBehaviour {
    public LevelInfoProvider level;

    [Header("Parameters")]
    public float jumpDelay;
    public float trackReactRange;
    public float trackReactBuffer;
    public float trackEvadeRange;
    public float trackEvadeBuffer;
    public float trackActionBuffer;
    public float trackActionDelay;
    public float trackJumpChance;
    public float trackDashChance;
    public float trackEndRange;
    public float attackDashDistance;
    public float attackDashBuffer;
    public float attackDashChance;
    public float attackJumpChance;
    public float retreatBuffer;
    public float retreatFarBuffer;
    public float retreatFarChance;
    public float retreatHardBuffer;
    public float retreatDuration;
    public float retreatAttackChance;
    public float crossRange;
    public float recoverSpeedFactor;

    GawrController controller;
    Entity entity;
    PlayerController player;

    private Coroutine loop;
    private bool stun;

    float playerDistance => Mathf.Abs(entity.position.x - player.entity.position.x);
    float playerVerticalDistance => player.transform.position.y - transform.position.y;
    Direction towardsPlayer => entity.Towards(player);
    Direction fromPlayer => towardsPlayer.Reverse();

    void Awake() {
        controller = GetComponent<GawrController>();
        entity = GetComponent<Entity>();
    }

    void Start() {
        player = GameManager.instance.player;
    }

    void Update() {
        if (controller.currentState == PlayerState.Stun) {
            stun = true;
        }
        if (loop != null && !controller.active) {
            StopCoroutine(loop);
            loop = null;
        } else if (loop == null && controller.active) {
            loop = StartCoroutine(BehaviourLoop());
        }
    }

    private IEnumerator BehaviourLoop() {
        if (stun) {
            yield return Recover();
            stun = false;
        }
        while (true) {
            yield return Track();
            yield return Attack();
            yield return Retreat();
        }
    }

    private IEnumerator Track() {
        float distance = playerDistance;
        bool attackDash = distance >= attackDashDistance && Random.value <= attackDashChance;
        while (playerDistance > trackEndRange) {
            controller.Move(towardsPlayer);
            if (playerDistance > trackActionBuffer) {
                bool jump = controller.grounded && Random.value <= trackJumpChance;
                bool dash = Random.value <= trackDashChance;
                if (jump) {
                    controller.Jump();
                    yield return new WaitForSeconds(jumpDelay);
                }
                if (dash) {
                    yield return controller.Dash(towardsPlayer.AsVector());
                }
                if (jump) {
                    yield return new WaitUntil(() => controller.grounded);
                }
                yield return new WaitForSeconds(trackActionDelay);
            } else if (controller.grounded && (
                playerDistance <= trackReactRange && (playerVerticalDistance > trackReactBuffer || player.currentState == PlayerState.Attack)
                || playerDistance > trackEvadeRange && Mathf.Abs(playerVerticalDistance) <= trackEvadeBuffer && player.currentState == PlayerState.Attack
            )) {
                controller.Jump();
                yield return new WaitForSeconds(jumpDelay);
            }
            if (attackDash && playerDistance < controller.dashDistance + attackDashBuffer) {
                if (controller.grounded && Random.value < attackJumpChance) {
                    controller.Jump();
                    yield return new WaitForSeconds(jumpDelay);
                }
                yield return controller.Dash(towardsPlayer.AsVector());
                attackDash = false;
            }
            yield return null;
        }
    }

    private IEnumerator Attack() {
        int moveDir = Math.Sign(player.velocity.x);
        if (moveDir != 0) {
            controller.Move((Direction)moveDir);
        } else {
            controller.Stop(towardsPlayer);
        }
        yield return controller.Attack(towardsPlayer);
    }

    private IEnumerator Retreat() {
        bool far = Random.value <= retreatFarChance;
        bool firstReach = false;
        float elapsedTime = 0;
        do {
            elapsedTime += Time.deltaTime;
            float target = player.entity.position.x + fromPlayer.Value() * (far ? retreatFarBuffer : retreatBuffer);
            if (!level.InBounds(target)) {
                yield return CrossOver();
            }
            if (playerDistance <= retreatHardBuffer) {
                yield return controller.Dash(fromPlayer.AsVector());
            } else if (Mathf.Abs(entity.position.x - target) > 0.1f) {
                controller.Move(entity.Towards(target));
            } else {
                controller.Stop(towardsPlayer);
                if (!firstReach) {
                    if (Random.value <= retreatAttackChance) {
                        yield return controller.Attack(entity.Towards(player));
                    }
                    firstReach = true;
                }
            }
            yield return null;
        } while (elapsedTime < retreatDuration);
    }

    private IEnumerator CrossOver() {
        Direction dir = towardsPlayer;
        while (playerDistance > crossRange) {
            controller.Move(dir);
            yield return null;
        }
        yield return controller.Dash(dir.AsVector() + Vector2.up);
    }

    private IEnumerator Recover() {
        controller.Move(fromPlayer, recoverSpeedFactor);
        if (controller.grounded) {
            controller.Jump();
        }
        yield return controller.Attack(towardsPlayer);
    }
}
