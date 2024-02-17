using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using System.IO;

public class ChatSystem : MonoBehaviour
{
    public static void Create(Transform parent, Vector3 localPosition, string text, float time)
    {
        Transform chatBubbleTransform = Instantiate(ResourceDataObject.Instance.ChatBox, parent);
        chatBubbleTransform.localPosition = localPosition;

        chatBubbleTransform.GetComponent<ChatSystem>().Setup(text);

        Destroy(chatBubbleTransform.gameObject, time);
    }

    private SpriteRenderer backgroundSpriteRenderer;
    private TextMeshPro textMeshPro;

    private void Setup(string text)
    {
        backgroundSpriteRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
        textMeshPro.SetText(text);
        textMeshPro.ForceMeshUpdate();
        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 padding = new Vector2(1f, 1f);

        backgroundSpriteRenderer.size = textSize + padding;

        backgroundSpriteRenderer.transform.localPosition = new Vector3(0f, 0f, 3f);

        StartCoroutine(WriteText(text));
    }

    private IEnumerator WriteText(string text)
    {
        textMeshPro.text = null;

        for(int i = 0; i < text.Length; i++)
        {
            textMeshPro.text += text[i];
            if (i % 2 == 0)
            {
                SoundManager.PlaySfx(KeySound.Effect_Pressed);
            }
            yield return new WaitForSeconds(0.03f);
        }
    }
}