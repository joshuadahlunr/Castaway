using System.Collections;
using TMPro;
using System.Linq;
using SQLite;
using UnityEngine;
using UnityEngine.UI;
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
        }
        
        public static ResourceManager instance;

        [SerializeField] private Slider shipSlider, crewSlider;

        [SerializeField] private Button confirmBtn;

        [SerializeField] private UpgradeData upgradeData;

        [SerializeField] private Sprite[] shipLevels;

        [SerializeField] private TextMeshProUGUI shipSliderTxt, crewSliderTxt, winTxt;

        [SerializeField] private int resourcesWon;

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
            table.Delete(_ => true);

            if (DatabaseManager.GetOrCreateTable<UpgradeInfo>().Any())
            {
                LoadUpgradeInfo();
            } 
            else
            {
                upgradeData.Level = 1;
                upgradeData.Cost = 10;
                upgradeData.Resources = 0;
                upgradeData.Progress = 0;
            }

            upgradeData.Resources += resourcesWon;

            winTxt.text = ("Congratulations! You've earned " + resourcesWon.ToString() + " resources for your ship and crew. You have "
                        + upgradeData.Resources.ToString() + " in total. How would you like to allocate them?");

            LoadShip();
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
            if (sliderTotal > upgradeData.Resources)
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
                if (upgradeData.CanBuy())
                {
                    upgradeData.Resources -= (int)sliderArr[i].value;
                    upgradeData.Progress += (int)sliderArr[i].value;
                }
                else
                {
                    break;
                }
            }
            LevelUp();
            LoadShip();
        }

        public void LevelUp()
        {
            if (upgradeData.Progress >= upgradeData.Cost)
            {
                upgradeData.Cost *= 2;
                upgradeData.Level += 1;
                upgradeData.ResetProgress();
                shipSlider.maxValue = upgradeData.Cost;
                NotificationHolder.instance.CreateNotification("Level up! Ship is now Level " + upgradeData.Level.ToString() + "!", 3f);
            }
        }

        public void FeedCrew()
        {
            var currentCrew = CrewManager.instance.crewList;
            currentCrew.RemoveAll(x => x.CrewTag != Crewmates.Status.CrewTag.InCrew);
            if (currentCrew == null) return;

            int count = 0;
            upgradeData.Resources -= (int)crewSlider.value;
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
        }

        public void LoadShip()
        {
            switch (upgradeData.Level)
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
            GameObject obj = new GameObject("obj");
            obj.AddComponent<UpgradeData>();

            var @out = DatabaseManager.GetOrCreateTable<UpgradeInfo>().First();
            if (@out is null) return;

            upgradeData.Level = @out.currentLvl;
            upgradeData.Progress = @out.currentProgress;
            upgradeData.Cost = @out.lvlCost;
            upgradeData.Resources = @out.currentResources;         
        }

        public void SaveUpgradeInfo()
        {
            var table = DatabaseManager.GetOrCreateTable<UpgradeInfo>();
            table.Delete(_ => true);
            DatabaseManager.database.Insert(new UpgradeInfo {
                id = 0,
                currentLvl = upgradeData.Level,
                currentProgress = upgradeData.Progress,
                lvlCost = upgradeData.Cost,
                currentResources = upgradeData.Resources
            });
        }
    }
}