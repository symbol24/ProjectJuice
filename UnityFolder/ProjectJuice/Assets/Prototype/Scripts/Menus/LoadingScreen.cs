using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

public class LoadingScreen : ExtendedMonobehaviour {
    MenuControls m_MenuControls;
    [SerializeField] private LoadingType _type = LoadingType.FullyRandom;
    private DelayManager _delayManager;
    [SerializeField] private List<Sprite> _tipScreens;
    [SerializeField] private List<Sprite> LoadingTextSprites;
    [SerializeField] private List<int> _randomRange;
    [SerializeField] private SpriteRenderer _LoadingText;
    [SerializeField] private GameObject _back;
    [SerializeField] private GameObject _coloredArea;
    [SerializeField] private GameObject _tipScreenGameObject;
    [SerializeField] private List<Color> _colorsForBack;
    private SpriteRenderer _tipScreenSprite;
    private Sprite _toDisplay;
    [Range(0,1)][SerializeField] private float _loadingTextDisplayTime = 0.2f;
    private bool _displayLoading = false;
    [Range(0, 60)][SerializeField] private float _minimumLoadTime = 10f;
    private Animator _animator;
    private LoadingState _loadingState = LoadingState.Idle;
    [HideInInspector] public int m_LevelToLoad;
    private PlayerSpawner m_spawner;
    private bool LoadEventSent = false;
    [Range(0, 60)][SerializeField] private float _debugDelayToCheckLoad = 5f;
    public enum LoadingState { Idle, Enter, Middle, Exit, Playing}
    [SerializeField] private bool _isGameplayScene = false;
    private bool _goScreen = false;
    [SerializeField] private bool _forceGoNoPress = false;
    [SerializeField] private GameState _IntendedGameStateAfterLoading = GameState.Playing;
    private bool _isSubscribed = false;
    private bool _isSpawnDone = false;
    private bool SpawnDone { get { if (_isGameplayScene) return _isSpawnDone; else return true; } }

    private bool CheckCanPress { get { return GameManager.instance.IsLoading && _loadingState == LoadingState.Middle && SpawnDone && !_goScreen && _delayManager.CanShoot; } }
    private bool CheckNoPressNeeded { get { return GameManager.instance.IsLoading && _loadingState == LoadingState.Middle && !_goScreen && _delayManager.CanShoot && _forceGoNoPress; } }

    // Use this for initialization
    void Start ()
    {
        GetObjects();

        StartCoroutine(CheckIfNotStuck());
        EnterScreens();
	}

    void OnLevelWasLoaded(int level)
    {
        if (level == m_LevelToLoad)
            OnLoadDone();
    }

    private void Update()
    {
        if (CheckNoPressNeeded)
        {
            _goScreen = GoScreen();
        }
        else if (CheckCanPress && _isGameplayScene)
        {
            for (int i = 0; i < m_spawner.ListOfPlayerDatas.Count; i++)
            {
                if (m_MenuControls.Confirm[i] || m_MenuControls._Start[i])
                {
                    _goScreen = GoScreen();
                }
            }

            if (_displayLoading)
            {
                _displayLoading = false;
            }
        }
        else if (CheckCanPress && !_isGameplayScene)
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_MenuControls.Confirm[i] || m_MenuControls._Start[i])
                {
                    _goScreen = GoScreen();
                }
            }

            if (_displayLoading)
            {
                _displayLoading = false;
            }
        }
    }

    private Sprite GetSpriteToDisplay()
    {
        Sprite ret = null;

        switch (_type)
        {
            case LoadingType.FullyRandom:
                ret = _tipScreens[UnityEngine.Random.Range(0, _tipScreens.Count - 1)];
                break;
            case LoadingType.PartiallyRandom:
                if (_randomRange.Count > 1) {
                    if (_randomRange[1] >= _randomRange.Count) _randomRange[1] = _randomRange[1] - 1;
                    ret = _tipScreens[UnityEngine.Random.Range(_randomRange[0], _randomRange[1] - 1)];
                } else
                    Debug.LogError("Loading screen random range does not contain 2 values a minimum and a maximum for ranged display.");
                break;

            case LoadingType.Single:
                if(_randomRange.Count > 0)
                {
                    if (_randomRange[0] < 0) _randomRange[0] = 0;
                    else if(_randomRange[0] >= _randomRange.Count) _randomRange[1] = _randomRange[1] - 1;
                    ret = _tipScreens[_randomRange[0]];
                }else
                    Debug.LogError("Loading screen random range does not contain a value for single display.");
                break;
        }

        return ret;
    }

    IEnumerator CheckIfNotStuck()
    {
        yield return new WaitForSeconds(_debugDelayToCheckLoad);
        if (!LoadEventSent) OnLoadDone();
    }

    private void SetDisplay()
    {
        _toDisplay = GetSpriteToDisplay();
        _tipScreenSprite.sprite = _toDisplay;
    }

    private void EnterScreens()
    {
        SetDisplay();
        _loadingState = LoadingState.Enter;
        _animator.SetInteger("Target", UnityEngine.Random.Range(1, 4));
        _delayManager.AddDelay(_minimumLoadTime);
        _displayLoading = true;
        StartCoroutine(ChangeLoadingText());
    }

    private bool GoScreen()
    {
        _loadingState = LoadingState.Exit;
        _animator.SetBool("Start", true);
        return true;
    }

    public void ResetGo()
    {
        _animator.SetBool("Start", false);
        _animator.SetBool("Restart", false);
        _animator.SetInteger("Target", 0);
        BroadcastLoadingAnimDone();
        _loadingState = LoadingState.Playing;
        _goScreen = false;
        if (!_isGameplayScene) GameManager.instance.SetGameState(_IntendedGameStateAfterLoading);
    }

    public void SetStateMiddle()
    {
        _loadingState = LoadingState.Middle;
    }

    public void Respawn()
    {
        _isSpawnDone = false;
        _animator.SetBool("Restart", true);
        EnterScreens();
    }

    public event EventHandler LoadDone;

    protected virtual void OnLoadDone()
    {
        EventHandler handler = LoadDone;
        if (handler != null) handler(this, EventArgs.Empty);
        LoadEventSent = true;
    }

    public void BroadcastLoadingAnimDone()
    {
        OnLoadingAnimDone();
    }

    public event EventHandler LoadingAnimDone;

    protected virtual void OnLoadingAnimDone()
    {
        EventHandler handler = LoadingAnimDone;
        if (handler != null) handler(this, EventArgs.Empty);
    }

    private void M_spawner_SpawnDone(object sender, EventArgs e)
    {
        _isSpawnDone = true;
    }

    private void GetObjects()
    {
        m_MenuControls = FindObjectOfType<MenuControls>();
        if (m_MenuControls == null) Debug.LogError("Loading Screen cannot find  Menu control object in scene.");

        _delayManager = GetComponent<DelayManager>();
        if (_delayManager == null) Debug.LogError("No delay manager found on loading screen");

        if (_tipScreenGameObject == null) Debug.LogError("Loading screen is missing the tip screen game object.");
        else
        {
            _tipScreenSprite = _tipScreenGameObject.GetComponent<SpriteRenderer>();
            SetRandomColor();
        }

        _animator = GetComponent<Animator>();
        if (_animator == null) Debug.LogError("Loading Screen is missing its animator. PANIC!");

        if (_isGameplayScene)
        {
            m_spawner = FindObjectOfType<PlayerSpawner>();
            if (m_spawner == null) Debug.LogError("Loading Screen cannot find player spawner. Is this a gameplay scene? If yes, set the IsGameplayScene to true.");
            else SubscribeToEvents(m_spawner);
        }
    }

    public void SubscribeToEvents(PlayerSpawner toSub)
    {
        if (!_isSubscribed)
        {
            if (toSub != null)
            {
                toSub.SpawnDone += M_spawner_SpawnDone;
                _isSubscribed = true;
            }
        }
    }

    public void SetRandomColor()
    {
        SpriteRenderer spr = _coloredArea.GetComponent<SpriteRenderer>();
        spr.color = _colorsForBack[UnityEngine.Random.Range(0,_colorsForBack.Count)];
    }

    private IEnumerator ChangeLoadingText()
    {
        int i = 0;
        while (_displayLoading)
        {
            yield return new WaitForSeconds(_loadingTextDisplayTime);
            i++;
            if (i > 3) i = 0;
            _LoadingText.sprite = LoadingTextSprites[i];
        }

        SetReadyText();
    }

    private void SetReadyText()
    {
        _LoadingText.sprite = LoadingTextSprites[4];
    }
}