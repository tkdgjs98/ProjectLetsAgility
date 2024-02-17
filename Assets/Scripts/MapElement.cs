using System;
using UnityEngine;

public class MapElement : MonoBehaviour
{
    [SerializeField] private Transform obstacleHolderTransform;
    private ObstacleContainer _obstacleContainer;

    private void Awake()
    {
        while (obstacleHolderTransform.childCount > 0)
        {
            var child = obstacleHolderTransform.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }
    }

    public void ChangeIntroMap()
    {
        if (_obstacleContainer != null)
        {
            Destroy(_obstacleContainer.gameObject);
        }

        var obstacleContainerPrefab = ResourceDataObject.Instance.IntroContainer;
        var obstacleContainer = Instantiate(obstacleContainerPrefab, obstacleHolderTransform);
        _obstacleContainer = obstacleContainer;
    }
    
    public void ChangeCatTunnelMap()
    {
        if (_obstacleContainer != null)
        {
            Destroy(_obstacleContainer.gameObject);
        }

        var obstacleContainerPrefab = ResourceDataObject.Instance.FirstCatContainer;
        var obstacleContainer = Instantiate(obstacleContainerPrefab, obstacleHolderTransform);
        _obstacleContainer = obstacleContainer;
    }

    public void ChangeClearMap()
    {
        if (_obstacleContainer != null)
        {
            Destroy(_obstacleContainer.gameObject);
        }

        var obstacleContainerPrefab = ResourceDataObject.Instance.ClearContainer;
        var obstacleContainer = Instantiate(obstacleContainerPrefab, obstacleHolderTransform);
        _obstacleContainer = obstacleContainer;
    }

    public void ChangeMap(ObstacleContainerLevel containerLevel = ObstacleContainerLevel.Random, int index = -1)
    {
        if (_obstacleContainer != null)
        {
            Destroy(_obstacleContainer.gameObject);
        }

        var testContainer = ResourceDataObject.Instance.TestContainer;
        var obstacleContainerPrefab = testContainer != null
            ? testContainer
            : ResourceDataObject.Instance.GetObstacleContainerByLevel(containerLevel, index);
        var obstacleContainer = Instantiate(obstacleContainerPrefab, obstacleHolderTransform);
        _obstacleContainer = obstacleContainer;
    }

    public void ChangeRandomMap()
    {
        if (_obstacleContainer != null)
        {
            Destroy(_obstacleContainer.gameObject);
        }

        var obstacleContainerPrefab =
            ResourceDataObject.Instance.GetRandomObstacleContainerByLevel(ObstacleContainerLevel.Random);
        var obstacleContainer = Instantiate(obstacleContainerPrefab, obstacleHolderTransform);
        _obstacleContainer = obstacleContainer;
    }
}