using TMPro;
using UnityEngine;

namespace Components
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private NetManager netManager;
        
        [Header("Menus")]
        [SerializeField] private GameObject lobbyMenu;
        [SerializeField] private GameObject gameMenu;
        [SerializeField] private GameObject winMenu;

        [Header("Win Menu")] 
        [SerializeField] private TMP_Text winnerLabel;

        [Header("Lobby Menu")] 
        [SerializeField] private TMP_InputField nicknameField;
        [SerializeField] private TMP_InputField hostField;
        [SerializeField] private TMP_InputField portField;

        private static UIManager _instance;

        public static UIManager Instance => _instance;
        public string Nickname => nicknameField.text;
        
        public void HideAllMenus()
        {
            HideLobbyMenu();
            HideGameMenu();
            HideWinnerMenu();
        }

        public void CancelClicked()
        {
            if (lobbyMenu.activeSelf)
                return;

            if (gameMenu.activeSelf)
            {
                HideGameMenu();
            }
            else
            {
                ShowGameMenu();
            }
        }

        public void ShowLobbyMenu()
        {
            HideAllMenus();
            inputManager.IsActive = false;
            lobbyMenu.SetActive(true);
            Cursor.visible = true;
        }
        
        public void HideLobbyMenu()
        {
            lobbyMenu.SetActive(false);
            Cursor.visible = false;
            inputManager.IsActive = true;
        }

        public void ShowGameMenu()
        {
            HideAllMenus();
            inputManager.IsActive = false;
            gameMenu.SetActive(true);
            Cursor.visible = true;
        }

        public void HideGameMenu()
        {
            gameMenu.SetActive(false);
            Cursor.visible = false;
            inputManager.IsActive = true;
        }

        public void ShowWinnerMenu(string winner)
        {
            HideAllMenus();
            winnerLabel.text = winner;
            winMenu.SetActive(true);
            inputManager.IsActive = false;
        }

        public void HideWinnerMenu()
        {
            winMenu.SetActive(false);
            inputManager.IsActive = true;
        }

        public void ConnectToServer()
        {
            netManager.ConnectToServer(hostField.text, ushort.Parse(portField.text), nicknameField.text);
            HideAllMenus();
        }

        public void StartHost()
        {
            netManager.StartHost(ushort.Parse(portField.text), nicknameField.text);
            HideAllMenus();
        }

        public void DisconnectFromServer()
        {
            netManager.DisconnectFromServer();
            HideAllMenus();
            ShowLobbyMenu();
        }

        public void Confirm()
        {
            HideGameMenu();
        }

        private void Awake()
        {
            _instance = this;
        }
     }
}