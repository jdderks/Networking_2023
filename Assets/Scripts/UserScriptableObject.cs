using System;
using UnityEngine;

[Serializable]
public class UserScriptableObject : ScriptableObject
{
    public string username;
    public string password;
    public string email;
    public string birthdate;
    public bool anonymous;
}

[Serializable]
public class UserData
{
    public int id;
    public string name;
    public string mailadress;
    public int reg_date;
    public int birth_date;
    public bool anonymous;
}
