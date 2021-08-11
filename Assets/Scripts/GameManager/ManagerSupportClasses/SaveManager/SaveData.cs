using System; // Serializable
using System.Collections.Generic; // List
using System.ComponentModel; // TypeDescriptor
using System.Xml.Serialization; // XmlRoot, XmlElement
using UnityEngine; // SerializeField

/// <summary>
/// Serializable SaveData class built to mirror PlayerPrefs implementation.
/// </summary>
[XmlRoot("SaveData")]
[Serializable]
public class SaveData
{
    // 'public' for XmlSerializer access
    // Do not modify m_keyValuePairs directly
    // Use a Public Instance Method
    [XmlElement("KeyValuePair")]
    [SerializeField]
    public List<KeyValuePair> m_keyValuePairs;

    public SaveData()
    {
        m_keyValuePairs = new List<KeyValuePair>();
    }

    #region Private Instance Methods
    private KeyValuePair GetKeyValuePair(string key)
    {
        KeyValuePair ret = null;

        foreach (KeyValuePair keyValuePair in m_keyValuePairs)
        {
            if (string.Equals(
                keyValuePair.key,
                key,
                StringComparison.CurrentCulture))
            {
                ret = keyValuePair;
                break;
            }
        }

        return ret;
    }

    private T GetValue<T>(string key, T defaultValue)
    {
        KeyValuePair keyValuePair = GetKeyValuePair(key);

        if (keyValuePair == null)
        {
            SetKeyValuePair(key, defaultValue.ToString());
        }
        else
        {
            defaultValue =
                (T)TypeDescriptor.GetConverter(typeof(T))
                .ConvertFromString(keyValuePair.value);
        }

        return defaultValue;
    }

    private void SetKeyValuePair(string key, string value,
        KeyValuePair keyValuePair = null)
    {
        if (keyValuePair == null)
        {
            // Generate pool and/or hold previously used KeyValuePairs?
            keyValuePair = new KeyValuePair();
            keyValuePair.key = key;
            keyValuePair.value = value;
            m_keyValuePairs.Add(keyValuePair);
        }
        else
        {
            keyValuePair.value = value;
        }
    }
    #endregion

    #region Public Instance Methods
    // Summary: Removes all keys and values from the preferences.
    // Use with caution.
    public void DeleteAll()
    {
        for (int i = m_keyValuePairs.Count - 1; i > -1; i--)
        {
            m_keyValuePairs[i].key = null;
            m_keyValuePairs[i].value = null;
            m_keyValuePairs.RemoveAt(i);
        }

        // TODO - Save/Sync?
    }
    // Summary: Removes key and its corresponding value from the preferences.
    //
    // Parameters:
    //   key:
    public void DeleteKey(string key)
    {
        KeyValuePair keyValuePair = GetKeyValuePair(key);

        if (keyValuePair != null)
        {
            keyValuePair.key = null;
            keyValuePair.value = null;
            m_keyValuePairs.Remove(keyValuePair);
        }

        // TODO - Save/Sync?
    }
    // Summary: Returns the value corresponding to key in the preference file
    // if it exists.
    //
    // Parameters:
    //   key:
    //
    //   defaultValue:
    public float GetFloat(string key, float defaultValue)
    {
        return GetValue(key, defaultValue);
    }
    // Summary: Returns the value corresponding to key in the preference file
    // if it exists.
    //
    // Parameters:
    //   key:
    //
    //   defaultValue:
    public float GetFloat(string key)
    {
        return GetFloat(key, 0);
    }
    // Summary: Returns the value corresponding to key in the preference file
    // if it exists.
    //
    // Parameters:
    //   key:
    //
    //   defaultValue:
    public int GetInt(string key, int defaultValue)
    {
        return GetValue(key, defaultValue);
    }
    // Summary: Returns the value corresponding to key in the preference file
    // if it exists.
    //
    // Parameters:
    //   key:
    //
    //   defaultValue:
    public int GetInt(string key)
    {
        return GetInt(key, 0);
    }
    // Summary: Returns the value corresponding to key in the preference file
    // if it exists.
    //
    // Parameters:
    //   key:
    //
    //   defaultValue:
    public string GetString(string key, string defaultValue)
    {
        return GetValue(key, defaultValue);
    }
    // Summary: Returns the value corresponding to key in the preference file
    // if it exists.
    //
    // Parameters:
    //   key:
    //
    //   defaultValue:
    public string GetString(string key)
    {
        return GetString(key, "");
    }
    // Summary: Returns true if key exists in the preferences.
    //
    // Parameters:
    //   key:
    public bool HasKey(string key)
    {
        return GetKeyValuePair(key) != null;
    }
    // Summary: Writes all modified preferences to disk.
    //[NativeMethod("Sync")]
    public void SaveDep() { /* TODO - Deprecate for other method */ }
    // Summary: Sets the value of the preference identified by key.
    //
    // Parameters:
    //   key:
    //
    //   value:
    public void SetFloat(string key, float value)
    {
        SetKeyValuePair(key, value.ToString(), GetKeyValuePair(key));

        // TODO - Save/Sync?
    }
    // Summary: Sets the value of the preference identified by key.
    //
    // Parameters:
    //   key:
    //
    //   value:
    public void SetInt(string key, int value)
    {
        SetKeyValuePair(key, value.ToString(), GetKeyValuePair(key));

        // TODO - Save/Sync?
    }
    // Summary: Sets the value of the preference identified by key.
    //
    // Parameters:
    //   key:
    //
    //   value:
    public void SetString(string key, string value)
    {
        SetKeyValuePair(key, value, GetKeyValuePair(key));

        // TODO - Save/Sync?
    }
    #endregion
}
