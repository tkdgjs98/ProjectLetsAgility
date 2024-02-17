using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private int damage = 1; // 장애물의 데미지
    public float Damage => damage;
    [SerializeField] private bool isQuickTimeEvent = false; // 특수한 장애물 설정 유무
    [SerializeField] private bool isCrashable = true;

    private GameManager gameManager;
    [SerializeField] private Transform modelTransform;

    private bool _isCrash = false;
    private Vector3 _moveDir;
    private float crashMoveSpeed = 0f;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        if (isQuickTimeEvent)
        {
            for (int i = 0; i < modelTransform.childCount; i++)
            {
                modelTransform.GetChild(i).gameObject.SetActive(false);
            }
            var targetModel = modelTransform.GetChild(Random.Range(0, modelTransform.childCount));
            targetModel.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (!isCrashable)
        {
            return;
        }

        if (!_isCrash)
        {
            return;
        }

        modelTransform.Rotate(new Vector3(0f, 0f, -crashMoveSpeed * 100f * Time.deltaTime));
        transform.Translate(_moveDir * (crashMoveSpeed * 2f * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어와 충돌 시
        if (collision.CompareTag("Player"))
        {
            collision.TryGetComponent(out Player player);
            if (!player.IsInvincible)
            {
                // 특수한(QTE) 장애물과 충돌 시
                if (isQuickTimeEvent)
                {
                    // 화면이 멈춘 것 처럼 연출 -> 미니게임 위임
                    gameManager.ShowMiniGame(this);
                }
                else
                {
                    Crash();
                }
            }
            else
            {
                Crash();
            }
        }
    }

    public void Crash(float moveSpeed = 0f)
    {
        Debug.Log("Crash");
        if (_isCrash)
        {
            return;
        }

        _isCrash = true;

        if (isCrashable)
        {
            _moveDir = new Vector3(Random.Range(0.5f, 1f), Random.Range(0.2f, 1f), 0f).normalized;
            crashMoveSpeed = moveSpeed == 0f ? gameManager.MoveSpeed : moveSpeed;
            gameManager.TakeDamage(damage);
            Destroy(gameObject, 5f);
        }
        else
        {
            gameManager.TakeDamage(damage);
        }
        SoundManager.PlaySfx(KeySound.Effect_Hit);
    }
}