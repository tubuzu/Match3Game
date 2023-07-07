// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public enum AnimationState
{
    idle,
    explode,
}

public class AnimatedSpriteRenderer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private Sprite[] activeSprites;
    public Sprite[] idleSprites;
    public Sprite[] explodeSprites;

    public float animationTime = 0.15f;
    private int animationFrame;

    public bool loop;
    public AnimationState state;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        loop = true;
        state = AnimationState.idle;
        activeSprites = idleSprites;
        spriteRenderer.enabled = true;
    }

    private void OnDisable()
    {
        spriteRenderer.enabled = false;
    }

    private void Start()
    {
        InvokeRepeating(nameof(NextFrame), animationTime, animationTime);
    }

    public void Explode()
    {
        state = AnimationState.idle;
        activeSprites = explodeSprites;
        loop = false;
    }

    private void NextFrame()
    {
        animationFrame++;

        if (animationFrame >= explodeSprites.Length)
        {
            if (loop)
                animationFrame = 0;
            else animationFrame--;
        }

        switch (this.state)
        {
            case AnimationState.idle:
                if (animationFrame >= 0 && animationFrame < idleSprites.Length)
                {
                    spriteRenderer.sprite = idleSprites[animationFrame];
                }
                break;
            case AnimationState.explode:
                if (animationFrame >= 0 && animationFrame < explodeSprites.Length)
                {
                    spriteRenderer.sprite = explodeSprites[animationFrame];
                }
                break;
        }
    }
}
