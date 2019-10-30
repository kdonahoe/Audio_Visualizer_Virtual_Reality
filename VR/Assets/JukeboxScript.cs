using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JukeboxScript : MonoBehaviour
{
    public GameObject songDisplay;
    private TextMeshProUGUI songName;
    List<string> songList = new List<string>();
    public Button prevButton, nextButton;
    private Button pb, nb;
    int curIndex;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started script");
        initiateSongList();
        curIndex = 0;
        songName = songDisplay.GetComponent<TextMeshProUGUI>();
        
        pb = prevButton.GetComponent<Button>();
        nb = nextButton.GetComponent<Button>();

        pb.onClick.AddListener(prevSongDisplay);
        nb.onClick.AddListener(nextSongDisplay);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(curIndex);
        Debug.Log(songName.text);
        songName.text = songList[curIndex];
    }

    void nextSongDisplay()
    {
        Debug.Log("Detected click");
        curIndex = ((curIndex + 1) % songList.Count);
    }

    void prevSongDisplay()
    {
        Debug.Log("Detected click");
        if (curIndex == 0)
        {
            curIndex = songList.Count - 1;
        }
        else
        {
            curIndex -= 1;
        }
    }

    void initiateSongList()
    {
        songList.Add("All Star - Smash Mouth");
        songList.Add("Hey Jude - The Beatles");
        songList.Add("Piano Man - Billy Joel");
        songList.Add("Rocket Man - Elton John");
    }
}
