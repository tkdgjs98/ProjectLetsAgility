using DG.Tweening;
using UnityEngine;

public class FadeCanvas : MonoBehaviour
{
    private static FadeCanvas _instance;

    public static FadeCanvas Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<FadeCanvas>();
            }

            return _instance;
        }
    }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void InitStatic()
    {
        _instance = null;
    }
    
    private CanvasGroup _canvasGroup;

    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }

            return _canvasGroup;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(Instance.gameObject);
        Instance.CanvasGroup.alpha = 1f;
    }

    public static void FadeIn(float duration = 0.5f)
    {
        Instance.CanvasGroup.DOKill();
        Instance.CanvasGroup.DOFade(0f, duration)
            .OnComplete(() =>
            {
                Instance.gameObject.SetActive(false);
            });
    }

    public static void FadeOut(float duration = 0.5f)
    {
        Instance.gameObject.SetActive(true);
        Instance.CanvasGroup.DOKill();
        Instance.CanvasGroup.DOFade(1f, duration);
    }
}
