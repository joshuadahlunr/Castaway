using UnityEngine;

/// <summary>
/// </summary>
/// <author>Jared White</author>

public class ChangeCreditPage : MonoBehaviour {

    public GameObject secondCreditPage;

    public void NextPage() {
        secondCreditPage.SetActive(true);
    }
    
    public void BackPage() {
        secondCreditPage.SetActive(false);
    }
}