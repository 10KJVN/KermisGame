using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] [Tooltip("Go to:")] private string sceneName;
    public static string CurrentSceneName => SceneManager.GetActiveScene().name;

    // Persistant instance of this GameObject across scenes.
    private static SceneLoader Instance { get; set; }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        print(CurrentSceneName);
        Time.timeScale = 1;

        // if (CurrentSceneName == "Cutscene01")
        // {
        //     StartCoroutine(TransitionToOutro());
        // }
    }

    private void Update()
    {
        // if (Input.GetKey(KeyCode.Backspace))
        // {
        //     ReloadCurrentScene();
        // }
    }

    public static void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(CurrentSceneName);
    }
    
    // It seems like Start() only works in editor
    // This method seems more reliable and sure hit for builds.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);
        Debug.Log("Load Mode: " + mode);
        
        if (CurrentSceneName == "NarrativeScene01")
        {
            StartCoroutine(TransitionToGameplay());
        }

        if (CurrentSceneName == "Cutscene01")
        {
            StartCoroutine(TransitionToOutro());
        }
        
    }
    
    // Hardcoded scene, don't forget to adjust eventually
    private IEnumerator TransitionToGameplay()
    {
        yield return new WaitForSeconds(34f);
        SceneManager.LoadScene("TimerScene"); // My gameplay scene for now
    }
    
    // TODO: Refactor to work for cutscene length.
    private IEnumerator TransitionToOutro()
    {
        yield return new WaitForSeconds(30);
        SceneManager.LoadScene("NarrativeScene02");
    }
}
