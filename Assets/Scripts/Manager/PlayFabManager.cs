using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PlayFab;
using PlayFab.Json;
using PlayFab.ClientModels;
using GooglePlayGames;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager Instance { get; private set; }
    private DataManager dataMgr;
    private PopUpManager popUpMgr;

    public string PlayFabId { get; private set; }

    public event Action OnPlayFabLoginSuccessAction;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = "76E48";

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        OnPlayFabLoginSuccessAction = null;
    }

    private void Start()
    {
        dataMgr = DataManager.Instance;
        popUpMgr = PopUpManager.Instance;
        StartCoroutine(GoogleLogInCo());
    }

    public IEnumerator GoogleLogInCo()
    {
        Social.localUser.Authenticate((success) =>
        {
            if (success)
            {
                popUpMgr.PopUp("구글 로그인 성공!", EPopUpType.Notice);
                StartCoroutine(PlayFabLogInCo());
            }
            else
            {
                popUpMgr.PopUp("구글 로그인 실패!", EPopUpType.Caution);
            }
        });

        yield return null;
    }

    public IEnumerator GoogleLogOutCo()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
        popUpMgr.PopUp("구글 로그아웃!", EPopUpType.Notice);

        yield return null;
    }

    public IEnumerator PlayFabLogInCo()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = Social.localUser.id + "@rand.com",
            Password = Social.localUser.id
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLogInSuccess, OnLogInFailure);

        yield return null;
    }

    private void OnLogInSuccess(LoginResult result)
    {
        PlayFabId = result.PlayFabId;
        popUpMgr.PopUp("플레이팹 로그인 성공\n" + Social.localUser.userName, EPopUpType.Notice);

        StartCoroutine(LoadDataCo());
    }

    private void OnLogInFailure(PlayFabError error)
    {
        StartCoroutine(PlayFabRegisterCo());
    }

    private IEnumerator LoadDataCo()
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();

        yield return dataMgr.StartCoroutine(dataMgr.LoadDataCo());
        yield return delay;

        while(true)
        {
            if (dataMgr.HeroData.isLoadComplete && dataMgr.GameData.isLoadComplete)
                break;

            yield return delay;
        }

        OnPlayFabLoginSuccessAction?.Invoke();
        //yield return AudioManager.Instance.StartCoroutine(AudioManager.Instance.InitializedAudioSettingCo());
        yield return null;
    }

    public IEnumerator PlayFabRegisterCo()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = Social.localUser.id + "@rand.com",
            Password = Social.localUser.id,
            Username = Social.localUser.userName
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
        yield return null;
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        popUpMgr.PopUp("플레이팹 회원가입 성공\n" + Social.localUser.userName, EPopUpType.Notice);
        StartCoroutine(PlayFabLogInCo());
        //popUpMgr.PopUp("회원가입 성공!\n", EPopUpType.NOTICE);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        popUpMgr.PopUp("플레이팹 회원가입 실패!", EPopUpType.Caution);
        //popUpMgr.PopUp("회원가입 실패!\n" + error.GenerateErrorReport(), EPopUpType.CAUTION);
    }

    public void PlayFabErrorDebugLog(PlayFabError error)
    {
        Debug.LogError(error.ToString());
        popUpMgr.PopUp(error.ToString(), EPopUpType.Caution);
    }
}
