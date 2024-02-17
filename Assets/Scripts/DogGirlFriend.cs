using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogGirlFriend : MonoBehaviour
{
    public enum Animation
    {
        Idle,
        Run,
    }
    
    private Animator _animator;

    public Animator Animator
    {
        get
        {
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>(true);
            }

            return _animator;
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void PlayAnimation(Animation animation)
    {
        Animator.Play(animation.ToString());
    }
}
