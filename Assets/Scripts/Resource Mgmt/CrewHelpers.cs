using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public class CrewHelpers : MonoBehaviour
{
    private static string[] Names = { }; // insert random names 
    private List<Preference> preferences = new List<Preference>();
    private int idNum = 0;

    public void SetId(Crewmate crewmate)
    {
        crewmate.ID = idNum;
        idNum++;
    }
    public void SetName(Crewmate crewmate)
    {
        int randInt = Random.Range(0, 1000);
        crewmate.name = Names[randInt];
    }

    public void SetLikes(Crewmate crewmate)
    {

    }

    public void SetDislikes(Crewmate crewmate)
    {

    }
}
