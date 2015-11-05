using UnityEngine;
using System.Collections;

public class Activator : MonoBehaviour {
    [SerializeField] private Canvas _Canvas;
    [SerializeField] private FadeOut _Loader;
    public FadeOut Loader { get { return _Loader; } }

    private PlayerSpawner _spawner;

	void Awake()
    {

        if (_Canvas != null && !_Canvas.gameObject.activeInHierarchy) _Canvas.gameObject.SetActive(true);
        else if (_Canvas == null) Debug.LogError("Scene " + Application.loadedLevel + " can't find a Canvas, is this normal?");

        
        if (_Loader != null && !_Loader.gameObject.activeInHierarchy) _Loader.gameObject.SetActive(true);
        else if (_Loader == null) Debug.LogError("Scene " + Application.loadedLevel + " can't find a Loader, is this normal?");


        _spawner = FindObjectOfType<PlayerSpawner>();
        if (_spawner != null && _Loader != null) _spawner.SubscribeToEvents(_Loader);
    }

}
