using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerBase : Core.Utilities.PersistentSingleton<AudioManagerBase> {
	
	
	// -- NamedAudioClip --


	// Class which maps a name to an audio clip, used for populating AudioPlayer tracks
	[System.Serializable]
	public struct NamedAudioClip {
		public string name; // The name
		public AudioClip clip; // The clip

		// Constructor
		public NamedAudioClip(string _name, AudioClip _clip){
			name = _name;
			clip = _clip;
		}

		// Automatically create a name pair from an audioclip's name
		static NamedAudioClip CreateNamedAudioClip(AudioClip clip){
			return new NamedAudioClip(clip.name, clip);
		}

		// Automatically create an array of name pairs from the names found on an array of audio clips
		static NamedAudioClip[] CreateNamedAudioClipArray(AudioClip[] clips){
			NamedAudioClip[] ret = new NamedAudioClip[clips.Length];
			for(int i = 0; i < clips.Length; i++)
				ret[i] = CreateNamedAudioClip(clips[i]);

			return ret;
		}
	}


	// -- AudioPlayer --


	// Class representing an a managed AudioSource
	public class AudioPlayer {
		// Exception thrown when a track can't be found
		public class TrackNotFoundException : System.Exception {
			public TrackNotFoundException(string track) : base("The track '" + track + "' could not be found!") {}
			public TrackNotFoundException(string track, System.Exception inner) : base("The track '" + track + "' could not be found!", inner) {}
		}

		// The AudioManager which owns this player (reference needed to start coroutines)
		AudioManagerBase owner;
		// The AudioSource this player uses to play
		public AudioSource source;
		// Map of tracks
		public Dictionary<string, AudioClip> tracks = new Dictionary<string, AudioClip>();

		// Property tracking the current volume of the player
		float _playerVolume = 1;
		public float volume {
			get => _playerVolume;
			set {
				_playerVolume = value;
				source.volume = _playerVolume;
			}
		}

		// Constructor
		public AudioPlayer(AudioManagerBase _owner){
			owner = _owner;

			// Create the initial audio source for this player
			GameObject sourceObject = new GameObject();
			sourceObject.transform.parent = owner.transform;
			source = sourceObject.AddComponent<AudioSource>();
		}

		// Add a track from a named clip
		public void AddTrack(NamedAudioClip clip){
			tracks[clip.name] = clip.clip;
		}
		// Add a track from a name and clip
		public void AddTrack(string name, AudioClip clip){ AddTrack(new NamedAudioClip(name, clip)); }

		// Add several tracks from an array of named clips
		public void AddTracks(NamedAudioClip[] clips){
			foreach(NamedAudioClip clip in clips)
				tracks[clip.name] = clip.clip;
		}

		// Gets a volume relative to the player's base volume
		protected float RelativeVolume(float baseVolume) { return baseVolume * volume; }

		// Creates a new audio source playing the given track
		protected AudioSource CreateTrackPlayer(string trackName, float desiredVolume = -1, bool looping = false){
			// If the track isn't found throw an exception
			if(!tracks.ContainsKey(trackName)) throw new TrackNotFoundException(trackName);
			// If the desired volume is negative, then use the player's volume
			if(desiredVolume < 0) desiredVolume = volume;

			// Create the source and configure it
			AudioSource outSource = Instantiate(source);
			outSource.name = trackName + " - Player";
			outSource.transform.parent = source.transform.parent;
			outSource.Stop();
			outSource.clip = tracks[trackName];
			outSource.volume = desiredVolume;
			outSource.loop = looping;
			outSource.Play();

			return outSource;
		}


		// -- FadeVolume --


		// Function which gradually changes the volume to the <desiredVolume> over a <duration>
		Coroutine fadeVolumeCoroutine = null; // Coroutine representing the volume fade
		public void FadeVolume(float desiredVolume, float duration){
			// Get rid of any current fades
			CancelFadeVolume();

			// If there is a duration, then start the volume fader coroutine...
			if(duration > 0){
				volumeFader_DesiredVolume = desiredVolume;
				fadeVolumeCoroutine = owner.StartCoroutine(volumeFader(duration));
			// If there isn't a duration then simply snap the volume to its target
			} else
				volume = desiredVolume;
		}

		// Function which cancels any gradual volume fades
		// NOTE: By default it will leave the volume wherever it happened to be, but if <snapVolume> is true then the volume will be snapped to its desired value
		public void CancelFadeVolume(bool snapVolume = false){
			if(fadeVolumeCoroutine is null) return;

			owner.StopCoroutine(fadeVolumeCoroutine);
			fadeVolumeCoroutine = null;

			// Snap the volume to the desired volume (if we should)
			if(snapVolume) volume = volumeFader_DesiredVolume;
		}

		// Coroutine which gradually fades the player volume to a new target
		float volumeFader_DesiredVolume;
		IEnumerator volumeFader(float duration){
			// Save the starting volume and time
			float startingVolume = volume;
			float startingTime = Time.time;

			// While the time passed is less than the duration...
			while(Time.time - startingTime <= duration){
				// Number between 0 and 1 which determines how far into the fade we currently are
				float elapsedTimeRatio = (Time.time - startingTime) / duration;

				volume = Mathf.Lerp(startingVolume, volumeFader_DesiredVolume, elapsedTimeRatio);
				yield return null;
			}

			// Snap the volume to the desired volume
			volume = volumeFader_DesiredVolume;
		}


		// -- PlayTrackImmediate --


		// Function which creates a new audio source playing the given track which is deleted as soon as the track is done playing
		public AudioSource PlayTrackImmediate(string trackName, float relativeVolume = 1){
			AudioSource tmpSource = CreateTrackPlayer(trackName, relativeVolume);
			// Coroutine which deletes the source once it is done playing
			owner.StartCoroutine(destroySourceWhenDonePlaying(tmpSource, relativeVolume));
			return tmpSource;
		}

		// Coroutine which deletes a source once it is done playing (and keeps its volume relative to the player volume)
		IEnumerator destroySourceWhenDonePlaying(AudioSource toDestroy, float relativeVolume){
			// Skip frames whil the source is playing
			while(toDestroy.isPlaying) {
				// Ensure that the managed source's volume adjusts with the player's volume
				toDestroy.volume = RelativeVolume(relativeVolume);
				yield return null;
			}

			// If the source still exists then destroy it
			if(toDestroy) Destroy(toDestroy.gameObject);
		}


		// -- SwitchTrack --


		// Function which switches to a new track (possibly crossfading to it over a duration)
		Coroutine trackFade = null; // Crossfade coroutine
		public void SwitchTrack(string trackName, float fadeDuration = 0, bool shouldCancelTrackCyling = true){
			// If the track isn't found throw an exception
			if(!tracks.ContainsKey(trackName)) throw new TrackNotFoundException(trackName);

			// Cancel track cycling (unless called from the track cycler)
			if(shouldCancelTrackCyling) CancelCycleTracks();
			// Cancel a track fade if one is currently on going
			CancelTrackSwitch();

			// If there is a fade duration, then start the fade routine...
			if(fadeDuration > 0){
				// Make sure there is a clip that we are fading from (pick the first clip and sets its volume to 0)
				if(source.clip is null){
					var e = tracks.GetEnumerator();
					e.MoveNext();
					source.clip = e.Current.Value;
					source.volume = 0;
				}

				trackFade = owner.StartCoroutine(fadeBetweenTracks(tracks[trackName], fadeDuration));
			// Otherwise just set the clip and play it
			} else {
				source.clip = tracks[trackName];
				source.Play();
			}
		}

		// Function which cancels a track crossfade
		public void CancelTrackSwitch(){
			if(trackFade is null) return;

			owner.StopCoroutine(trackFade);
			fadeBetweenTracksEnd();
			trackFade = null;
		}

		// Coroutine which fades to a new track
		AudioSource fadeBetweenTracks_SecondarySource = null; // Secondary audio source which is playing the track we are fading to
		IEnumerator fadeBetweenTracks(AudioClip newTrack, float fadeDuration){
			// Save the time the coroutine started
			float startTime = Time.time;

			// Create the secondary source (with all the same setting except, the new track, and a volume of 0)
			fadeBetweenTracks_SecondarySource = Instantiate(source);
			fadeBetweenTracks_SecondarySource.Stop();
			fadeBetweenTracks_SecondarySource.transform.parent = source.transform.parent;
			fadeBetweenTracks_SecondarySource.clip = newTrack;
			fadeBetweenTracks_SecondarySource.volume = 0;
			fadeBetweenTracks_SecondarySource.Play();

			// Save the volume we are fading from
			float initialVolume = source.volume;

			// While less time than the duration has passed...
			while(Time.time - startTime <= fadeDuration){
				// Number between 0 and 1 which determines how far into the fade we currently are
				float elapsedTimeRatio = (Time.time - startTime) / fadeDuration;

				// Fade the old track towards 0
				source.volume = Mathf.Lerp(initialVolume, 0, elapsedTimeRatio);
				// Fade the new track towards the player's volume
				fadeBetweenTracks_SecondarySource.volume = Mathf.Lerp(0, volume, elapsedTimeRatio);

				yield return null;
			}

			// Fade function cleanup
			fadeBetweenTracksEnd();
		}

		// Function which cleans up once the track fade is done
		void fadeBetweenTracksEnd(){
			fadeBetweenTracks_SecondarySource.name = source.name; // Petty debugging information that keeps the inspector clean
			// Destroy the old source
			Destroy(source.gameObject);

			// Set the new source as this player's source
			source = fadeBetweenTracks_SecondarySource;
			fadeBetweenTracks_SecondarySource = null;
			// Make 100% sure that we are at exactly the requested volume
			source.volume = volume;

			// Mark that we have finished fading
			trackFade = null;
		}


		// -- CycleTracks --


		// Function which starts cycling between the tracks in this player
		// Allows running through the cycle <once> or indefinitely, as well as setting the <fadeDuration> between tracks
		Coroutine trackCycle = null; // Coroutine representing the track cycling
		public void CycleTracks(bool once = false, float fadeDuration = 0){
			// Cancel the ongoing track cycle
			CancelCycleTracks();

			// Start the new track cycle
			trackCycle = owner.StartCoroutine(trackCycler(once, fadeDuration));
		}

		// Function which cancels the current cycle
		public void CancelCycleTracks(){
			if(trackCycle is null) return;

			source.loop = trackCycler_SavedLoop; // Ensure that the saved looping state is saved
			owner.StopCoroutine(trackCycle);
			trackCycle = null;
		}

		// Coroutine which cycles through all of the tracks
		bool trackCycler_SavedLoop; // Looping state of the source before we began
		IEnumerator trackCycler(bool once, float fadeDuration){
			// Save the loop state and set that tracks aren't looping
			trackCycler_SavedLoop = source.loop;
			source.loop = false;

			// For each track...
			foreach (string name in tracks.Keys){
				// Switch to that track
				SwitchTrack(name, fadeDuration, /*Don't cancel track cycling*/ false);

				// Wait for the fade to finish
				while(!(trackFade is null))
					yield return null; 

				// Wait for the track to be fadeDuration away from being finished playing
				while(source.clip.length - source.time > fadeDuration)
					yield return null; 
			}

			// Restore the saved loop state
			source.loop = trackCycler_SavedLoop;

			// If we should run indefinitely restart the coroutine
			if(!once)
				CycleTracks(once, fadeDuration);
		}
	}


	// -- Audio Manager --


	// Map storing all of the players
	Dictionary<string, AudioPlayer> players = new Dictionary<string, AudioPlayer>();

	// Function which creates a new player with the given name (possibly with an initial list of named audio clips)
	public AudioPlayer CreateAudioPlayer(string name, NamedAudioClip[] clips = null){
		// Create a new player with us set as its owner
		AudioPlayer player = new AudioPlayer(/*owner*/ this);
		player.source.name = name + " - Player";

		// Attempt to add tracks (null exception handling in place so that it can properly handle an empty array from the inspector)
		try{
			if(!(clips is null))
				player.AddTracks(clips);
		} catch (System.NullReferenceException) {}

		// Save the player into the map and return it
		players[name] = player;
		return player;
	}

	// Function which finds a player in the map
	public AudioPlayer FindPlayer(string name){
		if(!players.ContainsKey(name)) return null;

		return players[name];
	}
}
