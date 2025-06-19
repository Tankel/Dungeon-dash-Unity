using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System;
public class PlayerMenu : MonoBehaviour
{
    // Animation states
    Animator animator;
    public const string MENU = "MainMenu";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return;
        }
    }

    string currentAnimationState;
    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

}

