using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleManager : MonoBehaviour
{
    public GameObject crewPrefab;
    public void GenerateCrewmate()
    {
        GameObject crewmate;
        crewmate = Instantiate(crewPrefab, new Vector2(0, 0), Quaternion.identity);
        GlobalCrew.CREW.Add(crewmate.GetComponent<Crewmate>());
    }
}
