using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HtmlAgilityPack;
using static LyricResponse;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

public class CubeActions : MonoBehaviour
{
    AudioSource audio;
    WebUtils webUtils;
    List<LyricLine> songLyrics;
    public Text lyrics;
	public GameObject sceneController;

    // Start is called before the first frame update
    //void Start()
    //{
    //    audio = sceneController.GetComponent<AudioSource>();
    //    webUtils = new WebUtils();

    //    AudioClip lyric = Resources.Load<AudioClip>("Music/" + Properties.selectedSong);

    //    Debug.Log("Music/" + Properties.selectedSong);
    //    Debug.Log(lyric);
    //    audio.clip = lyric;

    //    songLyrics = convertToText();
    //    audio.Play();
    //}
    //void OnMouseUp()
    //{
    //    Debug.Log("hi");
    //}
    // Update is called once per frame
//    void Update()
//    {
//#if UNITY_EDITOR
//        if (Input.GetMouseButtonUp(0))
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit))
//                Interact(hit);
//        }
//#else
//        if (Input.touchCount > 0)
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit))
//                Interact(hit);
//        }
//#endif
//        if (songLyrics != null)
//        {
//            lyrics.text = songLyrics.First().text;
//            List<LyricLine> lyr = songLyrics.Where(x => x.time < Convert.ToDouble(audio.time) - 0.5).ToList();
//            if(lyr.Count > 0)
//            {
//                lyrics.text = lyr.Last().text;
//            }
//        }
//    }

//    void Interact(RaycastHit hit)
//    {
//        if (hit.collider.CompareTag("User"))
//        {
//            //songLyrics = webUtils.getTopLyrics(Properties.songsList[0]);
//            songLyrics = convertToText();
//            if (audio.isPlaying)
//            {
//                audio.Pause();
//            }
//            else
//            {
//                audio.Play();
//            }
//        }
//    }

    //List<LyricLine> convertToText()
    //{
    //    TextAsset lyric = Resources.Load<TextAsset>("Lyrics/" + Properties.lyricsFile[0]);
    //    //StreamReader inp_stm = new StreamReader(Application.dataPath + "/Lyrics/" + Properties.lyricsFile[0] + ".lrc");
    //    StreamReader inp_stm = new StreamReader(new MemoryStream(lyric.bytes));
    //    List<LyricLine> inp_ln = new List<LyricLine>();
    //    while (!inp_stm.EndOfStream)
    //    {
    //        string line = inp_stm.ReadLine();
    //        if (isValidReg(line))
    //        {
    //            var culture = new CultureInfo("en-US");
    //            var formats = new string[] {
    //                    @"mm\:ss\.ff"
    //                };
    //            string timeTxt = line.Substring(0, line.IndexOf("]") + 1).Replace("[", "").Replace("]", "");

    //            inp_ln.Add(new LyricLine(TimeSpan.ParseExact(timeTxt, formats, culture.NumberFormat).TotalSeconds, line.Substring(line.IndexOf("]") + 1)));
    //            // Do Something with the input. 
    //        }
    //    }

    //    inp_stm.Close(); inp_stm.Close();
    //    return inp_ln;
    //}

    bool isValidReg(string str)
    {
        Regex rgx = new Regex(@"^\[[0-9]{2}:[0-9]{2}\.[0-9]{2}\].*$");
        return rgx.IsMatch(str);
    }
}
