using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class MonsterController : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;

    public Vector3 initialTargetPosition;
    public Vector3 targetPosition;
    public bool isAttacking = false;

    private float attackRange = 15f;
    private LayerMask enemyLayer;
    private Transform playerTransform;
    private NavMeshAgent agent;
    private int avoidanceDirection = 0;

    private GameObject targetPosSphere;

    /// <summary>
    /// Added IK support for two handed weapons. Prerequisites: Animator with IK pass enabled.
    /// Weapons is well placed in the right hand and well placed in the left hand.
    /// Then add IK so that the left hand follows the movement of the weapon but not the rotation.
    /// </summary>
    public bool ikActive = false;

    /// <summary>
    /// Object for the left hand to grab onto.
    /// </summary>
    public Transform leftHandleObj = null;
    private Animator animator;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        initialTargetPosition = playerTransform.position;
        targetPosition = playerTransform.position;
        enemyLayer = LayerMask.GetMask("Enemy");
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        agent.avoidancePriority = Random.Range(1, 99);
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.autoBraking = true;
        agent.stoppingDistance = attackRange;

        // Spawn targetPos sphere (Debug)
        targetPosSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        targetPosSphere.GetComponent<Collider>().enabled = false;
        targetPosSphere.transform.position = targetPosition;
        targetPosSphere.transform.localScale = Vector3.one * 0.5f;
        targetPosSphere.GetComponent<Renderer>().material.color = Color.green;

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        RaycastHit hit;
        var avoidanceDistance = attackRange * 1.5f;
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * avoidanceDistance, Color.blue);

        // Check for monsters in front of the monster
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, avoidanceDistance, enemyLayer))
        {
            isAttacking = true;
            // Generate a random number between -1 and 1 if avoidanceDirection is not set
            if (avoidanceDirection == 0)
            {
                // Pick left or right
                avoidanceDirection = Random.value < 0.5f ? -1 : 1;
                Debug.Log("Avoidance direction: " + avoidanceDirection);
            }

            // Make the monster move to the side of another monster that is in front of it
            var offsetMultiplier = 5f;
            Vector3 newDestination = avoidanceDirection * offsetMultiplier * hit.transform.right;
            agent.SetDestination(newDestination);

            // Debugging target position sphere
            targetPosSphere.transform.position = newDestination;
        } else
        {
            if (!isAttacking)
            {
                // Apply an offset to the player based on the attack range
                agent.SetDestination(playerTransform.position);
                targetPosSphere.transform.position = playerTransform.position;

                // Rotate to face the target position
                transform.LookAt(agent.destination);
            }
        }

        // Go to initial target position
        if ((transform.position - agent.destination).sqrMagnitude <= (attackRange * attackRange))
        {
            isAttacking = true;
            agent.isStopped = true;
            transform.LookAt(playerTransform);
        }
    }

    private void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal.
            if (ikActive)
            {

                // Set the right hand target position and rotation, if one has been assigned
                if (leftHandleObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    // TODO: Fix the hand position in the avatar. AvatarIKGoal.LeftHand seems to be offset because the hand's position is at the wrist in the avatar's config..
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandleObj.position);
                }
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
