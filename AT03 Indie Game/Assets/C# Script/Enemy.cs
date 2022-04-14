using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : FiniteStateMachine
{
    public Bounds bounds;
    public float viewRadius;
    public Transform player;
    public EnemyIdleState idleState;
    public EnemyWanderState wanderState;
    public EnemyChaseState chaseState;

    public NavMeshAgent Agent { get; private set; }
    public Transform Target { get; private set; }
    public Animator Anim { get; private set; }
    public AudioSource AudioSource { get; private set; }

    protected override void Awake()
    {
        idleState = new EnemyIdleState(this, idleState);
        wanderState = new EnemyWanderState(this, wanderState);
        chaseState = new EnemyChaseState(this, chaseState);
        entryState = idleState;
        if (TryGetComponent(out NavMeshAgent agent) == true)
        {
            Agent = agent;
        }
        if (TryGetComponent(out AudioSource aSrc) == true)
        {
            AudioSource = aSrc;
        }

        if(transform.GetChild(0).TryGetComponent(out Animator anim) == true)
        {
            Anim = anim;
        }
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

}

public abstract class EnemyBehaviourState : IState
{
    protected Enemy Instance { get; private set; }


    public EnemyBehaviourState(Enemy instance)
    {
        Instance = instance;
    }

    public abstract void OnStateEnter();

    public abstract void OnStateExit();

    public abstract void OnStateUpdate();

    public virtual void DrawStateGizmo() { }

}

[System.Serializable]
public class EnemyIdleState : EnemyBehaviourState
{
    [SerializeField]
    private Vector2 idleTimeRange = new Vector2(3, 10);
    [SerializeField]
    private AudioClip idleClip;
    private float timer = -1;
    private float idleTime = 0;
    

    public EnemyIdleState(Enemy instance, EnemyIdleState idle) : base(instance)
    {
        idleTimeRange = idle.idleTimeRange;
        idleClip = idle.idleClip;
    }

    public override void OnStateEnter()
    {
        Instance.Agent.isStopped = true;
        idleTime = Random.Range(idleTimeRange.x, idleTimeRange.y);
        timer = 0;
        Instance.Anim.SetBool("isMoving", false);
        Debug.Log("Idle State entered, waiting for " + idleTime + " seconds.");
        Instance.AudioSource.PlayOneShot(idleClip);

    }
    public override void OnStateExit()
    {
        timer = -1;
        idleTime = 0;
        Debug.Log("Exiting the idle state.");
    }

    public override void OnStateUpdate()
    {
        if (Vector3.Distance(Instance.transform.position, Instance.player.position) <= Instance.viewRadius)
        {
            Instance.SetState(Instance.chaseState);
        }

        if (timer >= 0)
        {
            timer += Time.deltaTime;
            if (timer >= idleTime)
            {
                Debug.Log("Exiting Idle State after " + idleTime + " seconds.");
                Instance.SetState(Instance.wanderState);
            }
        }
    }
}
[System.Serializable]
public class EnemyWanderState : EnemyBehaviourState
{
    [SerializeField]
    private Vector3 targetPosition;
    private float wanderSpeed = 2.5f;
    [SerializeField]
    private AudioClip wanderClip;


    public EnemyWanderState(Enemy instance, EnemyWanderState wander) : base(instance)
    { 
    }

    public override void OnStateEnter()
    {

        Instance.Agent.speed = wanderSpeed;
        Instance.Agent.isStopped = false;
        Vector3 randomPosInBounds = new Vector3
            (
            Random.Range(-Instance.bounds.extents.x, Instance.bounds.extents.x),
            Instance.transform.position.y,
            Random.Range(-Instance.bounds.extents.z, Instance.bounds.extents.z)
            );
        targetPosition = randomPosInBounds;
        Instance.Agent.SetDestination(targetPosition);
        Instance.Anim.SetBool("isMoving", true);
        Instance.Anim.SetBool("isChasing", false);

        Debug.Log("Wander State Entered with a target pos of " + targetPosition);
    }

    public override void OnStateExit()
    {
        Debug.Log("Wander state exited. ");
    }

    public override void OnStateUpdate()
    {
        Vector3 t = targetPosition;
        t.y = 0;
        if (Vector3.Distance(Instance.transform.position, targetPosition) <= Instance.Agent.stoppingDistance)
        {
            Instance.SetState(Instance.idleState);
        }

        if (Vector3.Distance(Instance.transform.position, Instance.player.position) <= Instance.viewRadius)
        {
            Instance.SetState(Instance.chaseState);
        }



    }

    public override void DrawStateGizmo()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(targetPosition, 0.5f);
    }





}
[System.Serializable]
public class EnemyChaseState : EnemyBehaviourState
{
    [SerializeField]
    private float chasespeed = 2.5f;
    [SerializeField]
    private AudioClip chaseClip;

    public EnemyChaseState(Enemy instance, EnemyChaseState chase) : base(instance)
    { }
    

    public override void OnStateEnter()
    {
        Instance.Agent.isStopped = false;
        Instance.Agent.speed = chasespeed;
        Instance.Anim.SetBool("isMoving", true);
        Instance.Anim.SetBool("isChasing", true);
        Instance.AudioSource.PlayOneShot(chaseClip);
    }
    public override void OnStateExit()
    {
        Debug.Log("Exited chase state");
    }
    public override void OnStateUpdate()
    {
        if (Instance.Target != null)
        {
            Instance.Agent.SetDestination(Instance.Target.position);
        }
        else
        { 
            Instance.SetState(Instance.wanderState);
        }

        if (Vector3.Distance(Instance.transform.position, Instance.player.position) > Instance.viewRadius)
        {
            Instance.SetState(Instance.wanderState);
        }

    }

}