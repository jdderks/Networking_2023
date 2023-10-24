using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager : MonoBehaviour
{

    [SerializeField] private UserScriptableObject loggedInUser;

    public void RegisterUser()
    {
        var uiManager = GameManager.Instance.uiManager;

        loggedInUser.username = uiManager.usernameInputField.text;
        loggedInUser.email = uiManager.mailadressInputField.text;
        loggedInUser.password = uiManager.passwordInputField.text;
        loggedInUser.birthdate = uiManager.GetDateInputs();
        loggedInUser.anonymous = uiManager.anonymousBox.isOn;

        StartCoroutine(RegisterUserEnumerator(loggedInUser));
    }

    //From https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html
    private IEnumerator RegisterUserEnumerator(UserScriptableObject criteria)
    {
        string requestURL = "https://studentdav.hku.nl/~joris.derks/networking/register_user.php";
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
}