using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Pad : MonoBehaviour
{
    [SerializeField] bool isAI;

    [SerializeField, Min(0f)]
    float
        extents = 4f,
        speed = 10f;

    [SerializeField] TextMeshPro scoreText;
    int score;

    public void StartNewGame()
    {
        SetScore(0);
    }

    public bool ScorePoint(int pointsToWin)
    {
        SetScore(score + 1);
        return score >= pointsToWin;
    }

    public void Move(float target, float arenaExtents)
    {
        Vector3 position = transform.localPosition;
        
        // Input by Player or AI
        position.x = isAI ? AdjustByAI(position.x, target) : AdjustByPlayer(position.x);
        
        float limit = arenaExtents - extents;
        position.x = Mathf.Clamp(position.x, -limit, limit);
        transform.localPosition = position;
    }

    public bool HitBall(float ballX, float ballExtents, out float hitFactor)
    {
        hitFactor = (ballX - transform.localPosition.x) / (extents + ballExtents);
        return -1f <= hitFactor && hitFactor <= 1f;
    }

    float AdjustByPlayer(float x)
    {
        bool goRight = Input.GetKey(KeyCode.D);
        bool goLeft = Input.GetKey(KeyCode.A);

        if (goLeft && !goRight)
        {
            return x - speed * Time.deltaTime;
        }
        else if (!goLeft && goRight)
        {
            return x + speed * Time.deltaTime;
        }
        return x;
    }

    float AdjustByAI(float x, float target)
    {
        if(x < target)
        {
            return Mathf.Min(x + speed * Time.deltaTime, target);
        }
        return Mathf.Max(x - speed * Time.deltaTime, target);
    }

    void SetScore(int newScore)
    {
        score = newScore;
        scoreText.SetText("{0}", newScore);
    }
}
