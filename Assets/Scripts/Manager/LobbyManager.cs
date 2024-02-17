using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public void OnClickGameStartButton()
    {
        GlobalFlowManager.Instance.OnClickGameStartButton();
    }
}
