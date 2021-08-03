using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class SaveManager : MonoBehaviour
{
    static bool active = false;

    public GameObject savePanel;
    public Button loginButton;
    public InputField inputField;
    public Text loginStatus;

    private void Start()
    {
        if (!active)
        {
            DontDestroyOnLoad(gameObject);
            active = true;
        }
        else
        {
            Destroy(gameObject);
        }

        if (!PlayerPrefs.HasKey("LEVEL"))
        {
            savePanel.SetActive(true);
        }
        else
        {
            Login(PlayerPrefs.GetString("email"));
        }
    }

    public void LoginPlayer()
    {
        PlayerPrefs.SetString("email", inputField.text);
        loginButton.interactable = false;
        loginStatus.text = "Logging In...";
        Login(PlayerPrefs.GetString("email"));
    }

    public void Login(string email)
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = email,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    public void SaveProgress()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"Level", PlayerPrefs.GetInt("LEVEL").ToString() }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    public void GetProgress()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
    }

    //Callbacks
    void OnSuccess(LoginResult result)
    {
        Debug.Log("Login Successful");
        if (!PlayerPrefs.HasKey("LEVEL"))
        {
            GetProgress();
        }
        else
        {
            SaveProgress();
        }
    }

    void OnDataRecieved(GetUserDataResult result)
    {
        Debug.Log("Recieved User Data!");
        if (result.Data != null && result.Data.ContainsKey("Level"))
        {
            int index = int.Parse(result.Data["Level"].Value);
            PlayerPrefs.SetInt("LEVEL", index);
            loginStatus.text = "Login / Restore Complete";
        } else
        {
            Debug.Log("New User! Data not found");
            PlayerPrefs.SetInt("LEVEL", 1);
            SaveProgress();
        }

        Invoke(nameof(HidePanel), 2f);
    }

    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Data sending successful");
    }

    void OnError(PlayFabError error)
    {
        loginButton.interactable = true;
        loginStatus.text = "Login Failed";
    }

    void HidePanel()
    {
        savePanel.SetActive(false);
    }
}
