using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public enum  PlayerState
    {
        Idle,
        Run,
    }
    
    public PlayerState currentState;
    
    private Animator animator;
    
    [Serializable]
    public struct StateAnimationMapping
    {
        public PlayerState state;
        public AnimationClip animation;
    }
    
    public List<StateAnimationMapping> stateAnimations = new List<StateAnimationMapping>();
    
    private Dictionary<PlayerState, AnimationClip> stateNameDict = new Dictionary<PlayerState, AnimationClip>();
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        stateNameDict =  new Dictionary<PlayerState, AnimationClip>();
        foreach (var mapping in stateAnimations)
        {
            if (!stateNameDict.ContainsKey(mapping.state))
            {
                stateNameDict.Add(mapping.state, mapping.animation);
            }
        }
    }

    private void Start()
    { 
        ChangeState(PlayerState.Idle);
    }

    private void ChangeState(PlayerState newState)
    {
        if(currentState == newState) return;
        
        currentState = newState;
        if(stateNameDict.ContainsKey(currentState))
            animator.Play(stateNameDict[currentState].name);
    }

    public void IdleState()
    {
        ChangeState(PlayerState.Idle);
    }
    
    public Vector3 RunState(Vector2 moveInput)
    {
        if (moveInput.magnitude >= 0.1f)
        {
            ChangeState(PlayerState.Run);
        }
        else
        {
            ChangeState(PlayerState.Idle);
        }
        
        return new Vector3(moveInput.x, 0f, moveInput.y).normalized;
    }
}
