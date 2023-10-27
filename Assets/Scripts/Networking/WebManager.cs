using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class WebManager : MonoBehaviour
{

    [SerializeField] private UserScriptableObject loggedInUser;

    #region login
    UIManager uiManager;


    string hostingUrl = "https://studentdav.hku.nl/~joris.derks/networking/";

    private void Awake()
    {
        uiManager = GameManager.Instance.uiManager;
    }

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
            if (webRequest.downloadHandler.text == "0") //0 is non descriptive code for failed to log in
            {
                Debug.Log("Login failed");
            }
            else if (webRequest.downloadHandler.text == "1") // 1 is non descriptive for empty fields
            {
                Debug.Log("There are one or more fields not filled in");
            }
            else
            {
                string userDataString = webRequest.downloadHandler.text;

                UserData userData = JsonUtility.FromJson<UserData>(userDataString);
                Debug.Log("logged in succesfully");

                loggedInUser.username = userData.name;
                loggedInUser.email = userData.mailadress;
                loggedInUser.password = "Wouldn't you like to know.";
                loggedInUser.birthdate = userData.birth_date.ToString();
                loggedInUser.anonymous = userData.anonymous;

                GameManager.Instance.LoggedIn = true;

                uiManager.CurrentlyLoggedInText.text = "Currently logged in as: " + userData.name;
            }
        }
    }
    #endregion

    #region registration
    public void RegisterUser()
    {
        var tempUserData = new UserScriptableObject();

        tempUserData.username = uiManager.registrationUsernameInputField.text;
        tempUserData.email = uiManager.mailadressInputField.text;
        tempUserData.password = uiManager.registrationPasswordInputField.text;
        tempUserData.birthdate = uiManager.GetDateInputs();
        tempUserData.anonymous = uiManager.anonymousBox.isOn;

        StartCoroutine(RegisterUserEnumerator(tempUserData));
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

    private void OnApplicationQuit()
    {
        loggedInUser = new();
        GameManager.Instance.LoggedIn = false;

    }

    private void OnDestroy()
    {
        loggedInUser = new();
        GameManager.Instance.LoggedIn = false;
    }
}

