using System;
using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public int score = 0;
    public TextMeshProUGUI scoreText;
    
    public CinemachineVirtualCamera vcam;
    public bool IsInputLocked { get; private set; } = false;
    
    private Transform player;
    private Coroutine cameraCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        UpdateScoreUI();
    }

    public void ReturnCamera(float delay)
    {
        SetCameraTarget(player, delay);
    }

    public void SetCameraTarget(Transform target, float delay = 0f)
    {
        if (cameraCoroutine != null)
        {
            StopCoroutine(cameraCoroutine);
        }
        
        if (delay <= 0f)
        {
            if (vcam != null) vcam.Follow = target;
            IsInputLocked = (target != player);
        }
        
        else
        {
            cameraCoroutine = StartCoroutine(SwitchCameraRoutine(target, delay));
        }
    }

    private IEnumerator SwitchCameraRoutine(Transform target, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        
        if (vcam != null)
        {
            vcam.Follow = target;
        }
        
        IsInputLocked = (target != player);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddScore(int point)
    {
        score += point;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            if(score == 0) scoreText.text = "Score: 0";
            else
                scoreText.text = "Score: " + score;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
