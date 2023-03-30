using System.Collections.Generic;
using SQLite;
using CardBattle.Containers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Crew {
    /// <summary>
    /// Singleton manager responsible for handling all crew members
    /// </summary>
    /// <author>Misha Desear</author>
    public class CrewManager : MonoBehaviour
    {
        public class CrewData 
        {
            [PrimaryKey, Unique, AutoIncrement]
            /// <summary>
            ///     Gets or sets the unique identifier of the crewmate
            /// </summary>
            public int id { set; get; }

            /// <summary>
            ///     Gets or sets the type of the crewmate
            /// </summary>
            public int type { set; get; }

            /// <summary>
            ///     Gets or sets the name of the crewmate
            /// </summary>
            public string name { set; get; }

            /// <summary>
            ///     Gets or sets the status of the crewmate
            /// </summary>
            public int status { set; get; }

            /// <summary>
            ///     Gets or sets the level of the crewmate
            /// </summary>
            public int level { set; get; }

            /// <summary>
            ///     Gets or sets the morale of the crerwmate
            /// </summary>
            public int morale { set; get; }

            /// <summary>
            ///     Gets or sets the current XP of the crewmate
            /// </summary>
            public int currentXp { set; get; }

            /// <summary>
            ///     Gets or sets the XP needed for the crewmate's next level
            /// </summary>
            public int xpNeeded { set; get; }

            // Variables for sprite information

            /// <summary>
            ///     Gets or sets the file path for the base texture
            /// </summary>
            public string basePath { set; get; }
            
            /// <summary>
            ///     Gets or sets file path for the hair texture
            /// </summary>
            public string hairPath { set; get; }

            /// <summary>
            ///     Gets or sets the file path for the eyes texture
            /// </summary>
            public string eyePath { set; get; }

            /// <summary>
            ///     Gets or sets the file path for the brows texture
            /// </summary>
            public string browPath { set; get; }

            /// <summary>
            ///     Gets or sets the file path for the mouth texture
            /// </summary>
            public string mouthPath { set; get; }

            /// <summary>
            ///     Gets or sets the file path for the clothing texture
            /// </summary>
            public string clothesPath { set; get; }

            /// <summary>
            ///     Gets or sets the name of the associated card
            /// </summary>
            public string cardName { set; get; }
        }

        /// <summary>
        /// Singleton instance of this class
        /// <summary>
        public static CrewManager instance;

        [SerializeField] private string[] crewNames;

        [SerializeField] private CardDatabase cardDatabase, wizardDB, navigatorDB, entertainerDB, engineerDB, cookDB, occultistDB, mercenaryDB, deckhandDB;
        public Deck playerDeck;

        public List<Crewmates> crewList; 

        [SerializeField] private GameObject crewPrefab;

        /// <summary>
        /// When the game starts:
        /// </summary>
        public void Awake() 
        {
            // Set up singleton
            instance = this;

            // Load crew from SQL
            LoadCrew();
        }

        /// <summary>
        /// Converts crewmate data stored in SQL into Crewmates objects
        /// </summary> 
        public virtual void LoadCrew()
        {
            var crewmates = DatabaseManager.GetOrCreateTable<CrewData>();

            foreach (var crewmate in crewmates) 
            {
                // Create new Crewmates object to store loaded attributes
                Crewmates loadedCrew = new(); 

                // Load the data of the crewmate
                loadedCrew.Type = (Crewmates.CrewClass.Type)crewmate.type;
                loadedCrew.Name = crewmate.name;
                loadedCrew.CrewTag = (Crewmates.Status.CrewTag)crewmate.status;
                loadedCrew.Level = crewmate.level;
                loadedCrew.Morale = crewmate.morale;
                loadedCrew.CurrentXP = crewmate.currentXp;
                loadedCrew.XPNeeded = crewmate.xpNeeded;

                // Load the sprites for rendering the crewmate
                loadedCrew.BaseSprite = Resources.Load<Sprite>(crewmate.basePath);
                loadedCrew.HairSprite = Resources.Load<Sprite>(crewmate.hairPath);
                loadedCrew.EyeSprite = Resources.Load<Sprite>(crewmate.eyePath);
                loadedCrew.BrowSprite = Resources.Load<Sprite>(crewmate.browPath);
                loadedCrew.MouthSprite = Resources.Load<Sprite>(crewmate.mouthPath);
                loadedCrew.ClothesSprite = Resources.Load<Sprite>(crewmate.clothesPath);

                // Obtain the CardBase of the card that matches the saved name
                loadedCrew.CrewCard = cardDatabase.Instantiate(crewmate.cardName);

                // Add the fully loaded crewmate to the crew list
                crewList.Add(loadedCrew);
            }
        }

        /// <summary>
        /// Spawn in a randomly generated crewmate for crewmate encounters
        /// </summary>
        public void SpawnNewCrewmate()
        {
            // Instantiate the crewmate prefab
            GameObject crewmate = Instantiate(crewPrefab, new Vector2(400, 200), Quaternion.identity);
            crewmate.GetComponent<Crewmates>().transform.localScale = new Vector3(30,30,1);

            // Generate starting crewmate data and load into spawned prefab
            crewmate.GetComponent<Crewmates>().CrewTag = 0;
            crewmate.GetComponent<Crewmates>().Type = (Crewmates.CrewClass.Type)Random.Range(0,7);
            crewmate.GetComponent<Crewmates>().Name = crewNames[Random.Range(0, crewNames.Length)];
            crewmate.GetComponent<Crewmates>().Level = 1;
            crewmate.GetComponent<Crewmates>().Morale = 50;
            crewmate.GetComponent<Crewmates>().CurrentXP = 0;
            crewmate.GetComponent<Crewmates>().XPNeeded = 10;

            // Pick random sprites for each renderer!
            // Pick a random base
            Texture2D baseTexture = Resources.Load<Texture2D>("Crewmates/Bases/" + Random.Range(1,9).ToString()); 
            crewmate.GetComponent<Crewmates>().BaseSprite = Sprite.Create(baseTexture, new Rect(0, 0, 500, 900), new Vector2(250, 450));
            
            // If the crewmate is a wizard...
            if (crewmate.GetComponent<Crewmates>().CrewTag == 0) 
            {
                // ...exclude the first hairstyle since bandana in Style 1 and wizard hat overlap strangely
                crewmate.GetComponent<Crewmates>().HairSprite = Resources.Load<Sprite>("Crewmates/Hair/Style " + Random.Range(2,6).ToString() + "/" + Random.Range(1,4).ToString());
            }
            else
            {
                // Otherwise, pick any random hairstyle and random color
                crewmate.GetComponent<Crewmates>().HairSprite = Resources.Load<Sprite>("Crewmates/Hair/Style " + Random.Range(1,6).ToString() + "/" + Random.Range(1,4).ToString());
            }
            // Pick a random pair of eyes
            crewmate.GetComponent<Crewmates>().EyeSprite = Resources.Load<Sprite>("Crewmates/Eyes/" + Random.Range(1,6).ToString());
            // Pick a random pair of brows
            crewmate.GetComponent<Crewmates>().BrowSprite = Resources.Load<Sprite>("Crewmates/Eyebrows/" + Random.Range(1,10).ToString());
            // Pick a random mouth
            crewmate.GetComponent<Crewmates>().MouthSprite = Resources.Load<Sprite>("Crewmates/Mouths/" + Random.Range(1,9).ToString());

            // Pick a random colored outfit based on crew member's type
            // TODO: un-comment other cases as clothes are completed
            switch (crewmate.GetComponent<Crewmates>().Type)
            {
                case (Crewmates.CrewClass.Type)0: // For wizards
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Resources.Load<Sprite>("Crewmates/Clothes/Wizard/" + Random.Range(1,4).ToString());
                    break;
                /*case (Crewmates.CrewClass.Type)1: // For navigators
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Resources.Load<Sprite>("Crewmates/Clothes/Navigator" + Random.Range(1,4).ToString());
                    break;
                case (Crewmates.CrewClass.Type)2: // For entertainers
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Resources.Load<Sprite>("Crewmates/Clothes/Entertainer" + Random.Range(1,4).ToString());
                    break;
                case (Crewmates.CrewClass.Type)3: // For engineers
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Resources.Load<Sprite>("Crewmates/Clothes/Engineer" + Random.Range(1,4).ToString());
                    break;
                case (Crewmates.CrewClass.Type)4: // For cooks
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Resources.Load<Sprite>("Crewmates/Clothes/Cook" + Random.Range(1,4).ToString());
                    break;*/
                case (Crewmates.CrewClass.Type)5: // For occultists
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Resources.Load<Sprite>("Crewmates/Clothes/Occultist" + Random.Range(1,4).ToString());
                    break;
                /*case (Crewmates.CrewClass.Type)6: // For mercenaries
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Resources.Load<Sprite>("Crewmates/Clothes/Mercenary" + Random.Range(1,4).ToString());
                    break;*/
                case (Crewmates.CrewClass.Type)7: // For deckhands
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Resources.Load<Sprite>("Crewmates/Clothes/Deckhand" + Random.Range(1,4).ToString());
                    break;
                default:
                    break;
            }
            
            // Load sprites from attributes into corresponding sprite renderers
            crewmate.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = crewmate.GetComponent<Crewmates>().BaseSprite; 
            crewmate.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = crewmate.GetComponent<Crewmates>().HairSprite;
            crewmate.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = crewmate.GetComponent<Crewmates>().ClothesSprite; 
            crewmate.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = crewmate.GetComponent<Crewmates>().EyeSprite; 
            crewmate.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = crewmate.GetComponent<Crewmates>().BrowSprite; 
            crewmate.transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = crewmate.GetComponent<Crewmates>().MouthSprite; 

            // Add crewmates component into crew list
            crewList.Add(crewmate.GetComponent<Crewmates>());
        }

        public void AddToCrew(Crewmates crew)
        {
            crew.CrewTag = (Crewmates.Status.CrewTag)2; // Change crewmate's crew tag to InCrew (2)
            playerDeck.AddCard(Instantiate(crew.CrewCard)); // Add crewmate's associated card to the deck
            
        }

        public void RemoveFromCrew(Crewmates crew)
        {
            crew.CrewTag = (Crewmates.Status.CrewTag)1; // Change crewmate's crew tag to WasInCrew(1)
            playerDeck.RemoveCard(crew.CrewCard.name); // Remove the crewmate's associated card from the deck
        }
    }
}