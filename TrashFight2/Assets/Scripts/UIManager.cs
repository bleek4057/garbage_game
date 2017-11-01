using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public Text p0healthText;
    public Text p1HealthText;

    public Player p0, p1;

	void Start () {
		
	}
	
	void Update () {
        UpdatePlayerHealthUI();
    }
    private void UpdatePlayerHealthUI() {
        p0healthText.text = p0.health.ToString();
        p1HealthText.text = p1.health.ToString();
    }
}
