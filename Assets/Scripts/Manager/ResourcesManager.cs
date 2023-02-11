using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

public sealed class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }
    
    [SerializeField] private List<AssetReference> assetRefList = new List<AssetReference>();

    private Dictionary<int, Object> resourceDic = new Dictionary<int, Object>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        
        LoadAssets();
    }

    private void LoadAssets()
    {
        foreach (var assetRef in assetRefList)
            LoadAsset(assetRef);
    }

    public Object LoadAsset(AssetReference assetReference, bool isCaching = true)
    {
        int hashCode = assetReference.GetHashCode();
        Object obj = null;

        // 캐싱체크를 하면 Dictionary에서 먼저 검색하여 기존에 로드한 오브젝트를 찾음
        if (isCaching)
            resourceDic.TryGetValue(hashCode, out obj);

        // 로드한 오브젝트가 없을 경우 비동기로 오브젝트를 로드한 후, 로드가 끝나면 콜백을 보냄
        if (obj == null)
        {
            var handle = Addressables.LoadAssetAsync<Sprite>(assetReference);
            handle.Completed +=
                handle =>
                {
                    obj = handle.Result;

                    if (isCaching)
                    {
                        if(!resourceDic.ContainsKey(hashCode))
                            resourceDic.Add(hashCode, obj);
                    }
                    
                    Addressables.Release(handle);
                };
        }

        return obj;
    }

    public async UniTaskVoid LoadAsset(AssetReference assetReference, System.Action<Object> successCallback,
        System.Action failCallback, bool isCaching = true)
    {
        int hashCode = assetReference.GetHashCode();
        Object obj = null;

        // 캐싱체크를 하면 Dictionary에서 먼저 검색하여 기존에 로드한 오브젝트를 찾음
        if (isCaching)
        {
            resourceDic.TryGetValue(hashCode, out obj);
            if (obj != null)
                successCallback?.Invoke(obj);
        }

        // 로드한 오브젝트가 없을 경우 비동기로 오브젝트를 로드한 후, 로드가 끝나면 콜백을 보냄
        if (obj == null)
        {
            AsyncOperationHandle<Object> handle = assetReference.LoadAssetAsync<Object>();

            if (handle.IsValid())
            {
                await UniTask.WaitUntil(() => handle.IsDone);
                //yield return handle;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    obj = handle.Result;
                    Addressables.Release(handle);
                
                    if (isCaching)
                        resourceDic.Add(hashCode, obj);
                
                    successCallback?.Invoke(obj);
                }
                else
                {
                    failCallback?.Invoke();
                }
            }
        }
    }

    // 캐싱을 하지 않은 오브젝트는 사용이 끝나면 Unload로 오브젝트를 없애주어야함
    public void ReleaseAsset(AssetReference assetReference)
    {
        assetReference.ReleaseAsset();
    }

    public void ReleaseAssets(out AsyncOperation release_AsyncOp)
    {
        resourceDic.Clear();
        release_AsyncOp = Resources.UnloadUnusedAssets();
    }
}