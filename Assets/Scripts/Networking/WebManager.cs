using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static System.Collections.Specialized.BitVector32;
using static System.Net.WebRequestMethods;

public class WebManager : MonoBehaviour
{

    [SerializeField] private UserScriptableObject loggedInUser;
    [SerializeField] private List<ScoreObject> scoreObjects = new();

    UIManager uiManager;
    private string sessionName = "";

    string hostingUrl = "https://studentdav.hku.nl/~joris.derks/networking/";

    public List<ScoreObject> ScoreObjects { get => scoreObjects; set => scoreObjects = value; }

    private void Awake()
    {
        uiManager = GameManager.Instance.uiManager;
    }

    #region sessions

    public void CloseSession()
    {
        StartCoroutine(EndSession());
    }
    private IEnumerator EndSession()
    {
        WWWForm form = new();

        form.AddField("session_name", sessionName);

        UnityWebRequest webRequest = UnityWebRequest.Post(hostingUrl + "end_session.php", form);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            if (webRequest.downloadHandler.text == sessionName)
            {
                //Connected succesfully to the session
                Debug.Log(webRequest.downloadHandler.text);
            }
            else
            {
                //Something went wrong with connecting to the session
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    public void ConnectToSession(string session)
    {
        StartCoroutine(SessionConnect(session));
    }

    private IEnumerator SessionConnect(string session)
    {
        WWWForm form = new();

        form.AddField("session_name", session);

        UnityWebRequest webRequest = UnityWebRequest.Post(hostingUrl + "session_connect.php", form);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            if (webRequest.downloadHandler.text == session)
            {
                //Connected succesfully to the session
                Debug.Log(webRequest.downloadHandler.text);
            }
            else
            {
                //Something went wrong with connecting to the session
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }

    public void StartSession()
    {
        StartCoroutine(CreateSession());
    }

    private IEnumerator CreateSession()
    {
        WWWForm form = new();

        sessionName = UnityEngine.Random.Range(0, 9999).ToString("D4");

        form.AddField("session_name", sessionName);

        UnityWebRequest webRequest = UnityWebRequest.Post(hostingUrl + "session_create.php", form);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            if (webRequest.downloadHandler.text == sessionName)
            {
                //Connected succesfully
                Debug.Log(webRequest.downloadHandler.text);
                uiManager.SessionNameText.text = webRequest.downloadHandler.text;
            }
            else
            {
                //Something went wrong
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }
    #endregion

    #region login
    public void Login()
    {
        StartCoroutine(Login(uiManager.loginUsernameInputField.text, uiManager.loginPasswordInputField.text));
    }

    private IEnumerator Login(string username, string password)
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

    #region leaderboard

    public void GetMonthlyScores(bool lastMonth = false)
    {
        StartCoroutine(FetchMonthlyScores(lastMonth));
    }

    private IEnumerator FetchMonthlyScores(bool lastMonth = false)
    {
        ScoreObjects = new List<ScoreObject>(); // Reset scores
        WWWForm form = new WWWForm();

        form.AddField("topLastMonth", lastMonth == true ? 1 : 0);

        UnityWebRequest webRequest = UnityWebRequest.Post(hostingUrl + "get_highscores.php", form);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            if (webRequest.responseCode == 200) // Check if the request was successful
            {
                ScoreObjects.Clear();
                string json = webRequest.downloadHandler.text;
                Debug.Log(json);

                string[] jsonArray = json.Split(new string[] { "[", "]", "{", "}", ":", ",", "name", "score", "time", "\"" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < jsonArray.Length; i += 3)
                {
                    ScoreObject high = new ScoreObject();
                    high.name = jsonArray[i];
                    Debug.Log(high);
                    Debug.Log(high.name);
                    Debug.Log(jsonArray[i + 2]); // Changed index to i + 2
                    high.score = long.Parse(jsonArray[i + 2]).ToString(); // Changed index to i + 2
                    Debug.Log(high.score);
                    ScoreObjects.Add(high);
                }



                if (ScoreObjects != null)
                {
                    uiManager.UpdateLeaderBoard(ScoreObjects);
                }
                else
                {
                    Debug.Log("Error deserializing JSON");
                }
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
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

