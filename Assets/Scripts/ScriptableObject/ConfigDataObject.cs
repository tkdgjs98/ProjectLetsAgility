using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ConfigDataObject), menuName = nameof(ConfigDataObject))]
public class ConfigDataObject : ScriptableObject
{
    private static ConfigDataObject _instance;

    public static ConfigDataObject Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ConfigDataObject>(nameof(ConfigDataObject));
            }
            return _instance;
        }
    }

    public List<float> backgroundScrollFactorList;

    [Header("General")]
    public int gameClearMapCount = 10;

    public int lowContainerCount = 3;
    public int mediumContainerCount = 3;

    public float lobbyMoveSpeed = 5f;
    
    [Header("Parameter")]
    public float beginMoveSpeed = 3f;
    public float contactLostBoostGauge = 3;
    public float addMoveSpeedPerSec = 2f;
    public float subMoveSpeedPerSec = 4f;
    public float feverActiveAmount = 50f;
    public float lostFeverGaugePerSec = 10f;

    public float maxMoveSpeed;
    public float maxFeverMoveSpeed = 40f;
}
