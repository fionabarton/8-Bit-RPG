﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	[Header("Set in Inspector")]
	public AudioSource masterVolSelection;
	public List<AudioSource> bgmCS = new List<AudioSource>();
	public List<AudioSource> sfxCS = new List<AudioSource>();
	public AudioSource textSpeedSelection;

	[Header("Set Dynamically")]
	public int previousSongNdx;
	public int currentSongNdx;

	public float previousVolumeLvl;

	// On start battle, cache currently played song's time, then after battle when the scene is reloaded,
	// the song will played at the time it was stopped at
	public float previousTime;

	private static AudioManager _S;
	public static AudioManager S { get { return _S; } set { _S = value; } }

	void Awake() {
		S = this;
	}

	/*
	 * 1) On scene load, 
	 *		if the previous scene's name == "Battle", 
	 *			Store current songNdx & time
	 * 2) Play new song
	 * 3) On scene load, 
	 *		if the previous scene's name == "Battle", 
	 *			Play song with stored songNdx & time
	 */

	void Start() {
		// Add Loop() to UpdateManager
		UpdateManager.updateDelegate += Loop;

		// Set previous volume level
		previousVolumeLvl = AudioListener.volume;
	}

	public void Loop() {
		// (Un)mute audio
		if (Input.GetKeyDown(KeyCode.M)) {
			SetMuteAudioSliderValue();
		}
	}

	// Play a song that doesn't loop, then when it's over, resume playback of the song that was playing previously
	public IEnumerator PlaySongThenResumePreviousSong(int ndx) {
		// Get current song's playback time, then stop its playback
		float time = bgmCS[currentSongNdx].time;
		bgmCS[currentSongNdx].Stop();

		// Play new song
		bgmCS[ndx].Play();

		// Get new song length
		AudioClip a = bgmCS[ndx].clip;
		float songLength = a.length + 1;

		// Wait until new song is done playing
		yield return new WaitForSeconds(songLength);

		// Resume playback of the song that was playing previously
		bgmCS[currentSongNdx].time = time;
		bgmCS[currentSongNdx].Play();

		// Set volume to 0, then gradually raise to previousVolumeLvl
		//AudioListener.volume = 0;

		// Add VolumeSwell() to UpdateManager
		//UpdateManager.fixedUpdateDelegate += VolumeSwell;
	}

	// Gradually raise volume to previousVolumeLvl
	public void VolumeSwell() {
		if (AudioListener.volume >= previousVolumeLvl) {
			// Set volume and stop calling this coroutine
			AudioListener.volume = previousVolumeLvl;

			// Remove FixedLoop() from UpdateManager
			UpdateManager.fixedUpdateDelegate -= VolumeSwell;
		} else {
			// Raise volume 
			AudioListener.volume += 0.075f * Time.fixedDeltaTime;
		}
	}

	public void PlaySong(eSongName songName) {
		// Return if this song is already playing
		if (previousSongNdx != 999) { // Allows bgmCS[0] to play if it's the first song when the game starts
			if (currentSongNdx == (int)songName) {
				return;
			}
		}

		// Set previous song index
		previousSongNdx = currentSongNdx;

		// Set current song index
		currentSongNdx = (int)songName;

		// Reset current song time
		bgmCS[currentSongNdx].time = 0;

		// If previous scene was "Battle",
		// set song time to the cached value of when it was stopped (right before the battle started)
		if (GameManager.S.previousScene == "Battle" && songName != eSongName.startBattle) {
			bgmCS[currentSongNdx].time = previousTime;
        }

		// Stop ALL BGM
		for (int i = 0; i < bgmCS.Count; i++) {
			bgmCS[i].Stop();
		}

		// Play song
		switch (songName) {
			case eSongName.nineteenForty: bgmCS[0].Play(); break;
			case eSongName.never: bgmCS[1].Play(); break;
			case eSongName.ninja: bgmCS[2].Play(); break;
			case eSongName.soap: bgmCS[3].Play(); break;
			case eSongName.things: bgmCS[4].Play(); break;
			case eSongName.startBattle: bgmCS[5].Play(); break;
			case eSongName.win: bgmCS[6].Play(); break;
			case eSongName.lose: bgmCS[7].Play(); break;
			case eSongName.selection: bgmCS[8].Play(); break;
			case eSongName.gMinor: bgmCS[9].Play(); break;
			case eSongName.zelda: bgmCS[10].Play(); break;
		}
	}

    public void SetMuteAudioSliderValue() {
		// Sets 'Mute Audio' slider value on OptionsMenu,
		// which calls PauseAndMuteAudio() when its value is changed
		if (!AudioListener.pause) {
            OptionsMenu.S.slidersGO[5].value = 0;
		} else {
            OptionsMenu.S.slidersGO[5].value = 1;
		}
    }

    public void PauseAndMuteAudio() {
		// Pause and mute
		if (!AudioListener.pause) {
			previousVolumeLvl = AudioListener.volume;
			AudioListener.pause = true;

			bgmCS[currentSongNdx].Pause();
		// Unpause and unmute
		} else {
			AudioListener.volume = previousVolumeLvl;
			AudioListener.pause = false;

			bgmCS[currentSongNdx].Play();
		}
	}

	public void PlaySFX(int ndx) {
		sfxCS[ndx].Play();
	}
	public void PlaySFX(eSoundName soundName) {
		switch (soundName) {
			case eSoundName.dialogue: sfxCS[0].Play(); break;
			case eSoundName.selection: sfxCS[1].Play(); break;
			case eSoundName.damage1: sfxCS[2].Play(); break;
			case eSoundName.damage2: sfxCS[3].Play(); break;
			case eSoundName.damage3: sfxCS[4].Play(); break;
			case eSoundName.death: sfxCS[5].Play(); break;
			case eSoundName.confirm: sfxCS[6].Play(); break;
			case eSoundName.deny: sfxCS[7].Play(); break;
			case eSoundName.run: sfxCS[8].Play(); break;
			case eSoundName.fireball: sfxCS[9].Play(); break;
			case eSoundName.fireblast: sfxCS[10].Play(); break;
			case eSoundName.buff1: sfxCS[11].Play(); break;
			case eSoundName.buff2: sfxCS[12].Play(); break;
			case eSoundName.highBeep1: sfxCS[13].Play(); break;
			case eSoundName.highBeep2: sfxCS[14].Play(); break;
			case eSoundName.swell: sfxCS[15].Play(); break;
			case eSoundName.flicker: sfxCS[16].Play(); break;
		}
	}

	public void SetMasterVolume(float volume) {
		AudioListener.volume = volume;
		masterVolSelection.volume = volume;

		// Set previous volume level
		previousVolumeLvl = volume;
	}

	public void SetBGMVolume(float volume) {
		for (int i = 0; i < bgmCS.Count; i++) {
			bgmCS[i].volume = volume;
		}
	}

	public void SetSFXVolume(float volume) {
		for (int i = 0; i < sfxCS.Count; i++) {
			sfxCS[i].volume = volume;
		}
	}

	public void PlayRandomDamageSFX() {
		int randomInt = UnityEngine.Random.Range(2, 4);
		PlaySFX(randomInt);
	}
}