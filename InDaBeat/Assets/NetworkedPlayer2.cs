using UnityEngine;
using System.Collections;
using Photon;
using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;

// For use with Photon and SteamVR
public class NetworkedPlayer2 : MonoBehaviourPun, IPunObservable
{
    public GameObject avatar;

    public Transform playerGlobal;
    public Transform playerLocal;

    public GameObject lhandLocal;
    public Transform lhandPose;

    public GameObject rhandLocal;
    public Transform rhandPose;


    void Start()
    {
        Debug.Log("Player instantiated");

        if (photonView.IsMine)
        {
            Debug.Log("Player is mine");
            playerGlobal = GameObject.Find("PlayerController").transform;

            playerLocal = playerGlobal.Find("MainCamera/TrackingSpace/CenterEyeAnchor");

            lhandPose = GameObject.FindGameObjectWithTag("LHandAnchor").transform;

            rhandPose = GameObject.FindGameObjectWithTag("RHandAnchor").transform;

            this.transform.SetParent(playerLocal);
            this.transform.localPosition = Vector3.zero;

            lhandLocal.transform.SetParent(lhandPose);
            lhandLocal.transform.localPosition = Vector3.zero;

            rhandLocal.transform.SetParent(rhandPose);
            rhandLocal.transform.localPosition = Vector3.zero;


        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("Serializing view");
        if (stream.IsWriting)
        {
            stream.SendNext(playerGlobal.position);
            stream.SendNext(playerGlobal.rotation);
            stream.SendNext(playerLocal.localPosition);
            stream.SendNext(playerLocal.localRotation);
        }
        else
        {
            this.transform.position = (Vector3)stream.ReceiveNext();
            this.transform.rotation = (Quaternion)stream.ReceiveNext();
            avatar.transform.localPosition = (Vector3)stream.ReceiveNext();
            avatar.transform.localRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}