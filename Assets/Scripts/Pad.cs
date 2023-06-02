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

    [SerializeField] TextMeshPro scoreText;
    [SerializeField] MeshRenderer goalRenderer;

    [SerializeField, ColorUsage(true, true)]
    Color goalColor = Color.white;

    Material padMaterial, goalMaterial, scoreMaterial;

    static readonly int
        emissionColorID = Shader.PropertyToID("_EmissionColor"),
        faceColorID = Shader.PropertyToID("_FaceColor"),
        timeOfLastHitID = Shader.PropertyToID("_TimeOfLastHit");


    int score;
    float extents, targetingBias;


    private void Awake()
    {
        goalMaterial = goalRenderer.material;
        goalMaterial.SetColor(emissionColorID, goalColor);
        padMaterial = GetComponent<MeshRenderer>().material;
        scoreMaterial = scoreText.fontMaterial;
        SetScore(0);
    }

    public void StartNewGame()
    {
        SetScore(0);
        ChangeTargetingBias();
    }

    public bool ScorePoint(int pointsToWin)
    {
        goalMaterial.SetFloat(timeOfLastHitID, Time.time);
        SetScore(score + 1, pointsToWin);
        return score >= pointsToWin;
    }
    void SetScore(int newScore, float pointsToWin = 1000f)
    {
        score = newScore;
        scoreText.SetText("{0}", newScore);
        scoreMaterial.SetColor(faceColorID, goalColor * (newScore / pointsToWin));
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
        bool success = -1f <= hitFactor && hitFactor <= 1f;

        if (success)
        {
            padMaterial.SetFloat(timeOfLastHitID, Time.time);
        }
        return success;
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
