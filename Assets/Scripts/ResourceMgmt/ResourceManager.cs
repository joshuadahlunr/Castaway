using System.Collections;
using TMPro;
using System.Linq;
using SQLite;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CardBattle;
using Crew;

namespace ResourceMgmt
{
    /// <summary>
    ///     Singleton manager responsible for handling ship upgrade informtion and current resource count
    /// </summary>
    /// <author> Misha Desear </author>
    public class ResourceManager : MonoBehaviour
    {
        /// <summary>
        ///     Stores upgrade information (current level, progress towards next level, next level cost,
        ///     amount of current resources, current ship health)
        /// <summary>
        public class UpgradeInfo
        {
            public static int trash;

            // Unique identifier of the upgrade info table
            [PrimaryKey]
            public int id
            {
                get => 0;
                set => trash = value;
            }

            // Current ship level
            public int currentLvl { get; set; }

            // Progress made towards the next ship level
            public int currentProgress { get; set; }

            // Resource cost of upgrading to the next ship level
            public int lvlCost { get; set; }

            // Current amount of resources
            public int currentResources { get; set; }

            // The ship's current health
            public int currentShipHealth { get; set; }
        }

        /// <summary>
        ///     Singleton instance of this class
        /// </summary>
        public static ResourceManager instance;

        [SerializeField] private Slider shipSlider, crewSlider;

        [SerializeField] private Button confirmBtn, nameBtn;

        [SerializeField] private UpgradeInfo upgradeData;

        [SerializeField] private Sprite[] shipLevels;

        [SerializeField] private TextMeshProUGUI shipSliderTxt, crewSliderTxt, winTxt, shipLvlTxt, progressTxt;

        [SerializeField] private InputField nameInput;

        private int resourcesWon;

        private readonly float currentVal;

        public Image ship;

        private void Awake()
        {
            // Set up singleton
            instance = this;

            // Add listeners for the confirm button and allocation sliders so that 
            // upgrades and crew feeding are done appropriately, and sliders are limited
            // by amount of current resources
            confirmBtn.onClick.AddListener(Upgrade);
            confirmBtn.onClick.AddListener(FeedCrew);
            crewSlider.onValueChanged.AddListener(delegate { ValidateResources(); });
            shipSlider.onValueChanged.AddListener(delegate { ValidateResources(); });

            // Obtain the upgrade information table
            var table = DatabaseManager.GetOrCreateTable<UpgradeInfo>();
            //table.Delete(_ => true);

            if (DatabaseManager.GetOrCreateTable<UpgradeInfo>().Any())  // If we can obtain an upgrade info table...
            {
                LoadUpgradeInfo(); // ...load the upgrade info to restore upgrade progress
            }

            else    // Otherwise, use default vals
            {
                var shipUpgradeInfo = DatabaseManager.GetOrCreateTable<UpgradeInfo>().FirstOrDefault();
		        if (shipUpgradeInfo is null)
		        {
                    upgradeData.currentLvl = 1;
                    upgradeData.lvlCost = 10;
                    upgradeData.currentResources = 0;
                    upgradeData.currentProgress = 0;
                    upgradeData.currentShipHealth = 10;
                }
            }

            // Calculate resources won after a successful battle based on the monster level and the number of monsters killed in the encounter
            resourcesWon = CardGameManager.numberOfMonstersKilled * CardGameManager.monsterLevel * 10;
            upgradeData.currentResources += resourcesWon;

            winTxt.text = ("Congratulations! You've earned " + resourcesWon.ToString() + " resources for your ship and crew. You have "
                        + upgradeData.currentResources.ToString() + " in total. How would you like to allocate them?");

            LoadShip(); // Load the ship based on the table data
        }

        private void Start()
        {
            // Play calm music
            AudioManager.instance?.PlayCalmMusic();
            
            // Crew slider is only enabled if we actually have a crew to allocate resources to
            if (CrewManager.instance.crewList.Count == 0 || CrewManager.instance.crewList == null)
            {
                crewSlider.enabled = false;
            }
            else
            {
                crewSlider.enabled = true;
            }
        }

        void Update()
        {
            // Display the correct amount of resources to be allocated based on slider value
            shipSliderTxt.text = shipSlider.value.ToString();
            crewSliderTxt.text = crewSlider.value.ToString();
            shipLvlTxt.text = "Ship: Level " + upgradeData.currentLvl.ToString(); 
            progressTxt.text = "Progress to next level: " + upgradeData.currentProgress.ToString() + "/" + upgradeData.lvlCost.ToString();
        }

        void OnDestroy()
        {
            SaveUpgradeInfo();
        }

        /// <summary>
        ///     Ensures that the slider values cannot exceed the amount of resources we currently have
        /// </summary>
        public void ValidateResources()
        {
            int sliderTotal = (int)shipSlider.value + (int)crewSlider.value;
            if (sliderTotal > upgradeData.currentResources)
            {
                shipSlider.value = currentVal;
                crewSlider.value = currentVal;
            }
        }

        /// <summary>
        ///     Adds to progress and subtracts from current resources based on
        ///     slider value, then checks if we need to level up, and if so, 
        ///     loads the appropriate ship sprite
        /// </summary>
        public void Upgrade()
        {
            if (upgradeData.currentResources >= upgradeData.lvlCost) 
            {
                    // Subtract ship slider value from current resource count and add to progress
                    upgradeData.currentResources -= (int)shipSlider.value;
                    upgradeData.currentProgress += (int)shipSlider.value;
            }
            LevelUp(); // Check if we need to level up
            LoadShip(); // Load the appropriate ship sprite

            StartCoroutine(ReturnToMap(3f)); // Return to the encounter map
        }

        /// <summary>
        ///     Checks if the progress we've made towards level cost exceeds it,
        ///     and if so, levels up the ship and subtracts level cost from our
        ///     current progress
        /// </summary>
        public void LevelUp()
        {
            // If current progress meets or exceeds level cost...
            if (upgradeData.currentProgress >= upgradeData.lvlCost)
            {
                upgradeData.lvlCost *= 2; // ...multiply cost by two...
                upgradeData.currentLvl += 1; // ...increment current level...
                upgradeData.currentProgress -= upgradeData.lvlCost; // ...subtract the cost of leveling up from current progress...
                upgradeData.currentShipHealth += 2; // ...and restore two health to our ship!
                shipSlider.maxValue = upgradeData.lvlCost; // New ship slider max value is equivalent to the cost for the next level
                NotificationHolder.instance.CreateNotification("Level up! Ship is now Level " + upgradeData.currentLvl.ToString() + "!", 3f);
            }
        }

        /// <summary>
        ///     Evenly divides the amount of resources allocated to the crew slider
        ///     amongst the entire crew and adjusts their morale accordingly
        /// </summary>
        public void FeedCrew()
        {
            var currentCrew = CrewManager.instance.crewList; // Obtain the loaded crew list from the crew manager
            currentCrew.RemoveAll(x => x.CrewTag != Crewmates.Status.CrewTag.InCrew); // Remove crew members that aren't currently in the crew
            if (currentCrew == null || currentCrew.Count == 0) return; // If we currently don't have members in the crew, return to avoid a divide by zero error
            if (crewSlider.value == 0) 
            {
                StartCoroutine(ReturnToMap(3f)); // If crew slider value is 0, proceed with returning to the map without making any updates
                return;
            };

            int count = currentCrew.Count; // Count equals the number of members currently in the crew

            var remainder = (int)crewSlider.value % count; // Remainder of resources if we cannot divide total evenly by crew count
            var evenNum = (int)crewSlider.value - remainder; // Provides an evenly divisible number by subtracting remainder from slider value
            var quotient = evenNum / count; // The amount of resources to allocate to each crew member without taking remainder into account

            foreach(var crewmate in currentCrew) // Iterate through the current crew members list
            {
                crewmate.Morale += quotient; // Add the quotient to the crewmate's morale

                if (remainder != 0) // If we still have a remainder...
                {
                    crewmate.Morale += 1; // ...give 1 resource from the remainder to the current crewmate
                    remainder -= 1; // Decrement the remainder by 1 
                }
            }

            upgradeData.currentResources -= (int)crewSlider.value; // Subtract the amount of resources allocated to the crew from total current resources

            NotificationHolder.instance.CreateNotification("Your crew consumed " + crewSlider.value.ToString() + " resources!", 3f);

            StartCoroutine(ReturnToMap(3f)); // Return to the encounter map
        }

        /// <summary>
        ///     Loads the appropriate ship sprite based on current level
        /// </summary>
        public void LoadShip()
        {
            switch (upgradeData.currentLvl)
            {
                case >= 10: // If we are at level 10 or higher...
                    Sprite lvlThreeShip = shipLevels[2];
                    ship.sprite = lvlThreeShip; // ...load the third tier ship
                    break;
                case >= 5: // If we are at or above level 5 but below level 10...
                    Sprite lvlTwoShip = shipLevels[1];
                    ship.sprite = lvlTwoShip; // ...load the second tier ship
                    break;
                default:
                    break; // Otherwise, continue displaying the first tier raft
            }
        }

        /// <summary>
        ///     Loads ship upgrade information from the SQL table
        /// </summary>
        public void LoadUpgradeInfo()
        {
            var @out = DatabaseManager.GetOrCreateTable<UpgradeInfo>().First();
            if (@out is null) return; // Return if we can't find an upgrade info table for some reason
            upgradeData = @out;
        }

        /// <summary>
        ///     Saves the current upgrade information to the SQL table
        /// </summary>
        public void SaveUpgradeInfo()
        {
            var table = DatabaseManager.GetOrCreateTable<UpgradeInfo>(); // Obtain the table
            table.Delete(_ => true); // Delete the current table in order to overwrite with up-to-date info
            // Insert the new table with up-to-date upgrade information
            DatabaseManager.database.Insert(new UpgradeInfo {
                id = 0,
                currentLvl = upgradeData.currentLvl,
                currentProgress = upgradeData.currentProgress,
                lvlCost = upgradeData.lvlCost,
                currentResources = upgradeData.currentResources,
                currentShipHealth = upgradeData.currentShipHealth
            });
        }

        /// <summary>
        ///     Returns to the encounter map scene after a given number of seconds
        ///     in order to ensure notification readability
        /// </summary>
        IEnumerator ReturnToMap(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            SceneManager.LoadScene("EncounterMapScene");
        }
    }
}