using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crew;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CrewmateEncounter : MonoBehaviour
{
    public GameObject crewPrefab;
    private GameObject newCrew;

    // TODO: once crewmate DB has been implemented, allow crewmates from previous runs to be encountered
    public GameObject GenerateCrewmate()
    {
        GameObject crewmate;
        crewmate = Instantiate(crewPrefab, new Vector2(Random.Range(-29f, 20f), 200f), Quaternion.identity);
        crewmate.SetActive(true);
        crewmate.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        crewmate.GetComponent<Crewmate>().infoPrefab = GameObject.FindGameObjectWithTag("Info Panel");
        return crewmate;
    }

    public void AddToCrew()
    {
        if (newCrew != null)
        {
            GlobalCrew.CREW.Add(newCrew.GetComponent<Crewmate>());
        } else
        {
            Debug.Log("No crew member found!");
        }
        SceneManager.LoadScene("EncounterMapScene");
    }

    public void ReturnToMap()
    {
        SceneManager.LoadScene("EncounterMapScene");
    }

    private void Awake()
    {
        newCrew = GenerateCrewmate();
    }

    private void Start()
    {
        newCrew.GetComponent<Crewmate>().ShowInfo();
    }
}
