using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public int difficulty = 5;

    private bool isPause = false;
    private bool isMiniGame = false;
    public bool IsMiniGame => isMiniGame;
    public bool IsPause
    {
        get => isPause;

        set => isPause = value;
    }

    private bool _isFever;

    public bool IsFever
    {
        get => _isFever;
        set => _isFever = value;
    }

    [SerializeField] private float beginMoveSpeed = 5f;
    private float _moveSpeed = 5f;
    public float MoveSpeed => _moveSpeed;
    
    private float _boostGaugeChargeAmount;
    public float BoostGaugeChargeAmount => _boostGaugeChargeAmount;

    private float _feverBoostGauge;
    public float FeverBoostGauge => _feverBoostGauge;

    [SerializeField] private ParticleSystem feverParticle;

    private ConfigDataObject ConfigDataObject => ConfigDataObject.Instance;

    private Player _player;

    public Player Player
    {
        get
        {
            if (_player == null)
            {
                _player = FindObjectOfType<Player>(true);
            }

            return _player;
        }
    }
    private Map _map;

    public Map Map
    {
        get
        {
            if (_map == null)
            {
                _map = FindObjectOfType<Map>(true);
            }

            return _map;
        }
    }

    private Person _person;

    public Person Person
    {
        get
        {
            if (_person == null)
            {
                _person = FindObjectOfType<Person>(true);
            }

            return _person;
        }
    }

    private DogGirlFriend _dogGirlFriend;

    public DogGirlFriend DogGirlFriend
    {
        get
        {
            if (_dogGirlFriend == null)
            {
                _dogGirlFriend = FindObjectOfType<DogGirlFriend>(true);
            }

            return _dogGirlFriend;
        }
    }

    [SerializeField] private TextMeshProUGUI textMoveSpeed;
    [SerializeField] private ParticleSystem fireworksParticleSystem;
    
    private UIManager _uiManager;
    public UIManager UIManager
    {
        get
        {
            if (_uiManager == null)
            {
                _uiManager = FindObjectOfType<UIManager>();
            }
            return _uiManager;
        }
    }

    public List<KeyType> list = new List<KeyType>();
    private int cnt = 0;
    public int count => cnt;

    private float timeLimit = 5.0f;
    public float TimeLimit => timeLimit;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }
    
    private void Start()
    {
        Map.OnChangeMapCount += OnChangeMapCount;
    }

    private void OnChangeMapCount(int mapCount)
    {
    }

    private void Update()
    {
        if (GlobalFlowManager.Instance.FlowState != FlowState.Play)
        {
            return;
        }
        
        if (isPause)
        {
            return;
        }

        var dt = Time.deltaTime;

        if (Input.GetKey(KeyCode.DownArrow))
        {
            _boostGaugeChargeAmount -= ConfigDataObject.subMoveSpeedPerSec * dt;
        }
        
        if (IsFever)
        {
            if (_feverBoostGauge > 0f)
            {
                _feverBoostGauge -= ConfigDataObject.lostFeverGaugePerSec * dt;
                
            }
            else
            {
                _feverBoostGauge = 0f;
                _boostGaugeChargeAmount = 0f;
                EndFever();
            }
            _moveSpeed = ConfigDataObject.maxFeverMoveSpeed;
        }
        else
        {
            if (Player.CurrentState == PlayerState.Run)
            {
                _boostGaugeChargeAmount += ConfigDataObject.addMoveSpeedPerSec * dt;
                _boostGaugeChargeAmount =
                    Mathf.Min(_boostGaugeChargeAmount, ConfigDataObject.maxMoveSpeed - beginMoveSpeed);
            }
            else if (Player.CurrentState == PlayerState.Idle)
            {
                _boostGaugeChargeAmount = 0f;
            }

            _boostGaugeChargeAmount = Mathf.Max(0f, _boostGaugeChargeAmount);
            if (_feverBoostGauge < ConfigDataObject.feverActiveAmount)
            {
                _feverBoostGauge += _boostGaugeChargeAmount * dt;
            }
            else
            {
                BeginFever();
                _feverBoostGauge = ConfigDataObject.feverActiveAmount;
            }
            
            _moveSpeed = beginMoveSpeed + _boostGaugeChargeAmount;
            _moveSpeed = Mathf.Max(_moveSpeed, beginMoveSpeed);
            _moveSpeed = Mathf.Min(_moveSpeed, ConfigDataObject.maxMoveSpeed);
        }
        if (textMoveSpeed != null)
        {
            textMoveSpeed.text = $"{_moveSpeed:0.0} km/h";
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"TakeDamage : {damage}");
        if (IsFever)
        {
            return;
        }
        
        _boostGaugeChargeAmount -= ConfigDataObject.contactLostBoostGauge;
        Player.TakeDamage(damage);
    }

    public void Heal(int amount)
    {
        Debug.Log("Heal");
        Player.Heal(amount);
        _boostGaugeChargeAmount += ConfigDataObject.addMoveSpeedPerSec;
    }

    public void ShowMiniGame(Obstacle obstacle)
    {
        StartCoroutine(CoQuickTimeEventProcess(obstacle));
    }
    
    private IEnumerator CoQuickTimeEventProcess(Obstacle obstacle)
    {
        isPause = true;
        isMiniGame = true;
        
        // 미니게임 성공/실패 결과
        bool isSuccess = false;
        timeLimit = 5.0f;
        cnt = 0;

        PauseMove();
        Player.ChangeState(PlayerState.Run);

        // 미니게임 랜덤 방향키 설정
        for(int i = 0; i < difficulty; i++)
        {
            KeyType key = (KeyType) Random.Range(0, 4);
            list.Add(key);
        }

        while (true)
        {
            KeyType key = KeyType.None;
            timeLimit -= Time.deltaTime;
            UIManager.OnMiniGame();

            if (Input.GetKeyDown(KeyCode.UpArrow))     key = KeyType.Up;
            else if (Input.GetKeyDown(KeyCode.RightArrow))  key = KeyType.Right;
            else if (Input.GetKeyDown(KeyCode.DownArrow))   key = KeyType.Down;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))   key = KeyType.Left;

            if (key != KeyType.None)
            {
                SoundManager.PlaySfx(KeySound.Effect_Click);
            }

            if(key != KeyType.None)
            {
                if (key == list[cnt])
                {
                    cnt++;
                }
                else
                {
                    // 미니게임 실패 Key 잘못 입력
                    isSuccess = false;
                    break;
                }
            }

            // 미니게임 성공 시
            if (cnt == list.Count)
            {
                // 장애물 제거 or 파괴
                Debug.Log("Success");
                isSuccess = true;
                break;
            }

            // 미니게임 실패 시 (시간 제한 초과)
            if (timeLimit <= 0)
            {
                isSuccess = false;
                break;
            }

            string str = "result : ";
            foreach (var i in list)
            {
                
                str += i + " / ";
            }
            
            // Debug.Log(str);
            // Debug.Log($"LeftTime : {timeLimit}");
            yield return null;
        }

        // 실패
        if (!isSuccess)
        {
            obstacle.Crash(15f);
        }
        else
        {
            _player.Heal(1);
        }
        list.Clear();
        isMiniGame = false;
        UIManager.OnMiniGame();
        ResumeMove();
    }

    public void PauseMove()
    {
        Debug.Log("PauseMove");
        isPause = true;
        _moveSpeed = 0f;
    }

    public void ResumeMove()
    {
        Debug.Log("ResumeMove");
        Player.PlayAnimation(Player.Animation.Run);
        isPause = false;
    }

    public void BeginFever()
    {
        SoundManager.PlayBgm(KeySound.Fever_BGM);
        _isFever = true;
        feverParticle.gameObject.SetActive(true);
        _uiManager.SetFeverPanel(_isFever);
    }

    public void EndFever()
    {
        SoundManager.PlayBgm(KeySound.GamePlay_BGM);
        _isFever = false;
        feverParticle.gameObject.SetActive(false);
        _uiManager.SetFeverPanel(_isFever);
    }

    public void Lobby()
    {
        GlobalFlowManager.Instance.Lobby();
    }

    public void ShowIntro()
    {
        Debug.Log("ShowIntro");
        StartCoroutine(CoIntroProcess());
    }

    private Vector3 originMainCamPos;
    private float originMainCamSize;

    private IEnumerator CoIntroProcess()
    {
        Debug.Log("CoIntroProcess");
        SoundManager.PlayBgm(KeySound.Intro_BGM);
        FadeCanvas.FadeIn();
        UIManager.SetCutScenePanel(true);
        yield return new WaitForSeconds(9f);
        UIManager.SetCutScenePanel(false);
        Player.Animator.Play("Idle");
        var mainCam = Camera.main;
        originMainCamPos = mainCam.transform.position;
        originMainCamSize = mainCam.orthographicSize;

        var playerPos = Player.transform.position;
        playerPos.z = -10f;
        playerPos.y += 1f;
        playerPos.x += 2f;
        mainCam.transform.position = playerPos;
        mainCam.orthographicSize = 2.5f;
        yield return new WaitForSeconds(1f);

        yield return BackToOrigin(0.5f);
        ChatSystem.Create(_player.transform, new Vector2(1, 2.5f), "What the..", 2f);
        Debug.Log("What the..");
        // print: What the..

        yield return new WaitForSeconds(2f);
        mainCam.transform.DOMove(playerPos, 0.3f);
        mainCam.DOOrthoSize(2.5f, 0.3f);
        SoundManager.PlaySfx(KeySound.Effect_Mung);
        ChatSystem.Create(_player.transform, new Vector2(2, 2.5f), "What the!!!!!!", 2f);
        Debug.Log("What the!!!!!!");
        // print: What the!!!!!!!!!!!!
        yield return new WaitForSeconds(2f);
        
        // 암컷 등장
        DogGirlFriend.Show();
        yield return BackToOrigin(0.5f);

        yield return new WaitForSeconds(1f);
        // 인간 Shocked
        Person.PlayAnimation(Person.Animation.Surprise);
        // 강아지 Idle 무표정
        Player.PlayAnimation(Player.Animation.IdleNormalFace);

        DogGirlFriend.PlayAnimation(DogGirlFriend.Animation.Run);
        DogGirlFriend.transform.DOMoveX(15f, 4f);
        yield return new WaitForSeconds(2f);
        
        StartCoroutine(CoPlayHeartSound());

        Person.PlayAnimation(Person.Animation.Run);
        Person.transform.DOMoveX(8f, 3f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1.5f);
        // 강아지 Shoking 
        ChatSystem.Create(_player.transform, new Vector2(1, 3), "No no no... !!", 1.5f);
        Debug.Log("No no no... !!");
        // 무표정 Idle
        yield return new WaitForSeconds(1.5f);
        ChatSystem.Create(_player.transform, new Vector2(2, 3), "I need to follow him now!!", 1.5f);
        Debug.Log("I need to follow him now!!");
        yield return new WaitForSeconds(1.5f);
        ChatSystem.Create(_player.transform, new Vector2(2, 3), "If I won this competition..", 1.5f);
        Debug.Log("If I won this competition..");
        yield return new WaitForSeconds(1.5f);
        ChatSystem.Create(_player.transform, new Vector2(2, 3), "I could go back to my body.", 1.5f);
        Debug.Log("I could go back to my body.");

        // Tutorial
        yield return new WaitForSeconds(2f);
        UIManager.SetMainPanel(true);
        UIManager.SetTutorialPanel(true);
        yield return new WaitForSeconds(1.5f);
        UIManager.SetTutorialPanel(false);
        GlobalFlowManager.Instance.GameStart();
    }

    private IEnumerator CoPlayHeartSound()
    {
        for (int i = 0; i < 8; i++)
        {
            SoundManager.PlaySfx(KeySound.Effect_Heart);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator BackToOrigin(float duration)
    {
        var mainCam = Camera.main;
        mainCam.transform.DOMove(originMainCamPos, duration);
        mainCam.DOOrthoSize(originMainCamSize, duration);
        yield return new WaitForSeconds(duration + 0.1f);
    }
    
    public void GameStart()
    {
        Debug.Log("GameStart");
        StartCoroutine(CoGameStart());
    }

    public void GameRestart()
    {
        Time.timeScale = 1f;
        GlobalFlowManager.Instance.GameRestart();
    }

    private IEnumerator CoGameStart()
    {
        Debug.Log("CoGameStart");
        FadeCanvas.FadeIn();
        SoundManager.PlayBgm(KeySound.GamePlay_BGM);
        UIManager.SetMainPanel(true);
        PauseMove();
        Player.PlayAnimation(Player.Animation.Idle);
        
        var personPos = Person.transform.position;
        personPos.x = 8f;
        Person.PlayAnimation(Person.Animation.Run);
        Person.transform.position = personPos;
        beginMoveSpeed = _moveSpeed = 0f;

        SoundManager.PlaySfx(KeySound.Effect_GameStart);
        UIManager.SetStartPanel(true);
        yield return new WaitForSeconds(1.5f);
        _uiManager.SetStartPanel(false);
        
        beginMoveSpeed = ConfigDataObject.beginMoveSpeed;
        _moveSpeed = beginMoveSpeed;
        ResumeMove();
        Player.PlayAnimation(Player.Animation.Run);
    }

    public void GameClear()
    {
        Debug.Log("GameClear");
        Player.ChangeState(PlayerState.Idle);
        PauseMove();
        GlobalFlowManager.Instance.Ending();
    }

    public void ShowEnding()
    {
        StartCoroutine(CoEndingProcess());
    }

    private IEnumerator CoEndingProcess()
    {
        yield return null;
        FadeCanvas.FadeIn();
        Player.Rigidbody2D.isKinematic = true;
        Player.Rigidbody2D.velocity = Vector2.zero;

        var personPos = Person.transform.position;
        personPos.x = 8f;
        Person.PlayAnimation(Person.Animation.Run);
        Person.transform.position = personPos;
        Person.transform.DOMoveX(15f, 1.5f).SetEase(Ease.Linear);
        fireworksParticleSystem.gameObject.SetActive(true);
        fireworksParticleSystem.Play();
        SoundManager.PlayBgm(KeySound.GameEnding_BGM);
        SoundManager.PlaySfx(KeySound.Effect_Fireworks);
        
        Player.Animator.Play("Run");
        Player.transform.DOMove(new Vector3(-1f, -3.19f, 0f), 2f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(2f);
        Player.PlayAnimation(Player.Animation.IdleNormalFace);
        ChatSystem.Create(_player.transform, new Vector2(0.5f, 3), "?", 1.5f);
        Debug.Log("?");
        SoundManager.PlaySfx(KeySound.Effect_Fireworks);
        yield return new WaitForSeconds(1.5f);
        ChatSystem.Create(_player.transform, new Vector2(0.5f, 3), "Why isn't there any changes..?", 1.5f);
        Debug.Log("Why isn't there any changes..?");
        SoundManager.PlaySfx(KeySound.Effect_Fireworks);
        yield return new WaitForSeconds(1.5f);
        ChatSystem.Create(_player.transform, new Vector2(0.5f, 3), "Isn’t it a cliché!!?", 1.5f);
        Debug.Log("Isn’t it a cliché!!?");
        // 강아지 충격
        Player.PlayAnimation(Player.Animation.Shocked);
        yield return new WaitForSeconds(1.5f);
        SoundManager.PlaySfx(KeySound.Effect_Fireworks);

        DogGirlFriend.Show();
        DogGirlFriend.PlayAnimation(DogGirlFriend.Animation.Run);
        var dogGirlFriendPos = DogGirlFriend.transform.position;
        dogGirlFriendPos.x = 13f;
        Player.PlayAnimation(Player.Animation.IdleNormalFace);
        ChatSystem.Create(_player.transform, new Vector2(0.5f, 3), "!", 2f);
        Debug.Log("!");

        DogGirlFriend.transform.position = dogGirlFriendPos;
        DogGirlFriend.transform.localScale = new Vector3(-1f, 1f, 1f);
        DogGirlFriend.transform.DOMoveX(6.5f, 2f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(2f);
        DogGirlFriend.PlayAnimation(DogGirlFriend.Animation.Idle);
        SoundManager.PlaySfx(KeySound.Effect_Fireworks);

        // 강아지 무표정
        Player.PlayAnimation(Player.Animation.TailShake);
        ChatSystem.Create(_player.transform, new Vector2(-0.5f, 3), "Why.. Why is the tail vagging..?", 2f);
        yield return new WaitForSeconds(2f);

        Player.transform.DOMoveX(3f, 1.5f).SetEase(Ease.Linear);
        Player.PlayAnimation(Player.Animation.Run);
        // PlayBGM Ending
        yield return new WaitForSeconds(2f);
        Player.PlayAnimation(Player.Animation.TailShake);
        ChatSystem.Create(_player.transform, new Vector2(-0.5f, 3), "Don't know why..", 2f);
        Debug.Log("Don't know why..");
        yield return new WaitForSeconds(1f);
        ChatSystem.Create(DogGirlFriend.transform, new Vector2(0, 3), "♥", 2f);
        Debug.Log("\u2665"); // heart
        yield return new WaitForSeconds(2f);
        
        DogGirlFriend.transform.localScale = new Vector3(1f, 1f, 1f);
        DogGirlFriend.PlayAnimation(DogGirlFriend.Animation.Run);
        DogGirlFriend.transform.DOMoveX(17f, 4f).SetEase(Ease.Linear);
        Player.PlayAnimation(Player.Animation.Run);
        Player.transform.DOMoveX(15f, 4f).SetEase(Ease.Linear);
        SoundManager.PlaySfx(KeySound.Effect_Fireworks);

        // Show Toast THE END
        UIManager.SetEndPanel(true);
        yield return new WaitForSeconds(2f);
        UIManager.SetEndPanel(false);
        yield return new WaitForSeconds(2f);
        FadeCanvas.FadeOut();
        yield return new WaitForSeconds(1f);
        GlobalFlowManager.Instance.Credit();
    }
}