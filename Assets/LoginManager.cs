using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using WebSocketSharp;


public class LoginManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;

    [SerializeField] private GameObject UI_Login;
    [SerializeField] private GameObject UI_Message;
    [SerializeField] private GameObject UI_Home;
    [SerializeField] private GameObject UI_ResetPass;
    [SerializeField] private GameObject UI_StartUI;
    [SerializeField] private GameObject UI_BeforeStartUI;

    [SerializeField] private Button modeButton;
    [SerializeField] private Sprite onlineImg;
    [SerializeField] private Sprite offlineImg;
    [SerializeField] private TextMeshProUGUI statusText;

    private string username;
    public string password;
    [SerializeField] private Vector3 InitialPos;
    [SerializeField] private Vector3 FinalPos;
    [SerializeField] private float speed;
    private bool online;

    private string message;
    public float messageTime;
    public float height;

    #region Start UI Components
    public string player1Name;
    public string player2Name;

    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI playModeStatus;

    public Sprite[] avtarsprites;
    #endregion

    #region Before startUI Component
    public TextMeshProUGUI PanelTitleText;
    public TextMeshProUGUI fieldHeaderText;
    public TMP_InputField inputField;
    #endregion

    #region Home UI Components
    public string[] playModes;
    private string currentPlayMode;
    #endregion

    [SerializeField]private GameObject matchingRoll;
    [SerializeField]private Image player2;
    public Image myProfile;
    private string remoteSprite;

    public Slider sliderBar;
    public TextMeshProUGUI progressText;
    public GameObject userPanel;
    public TMP_InputField userField;
    private bool profileState = false;

    public TextMeshProUGUI stableText;
    public GameObject LoadingBar;
    public TextMeshProUGUI getReadyText;
    public Button[] ModeButtons;

    public RectTransform ButtonBg;
    public GameObject startButton;
    public GameObject UI_ModeCanvas;
    #region Unity Callbacks
    void Start()
    {


        Debug.Log(Screen.height);    
        LoadingBar.SetActive(false);
        getReadyText.text = "";
        InitialPos = Vector3.up * 1000;
        statusText.rectTransform.localPosition = InitialPos;
        username = PlayerPrefs.GetString("username").ToString();
        player1Name = username;
        password = PlayerPrefs.GetString("password").ToString();

     
        userPanel.SetActive(username.IsNullOrEmpty());

        UI_ModeCanvas.SetActive(false);
        UI_Login.SetActive(false);
        UI_ResetPass.SetActive(false);
        
        UI_Home.SetActive(true);
        CheckInternet();
    }

   
    #endregion

    #region Home UI Callbacks

    public void OnProfileButtonClicked()
    {
        profileState = !profileState;
        userPanel.SetActive(profileState);
        startButton.SetActive(!profileState);
    }

    public void OnChangeButtonClicked()
    {
        string name = userField.text;
        if(!name.IsNullOrEmpty())
        {
            username = name;
            PlayerPrefs.SetString("username", username);
            player1Name = username;    
        }
        userPanel.SetActive(false);
        startButton.SetActive(true);
    }
    public void OnChangeModeButtonClicked()
    {
        if (userPanel.activeInHierarchy)
        {
            userPanel.SetActive(false);
            profileState = false;
        }
        if(startButton.activeInHierarchy)
        {
        startButton.SetActive(false);
        }

        if (!online)
        {
            ConnectToPhotonNetwork();
        }
        
     
    }

    public void OnModeChnageButtonClicked(Button button)
    {
        if (userPanel.activeInHierarchy)
        {
            userPanel.SetActive(false);
            profileState = false;
            startButton.SetActive(true);
        }
        StartCoroutine(ButtonTransition(button));
       
        if (button.name == ModeButtons[0].name)
        {
            currentPlayMode = playModes[0];
        }
        else if(button.name == ModeButtons[1].name)
        {
            currentPlayMode = playModes[1];
        }
        else if(button.name == ModeButtons[2].name)
        {
            currentPlayMode = playModes[2];
        }
    }

    IEnumerator ButtonTransition(Button button)
    {
        Vector3 th = button.GetComponent<RectTransform>().anchoredPosition3D;
        th.y += 30;

        float factor = 10;
        while (factor < 10)
        {
            ButtonBg.anchoredPosition3D = Vector3.Lerp(ButtonBg.anchoredPosition3D, th, factor / 10);
            yield return null;
        }
        
        
    }

    public void OnStartButtonClicked()
    {
        statusText.text = "";
        if (currentPlayMode == playModes[1])
        {
            player2.sprite = null;
            player2Name = "Unknown";
        }
        DisplayStartUI(currentPlayMode);
    }

    void DisplayStartUI(string currentPlayMode)
    {

        if(currentPlayMode != playModes[0]  && !PhotonNetwork.IsConnected)
        {
            Debug.Log("Ok Bhai");

            StartCoroutine(DisplayStatusPop("No internet Connection"));
            return;
        }

        if(UI_Message.activeInHierarchy)
        {
            UI_Message.SetActive(false);
        }
        StopAllCoroutines();
        player1NameText.text = player1Name;
        player2NameText.text = player2Name;
        if (currentPlayMode == playModes[0])
        {
            UI_Home.SetActive(false);
            UI_BeforeStartUI.SetActive(true);
            PanelTitleText.text = "Play With Custom Friend";
            fieldHeaderText.text = "Enter Player2 Name";

        }
        else if(currentPlayMode == playModes[1])
        {
            UI_Home.SetActive(false);
            UI_BeforeStartUI.SetActive(true);
            PanelTitleText.text = "Join Custom Room";
            fieldHeaderText.text = "Enter Room ID";
        }
        else if(currentPlayMode == playModes[2])
        {
          //  UI_StartUI.SetActive(true);
            //By Default Entering in randomRoom
            OnEnterButtonClicked();
        }
    }

    #region BeforeStart UI Callbacks
    public void OnEnterButtonClicked()
    {
        if(currentPlayMode == playModes[0])
        {
            player2Name = inputField.text;
            player2NameText.text = player2Name;
            if (!player2Name.IsNullOrEmpty())
            {
                UI_StartUI.SetActive(true);
                matchingRoll.SetActive(false);
                UI_BeforeStartUI.SetActive(false);
                StartCoroutine(LoadSceneAcync());
            }
       
        }
        else if( currentPlayMode == playModes[1])
        {
            string roomID = inputField.text;
            if (!roomID.IsNullOrEmpty())
                JoinCustomRoom(roomID);
        }
        else
        {
            JoinCustomRoom("");
        }
    }
    #endregion
    IEnumerator LoadSceneAcync()
    {
        stableText.text = "";
        getReadyText.text = "Get Ready";
        LoadingBar.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync("GamePlayScene");
       
        operation.allowSceneActivation = false;
        float a = 0;
        float b = 1;
       
     
        for(float c = a+ b; c < 100; c = a+ b)
        {
            a = b;
            b = c;
            sliderBar.value = c/100;
            yield return new WaitForSeconds(1f);
        }
        sliderBar.value = 1;
        UI_ModeCanvas.SetActive(false);

        /*     while (operation.progress < 0.9f)
             {
                 Debug.Log(operation.progress);
             }

            operation.allowSceneActivation = true;*/
        /* while (!operation.isDone)
         {
             Debug.Log("OK");
             progress = operation.progress;
             Debug.Log(progress);
             if (progress >= 0.9f)
             {
                 operation.allowSceneActivation = true;
             }
             //   progress = Mathf.Clamp01(progress/.9f);

             //  sliderBar.value = progress * 100;
             //progressText.text = progress.ToString() + "%";
         }
        */

        yield return new WaitForSeconds(2f);
        operation.allowSceneActivation = true;
    }

    public void JoinCustomRoom(string roomID)
    {
        PhotonNetwork.NickName = username;
        RoomOptions options = new() { MaxPlayers = 2 };
        if(roomID.IsNullOrEmpty())
        {
            PhotonNetwork.JoinRandomOrCreateRoom();
        }
        else
        {
        PhotonNetwork.JoinOrCreateRoom(roomID,options,TypedLobby.Default);
        }
        message = "Joining...";
        StartCoroutine(DisplayStatusPop(message));
    }
    #endregion

    #region Login UI Callbacks
    public void OnBackHomeButtonClicked()
    {
        UI_Home.SetActive(true);
        UI_Login.SetActive(false);
    }

    public void OnLoginButtonClicked()
    {

        username = usernameField.text;
        Debug.Log(username);
        if(!username.IsNullOrEmpty())
        {
        player1Name = username;
        password = "";

        ConnectToPhotonNetwork();
        }
    }
    #endregion

    #region ResetPassword UI Callbacks
    public void OnBackButtonClicked()
    {
        UI_Login.SetActive(false);
    }
    public void OnForgotPasswordTextClicked()
    {
        UI_ResetPass.SetActive(true);
        UI_Login.SetActive(false);
    }

    #endregion

    #region UserDefined Functions
    void CheckInternet()
    {
        if (PhotonNetwork.IsConnected)
        {
            online = true;
            Debug.Log("Connected to Internet");
            modeButton.GetComponent<Image>().sprite = onlineImg;
        }
        else
        {
            online = false;
            Debug.Log("Disconnnected to Internet");
            modeButton.GetComponent<Image>().sprite = offlineImg;
        }
    }
    void ConnectToPhotonNetwork()
    {
        if (!PhotonNetwork.IsConnected)
        {
            if(!string.IsNullOrEmpty(username))
            {
            PhotonNetwork.LocalPlayer.NickName = username;
            PhotonNetwork.ConnectUsingSettings();
                message = "Connecting...";
                float finalY = 800;
                StartCoroutine(DisplayStatusPop(message,finalY));
            Debug.Log("Trying to connect to Internet");
            }
            else
            {
                UI_Login.SetActive(true);
                UI_Home.SetActive(false);
            }
        }
    }

    IEnumerator DisplayStatusPop(string message, float finalY = 800f)
    {
            Debug.Log(message + "Stop");
            UI_Message.SetActive(true);
            statusText.text = message;
            float dist = speed;
            InitialPos = new Vector3(0, 1000, 0);
            FinalPos = new Vector3(0, finalY, 0);
            while (statusText.rectTransform.localPosition != FinalPos)
            {
                statusText.rectTransform.localPosition = Vector3.Lerp(InitialPos, FinalPos, dist);
                dist += speed;
                yield return null;
            }


            yield return new WaitForSeconds(2f);
            UI_Message.SetActive(false);
      
        
    }

    #endregion

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
       message = "";
       
        online = true;
        modeButton.GetComponent<Image>().sprite = onlineImg;
        Debug.Log(PhotonNetwork.NickName + "Connected to Internet");
        message = "Connected! Ready to play with strangers?";

        if(UI_Login.activeInHierarchy)
        {
            UI_Login.SetActive(false);
        }

        if (!UI_Home.activeInHierarchy)
        {
            UI_Home.SetActive(true);
        }

        startButton.SetActive(true);
        int finalY = 0;
        StopAllCoroutines();
        StartCoroutine(DisplayStatusPop(message,finalY));
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
       
        if(!statusText.IsDestroyed())
        {

            modeButton.GetComponent<Image>().sprite = offlineImg;
            Debug.Log(cause);
            string message = "Oops! No Internet. Let's reconnect.";
            int finalY = 800;
            StartCoroutine(DisplayStatusPop(message, finalY));
        }

        startButton.SetActive(!startButton.IsDestroyed());
    }

    public override void OnJoinedRoom()
    {
        UI_StartUI.SetActive(true);
        UI_ModeCanvas.SetActive(true);
        if (currentPlayMode == playModes[2])
        {
            player2.enabled = false;
            stableText.text = "Matching...";
            matchingRoll.SetActive(true);
        }
        else
        {
            stableText.text = "Waiting...";
            matchingRoll.SetActive(false);
        }
        UI_BeforeStartUI.SetActive(false);

        message = "Joined Room Successfully";
        StartCoroutine(DisplayStatusPop(message));

        player1NameText.text = player1Name;

        if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {

            matchingRoll.SetActive(false);
            player2Name = PhotonNetwork.PlayerList[0].NickName.ToString();
            player2NameText.text = player2Name;
            /*     player2.sprite = remoteSprite;*/

            
            StartCoroutine(LoadSceneAcync());
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        message = "JoinRoom Failed";
        StartCoroutine(DisplayStatusPop(message));
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        matchingRoll.SetActive(false);
        player2Name = newPlayer.NickName;
        player2NameText.text = player2Name;
  /*      player2.sprite = remoteSprite;*/
        matchingRoll.SetActive(false);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartCoroutine(LoadSceneAcync());
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(myProfile.sprite.name);
            //stream.SendNext(player1Name);
        }
        else
        {
            remoteSprite = (string)stream.ReceiveNext();
          //player2Name = (string)stream.ReceiveNext();
           // Debug.Log(player2Name);

            UpdateAvtar();
        }
    }

    void UpdateAvtar()
    {
        foreach(Sprite avtar in avtarsprites)
        {
            if(avtar.name == remoteSprite)
            {
                player2.sprite = avtar;
            }
        }
    }

    #endregion

}
