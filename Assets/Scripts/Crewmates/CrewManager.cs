using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;
using Extensions;
using CardBattle;
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
            ///     Gets or sets the database index for the base texture
            /// </summary>
            public int baseIndex { set; get; }
            
            /// <summary>
            ///     Gets or sets the database index for the hair texture
            /// </summary>
            public int hairIndex { set; get; }

            /// <summary>
            ///     Gets or sets the database index for the eyes texture
            /// </summary>
            public int eyeIndex { set; get; }

            /// <summary>
            ///     Gets or sets the database index for the brows texture
            /// </summary>
            public int browIndex { set; get; }

            ///             var remainder = (int)crewSlider.value;<summary>
            ///     Gets or sets the database index for the mouth texture
            /// </summary>
            public int mouthIndex { set; get; }

            /// <summary>
            ///     Gets or sets the database index for the clothing texture
            /// </summary>
            public int clothesIndex { set; get; }

            /// <summary>
            ///     Gets or sets the name of the associated card
            /// </summary>
            public string cardName { set; get; }
        }


        /// <summary>
        ///     Singleton instance of this class
        /// <summary>
        public static CrewManager instance;

        [SerializeField] private string[] crewNames;

        [SerializeField] private CardDatabase cardDatabase, cookCards, deckhandCards, engineerCards, entertainerCards, mercenaryCards, navigatorCards, occultistCards, wizardCards;

        [SerializeField] private SpriteDatabase baseTextures, browTextures, eyeTextures, mouthTextures, hairTextures, clothesTextures;
        
        public List<Crewmates> crewList; 

        [SerializeField] private GameObject crewPrefab;

        /// <summary>
        ///     When the game starts:
        /// </summary>
        public void Awake() 
        {
            // Set up singleton
            instance = this;

            var table = DatabaseManager.GetOrCreateTable<CrewData>();
            //table.Delete(_ => true);
            if (table != null)
            {
                LoadCrew();
            }
        }

        void OnDestroy()
        {
            SaveCrew();
        }

        /// <summary>
        ///     Converts crewmate data stored in SQL into Crewmates objects for use in crewmate interaction
        /// </summary> 
        public virtual void LoadCrew()
        {
            var crewmates = DatabaseManager.GetOrCreateTable<CrewData>();

            foreach (var crewmate in crewmates) 
            {
                // Create new Crewmates object to store loaded attributes
                GameObject obj = new GameObject("obj");
                obj.AddComponent<Crewmates>();

                Crewmates loadedCrew = obj.GetComponent<Crewmates>();

                // Load the data of the crewmate
                loadedCrew.Type = (Crewmates.CrewClass.Type)crewmate.type;
                loadedCrew.Name = crewmate.name;
                loadedCrew.CrewTag = (Crewmates.Status.CrewTag)crewmate.status;
                loadedCrew.Level = crewmate.level;
                loadedCrew.Morale = crewmate.morale;
                loadedCrew.CurrentXP = crewmate.currentXp;
                loadedCrew.XPNeeded = crewmate.xpNeeded;
                loadedCrew.CrewCard = crewmate.cardName;

                // Load the textures for rendering the crewmate
                // Base sprite texture
                loadedCrew.BaseSprite = baseTextures.sprites.ElementAt(crewmate.baseIndex).Value;
                // Hair sprite texture
                loadedCrew.HairSprite = hairTextures.sprites.ElementAt(crewmate.hairIndex).Value;
                // Eye sprite texture
                loadedCrew.EyeSprite = eyeTextures.sprites.ElementAt(crewmate.eyeIndex).Value;
                // Brow sprite texture
                loadedCrew.BrowSprite = browTextures.sprites.ElementAt(crewmate.browIndex).Value;
                // Mouth sprite texture
                loadedCrew.MouthSprite = mouthTextures.sprites.ElementAt(crewmate.mouthIndex).Value;
                // Clothing sprite texture
                loadedCrew.ClothesSprite = clothesTextures.sprites.ElementAt(crewmate.clothesIndex).Value;

                // Add the fully loaded crewmate to the crew list
                crewList.Add(loadedCrew);
            }
        }

        /// <summary>
        ///     Saves data in the crew list to the SQL table containing crew information
        /// </summary>
        public static void SaveCrew()
        {
            if (instance.crewList == null || instance.crewList.Count == 0)
            {
                return;
            }
            var newList = instance.crewList;
            newList.RemoveAll(x => x.CrewTag == Crewmates.Status.CrewTag.NotInCrew);

            var crewmates = DatabaseManager.GetOrCreateTable<CrewData>();
            crewmates.Delete(_ => true);

            foreach (var crewmate in instance.crewList) 
            {
                DatabaseManager.database.Insert(new CrewData
                {
                    type = (int) crewmate.Type,
                    name = crewmate.Name,
                    status = (int) crewmate.CrewTag,
                    level = crewmate.Level,
                    morale = crewmate.Morale,
                    currentXp = crewmate.CurrentXP,
                    xpNeeded = crewmate.XPNeeded,
                    cardName = crewmate.CrewCard,
                    
                    baseIndex = instance.baseTextures.sprites.Where(pair => pair.Value == crewmate.BaseSprite).Select(pair => pair.Key).FirstOrDefault(),
                    hairIndex = instance.hairTextures.sprites.Where(pair => pair.Value == crewmate.HairSprite).Select(pair => pair.Key).FirstOrDefault(),
                    eyeIndex = instance.eyeTextures.sprites.Where(pair => pair.Value == crewmate.EyeSprite).Select(pair => pair.Key).FirstOrDefault(),
                    browIndex = instance.browTextures.sprites.Where(pair => pair.Value == crewmate.BrowSprite).Select(pair => pair.Key).FirstOrDefault(),
                    mouthIndex = instance.mouthTextures.sprites.Where(pair => pair.Value == crewmate.MouthSprite).Select(pair => pair.Key).FirstOrDefault(),
                    clothesIndex = instance.clothesTextures.sprites.Where(pair => pair.Value == crewmate.ClothesSprite).Select(pair => pair.Key).FirstOrDefault()
                });
            }
        }

        /// <summary>
        ///     Spawn in a random crewmate that was in a previous run for crewmate encounters
        /// </summary>
        public Crewmates SpawnOldCrewmate()
        {
            // Instantiate the crewmate prefab
            GameObject crewmate = Instantiate(crewPrefab, new Vector3(0, -2, 10), Quaternion.identity);
            crewmate.transform.localScale = new Vector3(1.5f,1.5f,1f);
            
            List<Crewmates> shuffledCrew = crewList;
            shuffledCrew.Shuffle();

            var randomCrewmate = shuffledCrew.Find(c => c.CrewTag == (Crewmates.Status.CrewTag)1);

            crewmate.GetComponent<Crewmates>().CrewTag = randomCrewmate.CrewTag;
            crewmate.GetComponent<Crewmates>().Type = randomCrewmate.Type;
            crewmate.GetComponent<Crewmates>().Name = randomCrewmate.Name;
            crewmate.GetComponent<Crewmates>().Level = randomCrewmate.Level;
            crewmate.GetComponent<Crewmates>().Morale = randomCrewmate.Morale;
            crewmate.GetComponent<Crewmates>().CurrentXP = randomCrewmate.CurrentXP;
            crewmate.GetComponent<Crewmates>().XPNeeded = randomCrewmate.XPNeeded;
            crewmate.GetComponent<Crewmates>().CrewCard = randomCrewmate.CrewCard;

            crewmate.GetComponent<Crewmates>().BaseSprite = randomCrewmate.BaseSprite;
            crewmate.GetComponent<Crewmates>().HairSprite = randomCrewmate.HairSprite;
            crewmate.GetComponent<Crewmates>().MouthSprite = randomCrewmate.MouthSprite;
            crewmate.GetComponent<Crewmates>().EyeSprite = randomCrewmate.EyeSprite;
            crewmate.GetComponent<Crewmates>().BrowSprite = randomCrewmate.BrowSprite;
            crewmate.GetComponent<Crewmates>().ClothesSprite = randomCrewmate.ClothesSprite;

            Texture2D baseTexture = crewmate.GetComponent<Crewmates>().BaseSprite;
            crewmate.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Sprite.Create(baseTexture, new Rect(0, 0, baseTexture.width, baseTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Texture2D hairTexture = crewmate.GetComponent<Crewmates>().HairSprite;
            crewmate.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Sprite.Create(hairTexture, new Rect(0, 0, hairTexture.width, hairTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Texture2D clothesTexture = crewmate.GetComponent<Crewmates>().ClothesSprite;
            crewmate.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Sprite.Create(clothesTexture, new Rect(0, 0, clothesTexture.width, clothesTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Texture2D eyeTexture = crewmate.GetComponent<Crewmates>().EyeSprite;
            crewmate.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Sprite.Create(eyeTexture, new Rect(0, 0, eyeTexture.width, eyeTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Texture2D browTexture = crewmate.GetComponent<Crewmates>().BrowSprite; 
            crewmate.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = Sprite.Create(browTexture, new Rect(0, 0, browTexture.width, browTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Texture2D mouthTexture = crewmate.GetComponent<Crewmates>().MouthSprite;
            crewmate.transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Sprite.Create(mouthTexture, new Rect(0, 0, mouthTexture.width, mouthTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            return crewmate.GetComponent<Crewmates>();
        }

        /// <summary>
        ///     Spawn in a randomly generated crewmate for crewmate encounters
        /// </summary>
        public Crewmates SpawnNewCrewmate()
        {
            // Instantiate the crewmate prefab
            GameObject crewmate = Instantiate(crewPrefab, new Vector3(0, -2, 10), Quaternion.identity);
            crewmate.transform.localScale = new Vector3(1.5f,1.5f,1f);

            // Generate starting crewmate data and load into spawned prefab
            crewmate.GetComponent<Crewmates>().CrewTag = (Crewmates.Status.CrewTag)0;
            crewmate.GetComponent<Crewmates>().Type = (Crewmates.CrewClass.Type)Random.Range(1,8);
            crewmate.GetComponent<Crewmates>().Name = crewNames[Random.Range(0, crewNames.Length)];
            crewmate.GetComponent<Crewmates>().Level = 1;
            crewmate.GetComponent<Crewmates>().Morale = 50;
            crewmate.GetComponent<Crewmates>().CurrentXP = 0;
            crewmate.GetComponent<Crewmates>().XPNeeded = 10;

            // Select a random card for the generated crewmate based on their type
            switch (crewmate.GetComponent<Crewmates>().Type)
            {
                case (Crewmates.CrewClass.Type)1: // For wizards
                    var randomCard = wizardCards.cards.Keys.Shuffle().First();
                    crewmate.GetComponent<Crewmates>().CrewCard = randomCard;
                    break;
                case (Crewmates.CrewClass.Type)2: // For navigators
                    randomCard = navigatorCards.cards.Keys.Shuffle().First();
                    crewmate.GetComponent<Crewmates>().CrewCard = randomCard;
                    break;
                case (Crewmates.CrewClass.Type)3: // For entertainers
                    randomCard = entertainerCards.cards.Keys.Shuffle().First();
                    crewmate.GetComponent<Crewmates>().CrewCard = randomCard;
                    break;
                case (Crewmates.CrewClass.Type)4: // For engineers
                    randomCard = engineerCards.cards.Keys.Shuffle().First();
                    crewmate.GetComponent<Crewmates>().CrewCard = randomCard;
                    break;
                case (Crewmates.CrewClass.Type)5: // For cooks
                    randomCard = cookCards.cards.Keys.Shuffle().First();
                    crewmate.GetComponent<Crewmates>().CrewCard = randomCard;
                    break;
                case (Crewmates.CrewClass.Type)6: // For occultists
                    randomCard = occultistCards.cards.Keys.Shuffle().First();
                    crewmate.GetComponent<Crewmates>().CrewCard = randomCard;
                    break;
                case (Crewmates.CrewClass.Type)7: // For mercenaries 
                    randomCard = mercenaryCards.cards.Keys.Shuffle().First();
                    crewmate.GetComponent<Crewmates>().CrewCard = randomCard;
                    break;
                case (Crewmates.CrewClass.Type)8: // For deckhands
                    randomCard = deckhandCards.cards.Keys.Shuffle().First();
                    crewmate.GetComponent<Crewmates>().CrewCard = randomCard;
                    break; 
                default: // If we can't find the crewmate's type, break
                    break;
            }

            // Pick random sprites for each renderer!
            // Pick a random base
            crewmate.GetComponent<Crewmates>().BaseSprite = baseTextures.sprites.ElementAt(Random.Range(0, baseTextures.sprites.Count)).Value;
            
            // If the crewmate is a wizard or chef...
            if (crewmate.GetComponent<Crewmates>().Type == (Crewmates.CrewClass.Type)0 || crewmate.GetComponent<Crewmates>().Type == (Crewmates.CrewClass.Type)4) 
            {
                // ...exclude the first hairstyle since bandana in Style 1 and hats in outfits overlap strangely
                crewmate.GetComponent<Crewmates>().HairSprite = hairTextures.sprites.ElementAt(Random.Range(4, hairTextures.sprites.Count)).Value;
            }

            else
            {
                // Otherwise, pick any random hairstyle and random color
                crewmate.GetComponent<Crewmates>().HairSprite = hairTextures.sprites.ElementAt(Random.Range(0, hairTextures.sprites.Count)).Value;
            }

            // Pick a random pair of eyes
            crewmate.GetComponent<Crewmates>().EyeSprite = eyeTextures.sprites.ElementAt(Random.Range(0, eyeTextures.sprites.Count)).Value;

            // Pick a random pair of brows
            crewmate.GetComponent<Crewmates>().BrowSprite = browTextures.sprites.ElementAt(Random.Range(0, browTextures.sprites.Count)).Value;

            // Pick a random mouth
            crewmate.GetComponent<Crewmates>().MouthSprite = mouthTextures.sprites.ElementAt(Random.Range(0, mouthTextures.sprites.Count)).Value;

            // Pick a randomly colored outfit based on crew member's type
            // TODO: un-comment other cases as clothes are completed
            switch (crewmate.GetComponent<Crewmates>().Type)
            {
                case (Crewmates.CrewClass.Type)1: // For wizards
                    crewmate.GetComponent<Crewmates>().ClothesSprite = clothesTextures.sprites.ElementAt(Random.Range(28, 31)).Value;
                    break;
                case (Crewmates.CrewClass.Type)2: // For navigators
                    crewmate.GetComponent<Crewmates>().ClothesSprite = clothesTextures.sprites.ElementAt(Random.Range(20, 23)).Value;
                    break;
                case (Crewmates.CrewClass.Type)3: // For entertainers
                    crewmate.GetComponent<Crewmates>().ClothesSprite = clothesTextures.sprites.ElementAt(Random.Range(12, 15)).Value;
                    break;
                case (Crewmates.CrewClass.Type)4: // For engineers
                    crewmate.GetComponent<Crewmates>().ClothesSprite = clothesTextures.sprites.ElementAt(Random.Range(8, 11)).Value;
                    break;
                case (Crewmates.CrewClass.Type)5: // For cooks
                    crewmate.GetComponent<Crewmates>().ClothesSprite = clothesTextures.sprites.ElementAt(Random.Range(0, 3)).Value;
                    break;
                case (Crewmates.CrewClass.Type)6: // For occultists
                    crewmate.GetComponent<Crewmates>().ClothesSprite = clothesTextures.sprites.ElementAt(Random.Range(24, 27)).Value;
                    break;
                case (Crewmates.CrewClass.Type)7: // For mercenaries
                    crewmate.GetComponent<Crewmates>().ClothesSprite = clothesTextures.sprites.ElementAt(Random.Range(16, 19)).Value;
                    break;
                case (Crewmates.CrewClass.Type)8: // For deckhands
                    crewmate.GetComponent<Crewmates>().ClothesSprite = clothesTextures.sprites.ElementAt(Random.Range(4, 7)).Value;
                    break;
                default: // If we can't load the appropriate resource, break (this means crewmate is naked :/)
                    break;
            }
            
            // Load sprites from attributes into corresponding sprite renderers

            Texture2D baseTexture = crewmate.GetComponent<Crewmates>().BaseSprite;
            crewmate.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Sprite.Create(baseTexture, new Rect(0, 0, baseTexture.width, baseTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Texture2D hairTexture = crewmate.GetComponent<Crewmates>().HairSprite;
            crewmate.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Sprite.Create(hairTexture, new Rect(0, 0, hairTexture.width, hairTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Texture2D clothesTexture = crewmate.GetComponent<Crewmates>().ClothesSprite;
            crewmate.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Sprite.Create(clothesTexture, new Rect(0, 0, clothesTexture.width, clothesTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Texture2D eyeTexture = crewmate.GetComponent<Crewmates>().EyeSprite;
            crewmate.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Sprite.Create(eyeTexture, new Rect(0, 0, eyeTexture.width, eyeTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Texture2D browTexture = crewmate.GetComponent<Crewmates>().BrowSprite; 
            crewmate.transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = Sprite.Create(browTexture, new Rect(0, 0, browTexture.width, browTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            Texture2D mouthTexture = crewmate.GetComponent<Crewmates>().MouthSprite;
            crewmate.transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Sprite.Create(mouthTexture, new Rect(0, 0, mouthTexture.width, mouthTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

            // Add crewmates component into crew list
            crewList.Add(crewmate.GetComponent<Crewmates>());

            // Return a reference to this new crewmate
            return crewmate.GetComponent<Crewmates>();
        }

        /// <summary>
        ///     Flags a crew member as currently in the player's crew and adds their card to the player deck database
        /// </summary>
        public void AddToCrew(Crewmates crew)
        {
            const string playerDeckName = "Player Deck";
            int listIndex = crewList.FindIndex(c => c == crew); // Obtain the crewmate's index in crew list
            // Fetch the ID of the SQL table containing the player deck
			var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
				.FirstOrDefault(l => l.name == playerDeckName).id;
            // Insert the crewmate's card into the database based on crewmate's attributes
            DatabaseManager.database.Insert(new Deck.DeckListCard
            {
                listID = playerDeckId,
                name = crew.CrewCard,
                level = crew.Level,
                associatedCrewmateID = crewList.FindIndex(c => c == crew)
            });
            crew.CrewTag = (Crewmates.Status.CrewTag)2; // Change crewmate's crew tag to InCrew (2)
            if (listIndex != 1) // If we were able to find a valid index...
            {
                crewList[listIndex] = crew; // ...update that crewmate in the list!
            }
            SaveCrew();
        }

        /// <summary>
        ///     Flags a crew member as formerly being in the player's crew and removes their card from the player deck database
        /// </summary>
        public void RemoveFromCrew(Crewmates crew)
        {
            const string playerDeckName = "Player Deck";
            int listIndex = crewList.FindIndex(c => c == crew); // Obtain the crewmate's index in crew list
            // Fetch the ID of the SQL table containing the player deck
            var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
                .FirstOrDefault(l => l.name == playerDeckName).id;
            // Find the matching card in the player's deck that shares the same index
            var cardMatch = DatabaseManager.GetOrCreateTable<Deck.DeckListCard>()
                .FirstOrDefault(c => c.associatedCrewmateID == listIndex && c.listID == playerDeckId);
            DatabaseManager.database.Delete(cardMatch); // Delete it!
            crew.CrewTag = (Crewmates.Status.CrewTag)1; // Change crewmate's crew tag to WasInCrew (1)
            if (listIndex != 1) // If we were able to find a valid index... 
            {
                crewList[listIndex] = crew; // ...update that crewmate in the list!
            }
            SaveCrew();
        }

        /// <summary>
        ///     Flags a crew member as formerly being in player's crew and removes card from player deck DB
        /// </summary>
        /// <param name="crewmateID">The ID of the crewmate to be removed</param> 
        public static void RemoveWithId(int crewmateID)
        {
            const string playerDeckName = "Player Deck"; // Obtain the crewmate's index in crew list
            // Fetch the ID of the SQL table containing the player deck
            var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
                .FirstOrDefault(l => l.name == playerDeckName).id;
            // Find the matching card in the player's deck that shares the same index
            var cardMatch = DatabaseManager.GetOrCreateTable<Deck.DeckListCard>()
                .FirstOrDefault(c => c.associatedCrewmateID == crewmateID && c.listID == playerDeckId);
            DatabaseManager.database.Delete(cardMatch); // Delete it!
            var crewmate = DatabaseManager.GetOrCreateTable<CrewData>()
                .FirstOrDefault(c => c.id == crewmateID);
            crewmate.id = 1; // Change to WasInCrew(1)
            SaveCrew(); // Save the newly updated crew list to SQL
        }

        /// <summary>
        ///     Flags all members of crew as formerly being in player's crew and removes their cards from player deck DB
        /// </summary>
        public static void SetAllCrewToFormer()
        {
            const string playerDeckName = "Player Deck"; // Obtain the crewmate's index in crew list
            // Fetch the ID of the SQL table containing the player deck
            var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
                .FirstOrDefault(l => l.name == playerDeckName).id;
            var crew = DatabaseManager.GetOrCreateTable<CrewData>();
            // For each member in the crew list...
            for (int i = 0; i > crew.Count(); i++)
            {
                // Find the matching card in the player's deck that shares the same index
                var cardMatch = DatabaseManager.GetOrCreateTable<Deck.DeckListCard>()
                    .FirstOrDefault(c => c.associatedCrewmateID == i && c.listID == playerDeckId);
                DatabaseManager.database.Delete(cardMatch); // Delete it!
                var crewmate = DatabaseManager.GetOrCreateTable<CrewData>()
                    .FirstOrDefault(c => c.id == i); 
                if (crewmate.status == 2) // If the crewmate was in the crew...
                {
                    crewmate.status = 1; // ...change crewmate's crew tag to WasInCrew(1)
                }
            }
        }

        /// <summary>
        ///     Flags a crew member as no longer being in player's crew and removes card from player deck DB
        /// </summary>
        /// <remarks>This makes it impossible for the specified crew member to be encountered in other runs!</remarks>
        /// <param name="crewmateID">The ID of the crewmate to be removed</param>
        public static void KillCrewmate(int crewmateID)
        {
            const string playerDeckName = "Player Deck"; // Obtain the crewmate's index in crew list
            // Fetch the ID of the SQL table containing the player deck
            var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
                .FirstOrDefault(l => l.name == playerDeckName).id;
            // Find the matching card in the player's deck that shares the same index
            var cardMatch = DatabaseManager.GetOrCreateTable<Deck.DeckListCard>()
                .FirstOrDefault(c => c.associatedCrewmateID == crewmateID && c.listID == playerDeckId);
            if (cardMatch != null) {
                DatabaseManager.database.Delete(cardMatch); // Delete it!
            }
            var crewmate = DatabaseManager.GetOrCreateTable<CrewData>()
                .FirstOrDefault(c => c.id == crewmateID); 
            crewmate.status = 0; // Change crewmate's crew tag to NotInCrew(0)
        }

        /// <summary>
        ///     Increases a crew member's level by one and replaces their associated card with the newly leveled up version
        /// </summary>
        public void LevelUp(Crewmates crew)
        {
            const string playerDeckName = "Player Deck";
            int listIndex = crewList.FindIndex(c => c == crew); // Obtain the crewmate's index in crew list
            crew.CurrentXP = crew.CurrentXP - crew.XPNeeded; // Subtract current XP by the amount of XP needed for the level upgrade
            crew.XPNeeded *= 2; // Multiply the amount of XP needed for next level by 2
            crew.Level += 1; // Increase level attribute by 1
            // Fetch the ID of the SQL table contianing the player deck
            var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
                .FirstOrDefault(l => l.name == playerDeckName).id;
            // Find the matching card in the player's deck that shares the same index
            var cardMatch = DatabaseManager.GetOrCreateTable<Deck.DeckListCard>()
                .FirstOrDefault(c => c.associatedCrewmateID == listIndex && c.listID == playerDeckId);
            DatabaseManager.database.Delete(cardMatch); // Delete it!
            // Insert the leveled up version of the crew member's card into the database
            DatabaseManager.database.Insert(new Deck.DeckListCard
            {
                listID = playerDeckId,
                name = crew.CrewCard,
                level = crew.Level,
                associatedCrewmateID = listIndex
            });
            SaveCrew(); // Save the newly updated crew list to SQL
        }

        /// <summary>
        ///     Increases the XP of each crew member currently in the crew by the number of monsters killed
        ///     times monster level, divided by morale as a multiplier
        /// </summary>
        public static void XPGain()
        {
            if (!DatabaseManager.GetOrCreateTable<CrewData>().Any())
            {
                return;
            }

            var crewmates = DatabaseManager.GetOrCreateTable<CrewData>();
            // Iterate over the crew list
            foreach (var crewmate in crewmates)
            {
                // If the crew member is currently in the crew...
                if (crewmate.status == 2) {
                    crewmate.morale -= 10 * CardGameManager.monsterLevel; // ...decrease their morale based on how difficult the encounter was
                    if (crewmate.morale <= 0) // If their morale has dropped to or below 0....
                    {
                        KillCrewmate(crewmate.id); // ...kill them off (replace this with corresponding event trigger(s) later)
                        return;
                    }
                    float morale = crewmate.morale;
                    // Otherwise, increase XP based on encounter difficulty, monsters killed, and current morale
                    crewmate.currentXp = (int)((CardGameManager.numberOfMonstersKilled * CardGameManager.monsterLevel) /
                                                    (morale / 100));
                    if (crewmate.currentXp >= crewmate.xpNeeded) // If current XP is at or exceeds XP needed for next level...
                    {
                        const string playerDeckName = "Player Deck";
                        var playerDeckId = DatabaseManager.GetOrCreateTable<Deck.DeckList>()
                            .FirstOrDefault(l => l.name == playerDeckName).id;
                        var cardMatch = DatabaseManager.GetOrCreateTable<Deck.DeckListCard>()
                            .FirstOrDefault(c => c.associatedCrewmateID == crewmate.id && c.listID == playerDeckId);
                        if (cardMatch != null)
                        {
                            DatabaseManager.database.Delete(cardMatch); // Delete it!
                        }
                        // Insert the leveled up version of the crew member's card into the database
                        DatabaseManager.database.Insert(new Deck.DeckListCard
                        {
                            listID = playerDeckId,
                            name = crewmate.cardName,
                            level = crewmate.level,
                            associatedCrewmateID = crewmate.id
                        });
                        crewmate.level += 1; // ...level up!
                        crewmate.currentXp -= crewmate.xpNeeded; // Subtract the amount of XP needed for the next level from current XP
                        crewmate.xpNeeded *= 2; // Multiply XP needed for next level by two
                    }
                }
            }
        }
    }
}