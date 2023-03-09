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
    }

    public Object LoadAsset<T>(AssetReference assetReference, bool isCaching = true)
    {
        int hashCode = assetReference.RuntimeKey.GetHashCode();

        // 캐싱체크를 하면 Dictionary에서 먼저 검색하여 기존에 로드한 오브젝트를 찾음
        if (isCaching && resourceDic.TryGetValue(hashCode, out Object cachedObj))
            return cachedObj;

        var handle = Addressables.LoadAssetAsync<T>(assetReference);
        Object loadedObject = null;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            loadedObject = handle.Result as Object;
            Addressables.Release(handle);
            
            if (isCaching)
            {
                if (resourceDic.ContainsKey(hashCode))
                    resourceDic[hashCode] = loadedObject;
                else
                    resourceDic.Add(hashCode, loadedObject);
            }
        }

        return loadedObject;
    }

    public async UniTask LoadAsset(AssetReference assetReference, System.Action<Object> successCallback = null,
        System.Action failCallback = null, bool isCaching = true)
    {
        int hashCode = assetReference.RuntimeKey.GetHashCode();

        if (isCaching && resourceDic.TryGetValue(hashCode, out Object cachedObject))
        {
            Debug.Log(Time.time + " find exist");
            successCallback?.Invoke(cachedObject);
            return;
        }

        AsyncOperationHandle<Object> handle = assetReference.LoadAssetAsync<Object>();

        await handle.ToUniTask();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Object loadedObject = handle.Result;
            Addressables.Release(handle);

            if (loadedObject)
            {
                if (isCaching)
                {
                    if (resourceDic.ContainsKey(hashCode))
                    {
                        Debug.Log(Time.time + " find exist, rewrite");
                        resourceDic[hashCode] = loadedObject;
                    }
                    else
                    {
                        Debug.Log(Time.time + " can't find, add");
                        resourceDic.Add(hashCode, loadedObject);
                    }
                }

                successCallback?.Invoke(loadedObject);
            }
            else
            {
                failCallback?.Invoke();
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