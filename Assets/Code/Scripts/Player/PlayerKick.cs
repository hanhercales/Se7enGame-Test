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
        
        if (GameManager.Instance != null && GameManager.Instance.IsInputLocked)
        {
            kickButton.SetActive(false);
            return;
        }

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
        if (GameManager.Instance != null && GameManager.Instance.IsInputLocked) return;
        
        Rigidbody targetBall = GetNearestBall();
        PerformKick(targetBall);
    }

    public void ExecuteAutoKick()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsInputLocked) return;
        
        Rigidbody furthestBall = GetFurthestBall();
        PerformKick(furthestBall);
    }

    private void PerformKick(Rigidbody targetBall)
    {
        if (targetBall == null || goals == null || goals.Length < 2) return;

        Transform targetGoal = GetTargetGoal(targetBall.position);

        Vector3 kickDirection = (targetGoal.position - targetBall.position).normalized;
        kickDirection.y = 0.25f;

        targetBall.linearVelocity = Vector3.zero; 
        targetBall.AddForce(kickDirection * kickForce, ForceMode.Impulse);
        
        GameManager.Instance.SetCameraTarget(targetBall.transform, 0f);
        GameManager.Instance.ReturnCamera(5f);
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
    
    private Rigidbody GetFurthestBall()
    {
        GameObject[] allBalls = GameObject.FindGameObjectsWithTag("Ball");
        Rigidbody furthestBallRb = null;
        float maxSqrDistance = -1f;

        foreach (GameObject ballObj in allBalls)
        {
            float sqrDistance = (ballObj.transform.position - transform.position).sqrMagnitude;
            if (sqrDistance > maxSqrDistance)
            {
                maxSqrDistance = sqrDistance;
                furthestBallRb = ballObj.GetComponent<Rigidbody>();
            }
        }
        return furthestBallRb;
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
