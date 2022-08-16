using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendManager : MonoBehaviour
{
    private static BackendManager m_Instance;
    public static BackendManager Instance
    {
        get
        {
            if (!m_Instance)
            {
                m_Container = new GameObject();
                m_Container.name = "BackendManager";
                m_Instance = m_Container.AddComponent(
                    typeof(BackendManager)) as BackendManager;
                m_Instance = m_Container.AddComponent(
                    typeof(BackendInitializer)) as BackendManager;
                m_Instance = m_Container.AddComponent(
                    typeof(BackendFederationAuth)) as BackendManager;
                m_Instance = m_Container.AddComponent(
                    typeof(BackendChart)) as BackendManager;
                DontDestroyOnLoad(m_Container);
            }

            return m_Instance;
        }
    }

    private static GameObject m_Container;

    public BackendInitializer BkendInizer { get; private set; }
    public BackendFederationAuth BkendFedAuth { get; private set; }
    public BackendChart BkendChart { get; private set; }

    private void Awake()
    {
        BkendInizer = GetComponent<BackendInitializer>();
        BkendFedAuth = GetComponent<BackendFederationAuth>();
        BkendChart = GetComponent<BackendChart>();
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