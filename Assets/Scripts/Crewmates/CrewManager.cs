using System;
using System.Collections.Generic;
using System.Linq;
using Crew;
using Crew.Globals;
using CardBattle.Containers;
using Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Crew.Globals {
    /// <summary>
    /// Singleton manager responsible for handling all crew members
    /// </summary>
    /// <author>Misha Desear</author>
    public class CrewManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of this class
        /// <summary>
        public static CrewManager instance;

        [SerializeField] private Sprite[] crewSprites;
        [SerializeField] private string[] crewNames;

        public CrewDatabase crewDatabase;
        public CardDatabase cardDatabase;
        public Deck playerDeck;

        public CrewList crewList; 

        public GameObject crewPrefab;
        public GameObject infoPrefab;

        /// <summary>
        /// When the game starts:
        /// </summary>
        public void Awake() 
        {
            // Set up singleton
            instance = this;

            const string crewListName = "Crew List";

            // If the crew list hasn't been defined yet, create an empty crew list
            if (!DatabaseManager.GetOrCreateTable<CrewList.GlobalCrewList>().Any())
            {
                DatabaseManager.database.Insert(new CrewList.GlobalCrewList()
                {
                    name = crewListName,
                });
            }

            crewList.DatabaseLoad(crewListName);
        }

        /*/// <summary>
        /// Generates a new crewmate with starter level and stats, 
        /// as well as random type and corresponding card
        /// </summary>
        public Crewmates GenerateNewCrewmate() 
        {
            // TODO: correspond associated card with the crewmate's type
            newCrew.global = crewList;
            newCrew.CrewTag = 0;
            newCrew.Type = (Crewmates.CrewClass.Type)Random.Range(0,7); // Select a random enum for type
            newCrew.Name = crewNames[Random.Range(0, crewNames.Length)];
            newCrew.Level = 1;
            newCrew.Morale = 50;
            newCrew.CurrentXP = 0;
            newCrew.XPNeeded = 10;
            newCrew.crewSprite = crewSprites[Random.Range(0, crewSprites.Length)];
            newCrew.crewCard = cardDatabase.Instantiate(cardDatabase.cards.Keys.Shuffle().First()); // Currently, this just picks a random card from the DB
            return newCrew; // Return a reference to the new crewmate
        }*/

        public void SpawnNewCrewmate()
        {
            GameObject crewmate = Instantiate(crewPrefab, new Vector2(Random.Range(-29f, 20f), 115f), Quaternion.identity);

            // Load data from generated crewmate into spawned prefab
            crewmate.GetComponent<Crewmates>().global = crewList;
            crewmate.GetComponent<Crewmates>().CrewTag = 0;
            crewmate.GetComponent<Crewmates>().Type = (Crewmates.CrewClass.Type)Random.Range(0,7);
            crewmate.GetComponent<Crewmates>().Name = crewNames[Random.Range(0, crewNames.Length)];
            crewmate.GetComponent<Crewmates>().Level = 1;
            crewmate.GetComponent<Crewmates>().Morale = 50;
            crewmate.GetComponent<Crewmates>().CurrentXP = 0;
            crewmate.GetComponent<Crewmates>().XPNeeded = 10;
            crewmate.GetComponent<Crewmates>().crewSprite = crewSprites[Random.Range(0, crewSprites.Length)];
            crewmate.GetComponent<Crewmates>().crewCard = cardDatabase.Instantiate(cardDatabase.cards.Keys.Shuffle().First());

            crewmate.GetComponent<SpriteRenderer>().sprite = crewmate.GetComponent<Crewmates>().crewSprite;

            crewList.AddCrewMember(crewmate.GetComponent<Crewmates>());
        }

        public void AddToCrew(Crewmates crew)
        {
            crew.CrewTag = (Crewmates.Status.CrewTag)2; // Change crewmate's crew tag to InCrew (2)
            playerDeck.AddCard(Instantiate(crew.crewCard)); // Add crewmate's associated card to the deck
            
        }

        public void RemoveFromCrew(Crewmates crew)
        {
            crew.CrewTag = (Crewmates.Status.CrewTag)1; // Change crewmate's crew tag to WasInCrew(1)
            playerDeck.RemoveCard(crew.crewCard.name); // Remove the crewmate's associated card from the deck
        }
    }
}