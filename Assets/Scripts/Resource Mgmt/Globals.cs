using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @author: Misha Desear
/// </summary>
public class Globals
{
    // Global resource value
    public static ShipResource SHIP_RESOURCE = new ShipResource(0);

    // Global upgrade info array
    public static UpgradeData[] UPGRADE_DATA = new UpgradeData[]
    {
        new UpgradeData("Cabin", 10, 0, 0),
        new UpgradeData("Steerage", 10, 0, 0),
        new UpgradeData("Gun Room", 10, 0, 0),
        new UpgradeData("Cargo Hold", 10, 0, 0),
        new UpgradeData("Gun Deck", 10, 0, 0),
        new UpgradeData("Forecastle", 10, 0, 0)
    };

    public static Crewmate[] CREW = new Crewmate[]
    {
        // TODO: Populate this with random crew members (besides Jerry)
    };
}
