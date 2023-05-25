using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
        maxXSpeed = 20f,
        maxStartXSpeed = 2f,
        constantYSpeed = 5f,
        ballExtents = 0.5f; // Ball's edge

    Vector2 position, velocity;
    
    // Ball will move using Game's update make it accessible
    public void UpdateVisualization() => transform.localPosition = new Vector3(position.x, 0f, position.y);
    public void Move() => position += velocity * Time.deltaTime;
    public float Extents => ballExtents;
    public Vector2 Position => position;
    public Vector2 Velocity => velocity;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void StartNewGame()
    {
        position = Vector3.zero;
        UpdateVisualization();
        velocity.x = Random.Range(-maxStartXSpeed, maxStartXSpeed);
        velocity.y = -constantYSpeed;
        gameObject.SetActive(true);
    }

    public void EndGame()
    {
        position.x = 0f;
        gameObject.SetActive(false);
    }

    public void SetXPositionAndSpeed(float start, float speedFactor, float deltaTime)
    {
        velocity.x = maxXSpeed * speedFactor;
        position.x = start + velocity.x * deltaTime;
    }

    public void BounceX(float boundary)
    {
        position.x = 2f * boundary - position.x;
        velocity.x = -velocity.x;
    }

    public void BounceY(float boundary)
    {
        position.y = 2f * boundary - position.y;
        velocity.y = -velocity.y;
    }
}
