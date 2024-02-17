using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.EndFever();
            GameManager.Instance.Player.Animator.speed = 1f;
            
            
            GameManager.Instance.GameClear();
        }
    }
}
