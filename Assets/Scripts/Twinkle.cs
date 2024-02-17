using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0618

public class Twinkle : MonoBehaviour
{
    public LoopType loopType;
    public Image image;

    private void Start()
    {
        image.DOFade(1.0f, 1).SetLoops(-1, loopType);
    }
}
