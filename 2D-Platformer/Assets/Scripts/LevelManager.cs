﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	[Header("Player Settings")]
	[SerializeField] int numPlayers = 2;
	[SerializeField] GameObject playerObject;
	[SerializeField] Sprite player1Sprite;
	[SerializeField] GameObject spawnParticles;

	[Header("Misc Settings")]
	[SerializeField] Vector2 scores;
	[SerializeField] GameObject[] spawnPoints;
	[SerializeField] [Range(0, 3)] float resetTime = 1;

	private bool isRestartLocked = false;
	private List<int> usedSpawnIndexes;

    // Start is called before the first frame update
    void Start()
    {
		scores = new Vector2(0, 0);
		spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
		usedSpawnIndexes = new List<int>();
		usedSpawnIndexes.Clear();
		StartGame();
    }


	private void StartGame() {
		//Spawn player
		Transform spawnPoint = FindSpawnPoint();
		GameObject player = Instantiate(playerObject, spawnPoint.position, spawnPoint.rotation);
		GameObject particles = Instantiate(spawnParticles, spawnPoint.position, spawnPoint.rotation);
		Destroy(particles, 2f);

		//Spawn player1
		spawnPoint = FindSpawnPoint();
		GameObject player1 = Instantiate(playerObject, spawnPoint.position, spawnPoint.rotation);
		particles = Instantiate(spawnParticles, spawnPoint.position, spawnPoint.rotation);
		Destroy(particles, 2f);
		player1.GetComponent<PlayerController>().SetPlayerNumber("1");
		player1.GetComponent<SpriteRenderer>().sprite = player1Sprite;

		isRestartLocked = false;
	}

	private Transform FindSpawnPoint() {
		//Get number of spawn points
		int numSpawns = spawnPoints.Length;

		//Make sure there are enough spawn points avaliable
		if((numSpawns + usedSpawnIndexes.Count) < numPlayers) {
			throw new System.ArgumentException("More spawnpoints required");
		}

		//Get spawn index to use
		int spawnIndex = Random.Range(0, numSpawns);
		//Spawn index already taken, retry
		if (usedSpawnIndexes.Contains(spawnIndex)) {
			return FindSpawnPoint();
		}

		//Spawn index not taken, return transform and add index to used list
		else {
			usedSpawnIndexes.Add(spawnIndex);
			return spawnPoints[spawnIndex].transform;
		}
	}

	public Vector2 GetScores() {
		return scores;
	}

	public void PlayerKilled(string deadPlayerNumber) {
		//Check if there is at least one player left
		PlayerController[] players = FindObjectsOfType<PlayerController>();
		if(players.Length - 1 <= 1 && isRestartLocked == false) {
			//A victory or draw has happened, reset game
			isRestartLocked = true;
			StartCoroutine(ResetGame(deadPlayerNumber));
		}
	}

	private IEnumerator ResetGame(string deadPlayerNumber) {
		//Wait a bit
		yield return new WaitForSeconds(1f);

		//if there was a draw do nothing
		PlayerController[] players = FindObjectsOfType<PlayerController>();
		if (players.Length == 0);
		//OR score to the WINNING player
		else if (deadPlayerNumber == "") { scores.y++; }
		else if (deadPlayerNumber == "1") { scores.x++; }

		//Find each player and destroy them
		foreach (PlayerController player in players) {
			Destroy(player.gameObject);
		}

		//Find any bullets remaining in the game and destroy them
		Bullet[] bullets = FindObjectsOfType<Bullet>();
		foreach (Bullet bullet in bullets) {
			Destroy(bullet.gameObject);
		}

		//Reset spawn points list
		usedSpawnIndexes.Clear();

		StartGame();
	}
}
