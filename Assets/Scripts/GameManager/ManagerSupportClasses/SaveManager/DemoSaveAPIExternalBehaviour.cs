using System;
using System.Collections;
using UnityEngine;

public class DemoSaveAPIExternalBehaviour : MonoBehaviour
{
    public SaveDataBehaviour m_saveDataBehaviour;

    void Start()
    {
        if (!m_saveDataBehaviour)
        {
            m_saveDataBehaviour = GetComponent<SaveDataBehaviour>();
        }

        StartCoroutine(IEnumerators.DoOnceDelayed(SetSaveDataPropertyTest, 5));
    }

    public void SetSaveDataPropertyTest()
    {
        m_saveDataBehaviour?.saveData
            .SetString("s_test_external", "testing_external");

        m_saveDataBehaviour?.Save();

        m_saveDataBehaviour?.Load();
    }
}
