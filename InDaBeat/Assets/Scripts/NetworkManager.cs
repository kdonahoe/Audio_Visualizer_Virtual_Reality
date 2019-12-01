using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.Android;
using TMPro;
using System;

public class NetworkManager : MonoBehaviourPunCallbacks, IMatchmakingCallbacks, IOnEventCallback, IConnectionCallbacks
{
    string _room = "SongSelector";

    public const byte InstantiateVrAvatarEventCode = 1; // example code, change to any value between 1 and 199
    public const byte ChangeSongIndexEventCode = 2;
    public const byte SendToAudioVisualScene = 3;
    public List<GameObject> avatarPrefabs = new List<GameObject>();
    public GameObject JukeBoxCtrl;
    GameObject localAvatar;

    public TextMeshProUGUI debugObject;
    public Recorder recorder;
    GameObject dialogue = null;

    bool joinedRoom;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        Debug.Log("NetworkManager enabled.");
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions roomOptions = new RoomOptions() { };
            PhotonNetwork.JoinOrCreateRoom(_room, roomOptions, TypedLobby.Default);
        }
        else
        {
            Debug.Log("Trying to connect using settings");
            if (PhotonNetwork.ConnectUsingSettings())
            {
                Debug.Log("Success connecting using Photon settings!");
            }
            else
            {
                Debug.LogError("Failed to connect using Photon settings");
            }
        }

#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            dialogue = new GameObject();
        }
#endif

        recorder.StartRecording();
    }

    void OnGUI()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            dialogue.AddComponent<PermissionsRationaleDialog>();
            return;
        }
        else if (dialogue != null)
        {
            Destroy(dialogue);
        }
#endif
    }

    void Update()
    {
        if (!recorder.IsRecording)
        {
            recorder.RestartRecording();
            recorder.StartRecording();
        }
        //string newText = "";
        //newText += recorder.LevelMeter.CurrentAvgAmp + "|" + recorder.LevelMeter.CurrentPeakAmp + Environment.NewLine;
        //newText += "Is Recording: " + recorder.IsRecording + Environment.NewLine;
        //newText += "Is ActiveAndEnabled: " + recorder.isActiveAndEnabled + Environment.NewLine;
        //newText += "Is CurrentlyTransmitting: " + recorder.IsCurrentlyTransmitting + Environment.NewLine;
        //newText += "Is Initialized: " + recorder.IsInitialized + Environment.NewLine;
        //newText += "PhotonMicrophoneDeviceId: " + recorder.PhotonMicrophoneDeviceId + System.Environment.NewLine;
        //newText += "PhotonMicrophoneEnumerator Count: " + Recorder.PhotonMicrophoneEnumerator.Count + Environment.NewLine;
        //var enumerator = Recorder.PhotonMicrophoneEnumerator;
        //string trying = "";
        //for (int i = 0; i < enumerator.Count; i++)
        //{
        //    trying = enumerator.IDAtIndex(i) + "";
        //    recorder.PhotonMicrophoneDeviceId = enumerator.IDAtIndex(i);
        //    recorder.IsRecording = true;
        //    recorder.TransmitEnabled = true;
        //    break;
        //}

        //newText += "Trying to set mic: " + trying + Environment.NewLine;
        //if (Microphone.devices == null || Microphone.devices.Length == 0)
        //{
        //    newText += "No microphone device detected! " + Environment.NewLine;
        //}
        //else if (Microphone.devices.Length == 1)
        //{
        //    newText += string.Format("Mic.: {0}", Microphone.devices[0]) + Environment.NewLine;
        //}
        //else
        //{
        //    newText += string.Format("Multi.Mic.Devices:\n0. {0} (active)\n", Microphone.devices[0]) + Environment.NewLine;
        //    for (int i = 1; i < Microphone.devices.Length; i++)
        //    {
        //        newText += string.Format("{0}. {1}\n", i, Microphone.devices[i]) + Environment.NewLine;
        //    }
        //}
        //debugObject.text = newText;

    }

    public void UpdateSongIndex(int newIndex)
    {
        Debug.Log("Entered UpdateSongIndex");
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.Others
        };

        PhotonNetwork.RaiseEvent(ChangeSongIndexEventCode, newIndex, raiseEventOptions, SendOptions.SendReliable);
    }

    public void SendClientsToAudioVisualScene()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            CachingOption = EventCaching.AddToRoomCache,
            Receivers = ReceiverGroup.All
        };

        PhotonNetwork.RaiseEvent(SendToAudioVisualScene, null, raiseEventOptions, SendOptions.SendReliable);
    }

    public override void OnConnectedToMaster() //Callback function for when the first connection is established successfully.
    {
        Debug.Log("Connected to master");

        RoomOptions roomOptions = new RoomOptions() { };
        PhotonNetwork.JoinOrCreateRoom(_room, roomOptions, TypedLobby.Default);
    }

    #region IMatchmakingCallbacks

    public override void OnJoinedRoom()
    {
        joinedRoom = true;

        // This is just a sphere that represents the player so we can easily see other players
        Debug.Log("Trying to instantiate player prefab");
        //PhotonNetwork.Instantiate("NetworkedPlayerLocal", Vector3.zero, Quaternion.identity, 0);
        GameObject localAvatar = Instantiate(Resources.Load("AJ Local")) as GameObject;

        PhotonView photonView = localAvatar.GetComponent<PhotonView>();

        if (PhotonNetwork.AllocateViewID(photonView))
        {
            Debug.Log("View ID allocated");
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.AddToRoomCache,
                Receivers = ReceiverGroup.Others
            };

            PhotonNetwork.RaiseEvent(InstantiateVrAvatarEventCode, photonView.ViewID, raiseEventOptions, SendOptions.SendReliable);
        }
        else
        {
            Debug.LogError("Failed to allocate a ViewId.");

            Destroy(localAvatar);
        }
    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created room");

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room failed.");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join room failed.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join random failed.");
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Karaoke Room");
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == InstantiateVrAvatarEventCode)
        {
            Debug.Log("Entered instantiate vr avatar event code");
            GameObject remoteAvatar = Instantiate(Resources.Load(avatarPrefabs[UnityEngine.Random.Range(0, avatarPrefabs.Count)].name)) as GameObject;

            PhotonView photonView = remoteAvatar.GetComponent<PhotonView>();
            photonView.ViewID = (int)photonEvent.CustomData;
            Debug.Log("Instantiated Remote Avatar with View ID:");
            Debug.Log(photonView.ViewID);
        }

        else if (photonEvent.Code == ChangeSongIndexEventCode)
        {
            Debug.Log("Entered event for change song index");
            JukeBoxCtrl.GetComponent<JukeboxScript>().curIndex = (int)photonEvent.CustomData;
        }

        else if (photonEvent.Code == SendToAudioVisualScene)
        {
            Debug.Log("Entered event for changing scene");
            PhotonNetwork.LeaveRoom();
        }
    }


    #endregion
}