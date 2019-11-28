using UnityEngine;
using System.Collections;
using Photon;
using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;

// For use with Photon and SteamVR
public class NetworkedPlayer : MonoBehaviourPun, IPunObservable
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
            playerGlobal = GameObject.Find("OVRPlayerController").transform;

            playerLocal = playerGlobal.Find("OVRCameraRig/TrackingSpace/CenterEyeAnchor");

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
            /*stream.SendNext(lhandPose.position);
            stream.SendNext(rhandPose.position);

            Debug.Log("Serializing these hand positions:");
            Debug.Log(lhandPose.position);
            Debug.Log(rhandPose.position);*/
        }
        else
        {
            this.transform.position = (Vector3)stream.ReceiveNext();
            this.transform.rotation = (Quaternion)stream.ReceiveNext();
            avatar.transform.localPosition = (Vector3)stream.ReceiveNext();
            avatar.transform.localRotation = (Quaternion)stream.ReceiveNext();
            lhandLocal.transform.position = (Vector3)stream.ReceiveNext();
            rhandLocal.transform.position = (Vector3)stream.ReceiveNext();

            /*Debug.Log("Received these hand positions:");
            Debug.Log(lhandLocal.transform.position);
            Debug.Log(lhandLocal.transform.position);*/
        }
    }
}