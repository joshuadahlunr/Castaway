using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Crew {
    public class MoraleManager : MonoBehaviour
    {
        public GameObject crewPrefab;
        public void GenerateCrewmate()
        {
            GameObject crewmate;
            crewmate = Instantiate(crewPrefab, new Vector2(Random.Range(-29f, 20f), 200f), Quaternion.identity);
            crewmate.SetActive(true);
            crewmate.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            crewmate.GetComponent<Crewmate>().infoPrefab = GameObject.FindGameObjectWithTag("Info Panel");
            GlobalCrew.CREW.Add(crewmate.GetComponent<Crewmate>());
        }
    }
}