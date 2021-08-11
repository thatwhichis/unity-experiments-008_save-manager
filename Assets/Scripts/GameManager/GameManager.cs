using UnityEngine;

/// <summary>
/// GameManager class to manage singleton GameManager Instance
/// Controls global game behaviors and persists between scenes
/// </summary>
[DefaultExecutionOrder(-9999)]
public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public static GameManager Instance 
    {
        get { return m_instance; } 
        private set { m_instance = value; }
    }

    [SerializeField]
    private bool m_escapeExitsApplication;

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        // get references if necessary
    }

    void Update()
    {
        if (m_escapeExitsApplication)
        {
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
    }

    /// <summary>
    /// Ensure only one GameManager exists and persists
    /// </summary>
    void Initialize()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }
}
