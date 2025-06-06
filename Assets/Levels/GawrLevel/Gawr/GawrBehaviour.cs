using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GawrBehaviour : MonoBehaviour {
    [Header("Parameters")]
    public float bufferRange;
    // public float hardBuffer;
    // public float actionRange;
    // public float hitRange;
    public float targetRange;
    public float targetRejectRange;

    [Header("Anchors")]
    public List<Transform> targets;

    [Header("Weights")]
    public float targetFront;
    public float targetBack;

    GawrController controller;
    Entity entity;
    PlayerController player;

    private Coroutine loop;
    private Transform target;

    void Awake() {
        controller = GetComponent<GawrController>();
        entity = GetComponent<Entity>();
    }

    void Start() {
        player = GameManager.instance.player;
    }

    void Update() {
        if (loop != null && !controller.active) {
            StopCoroutine(loop);
            loop = null;
        } else if (loop == null && controller.active) {
            loop = StartCoroutine(BehaviourLoop());
        }
    }

    private IEnumerator BehaviourLoop() {
        while (true) {
            target = transform.position.Farthest(targets);
            while (Entity.Distance(entity, player) > bufferRange) {
                controller.Move(entity.Towards(player).Value());
                yield return null;
            }
            if (controller.canAttack) {
                controller.Attack();
            }
            if (
                Entity.Distance(target, player) <= targetRejectRange
                && entity.Towards(player) == entity.Towards(target)
                && Entity.Distance(entity, player) <= Entity.Distance(entity, target)
            ) {
                target = target.position.Farthest(targets);
            }
            while (Entity.Distance(entity, target) > targetRange) {
                controller.Move(entity.Towards(target).Value());
                yield return null;
            }
        }
    }
}
