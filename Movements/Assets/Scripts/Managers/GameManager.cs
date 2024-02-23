using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Camera _cam;
    [SerializeField] private CinemachineVirtualCamera _cinemachineCam;
    private CinemachineConfiner2D _cinemachine;
    [SerializeField] private GameObject[] _boundary;
    private CompositeCollider2D _camColl;
    private Player _player;

    [SerializeField] private PlayerData[] _playerTypes;
    [SerializeField] private Tilemap[] _levels;
    [SerializeField] private Transform _spawnPoint;

    [SerializeField] private TextMeshProUGUI _nameText;

    [SerializeField] private Material _frontMaterial;

    [HideInInspector] public bool IsChangeable { get; private set; } = true;

    [HideInInspector] public int _currentPlayerTypeIndex;
    private int _currentTileMapIndex;
    private Color _currentForegroundColor;

    public SceneData[] SceneData;
    private SceneData currentSceneData;
    private int _currentSceneDataIndex;

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }

        _cam = FindObjectOfType<Camera>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _cinemachine = _cinemachineCam.GetComponent<CinemachineConfiner2D>();
        _camColl  = _boundary[0].GetComponent<CompositeCollider2D>();
    }

    private void Start() 
    {
        
        SetSceneData(SceneData[0]);
        SwitchLevel(0);
        SwitchPlayerType(0);
    }

    private void Update() 
    {
        if(UserInput.instance.SwitchInput)
        {
            // swithc to next level. uses "?" to indicate that if the expression in the brackets before is true then 0 will be passed else it will increase by 1
            SwitchPlayerType((_currentPlayerTypeIndex == _playerTypes.Length - 1) ? 0 : _currentPlayerTypeIndex + 1);
        }
        if(UserInput.instance.LevelInput)
        {
            int _lvlIndex = (_currentTileMapIndex == _levels.Length - 1) ? 0 : _currentTileMapIndex + 1;
            SetSceneData(SceneData[_lvlIndex]);
            SwitchLevel(_lvlIndex);
            _camColl  = _boundary[_lvlIndex].GetComponent<CompositeCollider2D>();
            _cinemachine.m_BoundingShape2D = _camColl;
        }
    }

    public void SetSceneData(SceneData sceneData)
    {
        currentSceneData = sceneData;

        // update the camera and tilemap color according ot the new data
        _cam.orthographicSize   = sceneData.camSize;
        _cam.backgroundColor    = sceneData.backgroundColor;
        _frontMaterial.color    = sceneData.foregroundColor;
        //_levels[_currentTileMapIndex].color = sceneData.foregroundColor;

        _currentForegroundColor = sceneData.foregroundColor;
    }

    public void SwitchLevel(int index)
    {
        // switch tilemap active and apply color
        _levels[_currentTileMapIndex].gameObject.SetActive(false);
        _levels[_currentTileMapIndex].transform.GetChild(0).gameObject.SetActive(false);
        _levels[index].gameObject.SetActive(true);
        _levels[index].transform.GetChild(0).gameObject.SetActive(true);
        _levels[index].color = _currentForegroundColor;
        _levels[_currentTileMapIndex] = _levels[index];

        _player.transform.position = _spawnPoint.position;

        _currentTileMapIndex = index;
    }

    

    public void SwitchPlayerType(int index)
    {
        _player.playerData = _playerTypes[index];
        _currentPlayerTypeIndex = index;

        GameEvents.instance.PlayerSwitch();

        // PlayerDataManager.instance.ChangePlayerData(_playerTypes[index]);

        IsChangeable = true;

        switch (index)
        {
            case 0:
                _nameText.text = "Celeste";
                IsChangeable = false;
                break;
            case 1:
                _nameText.text = "Hollow Knight";
                IsChangeable = false;
                break;
            case 2:
                _nameText.text = "Custom Profile 1";
                IsChangeable = true;
                break;
            case 3:
                _nameText.text = "Custom Profile 2";
                IsChangeable = true;
                break;
            default:
                IsChangeable = true;
                break;
        }
    }
}