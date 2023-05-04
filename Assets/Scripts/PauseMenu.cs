using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using MainMenuClass = MainMenu;
using UnityEngine.InputSystem;

/// <summary>
/// <author>Jared White</author>
/// </summary>

/// <summary>
/// Class that handles the pause menu and its needed functionality
/// </summary>
public class PauseMenu : MonoBehaviour {
    // For handling the actual pausing
    PauseAction inputAction;
    public GameObject pauseMenu;
    public static bool isPaused = false;

    // For handling player preferences
    public Toggle fullScreen;
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    private int screenToggle;
    private bool isFullScreen = false;

    // Player Pref Keys
    private const string qualityOption = "Quality Option";
    private const string resOption = "Resolution Option";
    private const string fullScreenKey = "FullScreen Toggle";

    void Awake() {
        inputAction = new PauseAction(); // set the inputAction var to our PauseAction input map

        /// <summary>
        /// Saves the player's selection to PlayerPrefs for future access
        /// </summary>
        screenToggle = PlayerPrefs.GetInt("Fullscreen Toggle");

        // Save fullscreen toggle
        if(screenToggle == 1) {
            isFullScreen = true;
            fullScreen.isOn = true;
        } else {
            isFullScreen = false;
            fullScreen.isOn = false;
        }

        // Save resolution option
        resolutionDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
        {
            PlayerPrefs.SetInt(resOption, resolutionDropdown.value);
            PlayerPrefs.Save();
        }));

        // Save quality option
        qualityDropdown.onValueChanged.AddListener(new UnityAction<int>(index =>
        {
            PlayerPrefs.SetInt(qualityOption, qualityDropdown.value);
            PlayerPrefs.Save();
        }));
        
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        // end
    }

    void Start() {
        inputAction.PauseGame.Pause.performed += _ => CheckPause();

        /// <summary>
        /// Creates the dropdown options for resolution
        resolutions = Screen.resolutions;  // Set the resolution array with the provided resolutions
        resolutionDropdown.ClearOptions(); // Clear the options of the dropdown

        List<string> resOptions = new List<string>(); // Create a list of strings to be the options
        int currentResolutionIndex = 0; // The selected resolution index
        for (int i = 0; i < resolutions.Length; i++) {
            string resOption = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRateRatio.value + "Hz";
            resOptions.Add(resOption); // Add the new option to the list of options

            // Checks if the selected resolution matches the current resolution
            if (resolutions[i].width == Screen.currentResolution.width 
            && resolutions[i].height == Screen.currentResolution.height 
            && resolutions[i].refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value)  
            {
                currentResolutionIndex = i; // Then sets the currentResolution index to this index
            }
        }

        resolutionDropdown.AddOptions(resOptions); // Adds the options to the dropdown
        resolutionDropdown.value = PlayerPrefs.GetInt(resOption, currentResolutionIndex); //
        resolutionDropdown.RefreshShownValue(); // Shows the new selected value

        IEnumerator SetVolumeNextFrame() {
            yield return null;
            OnMusicVolumeChanged(musicSlider.value = PlayerPrefs.GetFloat("musicVolume", .5f));
            OnSFXVolumeChanged(sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1));
        }
        StartCoroutine(SetVolumeNextFrame());
        // end
    }

    // Set the graphics quality
    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Set the screen resolution
    public void SetResolution(int resIndex) {
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // Set fullscreen
    public void SetFullScreen(bool isFullScreen) {
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("fullScreenKey", isFullScreen == false ? 0 : 1);
        this.isFullScreen = isFullScreen;
    }

    // Quit the application
    public void Quit() {
        MainMenuClass.Quit();
        Debug.Log("Application stopped.");
    }

    // Go back to the main menu
    public void MainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Functions needed for pausing the game
    /// </summary>

    private void OnEnable() {
        inputAction.Enable();
    }

    private void OnDisable() {
        inputAction.Disable();
        PlayerPrefs.Save();
    }

    public void OnMusicVolumeChanged(float volume) {
        PlayerPrefs.SetFloat("musicVolume", volume);

        if (AudioManager.instance == null) return;
        AudioManager.instance.musicVolume = volume;
        if (AudioManager.instance.playingBattleMusic)
            AudioManager.instance.PlayBattleMusic(.001f);
        else AudioManager.instance.PlayCalmMusic(.001f);
    }

    public void OnSFXVolumeChanged(float volume) {
        PlayerPrefs.SetFloat("sfxVolume", volume);

        if (AudioManager.instance == null) return;
        AudioManager.instance.soundFXPlayer.volume = volume;
        AudioManager.instance.uiSoundFXPlayer.volume = volume;
    }
    
    // Check if the game is paused or not
    public void CheckPause() {
        if(isPaused == true) {
            ResumeGame();
        } else {
            PauseGame();
        }
    }

    public void PauseGame() {
        Time.timeScale = 0; // paused the game;
        isPaused = true;    // set the isPaused bool to true
        pauseMenu.SetActive(true);  // set the menu UI component to true so it appears for the player
    }

    public void ResumeGame() {
        Time.timeScale = 1; // resume the game
        isPaused = false;   // set the isPaused bool to false
        pauseMenu.SetActive(false); // set the menu UI component to false so it does not show for the player
    }
}
