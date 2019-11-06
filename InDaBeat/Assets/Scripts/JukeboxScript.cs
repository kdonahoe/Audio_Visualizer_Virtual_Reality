using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class JukeboxScript : MonoBehaviour
{
    public GameObject songDisplay;
    private TextMeshProUGUI songName;
    public List<string> songList = new List<string>();
    public Button prevButton, nextButton, selectButton;
    private Button pb, nb, sel;
    int curIndex;
    // Start is called before the first frame update
    void Start()
    {
        initiateSongList();
        curIndex = 0;
        songName = songDisplay.GetComponent<TextMeshProUGUI>();
        
        pb = prevButton.GetComponent<Button>();
        nb = nextButton.GetComponent<Button>();
        sel = selectButton.GetComponent<Button>();

        pb.onClick.AddListener(prevSongDisplay);
        nb.onClick.AddListener(nextSongDisplay);
        sel.onClick.AddListener(selectSongDisplay);
    }

    // Update is called once per frame
    void Update()
    {
        songName.text = songList[curIndex];
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
        Properties.selectedSong = songName.text;
        SceneManager.LoadScene("AudioVisualScene");
    }

    void initiateSongList()
    {
        AudioClip[] songs = Resources.LoadAll<AudioClip>("Music/");
        if (songs != null && songs.Length > 0)
        {
            foreach (AudioClip song in songs)
            {
                songList.Add(song.name);
            }
        }
    }
}
