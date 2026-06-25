using System;
using UnityEngine;

public class PlayerKick : MonoBehaviour
{
    public GameObject kickButton;

    public float interactRadius = 1.5f;
    public float kickForce = 10f;
    public LayerMask obstacleMask;

    private GameObject[] goals;

    private void Start()
    {
        if(kickButton != null) kickButton.SetActive(false);
        
        goals = GameObject.FindGameObjectsWithTag("Goal");
    }

    private void Update()
    {
        if (kickButton == null) return;

        bool isNearBall = CheckInRadius();
        
        if(kickButton.activeSelf != isNearBall)
            kickButton.SetActive(isNearBall);
    }

    private bool CheckInRadius()
    {
        Collider[] hitCols = Physics.OverlapSphere(transform.position, interactRadius);

        foreach (var hit in hitCols)
        {
            if(hit.CompareTag("Ball"))
                return true;
        }
        
        return false;
    }

    public void ExecuteKick()
    {
        Rigidbody ballRb = GetNearestBall();
        if (ballRb == null || goals.Length < 2) return;

        Transform targetGoal = GetTargetGoal(ballRb.transform.position);
        
        Vector3 kickDirection = (targetGoal.position - ballRb.transform.position).normalized;
        kickDirection.y = 0.25f;
        
        ballRb.AddForce(kickDirection * kickForce, ForceMode.Impulse);
    }

    private Rigidbody GetNearestBall()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactRadius);
        Rigidbody nearestBall = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Ball"))
            {
                float sqrDistance = (transform.position - hit.transform.position).sqrMagnitude;
                if (sqrDistance < minDistance)
                {
                    minDistance = sqrDistance;
                    nearestBall = hit.GetComponent<Rigidbody>();
                }
            }
        }
        
        return  nearestBall;
    }

    private Transform GetTargetGoal(Vector3 position)
    {
        Transform goal1 = goals[0].transform;
        Transform goal2 = goals[1].transform;
        
        float dist1 = (goal1.position - position).sqrMagnitude;
        float dist2 = (goal2.position - position).sqrMagnitude;
        
        Transform nearestGoal = dist1 < dist2 ? goal1 : goal2;
        Transform furthestGoal = dist1 < dist2 ? goal2 : goal1;
        
        Vector3 directionToNearest = nearestGoal.position - position;
        float distance = directionToNearest.magnitude;
        
        float ballRadius = 0.5f;
        
        if(Physics.SphereCast(position, ballRadius, directionToNearest.normalized, out RaycastHit hit, distance, obstacleMask))
            return furthestGoal;
        
        return nearestGoal;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
