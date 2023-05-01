using System.Collections;
using UnityEngine;
using Crew;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Crew {
    /// <summary>
    ///     Code for handling crewmate encounters
    /// </summary>
    /// <author> Misha Desear </author>
    public class CrewmateEncounter : MonoBehaviour
    {
        private Crewmates newCrew = null;

        /// <summary>
        ///     Adds a crewmate to the player's crew and their associated card to their deck
        /// </summary>
        public void AddCrewmate()
        {
            CrewManager.instance.AddToCrew(newCrew);
            NotificationHolder.instance.CreateNotification("New card obtained: " + newCrew.CrewCard + "!");
            StartCoroutine(ReturnToMap(3f));
        }

        /// <summary>
        ///     Returns to the encounter map after a specified number of seconds
        /// </summary>
        IEnumerator ReturnToMap(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            SceneManager.LoadScene("EncounterMapScene");
        }

        private void Start()
        {
            var crew = DatabaseManager.GetOrCreateTable<CrewManager.CrewData>(); // Obtain the crew data table
            if (crew is null) // If null, spawn a new crewmate
            {
                newCrew = CrewManager.instance.SpawnNewCrewmate();
            }
            else
            {
                var random = Random.Range(1, 5); // RNG for determining if an old crewmate will be encountered
                if (random == 5) // 20% chance to encounter an old crewmate
                {
                    newCrew = CrewManager.instance.SpawnOldCrewmate();
                }
                else // 80% chance to encounter a new crewmate
                {
                    newCrew = CrewManager.instance.SpawnNewCrewmate();
                }
            }
            newCrew.ShowInfo(); // Show the information of the spawned crewmate
        }
    }
}