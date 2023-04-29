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
    public class ResourceManager : MonoBehaviour
    {
        public class UpgradeInfo
        {
            public static int trash;

            [PrimaryKey]
            public int id
            {
                get => 0;
                set => trash = value;
            }

            public int currentLvl { get; set; }

            public int currentProgress { get; set; }

            public int lvlCost { get; set; }

            public int currentResources { get; set; }

            public int currentShipHealth { get; set; }
        }

        public static ResourceManager instance;

        [SerializeField] private Slider shipSlider, crewSlider;

        [SerializeField] private Button confirmBtn;

        [SerializeField] private UpgradeInfo upgradeData;

        [SerializeField] private Sprite[] shipLevels;

        [SerializeField] private TextMeshProUGUI shipSliderTxt, crewSliderTxt, winTxt;

        private int resourcesWon;

        private readonly float currentVal;

        public Image ship;

        private void Awake()
        {
            instance = this;

            confirmBtn.onClick.AddListener(Upgrade);
            confirmBtn.onClick.AddListener(FeedCrew);
            crewSlider.onValueChanged.AddListener(delegate { ValidateResources(); });
            shipSlider.onValueChanged.AddListener(delegate { ValidateResources(); });

            var table = DatabaseManager.GetOrCreateTable<UpgradeInfo>();
            //table.Delete(_ => true);

            if (DatabaseManager.GetOrCreateTable<UpgradeInfo>().Any())
            {
                LoadUpgradeInfo();
            }
            else
            {
                upgradeData.currentLvl = 1;
                upgradeData.lvlCost = 10;
                upgradeData.currentResources = 0;
                upgradeData.currentProgress = 0;
                upgradeData.currentShipHealth = 10;
            }

            resourcesWon = CardGameManager.numberOfMonstersKilled * CardGameManager.monsterLevel * 10;
            upgradeData.currentResources += resourcesWon;

            winTxt.text = ("Congratulations! You've earned " + resourcesWon.ToString() + " resources for your ship and crew. You have "
                        + upgradeData.currentResources.ToString() + " in total. How would you like to allocate them?");

            LoadShip();
        }

        private void Start()
        {
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
            shipSliderTxt.text = shipSlider.value.ToString();
            crewSliderTxt.text = crewSlider.value.ToString();
        }

        void OnDestroy()
        {
            SaveUpgradeInfo();
        }

        public void ValidateResources()
        {
            int sliderTotal = (int)shipSlider.value + (int)crewSlider.value;
            if (sliderTotal > upgradeData.currentResources)
            {
                shipSlider.value = currentVal;
                crewSlider.value = currentVal;
            }
        }

        public void Upgrade()
        {
            Slider[] sliderArr = { shipSlider, crewSlider };
            for (int i = 0; i < sliderArr.Length; i++)
            {
                if (upgradeData.currentResources >= upgradeData.lvlCost)
                {
                    upgradeData.currentResources -= (int)sliderArr[i].value;
                    upgradeData.currentProgress += (int)sliderArr[i].value;
                }
                else
                {
                    break;
                }
            }
            LevelUp();
            LoadShip();

            StartCoroutine(ReturnToMap(3f));
        }

        public void LevelUp()
        {
            if (upgradeData.currentResources >= upgradeData.lvlCost)
            {
                upgradeData.lvlCost *= 2;
                upgradeData.currentLvl += 1;
                upgradeData.currentProgress = 0;
                upgradeData.currentShipHealth += 2;
                shipSlider.maxValue = upgradeData.lvlCost;
                NotificationHolder.instance.CreateNotification("Level up! Ship is now Level " + upgradeData.currentLvl.ToString() + "!", 3f);
            }
        }

        public void FeedCrew()
        {
            var currentCrew = CrewManager.instance.crewList;
            currentCrew.RemoveAll(x => x.CrewTag != Crewmates.Status.CrewTag.InCrew);
            if (currentCrew == null || currentCrew.Count == 0) return;
            if (crewSlider.value == 0)
            {
                StartCoroutine(ReturnToMap(3f));
            };

            int count = 0;
            upgradeData.currentResources -= (int)crewSlider.value;
            foreach (var crewmate in CrewManager.instance.crewList)
            {
                if (crewmate.CrewTag == Crewmates.Status.CrewTag.InCrew)
                {
                    count++;
                }
            }

            var remainder = (int)crewSlider.value % count;
            var evenNum = (int)crewSlider.value - remainder;
            var quotient = evenNum / count;

            foreach(var crewmate in CrewManager.instance.crewList)
            {
                if (crewmate.CrewTag == Crewmates.Status.CrewTag.InCrew)
                {
                    crewmate.Morale += quotient;
                }
                if (remainder != 0)
                {
                    crewmate.Morale += 1;
                    remainder -= 1;
                }
            }

            NotificationHolder.instance.CreateNotification("Your crew consumed " + crewSlider.value.ToString() + " resources!", 3f);

            StartCoroutine(ReturnToMap(3f));
        }

        public void LoadShip()
        {
            switch (upgradeData.currentLvl)
            {
                case >= 10:
                    Sprite lvlThreeShip = shipLevels[2];
                    ship.sprite = lvlThreeShip;
                    break;
                case >= 5:
                    Sprite lvlTwoShip = shipLevels[1];
                    ship.sprite = lvlTwoShip;
                    break;
                default:
                    break;
            }
        }

        public void LoadUpgradeInfo()
        {
            var @out = DatabaseManager.GetOrCreateTable<UpgradeInfo>().First();
            if (@out is null) return;

            upgradeData.currentLvl = @out.currentLvl;
            upgradeData.currentProgress = @out.currentProgress;
            upgradeData.lvlCost = @out.lvlCost;
            upgradeData.currentResources = @out.currentResources;
            upgradeData.currentShipHealth = @out.currentShipHealth;
        }

        public void SaveUpgradeInfo()
        {
            var table = DatabaseManager.GetOrCreateTable<UpgradeInfo>();
            table.Delete(_ => true);
            DatabaseManager.database.Insert(new UpgradeInfo {
                id = 0,
                currentLvl = upgradeData.currentLvl,
                currentProgress = upgradeData.currentProgress,
                lvlCost = upgradeData.lvlCost,
                currentResources = upgradeData.currentResources,
                currentShipHealth = upgradeData.currentShipHealth
            });
        }

        IEnumerator ReturnToMap(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            SceneManager.LoadScene("EncounterMapScene");
        }
    }
}