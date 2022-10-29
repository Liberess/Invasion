using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendManager : MonoBehaviour
{
    private static BackendManager m_Instance;
    public static BackendManager Instance { get => m_Instance; }

    public BackendInitializer BkendInizer { get; private set; }
    public BackendFederationAuth BkendFedAuth { get; private set; }
    public BackendChart BkendChart { get; private set; }

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (m_Instance != this)
        {
            Destroy(this.gameObject);
        }
        
        BkendInizer = GetComponent<BackendInitializer>();
        BkendInizer.enabled = true;
        
        BkendFedAuth = GetComponent<BackendFederationAuth>();
        BkendFedAuth.enabled = true;
        
        BkendChart = GetComponent<BackendChart>();
        BkendChart.enabled = true;
    }

    private void Start()
    {
        try
        {
            BkendFedAuth.OnClickGPGSLogin();
        }
        catch
        {

        }
    }
}