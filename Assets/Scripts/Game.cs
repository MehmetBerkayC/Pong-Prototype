using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Game : MonoBehaviour
{
    [SerializeField] LivelyCamera livelyCamera;
    
    [SerializeField] Ball ball;
    [SerializeField] Pad bottomPad, topPad;

    [SerializeField, Min(0f)] Vector2 arenaExtents = new Vector2(10f, 10f);

    [SerializeField, Min(2)] int pointsToWin = 3;

    [SerializeField] TextMeshPro countdownText;
    [SerializeField, Min(1f)] float newGameDelay = 3f;

    float countdownUntilNewGame;

    private void Awake()
    {
        countdownUntilNewGame = newGameDelay;
    }

    private void Update()
    {
        // Pads
        bottomPad.Move(ball.Position.x, arenaExtents.x);
        topPad.Move(ball.Position.x, arenaExtents.x);

        if(countdownUntilNewGame <= 0f)
        {
            UpdateGame();
        }
        else 
        {
            UpdateCountdown();
        }
    }

    private void UpdateGame()
    {
        // Ball
        ball.Move();
        BounceYIfNeeded();
        BounceXIfNeeded(ball.Position.x);
        ball.UpdateVisualization();
    }

    private void UpdateCountdown()
    {
        countdownUntilNewGame -= Time.deltaTime;
        
        if(countdownUntilNewGame <= 0f)
        {
            countdownText.gameObject.SetActive(false);
            StartNewGame();
        }
        else
        {
            float displayValue = Mathf.Ceil(countdownUntilNewGame);
            if(displayValue < newGameDelay)
            {
                countdownText.SetText("{0}", displayValue);
            }
        }
    }

    private void StartNewGame()
    {
        ball.StartNewGame();
        bottomPad.StartNewGame();
        topPad.StartNewGame();
    }

    void EndGame()
    {
        countdownUntilNewGame = newGameDelay;
        countdownText.SetText("Game Over");
        countdownText.gameObject.SetActive(true);
        ball.EndGame();
    }

    void BounceYIfNeeded()
    {
        float yExtents = arenaExtents.y - ball.Extents;
        if(ball.Position.y < -yExtents)
        {
            BounceY(-yExtents, bottomPad, topPad);
        }
        else if (ball.Position.y > yExtents)
        {
            BounceY(yExtents, topPad, bottomPad);
        }
    }

    void BounceY(float boundary, Pad defender, Pad attacker)
    {
        // How long ago the bounce happened
        float durationAfterBounce = (ball.Position.y - boundary) / ball.Velocity.y;
        // Ball's X position when the bounce happened
        float bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;

        BounceXIfNeeded(bounceX);
        // After bounce
        bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;
        livelyCamera.PushXZ(ball.Velocity);
        ball.BounceY(boundary);

        if (defender.HitBall(bounceX, ball.Extents, out float hitFactor))
        {
            ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
        }
        else
        {
            livelyCamera.JostleY();
            if (attacker.ScorePoint(pointsToWin))
            {
                EndGame();
            }
        }
    }

    void BounceXIfNeeded(float xPosition)
    {
        float xExtents = arenaExtents.x - ball.Extents;
        if(xPosition < -xExtents)
        {
            livelyCamera.PushXZ(ball.Velocity);
            ball.BounceX(-xExtents);
        }
        else if (xPosition > xExtents)
        {
            livelyCamera.PushXZ(ball.Velocity);
            ball.BounceX(xExtents);
        }
    }
}
