using DG.Tweening;
using UnityEngine;

public class AlphaChecker : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _spriteRenderer.DOKill();
        if (other.CompareTag("Player"))
        {
            _spriteRenderer.DOFade(0.5f, 0.2f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _spriteRenderer.DOKill();
        if (other.CompareTag("Player"))
        {
            _spriteRenderer.DOFade(1f, 0.2f);
        }
    }
}
