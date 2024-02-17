using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalFlowManager : MonoBehaviour
{
    private static GlobalFlowManager _instance;

    public static GlobalFlowManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GlobalFlowManager>();
            }

            return _instance;
        }
    }

    [SerializeField] private FlowState flowState;
    public FlowState FlowState => flowState;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(Instance);
    }

    private void Start()
    {
        switch (flowState)
        {
            case FlowState.None:
                ChangeFlow(FlowState.Lobby);
                break;
            case FlowState.Lobby:
                Lobby();
                break;
            case FlowState.Intro:
                Intro();
                break;
            case FlowState.Play:
                GameStart();
                break;
            case FlowState.Ending:
                Ending();
                break;
            case FlowState.Credit:
                Credit();
                break;
        }
    }

    private void ChangeFlow(FlowState flowState)
    {
        this.flowState = flowState;
    }

    public void Lobby()
    {
        ChangeFlow(FlowState.Lobby);
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "Lobby")
        {
            SceneManager.LoadScene("Lobby");
        }

        FadeCanvas.FadeIn();
        SoundManager.PlayBgm(KeySound.Main_BGM);
    }

    public void OnClickGameStartButton()
    {
        FindObjectOfType<CanvasGroup>().interactable = false;
        StartCoroutine(CoClickGameStartButton());
    }

    private IEnumerator CoClickGameStartButton()
    {
        FadeCanvas.FadeOut();
        yield return new WaitForSeconds(0.5f);
        Intro();
    }

    public void Intro()
    {
        ChangeFlow(FlowState.Intro);
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "Main")
        {
            SceneManager.LoadScene("Main");
        }

        StartCoroutine(CoIntro());
    }

    private IEnumerator CoIntro()
    {
        yield return null;
        GameManager.Instance.ShowIntro();
    }

    public void GameStart()
    {
        ChangeFlow(FlowState.Play);
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "Main")
        {
            SceneManager.LoadScene("Main");
        }

        GameManager.Instance.GameStart();
    }

    public void GameRestart()
    {
        StartCoroutine(CoRestart());
    }

    private IEnumerator CoRestart()
    {
        yield return null;
        ChangeFlow(FlowState.Play);
        SceneManager.LoadScene("Main");
        yield return null;
        GameManager.Instance.GameStart();
    }

    public void Ending()
    {
        ChangeFlow(FlowState.Ending);
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "Main")
        {
            SceneManager.LoadScene("Main");
        }
        GameManager.Instance.ShowEnding();
    }

    public void Credit()
    {
        ChangeFlow(FlowState.Credit);
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "Credit")
        {
            SceneManager.LoadScene("Credit");
        }
        FadeCanvas.FadeIn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FadeCanvas.FadeIn();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            FadeCanvas.FadeOut();
        }
        
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            Time.timeScale -= 0.2f;
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            Time.timeScale += 0.2f;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 1f;
        }
        #endif
    }
}