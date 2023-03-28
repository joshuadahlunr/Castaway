using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : AudioManagerBase {
	// List of music clips
	public NamedAudioClip[] musicClips;
	// List of ui sound effect clips
	public NamedAudioClip[] uiSoundFxClips;
	// List of sound effect clips
	public NamedAudioClip[] soundFxClips;

	// References to the players we create
	public AudioManagerBase.AudioPlayer musicPlayer, uiSoundFXPlayer, soundFXPlayer;

	// Override instance to represent the Whitehat type
	new static public AudioManager instance {
		get => AudioManagerBase.instance as AudioManager;
	}

	void Start(){
		// Create a music player and set it off cycling through the music tracks indefinitely
		musicPlayer = CreateAudioPlayer("music", musicClips);
		musicPlayer.volume = .8f;
		musicPlayer.CycleTracks(/*once*/ false, 10);

		// Create a UI SoundFX player
		uiSoundFXPlayer = CreateAudioPlayer("uiSoundFX", uiSoundFxClips);
		uiSoundFXPlayer.source.loop = false;

		// Create a SoundFX player
		soundFXPlayer = CreateAudioPlayer("soundFX", soundFxClips);
		soundFXPlayer.source.loop = false;
	}
}
