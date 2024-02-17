using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Serialization;

public class UIManager : MonoBehaviour
{
    [Header("Image & Sprite")]
    [SerializeField] private Image    imgProfile;        // 프로필 이미지
    [SerializeField] private Sprite[] sprProfiles;       // 캐릭별 프로필 이미지 리스트
    [SerializeField] private Image[]  imgHeart;          // 개껌 이미지
    [SerializeField] private Sprite[] sprHearts;         // 개껌 이미지 리스트
    [SerializeField] private Image[]  imgKeypad;         // 키보드 이미지
    [SerializeField] private Sprite[] sprKeypad;         // 키보드 이미지 리스트
    [SerializeField] private Image    imgSlider;         // 슬라이더 이미지
    [SerializeField] private Sprite[] sprSlider;         // 슬라이더 이미지 리스트

    [Header("Slider")]
    [SerializeField] private Slider slider;              // 가속 게이지
    [SerializeField] private Slider timeLimit;           // 제한 시간

    [Header("Button")]
    [SerializeField] private Button btnMenu;             // 메뉴 버튼

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI txtStartGame;  // GAME START / BICHON TIME / THE END, 카운트 다운 등
                                        // 상태를 나타내는 텍스트
    [SerializeField] private TextMeshProUGUI txtEndGame;
    [SerializeField] private TextMeshProUGUI txtLimit;   // 제한 시간 텍스트

    [Header("Panel")]
    [SerializeField] private GameObject panelMain;       // 메인 메뉴 패널
    [SerializeField] private GameObject panelPause;      // 일시정지 메뉴 패널
    [SerializeField] private GameObject panelTutorial;   // 튜토리얼 패널
    [SerializeField] private GameObject panelMiniGame;   // 미니게임 패널   
    [SerializeField] private GameObject panelGameOver;   // 게임오버 패널
    [SerializeField] private GameObject panelFever;      // 피버 타임 효과 패널
    [SerializeField] private GameObject panelCutScene;
    
    [SerializeField] private GameManager gameManager;    // 게임과 관련된 값을 가져올 변수
    [SerializeField] private Player player;              // 플레이어와 관련된 값을 가져올 변수
    private ConfigDataObject ConfigDataObject => ConfigDataObject.Instance;

    private void Start()
    {
        player.OnHpChanged += OnHpChanged;
        slider.maxValue = ConfigDataObject.feverActiveAmount;
    }

    private void Update()
    {
        slider.value = gameManager.FeverBoostGauge;
    }

    private void OnDestroy()
    {
        if (player == null)
        {
            return;
        }

        player.OnHpChanged -= OnHpChanged;
    }

    public void SetCutScenePanel(bool isActive)
    {
        panelCutScene.SetActive(isActive);
    }

    public void SetMainPanel(bool status)
    {
        panelMain.SetActive(status);
    }

    public void SetTutorialPanel(bool isActive)
    {
        panelTutorial.SetActive(isActive);
    }

    public void SetStartPanel(bool isActive)
    {
        txtStartGame.gameObject.SetActive(isActive);
    }

    public void SetEndPanel(bool isActive)
    {
        txtEndGame.gameObject.SetActive(isActive);
    }

    public void OnClickMenuBtn()
    {
        if(!gameManager.IsPause)
        {
            gameManager.IsPause = true;
            gameManager.PauseMove();
        }
        else
        {
            gameManager.IsPause = false;
            gameManager.ResumeMove();
        }
    }

    // 게임 재시작 버튼을 눌렀을 때 호출
    public void OnClickRestartBtn()
    {

    }

    // 게임 나가기 버튼을 눌렀을 때 호출
    public void OnClickExitBtn()
    {
        Application.Quit();
    }

    public void OnHpChanged()
    {
        // 플레이어 hp에 따라 왼쪽 위의 HP Bar UI 변경
        Debug.Log("1");
        for (int i = 0; i < imgHeart.Length; i++)
        {
            imgHeart[i].sprite = i < player.Hp ? sprHearts[0] : sprHearts[1];
        }
        
        if(player.Hp == 0)
            OnGameOver();
    }
    
    public void OnGameOver()
    {
        Time.timeScale = 0.0f;
        SoundManager.PlaySfx(KeySound.Effect_GameOver);
        panelGameOver.SetActive(true);
    }

    public void OnMiniGame()
    {
        if (gameManager.IsMiniGame)
            panelMiniGame.SetActive(true);
        else
        {
            panelMiniGame.SetActive(false);
            return;
        }

        for (int i = 0; i < gameManager.list.Count; i++)
        {
            imgKeypad[i].sprite = sprKeypad[(int)gameManager.list[i]];
        }

        for (int i = 0; i < gameManager.count; i++)
        {
            imgKeypad[i].sprite = sprKeypad[(int)gameManager.list[i] + 4];
        }

        timeLimit.value = gameManager.TimeLimit;
        txtLimit.text = timeLimit.value.ToString("#.#");
    }

    public void SetFeverPanel(bool status)
    {
        panelFever.SetActive(status);
    }
}
