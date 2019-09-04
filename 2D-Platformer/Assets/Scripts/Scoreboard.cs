using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
	private LevelManager lm;
	private TextMeshProUGUI tmp;

    // Start is called before the first frame update
    void Start()
    {
		lm = FindObjectOfType<LevelManager>();
		tmp = GetComponent<TextMeshProUGUI>();
		UpdateScores();

	}

    // Update is called once per frame
    void Update() {
		UpdateScores();
	}

	private void UpdateScores() {
		//Get scores
		Vector2 scores = lm.GetScores();

		//Build text and update component
		string scoreString = scores.x.ToString() + " : " + scores.y.ToString();
		tmp.SetText(scoreString);
	}
}
