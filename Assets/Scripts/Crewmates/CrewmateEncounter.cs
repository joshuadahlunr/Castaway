using System.Collections;
using UnityEngine;
using Crew;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Crew {
    public class CrewmateEncounter : MonoBehaviour
    {
        private Crewmates newCrew = null;
        public void ReturnToMap()
        {
            SceneManager.LoadScene("EncounterMapScene");
        }

        public void AddCrewmate()
        {
            CrewManager.instance.AddToCrew(newCrew);
            NotificationHolder.instance.CreateNotification("New card obtained: " + newCrew.CrewCard + "!");
            StartCoroutine(ReturnToMap(3f));
        }

        IEnumerator ReturnToMap(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            SceneManager.LoadScene("EncounterMapScene");
        }

        private void Start()
        {
            newCrew = CrewManager.instance.SpawnNewCrewmate();
            newCrew.ShowInfo();
        }
    }
}