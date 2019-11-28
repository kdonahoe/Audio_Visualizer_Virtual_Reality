using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks, IMatchmakingCallbacks, IOnEventCallback, IConnectionCallbacks
{
    string _room = "SongSelector";

    public const byte InstantiateVrAvatarEventCode = 1; // example code, change to any value between 1 and 199
    public const byte ChangeSongIndexEventCode = 2;

    public GameObject SceneCtrl;
    GameObject localAvatar;

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
        PhotonNetwork.AutomaticallySyncScene = true;

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

        /* // Instantiate localAvatar and make it follow the OVR tracking space
        localAvatar = Instantiate(Resources.Load("LocalAvatar")) as GameObject;
        localAvatar.GetComponent<OvrAvatar>().LeftHandCustomPose.transform.SetParent(GameObject.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor").transform);
        localAvatar.GetComponent<OvrAvatar>().RightHandCustomPose.transform.SetParent(GameObject.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor").transform);
        */

        // This is just a sphere that represents the player so we can easily see other players
        Debug.Log("Trying to instantiate player prefab");
        //PhotonNetwork.Instantiate("NetworkedPlayerLocal", Vector3.zero, Quaternion.identity, 0);
        GameObject localAvatar = Instantiate(Resources.Load("NetworkedPlayerLocal")) as GameObject;


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
        joinedRoom = false;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == InstantiateVrAvatarEventCode)
        {
            Debug.Log("Entered instantiate vr avatar event code");

            GameObject remoteAvatar = Instantiate(Resources.Load("NetworkedPlayerRemote")) as GameObject;
            PhotonView photonView = remoteAvatar.GetComponent<PhotonView>();
            photonView.ViewID = (int)photonEvent.CustomData;
            Debug.Log("Instantiated Remote Avatar with View ID:");
            Debug.Log(photonView.ViewID);
        }

        else if (photonEvent.Code == ChangeSongIndexEventCode)
        {
            Debug.Log("Entered event for change song index");
            SceneCtrl.GetComponent<SceneController1>().curIndex = (int)photonEvent.CustomData;
        }
    }

    #endregion
}