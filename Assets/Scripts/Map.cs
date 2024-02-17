using System;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public event Action<int> OnChangeMapCount;
    
    public float xSize = 20f;
    public List<MapElement> mapElementList;

    private int _mapCount = 0;

    private void Start()
    {
        if (GlobalFlowManager.Instance.FlowState == FlowState.Lobby
            || GlobalFlowManager.Instance.FlowState == FlowState.Credit)
        {
            foreach (var mapElement in mapElementList)
            {
                mapElement.ChangeIntroMap();
            }
        }
        else
        {
            mapElementList[0].ChangeIntroMap();
            mapElementList[1].ChangeCatTunnelMap();
        }
    }

    private void Update()
    {
        var firstMap = mapElementList[0];

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
        
        foreach (var mapElement in mapElementList)
        {
            mapElement.transform.Translate(Vector3.left * (moveSpeed * Time.deltaTime));
        }

        var mapElementPos = firstMap.transform.position;
        
        if (!(mapElementPos.x < -xSize)) return;
        
        mapElementPos.x = mapElementList[1].transform.position.x + xSize;
        firstMap.transform.position = mapElementPos;
        if (GlobalFlowManager.Instance.FlowState == FlowState.Lobby
            || GlobalFlowManager.Instance.FlowState == FlowState.Credit)
        {
            firstMap.ChangeIntroMap();
        }
        else
        {
            var configDataObject = ConfigDataObject.Instance;
            if (_mapCount < configDataObject.gameClearMapCount)
            {
                if (_mapCount < configDataObject.lowContainerCount)
                {
                    firstMap.ChangeMap(ObstacleContainerLevel.Low);
                }
                else if (_mapCount < configDataObject.lowContainerCount + configDataObject.mediumContainerCount)
                {
                    firstMap.ChangeMap(ObstacleContainerLevel.Medium);
                }
                else
                {
                    firstMap.ChangeMap(ObstacleContainerLevel.High);
                }
            }
            else
            {
                firstMap.ChangeClearMap();
            }
        }
        AddChangeMapCount();
        (mapElementList[0], mapElementList[1]) = (mapElementList[1], mapElementList[0]);
    }

    private void AddChangeMapCount()
    {
        _mapCount++;
        Debug.Log($"MapCount : {_mapCount}");
        OnChangeMapCount?.Invoke(_mapCount);
    }
}
