using System; // Environment
using System.Collections.Generic; // Dictionary
using System.IO; // StreamWriter, File
using System.Xml.Serialization; // XmlSerializer
using System.Runtime.Serialization.Formatters.Binary; // BinaryFormatter
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private static SaveManager m_instance;
    public static SaveManager Instance 
    {
        get { return m_instance; }
        private set { m_instance = value; }
    }

    public enum Path
    {
        None,
        DataPath,
        DocumentsDirectory,
        PersistentDataPath,
        StreamingAssetsPath,
        TemporaryCachePath,
    }
    public enum DataType
    {
        BINARY,
        JSON,
        XML,
    }

    // SaveManager Default Values set in Inspector
    [SerializeField]
    private Path m_defaultPath = Path.PersistentDataPath;
    [SerializeField]
    private DataType m_defaultDataType = DataType.JSON;
    [SerializeField]
    private string m_defaultFileName = "save";

    private static Dictionary<DataType, string> m_dataTypeStringDictionary;
    private static Dictionary<Path, string> m_pathStringDictionary;
    private static Dictionary<string, SaveData> m_stringSaveDataDictionary;

    #region Unity Lifecycle
    private void Awake()
    {
        CheckInitialization();
    }

    void Start()
    {
        CheckInitialization();
    }

    void OnEnable()
    {
        CheckInitialization();
    }

    void OnDestroy()
    {
        // TODO
    }

    void OnValidate()
    {
        CheckInitialization();
    }
    #endregion

    #region Public Static Methods
    public static void RegisterSaveData(string saveDataName,
        ref SaveData saveData, bool overwriteDictionary = false)
    {
        if (m_stringSaveDataDictionary.ContainsKey(saveDataName))
        {
            if (overwriteDictionary)
            {
                m_stringSaveDataDictionary[saveDataName] = saveData;
            }
            else
            {
                saveData = m_stringSaveDataDictionary[saveDataName];
            }
        }
        else
        {
            m_stringSaveDataDictionary.Add(saveDataName, saveData);
        }
    }

    // TODO - check m_stringSaveDataDictionary.Contains(saveDataName)
    // TODO - take in a delegate and notify watchers to update saveData ref?
    // Could use EventDispatcher, but was hoping not to marry these...
    // Holding off; no sense solving current non-problems
    public static SaveData Load(string saveDataName, string filePath = "")
    {
        Instance.BuildLimitFilePath(ref filePath);

        SaveData saveData = null;

        if (File.Exists(filePath))
        {
            switch (Instance.m_defaultDataType)
            {
                case DataType.BINARY:
                    m_stringSaveDataDictionary[saveDataName] =
                        DeserializeBinaryFile<SaveData>(filePath);
                    saveData = m_stringSaveDataDictionary[saveDataName];
                    break;
                case DataType.XML:
                    m_stringSaveDataDictionary[saveDataName] =
                        DeserializeXmlFile<SaveData>(filePath);
                    saveData = m_stringSaveDataDictionary[saveDataName];
                    break;
                case DataType.JSON:
                default:
                    m_stringSaveDataDictionary[saveDataName] =
                        DeserializeJsonFile<SaveData>(filePath);
                    saveData = m_stringSaveDataDictionary[saveDataName];
                    break;
            }
        }
        else
        {
            // TODO
            Debug.Log("Failed to load: File does not exist: " + filePath);
        }

        return saveData;
    }

    public static void Save(string saveDataName, string filePath = "",
        bool overwriteExistingFile = true)
    {
        Instance.BuildLimitFilePath(ref filePath);

        if (overwriteExistingFile || !File.Exists(filePath))
        {
            Action<object, string> action;

            switch (Instance.m_defaultDataType)
            {
                case DataType.BINARY:
                    action = SerializeBinaryFile;
                    break;
                case DataType.XML:
                    action = SerializeXmlFile;
                    break;
                case DataType.JSON:
                default:
                    action = SerializeJsonFile;
                    break;
            }

            action.Invoke(m_stringSaveDataDictionary[saveDataName], filePath);
        }
        else
        {
            // TODO
            Debug.Log("Failed to save: File exists and isn't overwritten: " + filePath);
        }
    }

    public static void SetDefaultDataType(DataType dataType)
    {
        Instance.m_defaultDataType = dataType;
    }

    public static void SetDefaultFileName(string fileName)
    {
        Instance.m_defaultFileName = fileName;
    }

    public static void  SetDefaultPath(Path path)
    {
        Instance.m_defaultPath = path;
    }
    #endregion

    #region Private Instance Methods
    private void BuildLimitFilePath(ref string filePath)
    {
        if (filePath == "")
        {
            filePath =
                m_pathStringDictionary[m_defaultPath] +
                m_defaultFileName +
                m_dataTypeStringDictionary[Instance.m_defaultDataType];
        }
        else
        {
            filePath =
                // Confine the user to default directory
                // Try not to allow unrestricted access
                m_pathStringDictionary[Instance.m_defaultPath] +
                filePath +
                m_dataTypeStringDictionary[Instance.m_defaultDataType];
        }
    }

    private void CheckInitialization()
    {
        // Paired with GameManager Prefab
        // GameManager executes first in Script Execution Order
        // Evaluation never reaches if GameManager destroys a duplicate
        if (!Instance )Instance = this;

        if (m_dataTypeStringDictionary == null)
        {
            InitializeDataTypeStringDictionary();
        }

        if (m_pathStringDictionary == null)
        {
            InitializePathStringDictionary();
        }

        if (m_stringSaveDataDictionary == null)
        {
            m_stringSaveDataDictionary = new Dictionary<string, SaveData>();
        }
    }

    private void InitializeDataTypeStringDictionary()
    {
        m_dataTypeStringDictionary = new Dictionary<DataType, string>();

        m_dataTypeStringDictionary.Add(DataType.BINARY, ".bytes");
        m_dataTypeStringDictionary.Add(DataType.JSON, ".json");
        m_dataTypeStringDictionary.Add(DataType.XML, ".xml");
    }

    private void InitializePathStringDictionary()
    {
        m_pathStringDictionary = new Dictionary<Path, string>();

        m_pathStringDictionary.Add(Path.DataPath,
            Application.dataPath);
        // At least on Windows this uses "\" instead of "/"
        // BUT, still works per expectation
        m_pathStringDictionary.Add(Path.DocumentsDirectory,
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        m_pathStringDictionary.Add(Path.PersistentDataPath,
            Application.persistentDataPath);
        m_pathStringDictionary.Add(Path.StreamingAssetsPath,
            Application.streamingAssetsPath);
        m_pathStringDictionary.Add(Path.TemporaryCachePath,
            Application.temporaryCachePath);

        List<Path> keys = new List<Path>(m_pathStringDictionary.Keys);

        foreach (Path key in keys)
        {
            m_pathStringDictionary[key] += "/";
        }
    }
    #endregion

    // TODO - can some/all of this be replaced with UnityWebRequest?
    #region Public Static Serialization Methods - Move?
    /// <summary>
    /// Serializes Binary configured object by Type to filePath
    /// </summary>
    /// <param name="item">Object to be serialized</param>
    /// <param name="filePath">Filepath to write out</param>
    public static void SerializeBinaryFile(object item, string filePath)
    {
        using (BinaryWriter binaryWriter = 
            new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(binaryWriter.BaseStream, item);
        }
    }

    /// <summary>
    /// Deserialized Binary configured object by given Type from filePath
    /// </summary>
    /// <typeparam name="T">Binary configured object type to return</typeparam>
    /// <param name="s_file">Filepath to read in</param>
    /// <returns>Given Binary configured object type</returns>
    public static T DeserializeBinaryFile<T>(string filePath) where T : class
    {
        T deserialized = null;

        using (BinaryReader binaryReader = 
            new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            deserialized = 
                (T)binaryFormatter.Deserialize(binaryReader.BaseStream);
        }

        return deserialized;
    }

    /// <summary>
    /// Serializes JSON configured object by Type to filePath
    /// </summary>
    /// <param name="item">Object to be serialized</param>
    /// <param name="filePath">Filepath to write out</param>
    public static void SerializeJsonFile(object item, string s_filePath)
    {
        // Second field is bool prettyPrint
        string json = JsonUtility.ToJson(item, true);

        File.WriteAllText(s_filePath, json);
    }

    /// <summary>
    /// Deserialized JSON configured object by given Type from filePath
    /// </summary>
    /// <typeparam name="T">JSON configured object type to return</typeparam>
    /// <param name="s_file">Filepath to read in</param>
    /// <returns>Given JSON configured object type</returns>
    public static T DeserializeJsonFile<T>(string filePath)
    {
        string json = File.ReadAllText(filePath);

        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// Serializes XML configured object by Type to filePath
    /// </summary>
    /// <param name="item">Object to be serialized</param>
    /// <param name="filePath">Filepath to write out</param>
    public static void SerializeXmlFile(object item, string filePath)
    {
        using (StreamWriter streamWriter = new StreamWriter(filePath))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(item.GetType());

            xmlSerializer.Serialize(streamWriter.BaseStream, item);
        }
    }

    /// <summary>
    /// Deserialized XML configured object by given Type from filePath
    /// </summary>
    /// <typeparam name="T">XML configured object type to return</typeparam>
    /// <param name="s_file">Filepath to read in</param>
    /// <returns>Given XML configured object type</returns>
    public static T DeserializeXmlFile<T>(string filePath) where T : class
    {
        T deserialized = null;

        using (StreamReader streamReader = new StreamReader(filePath))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            deserialized = (T)xmlSerializer.Deserialize(streamReader);
        }

        return deserialized;
    }
    #endregion
}
