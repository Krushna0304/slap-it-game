using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class LoadHomeScene : MonoBehaviour
{
   public void OnQuitButtonclicked()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Home_Scene");
    }
}
