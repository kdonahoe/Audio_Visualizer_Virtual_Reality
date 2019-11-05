using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneController1 : MonoBehaviour
{
    //static List<RaycastHit> hits = new List<RaycastHit>();
    OVRRaycaster raycastManager;
    public GameObject leftCtrl, rightCtrl, songDisplay;
    private TextMeshProUGUI songName;
    public List<string> songList = new List<string>();
    float sightLength = 2.0F;
    int curIndex;
    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<OVRRaycaster>();
        initiateSongList();
        curIndex = 0;
        songName = songDisplay.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        songName.text = songList[curIndex];

        RaycastHit leftHit, rightHit;
        Ray leftRay = new Ray(leftCtrl.transform.position, leftCtrl.transform.forward);
        Ray rightRay = new Ray(rightCtrl.transform.position, rightCtrl.transform.forward);

        if (Physics.Raycast(leftRay, out leftHit, sightLength))
        {
            if (leftHit.collider.tag == "PrevButton")
            {
                prevSongDisplay();
            }
            if (leftHit.collider.tag == "NextButton")
            {
                nextSongDisplay();
            }
            if (leftHit.collider.tag == "SelectButton")
            {
                selectSongDisplay();
            }
        }
        if (Physics.Raycast(rightRay, out rightHit, sightLength))
        {
            if (rightHit.collider.tag == "PrevButton")
            {
                prevSongDisplay();
            }
            if (rightHit.collider.tag == "NextButton")
            {
                nextSongDisplay();
            }
            if (rightHit.collider.tag == "SelectButton")
            {
                selectSongDisplay();
            }
        }
    }

    void nextSongDisplay()
    {
        curIndex = ((curIndex + 1) % songList.Count);
    }

    void prevSongDisplay()
    {
        if (curIndex == 0)
        {
            curIndex = songList.Count - 1;
        }
        else
        {
            curIndex -= 1;
        }
    }
    void selectSongDisplay()
    {
        SceneManager.LoadScene("AudioVisualScene");
    }

    void initiateSongList()
    {
        songList.AddRange(Properties.availableSongs);
    }
}
