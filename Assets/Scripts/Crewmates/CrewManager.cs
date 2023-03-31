using System.Collections.Generic;
using SQLite;
using CardBattle.Containers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Crew {
    /// <summary>
    ///     Singleton manager responsible for handling all crew members
    /// </summary>
    /// <author>Misha Desear</author>
    public class CrewManager : MonoBehaviour
    {
        /// <summary>
        ///     Represents a single crewmate in the SQL database
        /// </summary>
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
        ///     Singleton instance of this class
        /// <summary>
        public static CrewManager instance;

        [SerializeField] private string[] crewNames, cookCards, deckhandCards, engineerCards, entertainerCards, mercenaryCards, occultistCards, wizardCards;

        [SerializeField] private CardDatabase cardDatabase;

        public List<Crewmates> crewList; 

        [SerializeField] private GameObject crewPrefab;

        /// <summary>
        ///     When the game starts:
        /// </summary>
        public void Awake() 
        {
            // Set up singleton
            instance = this;

            // Load crew from SQL
            LoadCrew();
        }

        /// <summary>
        ///     Converts crewmate data stored in SQL into Crewmates objects
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
                loadedCrew.CrewCard = crewmate.cardName;

                // Load the sprites for rendering the crewmate
                loadedCrew.BaseSprite = Resources.Load<Sprite>(crewmate.basePath);
                loadedCrew.HairSprite = Resources.Load<Sprite>(crewmate.hairPath);
                loadedCrew.EyeSprite = Resources.Load<Sprite>(crewmate.eyePath);
                loadedCrew.BrowSprite = Resources.Load<Sprite>(crewmate.browPath);
                loadedCrew.MouthSprite = Resources.Load<Sprite>(crewmate.mouthPath);
                loadedCrew.ClothesSprite = Resources.Load<Sprite>(crewmate.clothesPath);

                // Add the fully loaded crewmate to the crew list
                crewList.Add(loadedCrew);
            }
        }

        /// <summary>
        ///     Spawn in a randomly generated crewmate for crewmate encounters
        /// </summary>
        public void SpawnNewCrewmate()
        {
            // Instantiate the crewmate prefab
            GameObject crewmate = Instantiate(crewPrefab, new Vector2(0, 0), Quaternion.identity);
            crewmate.GetComponent<Crewmates>().transform.localScale = new Vector3(1,1,1);

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
            crewmate.GetComponent<Crewmates>().BaseSprite = Sprite.Create(baseTexture, new Rect(0.0f, 0.0f, baseTexture.width, baseTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            
            // If the crewmate is a wizard...
            if (crewmate.GetComponent<Crewmates>().CrewTag == 0) 
            {
                // ...exclude the first hairstyle since bandana in Style 1 and wizard hat overlap strangely
                Texture2D hairTexture = Resources.Load<Texture2D>("Crewmates/Hair/Style " + Random.Range(2,6).ToString() + "/" + Random.Range(1,4).ToString());
                crewmate.GetComponent<Crewmates>().HairSprite = Sprite.Create(hairTexture, new Rect(0.0f, 0.0f, hairTexture.width, hairTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }

            else
            {
                // Otherwise, pick any random hairstyle and random color
                Texture2D hairTexture = Resources.Load<Texture2D>("Crewmates/Hair/Style " + Random.Range(1,6).ToString() + "/" + Random.Range(1,4).ToString());
                crewmate.GetComponent<Crewmates>().HairSprite = Sprite.Create(hairTexture, new Rect(0.0f, 0.0f, hairTexture.width, hairTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
            }

            // Pick a random pair of eyes
            Texture2D eyeTexture = Resources.Load<Texture2D>("Crewmates/Eyes/" + Random.Range(1,6).ToString());
            crewmate.GetComponent<Crewmates>().EyeSprite = Sprite.Create(eyeTexture, new Rect(0.0f, 0.0f, eyeTexture.width, eyeTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            // Pick a random pair of brows
            Texture2D browTexture = Resources.Load<Texture2D>("Crewmates/Eyebrows/" + Random.Range(1,10).ToString());
            crewmate.GetComponent<Crewmates>().BrowSprite = Sprite.Create(browTexture, new Rect(0, 0, browTexture.width, browTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            // Pick a random mouth
            Texture2D mouthTexture = Resources.Load<Texture2D>("Crewmates/Mouths/" + Random.Range(1,9).ToString());
            crewmate.GetComponent<Crewmates>().MouthSprite = Sprite.Create(mouthTexture, new Rect(0, 0, mouthTexture.width, mouthTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            // Pick a random colored outfit based on crew member's type
            // TODO: un-comment other cases as clothes are completed
            switch (crewmate.GetComponent<Crewmates>().Type)
            {
                case (Crewmates.CrewClass.Type)0: // For wizards
                    Texture2D clothesTexture = Resources.Load<Texture2D>("Crewmates/Clothes/Wizard/" + Random.Range(1,4).ToString());
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Sprite.Create(clothesTexture, new Rect(0, 0, clothesTexture.width, clothesTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;
                /*case (Crewmates.CrewClass.Type)1: // For navigators
                    clothesTexture = Resources.Load<Texture2D>("Crewmates/Clothes/Navigator/" + Random.Range(1,4).ToString());
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Sprite.Create(clothesTexture, new Rect(0, 0, clothesTexture.width, clothesTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;
                case (Crewmates.CrewClass.Type)2: // For entertainers
                    clothesTexture = Resources.Load<Texture2D>("Crewmates/Clothes/Entertainer/" + Random.Range(1,4).ToString());
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Sprite.Create(clothesTexture, new Rect(0, 0, clothesTexture.width, clothesTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;
                case (Crewmates.CrewClass.Type)3: // For engineers
                    clothesTexture = Resources.Load<Texture2D>("Crewmates/Clothes/Engineer/" + Random.Range(1,4).ToString());
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Sprite.Create(clothesTexture, new Rect(0, 0, clothesTexture.width, clothesTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;
                case (Crewmates.CrewClass.Type)4: // For cooks
                    clothesTexture = Resources.Load<Texture2D>("Crewmates/Clothes/Cook/" + Random.Range(1,4).ToString());
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Sprite.Create(clothesTexture, new Rect(0, 0, clothesTexture.width, clothesTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;*/
                case (Crewmates.CrewClass.Type)5: // For occultists
                    clothesTexture = Resources.Load<Texture2D>("Crewmates/Clothes/Occultist/" + Random.Range(1,4).ToString());
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Sprite.Create(clothesTexture, new Rect(0, 0, clothesTexture.width, clothesTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;
                /*case (Crewmates.CrewClass.Type)6: // For mercenaries
                    clothesTexture = Resources.Load<Texture2D>("Crewmates/Clothes/Mercenary/" + Random.Range(1,4).ToString());
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Sprite.Create(clothesTexture, new Rect(0, 0, clothesTexture.width, clothesTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;*/
                case (Crewmates.CrewClass.Type)7: // For deckhands
                    clothesTexture = Resources.Load<Texture2D>("Crewmates/Clothes/Deckhand/" + Random.Range(1,4).ToString());
                    crewmate.GetComponent<Crewmates>().ClothesSprite = Sprite.Create(clothesTexture, new Rect(0, 0, clothesTexture.width, clothesTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    break;
                default: // If we can't load the appropriate resource, break (this means crewmate is naked :/)
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

        /// <summary>
        ///     Flags a crew member as currently in the player's crew and adds their card to the player deck database
        /// </summary>
        public void AddToCrew(Crewmates crew)
        {
            const string playerDeckName = "Player Deck";
            crew.CrewTag = (Crewmates.Status.CrewTag)2; // Change crewmate's crew tag to InCrew (2)
            // Fetch the SQL table containing the player deck
			var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
				.FirstOrDefault(l => l.name == playerDeckName).id;
            // Insert the crewmate's card into the database based on crewmate's attributes
            DatabaseManager.database.Insert(new Deck.DeckListCard
            {
                listID = playerDeckId,
                name = crew.CrewCard,
                level = crew.Level
            });
        }

        /// <summary>
        ///     Flags a crew member as formerly being in the player's crew and removes their card from the player deck database
        /// </summary>
        public void RemoveFromCrew(Crewmates crew)
        {
            const string playerDeckName = "Player Deck";
            crew.CrewTag = (Crewmates.Status.CrewTag)1; // Change crewmate's crew tag to WasInCrew(1)
            var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
                .FirstOrDefault(l => l.name == playerDeckName).id;
            
        }
    }
}