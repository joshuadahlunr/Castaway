using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : AudioManagerBase {
	// List of music clips
	public NamedAudioClip[] battleMusicClips, calmMusicClips;
	// List of ui sound effect clips
	public NamedAudioClip[] uiSoundFxClips;
	// List of sound effect clips
	public NamedAudioClip[] soundFxClips;

	public float musicVolume = 1;
	public bool playingBattleMusic = false;

	// References to the audio players we create
	public AudioManagerBase.AudioPlayer battleMusicPlayer, calmMusicPlayer, uiSoundFXPlayer, soundFXPlayer;

	public new static AudioManager instance => AudioManagerBase.instance as AudioManager;

	void Start(){
		// Create a music player and set it off cycling through the music tracks indefinitely
		battleMusicPlayer = CreateAudioPlayer("battleMusic", battleMusicClips);
		battleMusicPlayer.volume = 0;
		battleMusicPlayer.CycleTracks(/*once*/ false, true, 5); // Since our tracks are kinda short, play each track 3 times before switching to a new one!

		calmMusicPlayer = CreateAudioPlayer("music", calmMusicClips);
		calmMusicPlayer.volume = musicVolume;
		calmMusicPlayer.CycleTracks(/*once*/ false, true,5, 3); // Since our tracks are kinda short, play each track 3 times before switching to a new one!

		// Create a UI SoundFX player
		uiSoundFXPlayer = CreateAudioPlayer("uiSoundFX", uiSoundFxClips);
		uiSoundFXPlayer.source.loop = false;

		// Create a SoundFX player
		soundFXPlayer = CreateAudioPlayer("soundFX", soundFxClips);
		soundFXPlayer.source.loop = false;
	}

	public void PlayBattleMusic(float fadeDuration = 3) {
		StartCoroutine(PlayMusicImpl(.8f * musicVolume, 0, fadeDuration));
		playingBattleMusic = true;
	}
	public void PlayCalmMusic(float fadeDuration = 3) {
		StartCoroutine(PlayMusicImpl(0, musicVolume, fadeDuration));
		playingBattleMusic = false;
	}

	private IEnumerator PlayMusicImpl(float targetBattleVolume, float targetCalmVolume, float fadeDuration = 3) {
		float initialBattleVolume = battleMusicPlayer.volume;
		float initialCalmVolume = calmMusicPlayer.volume;
		for (float time = 0; time < 1; time += Time.unscaledDeltaTime / fadeDuration) { // The divide by 3 means it will take 3 seconds to fade
			yield return null;
			battleMusicPlayer.volume = Mathf.Lerp(initialBattleVolume, targetBattleVolume, time);
			calmMusicPlayer.volume = Mathf.Lerp(initialCalmVolume, targetCalmVolume, time);
		}
	}
}
