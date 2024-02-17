using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = nameof(ResourceDataObject), menuName = nameof(ResourceDataObject))]
public class ResourceDataObject : ScriptableObject
{
    private static ResourceDataObject _instance;

    public static ResourceDataObject Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ResourceDataObject>(nameof(ResourceDataObject));
                _instance.Init();
            }
            return _instance;
        }
    }

    private void Init()
    {
        _obstacleContainerMap = new Dictionary<ObstacleContainerLevel, List<ObstacleContainer>>
        {
            { ObstacleContainerLevel.Low, lowObstacleContainerList },
            { ObstacleContainerLevel.Medium, mediumObstacleContainerList },
            { ObstacleContainerLevel.High, highObstacleContainerList }
        };
    }

    [SerializeField] private GameObject gumParticle;
    public GameObject GumParticle => gumParticle;
    [SerializeField] private GameObject sweetPotatoParticle;
    public GameObject SweetPotatoParticle => sweetPotatoParticle;

    [SerializeField] private Transform chatBox;
    public Transform ChatBox => chatBox;
    
    [SerializeField] private ObstacleContainer testContainer;
    public ObstacleContainer TestContainer => testContainer;

    [SerializeField] private ObstacleContainer introContainer;
    public ObstacleContainer IntroContainer => introContainer;

    [SerializeField] private ObstacleContainer firstCatContainer;
    public ObstacleContainer FirstCatContainer => firstCatContainer; 
    
    [SerializeField] private ObstacleContainer clearContainer;
    public ObstacleContainer ClearContainer => clearContainer;
    
    [SerializeField] private List<ObstacleContainer> lowObstacleContainerList;
    [SerializeField] private List<ObstacleContainer> mediumObstacleContainerList;
    [SerializeField] private List<ObstacleContainer> highObstacleContainerList;

    private Dictionary<ObstacleContainerLevel, List<ObstacleContainer>> _obstacleContainerMap;
    
    public ObstacleContainer GetRandomObstacleContainerByLevel(ObstacleContainerLevel obstacleContainerLevel)
    {
        List<ObstacleContainer> targetList = null;
        switch (obstacleContainerLevel)
        {
            case ObstacleContainerLevel.Random:
                var index = Random.Range(1, _obstacleContainerMap.Count + 1);
                targetList = _obstacleContainerMap[(ObstacleContainerLevel)index];
                break;
            case ObstacleContainerLevel.Low:
            case ObstacleContainerLevel.Medium:
            case ObstacleContainerLevel.High:
                targetList = _obstacleContainerMap[obstacleContainerLevel];
                break;
        }

        if (targetList == null)
        {
            return null;
        }
        
        return targetList[Random.Range(0, targetList.Count)];
    }
    
    public ObstacleContainer GetObstacleContainerByLevel(ObstacleContainerLevel obstacleContainerLevel, int index = -1)
    {
        List<ObstacleContainer> targetList = null;
        switch (obstacleContainerLevel)
        {
            case ObstacleContainerLevel.Low:
            case ObstacleContainerLevel.Medium:
            case ObstacleContainerLevel.High:
                targetList = _obstacleContainerMap[obstacleContainerLevel];
                break;
        }

        if (targetList == null)
        {
            return null;
        }

        if (index == -1)
        {
            index = Random.Range(0, targetList.Count);
        }
        
        Debug.Log($"Spawn NewMap {obstacleContainerLevel} / {index}");
        
        return targetList[index];
    }
    
}