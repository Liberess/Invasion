using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class BackendFederationAuth : MonoBehaviour
{
    public void SetupGPGS()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
                .Builder()
            .RequestServerAuthCode(false)
            .RequestEmail()
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;

        PlayGamesPlatform.Activate();
        GPGSLogin();
    }

    private void GPGSLogin()
    {
        if (Social.localUser.authenticated == true)
        {
            BackendLoginWithGPGS();
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    BackendLoginWithGPGS();
                }
                else
                {
                    Debug.Log("Login failed for some reason");
                }
            });
        }
        
        if (PlayGamesPlatform.Instance.localUser.authenticated == false)
        {
            Social.localUser.Authenticate(Success =>
            {
                if (Success == false)
                {
                    Debug.Log("구글 로그인 실패");
                    //NoticeManager.Instance.Notice("구글 로그인 실패");
                    //BackEndManager.Instance.logErrorTxt.text = "구글 로그인 실패";
                    return;
                }

                Debug.Log("GetIDToken : " + PlayGamesPlatform.Instance.GetIdToken());
                Debug.Log("Email : " + ((PlayGamesLocalUser)Social.localUser).Email);
                Debug.Log("GoogleID : " + Social.localUser.id);
                Debug.Log("UserName : " + Social.localUser.userName);
                Debug.Log("UserName : " + PlayGamesPlatform.Instance.GetUserDisplayName());
            });
        }
        else
        {
            Debug.Log("이미 구글 로그인 함");
        }
    }
    
    public void OnClickGPGSLogin()
    {
        GPGSLogin();
    }

    private string GetTokens()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            string IDToken = PlayGamesPlatform.Instance.GetIdToken();
            return IDToken;
        }
        else
        {
            Debug.Log("접속되어있지 않습니다. 잠시 후 다시 시도하세요.");
            return null;
        }
    }

    private void BackendLoginWithGPGS()
    {
        Debug.Log("BackendLoginWithGPGS");
        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(
            GetTokens(), FederationType.Google, "gpgs");

        if (BRO.IsSuccess())
        {
            Debug.Log("구글 토큰으로 뒤끝서버 로그인 성공 (동기 방식)");
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "200":
                    Debug.Log("이미 회원가입된 회원입니다.");
                    break;
                case "403":
                    Debug.Log("차단된 사용자입니다. 차단 사유 : " + BRO.GetErrorCode());
                    break;
                default:
                    Debug.Log("서버 공통 에러 발생 : " + BRO.GetMessage());
                    break;
            }
        }
    }
    
    //이미 가입한 회원의 이메일 정보 저장
    public void OnClickUpdateEmail()
    {
        BackendReturnObject BRO = Backend.BMember.UpdateFederationEmail(GetTokens(), FederationType.Google);

        if (BRO.IsSuccess())
        {
            Debug.Log("이메일 주소 저장 완료");
        }
        else
        {
            if (BRO.GetStatusCode() == "404")
            {
                Debug.Log("FederationID not found, Federation을 찾을 수 없습니다.");
            }
            else
            {
                Debug.Log("서버 공통 에러 발생 : " + BRO.GetMessage());
            }
        }
    }

    //이미 가입된 상태인지 확인
    public void OnClickCheckUserAuthenticate()
    {
        BackendReturnObject BRO = Backend.BMember.CheckUserInBackend(GetTokens(), FederationType.Google);

        if (BRO.GetStatusCode() == "200")
        {
            Debug.Log("가입된 계정입니다.");
            Debug.Log(BRO.GetReturnValue());
            //BackEndManager.Instance.logTxt.text = "가입된 계정입니다. " + BRO.GetReturnValue();
        }
        else
        {
            Debug.Log("가입된 계정이 아닙니다.");
            //BackEndManager.Instance.logTxt.text = "가입된 계정이 아닙니다.";
        }
    }

    //커스텀 계정을 페더레이션 계정으로 변경
    public void OnClickChangeCustomToFederation()
    {
        BackendReturnObject BRO = Backend.BMember.ChangeCustomToFederation(GetTokens(), FederationType.Google);

        if (BRO.IsSuccess())
        {
            Debug.Log("페더레이션 계정으로 변경 완료");
            //BackEndManager.Instance.logTxt.text = "페더레이션 계정으로 변경 완료";
        }
        else
        {
            switch (BRO.GetStatusCode())
            {
                case "400":
                    if (BRO.GetErrorCode() == "BadParameterException")
                    {
                        Debug.Log("이미 ChangeCustomToFederation이 완료되었는데 다시 시도한 경우");
                        //BackEndManager.Instance.logErrorTxt.text = "이미 ChangeCustomToFederation이 완료되었는데 다시 시도한 경우";
                    }
                    else if (BRO.GetErrorCode() == "UndefinedParameterException")
                    {
                        Debug.Log("CustomLogin을 하지 않은 상황에서 시도한 경우");
                        //BackEndManager.Instance.logErrorTxt.text = "CustomLogin을 하지 않은 상황에서 시도한 경우";
                    }
                    break;
                default:
                    //BackEndManager.Instance.logErrorTxt.text = "서버 공통 에러 발생 : " + BRO.GetMessage();
                    break;
            }
        }
    }
}