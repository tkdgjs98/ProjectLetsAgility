using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credit : MonoBehaviour
{
    public RawImage _rawImage;
    public GameObject panelCredit;
    private float waitingTime = 2.0f;
    private float moveTime = 2.0f;

    private void Start()
    {
        ShowCredit();
    }

    private void ShowCredit()
    {
        StartCoroutine(OnCredit());
    }

    private IEnumerator OnCredit()
    {
        // yield return new WaitForSeconds(waitingTime);
        Debug.Log("1");

        // moveImage(moveTime);
        _rawImage.rectTransform.DOSizeDelta(new Vector2(1920f * 0.6f, 1080f * 0.6f), moveTime).SetDelay(waitingTime);
        _rawImage.rectTransform.DOLocalMoveX(-200f, moveTime + 1f).SetDelay(waitingTime + 1f);

        Debug.Log("2");
        panelCredit.SetActive(true);
        yield return new WaitForSeconds(30f);
        Debug.Log("3");
        FadeCanvas.FadeOut();
        yield return new WaitForSeconds(3f);
        Debug.Log("4");
        SceneManager.LoadScene("Lobby");
        FadeCanvas.FadeIn();
        SoundManager.PlayBgm(KeySound.Main_BGM);
    }
}