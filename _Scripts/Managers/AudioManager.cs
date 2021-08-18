using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	[Header ("Set in Inspector")]
	public List <AudioSource>	bgmCS = new List<AudioSource>();
	public List <AudioSource>	sfxCS = new List<AudioSource>();

	[Header("Set Dynamically")]
	public int 					currentSong;

	public AudioListener		audioListenerCS;

	// Singleton
	private static AudioManager _S;
	public static AudioManager S { get { return _S; } set { _S = value; } }

	void Awake() {
		// Singleton
		S = this;
	}

	void Start() {
		audioListenerCS = GetComponent<AudioListener>();

		// Add Loop() to UpdateManager
		UpdateManager.updateDelegate += Loop;
	}

    public void Loop(){
		if (Input.GetKeyDown (KeyCode.M)) {
			PauseMuteSong ();
		}
	}

	public void PlaySong(bool playOrStop = true, int ndx = 0){
		// Change index
		currentSong = ndx;

		// Stop ALL BGM
		for (int i = 0; i < bgmCS.Count; i++) {
			bgmCS [i].Stop ();
		}

		if (!AudioListener.pause) {
			if (playOrStop) {
				bgmCS[ndx].Play();
            } else {
				for (int i = 0; i < bgmCS.Count; i++) {
					bgmCS[i].Stop();
				}
			}
		}
	}

	public void PauseMuteSong(int songNdx = 0){
		if (!AudioListener.pause) {
			AudioListener.volume = 0;
			AudioListener.pause = true;

			bgmCS [currentSong].Pause ();
		} else {
			AudioListener.volume = 1;
			AudioListener.pause = false;

			bgmCS [currentSong].Play ();
		}
	}

	public void PlaySFX(int ndx) {
		sfxCS[ndx].Play();
    }
}
