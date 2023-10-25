using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class WebManager : MonoBehaviour
{

    [SerializeField] private UserScriptableObject loggedInUser;

    #region login
    UIManager uiManager = GameManager.Instance.uiManager;

    string hostingUrl = "https://studentdav.hku.nl/~joris.derks/networking/";

    public void Login()
    {
        StartCoroutine(Login(uiManager.loginUsernameInputField.text, uiManager.loginPasswordInputField.text));
    }

    public IEnumerator Login(string username, string password)
    {
        WWWForm form = new();

        form.AddField("username", username);
        form.AddField("password", password);

        UnityWebRequest webRequest = UnityWebRequest.Post(hostingUrl + "user_login.php", form);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
        } 
        else
        {

        }
    }

    #endregion



    #region registration
    public void RegisterUser()
    {

        loggedInUser.username = uiManager.registrationUsernameInputField.text;
        loggedInUser.email = uiManager.mailadressInputField.text;
        loggedInUser.password = uiManager.registrationPasswordInputField.text;
        loggedInUser.birthdate = uiManager.GetDateInputs();
        loggedInUser.anonymous = uiManager.anonymousBox.isOn;

        StartCoroutine(RegisterUserEnumerator(loggedInUser));
    }

    //From https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html
    private IEnumerator RegisterUserEnumerator(UserScriptableObject criteria)
    {
        string requestURL = hostingUrl + "register_user.php";
        WWWForm form = new();

        //Form add criteria
        form.AddField("name", criteria.username);
        form.AddField("mailadress", criteria.email);
        form.AddField("password", criteria.password);
        form.AddField("birth_date", criteria.birthdate.ToString());
        form.AddField("anonymous", criteria.anonymous == true ? 1 : 0);

        UnityWebRequest webRequest = UnityWebRequest.Post(requestURL, form);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(webRequest.error);
        else
            Debug.Log(webRequest.downloadHandler.text);

        yield return null;
    }
    #endregion
}