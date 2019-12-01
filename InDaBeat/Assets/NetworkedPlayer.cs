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

    void Update()
    {
        if (photonView.IsMine)
        {
            Quaternion rot = new Quaternion(0, Camera.main.transform.rotation.y, 0, Camera.main.transform.rotation.w);
            this.transform.rotation = rot;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("Serializing view");
        if (stream.IsWriting)
        {
            Vector3 pos = new Vector3(playerGlobal.position.x, 0.05f, playerGlobal.position.z);
            stream.SendNext(playerGlobal.position);
            Quaternion rot = new Quaternion(0, playerGlobal.rotation.y, 0, playerGlobal.rotation.w);
            stream.SendNext(rot);
            Vector3 pos1 = new Vector3(playerLocal.localPosition.x, 0.05f, playerLocal.localPosition.z);
            stream.SendNext(pos1);
            Quaternion rot2 = new Quaternion(0, playerLocal.localRotation.y, 0, playerLocal.localRotation.w);
            stream.SendNext(rot2);
        }
        else
        {
            Vector3 pos = (Vector3)stream.ReceiveNext();
            this.transform.position = new Vector3(pos.x, 0.05f, pos.z);
            Quaternion rot = (Quaternion)stream.ReceiveNext();
            this.transform.rotation = new Quaternion(0, rot.y, 0, rot.w);
            Vector3 pos1 = (Vector3)stream.ReceiveNext();
            avatar.transform.localPosition = new Vector3(pos1.x, 0.05f, pos1.z);
            Quaternion rot1 = (Quaternion)stream.ReceiveNext();
            avatar.transform.localRotation = new Quaternion(0, rot1.y, 0, rot1.w);
        }
    }
}