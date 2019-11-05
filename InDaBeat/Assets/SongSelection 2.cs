using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSelection : MonoBehaviour
{

    public void songSelect()
	{
        Properties.selectedSong = "Killshot";
		SceneManager.LoadScene("Music Room", LoadSceneMode.Single);
	}
}
