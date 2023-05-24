using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] Ball ball;

    [SerializeField] Pad bottomPad, topPad;

    [SerializeField, Min(0f)] Vector2 arenaExtents = new Vector2(10f, 10f);

    [SerializeField, Min(2)] int pointsToWin = 3;

    private void Awake()
    {
        StartNewGame();
    }
    private void Update()
    {
        // Pads
        bottomPad.Move(ball.Position.x, arenaExtents.x);
        topPad.Move(ball.Position.x, arenaExtents.x);

        // Ball
        ball.Move();
        BounceYIfNeeded();
        BounceXIfNeeded(ball.Position.x);
        ball.UpdateVisualization();
    }


    private void StartNewGame()
    {
        ball.StartNewGame();
        bottomPad.StartNewGame();
        topPad.StartNewGame();
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

        ball.BounceY(boundary);

        if(defender.HitBall(bounceX, ball.Extents, out float hitFactor))
        {
            ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
        }
        else if (attacker.ScorePoint(pointsToWin))
        {
            StartNewGame();
        }
    }

    void BounceXIfNeeded(float xPosition)
    {
        float xExtents = arenaExtents.x - ball.Extents;
        if(xPosition < -xExtents)
        {
            ball.BounceX(-xExtents);
        }
        else if (xPosition > xExtents)
        {
            ball.BounceX(xExtents);
        }
    }
}
