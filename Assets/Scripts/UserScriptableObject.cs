using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoginCriteria", menuName = "ScriptableObjects/Login Criteria", order = 1)]
public class UserScriptableObject : ScriptableObject
{
    public string username;
    public string password;
    public string email;
    public string birthdate;
    public bool anonymous;
}
