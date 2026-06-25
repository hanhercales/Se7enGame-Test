using System;
using System.Collections;
using UnityEngine;

public class GoalCheck : MonoBehaviour
{
    public ParticleSystem confettiParticle;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            other.tag = "ScoredBall";
            
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity *= 0.2f;
            
            StartCoroutine(ProcessGoal());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ScoredBall"))
        {
            other.tag = "Ball";
        }
    }

    private IEnumerator ProcessGoal()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (confettiParticle != null)
        {
            confettiParticle.Play();
        }
        
        GameManager.Instance.AddScore(1);
        GameManager.Instance.ReturnCamera(1.5f);
    }
}
