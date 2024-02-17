using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Animation
    {
        Idle,
        IdleNormalFace,
        Shocked,
        TailShake,
        Run,
        BeginJump,
        EndJump,
        Slide
    }
    
    public event Action OnHpChanged;

    public Rigidbody2D Rigidbody2D => _rb;
    
    private PlayerState     _currentState;
    public PlayerState CurrentState => _currentState;
    private BoxCollider2D   _boxCollider;
    private Rigidbody2D     _rb;
    private SpriteRenderer  _spriteRenderer;
    private Animator        _animator;
    public Animator Animator => _animator;

    [SerializeField] private Animator effectAnimator;

    [SerializeField] private float  jumpForce           = 10.0f;   // 점프 세기
    [SerializeField] private float  invincibleTime      = 2.0f;
    [SerializeField] private int    maxJumpCount        = 2;
                     private int    currentJumpCount    = 0;

    [SerializeField] private LayerMask groundLayer;     // 지면(땅) 레이어
                     private Vector3 footPosition;      // 플레이어의 발 밑 좌표
                     private bool isGrounded;           // 플레이어의 착지 여부

    private float leftInvincibleTime;
    public bool IsInvincible => GameManager.Instance.IsFever || leftInvincibleTime > 0f;

    [SerializeField] private int maxHp = 3;
    
    private int hp;

    public int Hp
    {
        get => hp;
        set
        {
            hp = value;
            hp = Mathf.Min(maxHp, hp);
            hp = Mathf.Max(hp, 0);
            OnHpChanged?.Invoke();
        }
    }
    
    private void Awake()
    {
        _currentState   = PlayerState.Run;
        _boxCollider    = GetComponent<BoxCollider2D>();
        _rb             = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator       = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        currentJumpCount = maxJumpCount;
        Hp = maxHp;
    }

    private void Update()
    {
        if (leftInvincibleTime > 0)
        {
            leftInvincibleTime -= Time.deltaTime;
            _spriteRenderer.color = Color.Lerp(Color.white, Color.red, leftInvincibleTime / invincibleTime);
        }
        else
        {
            leftInvincibleTime = 0f;
            _spriteRenderer.color = Color.white;
        }

        if (GlobalFlowManager.Instance.FlowState != FlowState.Play)
        {
            return;
        }
        
        // bound = 플레이어의 collider 범위
        switch (_currentState)
        {
            case PlayerState.Idle:
                _animator.speed = 1f;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Run();
                }
                break;
            case PlayerState.Run:
            {
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    Idle();
                }
                
                _animator.speed = GameManager.Instance.IsPause ? 0f : 0.1f + GameManager.Instance.MoveSpeed / 5f;
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    Jump();
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    if (_currentState == PlayerState.Run)
                    {
                        Slide();
                    }
                }
            }
                break;
            case PlayerState.Jump:
                _animator.speed = 1f;
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    Jump();
                }
                
                Bounds bound = _boxCollider.bounds;
                footPosition = new Vector2(bound.center.x, bound.min.y);
                
                // 플레이어가 땅 위에 있는지 공중에 있는지 검사 후 결과 반환
                isGrounded = Physics2D.OverlapCircle(footPosition, 0.1f, groundLayer);


                // 플레이어가 바닥에 있을 경우
                if (isGrounded == true && _rb.velocity.y <= 0 && _currentState != PlayerState.Slide)
                {
                    Run();
                    currentJumpCount = maxJumpCount;
                }
                break;
            case PlayerState.Slide:
                if (!GameManager.Instance.IsPause)
                {
                    _animator.speed = GameManager.Instance.IsPause ? 0f : 0.1f + GameManager.Instance.MoveSpeed / 5f;
                    if (Input.GetKeyUp(KeyCode.DownArrow))
                    {
                        Run();
                    }

                    if (GameManager.Instance.MoveSpeed <= 1f)
                    {
                        Idle();
                    }
                }
                break;
            case PlayerState.Hit:
                _animator.speed = 1f;
                break;
            case PlayerState.Dead:
                _animator.speed = 1f;
                break;
        }
    }

    public void ChangeState(PlayerState playerState)
    {
        var beforeState = _currentState;
        _currentState = playerState;
        // Debug.Log($"BeforeState : {beforeState} / CurrentState : {_currentState}");
        
        switch (_currentState)
        {
            case PlayerState.Idle:
                GameManager.Instance.PauseMove();
                _animator.Play("Idle");
                break;
            case PlayerState.Run:
                if (beforeState == PlayerState.Jump)
                {
                    _animator.Play("EndJump");
                    PlayEffectAnimation("EndJump");
                }
                else if (beforeState == PlayerState.Idle)
                {
                    GameManager.Instance.ResumeMove();
                    _animator.Play("Run");
                }
                else
                {
                    _animator.Play("Run");
                }
                break;
            case PlayerState.Jump:
                _animator.Play("BeginJump");
                PlayEffectAnimation("BeginJump");
                break;
            case PlayerState.Slide:
                _animator.Play("Slide");
                break;
            case PlayerState.Hit:
                break;
            case PlayerState.Dead:
                break;
        }
    }
    
    public void Idle()
    {
        ChangeState(PlayerState.Idle);
    }

    public void Run()
    {
        ChangeState(PlayerState.Run);
        _boxCollider.offset = new Vector3(0f, 0.7f);
        _boxCollider.size   = new Vector3(1f, 1.2f);
    }

    public void Jump()
    {
        if (GameManager.Instance.IsPause)
        {
            return;
        }
        
        if(currentJumpCount > 0)
        {
            _rb.velocity = Vector2.up * jumpForce;
            currentJumpCount--;
            SoundManager.PlaySfx(KeySound.Effect_Jump);
        }
        
        ChangeState(PlayerState.Jump);
    }
    
    public void Slide()
    {
        if (GameManager.Instance.IsPause)
        {
            return;
        }
        
        ChangeState(PlayerState.Slide);
        _boxCollider.offset = new Vector3(0f, 0.5f);
        _boxCollider.size   = new Vector3(1.2f, 0.9f);
        SoundManager.PlaySfx(KeySound.Effect_Slide);
    }

    public void TakeDamage(int amount)
    {
        if (IsInvincible)
        {
            return;
        }
        
        Rigidbody2D.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        _spriteRenderer.color = Color.red;
        leftInvincibleTime = invincibleTime;
        Hp -= amount;
        PlayEffectAnimation("Hit");
        SoundManager.PlaySfx(KeySound.Effect_Hurt);
    }

    public void Heal(int amount)
    {
        Rigidbody2D.AddForce(Vector2.up * 4f, ForceMode2D.Impulse);
        _spriteRenderer.color = Color.cyan;
        Hp += amount;
        SoundManager.PlaySfx(KeySound.Effect_Food);
    }
    
    public void PlayAnimation(Animation animation)
    {
        Animator.Play(animation.ToString());
    }

    public void PlayEffectAnimation(string animationName)
    {
        effectAnimator.Play(animationName);
    }
}
