using System.Collections;
using System.Collections.Generic;
using TikTokLiveUnity;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class TestScript : MonoBehaviour
{
    private string userName;

    [SerializeField]
    private Animator _playerAnimator1;

    [SerializeField]
    private Animator _playerAnimator2;

    [SerializeField]
    private GameObject _settingsPanel;

    [SerializeField]
    private TMP_InputField _usernameInput;

    [SerializeField]
    private TMP_Text _warningTxt;

    [SerializeField]
    private Button _connectBtn;

    [SerializeField]
    private TMP_Dropdown _playerGifts1;

    [SerializeField]
    private TMP_Dropdown _playerGifts2;

    [SerializeField]
    private Button _playerGifts1Btn;

    [SerializeField]
    private Button _playerGifts2Btn;

    [SerializeField]
    private TMP_Text _playerSelectedGiftsTxt1;

    [SerializeField]
    private TMP_Text _playerSelectedGiftsTxt2;

    [SerializeField]
    private Slider _startHealthSlider;

    [SerializeField]
    private TMP_Text _startHealthTxt;

    [SerializeField]
    private PlayerController _player1Controller;

    [SerializeField]
    private PlayerController _player2Controller;

    private int _startHealth = 1000;

    private bool isUsernameValid = false;
    private bool isConnected = false;

    private List<GiftData> _playerGifts1Data = new List<GiftData>();
    private List<GiftData> _playerGifts2Data = new List<GiftData>();


    public List<GiftData> CachedGifts { get; private set; }


    private void LoadGiftData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("tiktok_gifts");

        if (jsonFile != null)
        {
            try
            {
                CachedGifts = JsonConvert.DeserializeObject<List<GiftData>>(jsonFile.text);
                _playerGifts1.ClearOptions();
                _playerGifts2.ClearOptions();
                List<string> giftNames = new List<string>();
                foreach (var gift in CachedGifts)
                {
                    giftNames.Add(gift.Name);
                }
                _playerGifts1.AddOptions(giftNames);
                _playerGifts2.AddOptions(giftNames);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("JSON parsing hatasý: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("tiktok_gifts.json bulunamadý!");
        }
    }


    void Awake()
    {
        LoadGiftData();
    }

    // Start is called before the first frame update
    async void Start()
    {
        StartingHealthSliderChanged();

        TikTokLiveManager.Instance.OnLike += (sender, e) =>
        {
            Debug.Log($"Received like from {e.Sender.NickName} with count {e.Count}");
        };
        TikTokLiveManager.Instance.OnGift += (sender, e) =>
        {
            Debug.Log($"Received gift from {e.Sender.NickName} with name {e.Gift.Name}");
            var giftData = CachedGifts.Find(g => g.Name == e.Gift.Name);
            if (giftData != null)
            {
                Debug.Log($"Gift data found: Name = {giftData.Name}, Price = {giftData.Price}");
                if(_playerGifts1Data.Exists(g => g.Name == giftData.Name) && _playerAnimator1 != null)
                    _player1Controller.Attack(giftData.Price);

                if (_playerGifts2Data.Exists(g => g.Name == giftData.Name) && _playerAnimator2 != null)
                    _player2Controller.Attack(giftData.Price);
            }
            else
            {
                Debug.LogWarning($"Gift data not found for gift: {e.Gift.Name}");
            }
        };
        TikTokLiveManager.Instance.OnChatMessage += (sender, e) =>
        {
            Debug.Log($"Received chat message from {e.Sender.NickName}: {e.Message}");
        };
        TikTokLiveManager.Instance.OnConnected += (sender, e) =>
        {
            Debug.Log("Connected to TikTok Live!");
        };
        TikTokLiveManager.Instance.OnSocialMessage += (sender, e) =>
        {
            Debug.Log($"Received social message from {e.Sender.NickName}");
        };

        //await TikTokLiveManager.Instance.ConnectToStream(userName);
    }

    // Update is called once per frame
    async void Update()
    {


        if (isUsernameValid && !isConnected)
        {
            
        }

    }

    public void ConnectBtn()
    {
        userName = _usernameInput.text.Trim();
        TikTokLiveManager.Instance.ConnectToStream(userName);
        isConnected = true;
        _settingsPanel.SetActive(false);
    }

    public void PlayerGiftAddBtn(int playerId) // 0: Player 1, 1: Player 2
    {
        int selectedIndex;
        switch (playerId)
        {
            case 0:
                selectedIndex = _playerGifts1.value;
                if (selectedIndex >= 0 && selectedIndex < CachedGifts.Count)
                {
                    GiftData selectedGift = CachedGifts[selectedIndex];
                    _playerGifts1Data.Add(selectedGift);
                    Debug.Log($"Player 1 added gift: {selectedGift.Name}");
                    _playerSelectedGiftsTxt1.text += $"{selectedGift.Name},\n";
                }
                break;
            case 1:
                selectedIndex = _playerGifts2.value;
                if (selectedIndex >= 0 && selectedIndex < CachedGifts.Count)
                {
                    GiftData selectedGift = CachedGifts[selectedIndex];
                    _playerGifts2Data.Add(selectedGift);
                    Debug.Log($"Player 2 added gift: {selectedGift.Name}");
                    _playerSelectedGiftsTxt2.text += $"{selectedGift.Name},\n";
                }
                break;
            default:
                Debug.LogWarning("Geçersiz oyuncu ID'si");
                break;
        }
    }

    public void StartingHealthSliderChanged()
    {
        _startHealth = (int)_startHealthSlider.value;
        _startHealthTxt.text = $"{_startHealth}";
        Debug.Log($"Baþlangýç Saðlýðý: {_startHealth}");

        _player1Controller.SetStartingHealth(_startHealth);
        _player2Controller.SetStartingHealth(_startHealth);
    }
}
