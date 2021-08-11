using UnityEngine;

public class SaveDataBehaviour : MonoBehaviour
{
    [SerializeField]
    private bool m_runSaveLoadTestOnStart = false;
    [SerializeField]
    private bool m_initializeSaveFileIfNonexistent = false;
    [SerializeField]
    private bool m_loadSaveFileOnStart = false;
    [SerializeField]
    private string m_saveDataName = "save";
    [SerializeField]
    private SaveData m_saveData = new SaveData();

    public SaveData saveData
    {
        get { return m_saveData; }
        set { m_saveData = value; }
    }

    #region Unity Lifecycle
    void Start()
    {
        SaveManager.RegisterSaveData(m_saveDataName, ref m_saveData);

        // Defaults set via the UnityEditor SaveManager inspector
        if (m_initializeSaveFileIfNonexistent)
        {
            // Leaving the filePath (name of the file on disk) default
            SaveManager.Save(m_saveDataName, overwriteExistingFile: false);
        }

        if (m_loadSaveFileOnStart)
        {
            // Leaving the filePath (name of the file on disk) default
            m_saveData = SaveManager.Load(m_saveDataName);
        }

        if (m_runSaveLoadTestOnStart)
        {
            StartCoroutine(IEnumerators
                .DoOnceDelayed(RunSaveLoadTest, float.Epsilon));
        }
    }

    void OnDestroy()
    {
        // TODO
    }
    #endregion

    #region Private Instance Methods
    // Test run on Start should produce three save files
    // *.bytes, *.xml, and *.json
    private void RunSaveLoadTest()
    {
        // Set path input/output
        SaveManager.SetDefaultPath(SaveManager.Path.PersistentDataPath);

        // Set file name input/output
        SaveManager.SetDefaultFileName("saveLoadTest");

        // Set binary input/output
        SaveManager.SetDefaultDataType(SaveManager.DataType.BINARY);

        // Register the SaveData with the SaveManager
        SaveManager.RegisterSaveData(m_saveDataName, ref m_saveData, true);

        // Set some initial values
        m_saveData.SetInt("i_test_7", 7);
        m_saveData.SetFloat("f_test_4.2", 4.2f);
        m_saveData.SetString("s_test_helloWorld", "helloWorld");

        // Save in binary
        SaveManager.Save(m_saveDataName);

        // Purge
        m_saveData.DeleteAll();

        // Load from binary
        SaveManager.Load(m_saveDataName);

        // If we don't re-localize while loading, example:
        // (m_saveData = SaveManager.Load(m_saveDataName);)
        // re-register the reference
        SaveManager.RegisterSaveData(m_saveDataName, ref m_saveData);

        // Check data
        if (m_saveData.HasKey("s_test_helloWorld")) Debug.Log("helloWorld");

        // Delete from checked data
        m_saveData.DeleteKey("s_test_helloWorld");

        // Confirm deletion
        if (!m_saveData.HasKey("s_test_helloWorld")) Debug.Log("goodbye!");

        // Purge
        m_saveData.DeleteAll();

        // Test Get with defaultValue sets initial value
        int i = m_saveData.GetInt("i_test_5", 5);
        float f = m_saveData.GetFloat("f_test_6.3", 6.3f);
        string s = m_saveData.GetString("s_test_whatNow", "whatNow");

        // Set XML input/output
        SaveManager.SetDefaultDataType(SaveManager.DataType.XML);

        // Save in XML
        SaveManager.Save(m_saveDataName);

        // Purge
        m_saveData.DeleteAll();

        // Load from XML
        m_saveData = SaveManager.Load(m_saveDataName);

        // Check data and output all
        if (m_saveData.HasKey("i_test_5"))
            // i: 5, f: 6.3, s: whatNow
            Debug.Log("i: " + i + ", f: " + f + ", s: " + s);

        // Test Get without defaultValue for blank initialization
        s = m_saveData.GetString("s_test_null");
        i = m_saveData.GetInt("i_test_null");
        f = m_saveData.GetFloat("f_test_null");

        // Check data and output all
        if (m_saveData.HasKey("s_test_null"))
            // i: 0, f: 0, s: 
            Debug.Log("i: " + i + ", f: " + f + ", s: " + s);

        // Set JSON input/output
        SaveManager.SetDefaultDataType(SaveManager.DataType.JSON);

        // Save to JSON
        SaveManager.Save(m_saveDataName);

        // Purge
        m_saveData.DeleteAll();

        // Load from JSON
        m_saveData = SaveManager.Load(m_saveDataName);

        // Reset default file name
        SaveManager.SetDefaultFileName("save");
    }
    #endregion

    #region Public Instance Methods
    // For demo purposes, assuming the defaults have not been changed
    public void Load()
    {
        SaveManager.Load(m_saveDataName);
    }

    // For demo purposes, assuming the defaults have not been changed
    public void Save()
    {
        SaveManager.Save(m_saveDataName);
    }
    #endregion
}
