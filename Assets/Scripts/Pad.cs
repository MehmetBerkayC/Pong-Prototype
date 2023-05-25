using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Pad : MonoBehaviour
{
    [SerializeField] bool isAI;

    [SerializeField, Min(0f)]
    float
        minExtents = 4f,
        maxExtents = 4f,
        speed = 10f,
        maxTargetingBias = 0.75f;

    [SerializeField] TextMeshProUGUI scoreText;
    
    int score;
    float extents, targetingBias;

    private void Awake()
    {
        SetScore(0);
    }

    public void StartNewGame()
    {
        SetScore(0);
        ChangeTargetingBias();
    }

    public bool ScorePoint(int pointsToWin)
    {
        SetScore(score + 1, pointsToWin);
        return score >= pointsToWin;
    }
    void SetScore(int newScore, float pointsToWin = 1000f)
    {
        score = newScore;
        scoreText.SetText("{0}", newScore);
        SetExtents(Mathf.Lerp(maxExtents, minExtents, newScore / (pointsToWin - 1f)));
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
        ChangeTargetingBias();
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
        // Make AI differ where it hits the ball
        target += targetingBias * extents;

        if(x < target)
        {
            return Mathf.Min(x + speed * Time.deltaTime, target);
        }
        return Mathf.Max(x - speed * Time.deltaTime, target);
    }

    void ChangeTargetingBias()
    {
        targetingBias = Random.Range(-maxTargetingBias, maxTargetingBias);
    }

    void SetExtents(float newExtents)
    {
        extents = newExtents;
        Vector3 scale = transform.localScale;
        scale.x = 2f * newExtents;
        transform.localScale = scale;
    }
   
}
