using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public enum Animation
    {
        Idle,
        Run,
        Surprise
    }
    
    private Animator _animator;
    public Animator Animator => _animator;
    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void PlayAnimation(Animation animation)
    {
        Animator.Play(animation.ToString());
    }
}
