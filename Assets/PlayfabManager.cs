using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class PlayfabManager : MonoBehaviour
{
    public TMP_InputField emailInput, passwordInput, usernameInput;
    public GameObject LoginPanel, VolumePanel;
    public Slider volumeSlider, volumeSlider1;
    public Text usernameText, displayNameText;
    string username = "", myId, displayName = "";

    public void RegisterButton()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            Username = usernameInput.text,
            DisplayName = usernameInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        LoginPanel.SetActive(false);
        VolumePanel.SetActive(true);
        myId = result.PlayFabId;
        GetPlayerData();
        username = result.Username;
        displayName = result.Username;
        usernameText.text = username;
        displayNameText.text = displayName;
        print("Register successfull!");
    }

    void OnError(PlayFabError error)
    {
        print(error.ErrorMessage);
    }

    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }

    void OnLoginSuccess(LoginResult result)
    {
        LoginPanel.SetActive(false);
        VolumePanel.SetActive(true);
        print(result.InfoResultPayload.PlayerProfile.DisplayName);
        print("Logged in");
        username = result.InfoResultPayload.PlayerProfile.DisplayName;
        displayName = result.InfoResultPayload.PlayerProfile.DisplayName;
        usernameText.text = username;
        displayNameText.text = displayName;
        myId = result.PlayFabId;
        GetPlayerData();
    }

    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput.text,
            TitleId = "7B8D9"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        print("Recovery mail sent!");
    }

    public void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myId,
            Keys = null
        }, UserDataSuccess, OnError);
    }

    void UserDataSuccess(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("Volume"))
        {
            float value = float.Parse(result.Data["Volume"].Value);
            volumeSlider.value = value;
        }

        if (result.Data != null && result.Data.ContainsKey("Volume1"))
        {
            float value = float.Parse(result.Data["Volume1"].Value);
            volumeSlider1.value = value;
        }
    }

    public void SaveButton() { SetUserData(volumeSlider.value, volumeSlider1.value); }

    public void SetUserData(float volume, float volume1)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"Volume",  volume.ToString()},
                {"Volume1",  volume1.ToString() }
            }
        }, SetDataSuccess, OnError);
    }

    void SetDataSuccess(UpdateUserDataResult result)
    {
        print("Updated");
    }
}
