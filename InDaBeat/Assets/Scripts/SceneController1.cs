using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneController1 : MonoBehaviour
{
    public GameObject songDisplay;
    private TextMeshProUGUI songName;
    public List<string> songList = new List<string>();
    public int curIndex;
    public GameObject NetMgr;
    // Start is called before the first frame update
    void Start()
    {
        initiateSongList();
        curIndex = 0;
        songName = songDisplay.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        songName.text = songList[curIndex];
    }

    void nextSongDisplay()
    {
        curIndex = ((curIndex + 1) % songList.Count);
        NetMgr.GetComponent<NetworkManager>().UpdateSongIndex(curIndex);
        Debug.Log("Called UpdateSongIndex from SceneCtrl");
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
        NetMgr.GetComponent<NetworkManager>().UpdateSongIndex(curIndex);
        Debug.Log("Called UpdateSongIndex from SceneCtrl");
    }
    void selectSongDisplay()
    {
        Properties.selectedSong = songName.text;
        SceneManager.LoadScene("Karaoke Room");
    }

    void initiateSongList()
    {
        AudioClip[] songs = Resources.LoadAll<AudioClip>("Music/");
        if(songs != null && songs.Length > 0)
        {
            foreach(AudioClip song in songs)
            {
                songList.Add(song.name);
            }
        }
    }
}
