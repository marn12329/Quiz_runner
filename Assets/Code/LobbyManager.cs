using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("ButtonMashScene");
    }
}
