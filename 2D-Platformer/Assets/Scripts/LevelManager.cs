﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	[SerializeField] GameObject playerObject;
	[SerializeField] int numPlayers = 2;
	[SerializeField] Transform[] spawnPoints;
	[SerializeField] [Range(0, 3)] float resetTime = 1;
	private List<int> usedSpawnIndexes;

    // Start is called before the first frame update
    void Start()
    {
		usedSpawnIndexes = new List<int>();
		usedSpawnIndexes.Clear();
		StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void StartGame() {
		//Spawn player
		Transform spawnPoint = FindSpawnPoint();
		GameObject player = Instantiate(playerObject, spawnPoint.position, spawnPoint.rotation);

		//Spawn player1
		spawnPoint = FindSpawnPoint();
		GameObject player1 = Instantiate(playerObject, spawnPoint.position, spawnPoint.rotation);
		player1.GetComponent<PlayerController>().SetPlayerNumber("1");
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
			return spawnPoints[spawnIndex];
		}
	}

	public void ResetGame() {
		//Wait time before reset
		yield return new WaitForSeconds(resetTime);

		//Find each player and destroy them
		PlayerController[] players = FindObjectsOfType<PlayerController>();
		foreach (PlayerController player in players) {
			Destroy(player.gameObject);
		}


		//Reset spawn points list
		usedSpawnIndexes.Clear();

		StartGame();
	}
}
