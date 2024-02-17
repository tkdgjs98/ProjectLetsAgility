using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public enum BackgroundScrollType
    {
        Near,
        Medium,
        Far,
        Cloud,
        VeryFar,
    }
    
    public float xSize = 20f;
    [SerializeField] private BackgroundScrollType backgroundScrollType;
    public List<Transform> bgElementList;

    private void Update()
    {
        var firstElement = bgElementList[0];
        var factor = ConfigDataObject.Instance.backgroundScrollFactorList[(int)backgroundScrollType];

        var moveSpeed = 0f;
        if (GlobalFlowManager.Instance.FlowState == FlowState.Lobby
            || GlobalFlowManager.Instance.FlowState == FlowState.Credit)
        {
            moveSpeed = ConfigDataObject.Instance.lobbyMoveSpeed;
        }
        else if (GlobalFlowManager.Instance.FlowState == FlowState.Play)
        {
            moveSpeed = GameManager.Instance.MoveSpeed;
        }
        
        foreach (var bgElement in bgElementList)
        {
            bgElement.transform.Translate(Vector3.left * (moveSpeed * factor * Time.deltaTime));
        }

        var mapElementPos = firstElement.transform.position;
        
        if (!(mapElementPos.x < -xSize)) return;
        
        mapElementPos.x = bgElementList[1].transform.position.x + xSize;
        firstElement.transform.position = mapElementPos;
        (bgElementList[0], bgElementList[1]) = (bgElementList[1], bgElementList[0]);
    }
}
