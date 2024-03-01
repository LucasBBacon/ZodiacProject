using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using System;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera _cinemachineCam;
    [SerializeField] private GameObject[] _camBoundaries;

    [Space(20)]

    [Header("Player Types")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private PlayerData[] _playerTypes;

    [Space(20)]

    [Header("Level")]
    [SerializeField] private GameObject _gridObj;
    [SerializeField] private Tilemap[] _levels;
    [SerializeField] private Transform _spawnPoint;
    [Space(5)]
    public SceneData[] SceneData;
    [SerializeField] private Material _frontMaterial;

    [HideInInspector] public bool IsChangeable { get; private set; } = true;
    [HideInInspector] public int _currentPlayerTypeIndex;


    private Player _player;
    
    private int _currentTileMapIndex;
    private Color _currentForegroundColor;
    private SceneData currentSceneData;
    
    private bool _gridEnabled = false;

    private Camera _cam;
    private CompositeCollider2D _camColl;
    private CinemachineConfiner2D _cameraConfiner;

    private void Awake() 
    {
        if(instance == null)
            instance = this;
        
        _cam            = FindObjectOfType<Camera>();
        _player         = GameObject.FindWithTag("Player").GetComponent<Player>();

        _cameraConfiner = _cinemachineCam.GetComponent<CinemachineConfiner2D>();
        _camColl        = _camBoundaries[0].GetComponent<CompositeCollider2D>();
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
            // switch to next level. uses "?" to indicate that if the expression in the brackets before is true then 0 will be passed else it will increase by 1
            SwitchPlayerType((_currentPlayerTypeIndex == _playerTypes.Length - 1) ? 0 : _currentPlayerTypeIndex + 1);
        }
        if(UserInput.instance.LevelInput)
        {
            int _lvlIndex = (_currentTileMapIndex == _levels.Length - 1) ? 0 : _currentTileMapIndex + 1;
            SetSceneData(SceneData[_lvlIndex]);
            SwitchLevel(_lvlIndex);
            _camColl                            = _camBoundaries[_lvlIndex].GetComponent<CompositeCollider2D>();
            _cameraConfiner.m_BoundingShape2D   = _camColl;
        }

        if(Input.GetKeyDown(KeyCode.G)) _gridEnabled = !_gridEnabled;

        if(_gridEnabled)                _gridObj.SetActive(true);
        else if(!_gridEnabled)          _gridObj.SetActive(false);
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
        for (int i = 0; i < _levels[_currentTileMapIndex].transform.childCount; i++)
        {
            _levels[_currentTileMapIndex].transform.GetChild(i).gameObject.SetActive(true);
        }

        _levels[_currentTileMapIndex].gameObject.SetActive(false);
        _levels[index].gameObject.SetActive(true);

        for (int i = 0; i < _levels[index].transform.childCount; i++)
        {
            _levels[index].transform.GetChild(i).gameObject.SetActive(true);
        }

        _levels[index].color        = _currentForegroundColor;
        //_levels[_currentTileMapIndex] = _levels[index];

        _player.transform.position  = _spawnPoint.position;

        _currentTileMapIndex        = index;
    }

    

    public void SwitchPlayerType(int index)
    {
        _player.playerData      = _playerTypes[index];
        _currentPlayerTypeIndex = index;

        GameEvents.instance.PlayerSwitch();

        IsChangeable            = true;

        switch (index)
        {
            case 0:
                _nameText.text  = "Celeste";
                IsChangeable    = false;
                break;
            case 1:
                _nameText.text  = "Hollow Knight";
                IsChangeable    = false;
                break;
            case 2:
                _nameText.text  = "Custom Profile 1";
                IsChangeable    = true;
                break;
            case 3:
                _nameText.text  = "Custom Profile 2";
                IsChangeable    = true;
                break;
            default:
                IsChangeable    = true;
                break;
        }
    }
}