using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class MinigameTest {
	bool sceneLoaded = false;
	GameObject player;
	PlayerMovement playerScript;

	public IEnumerator Setup() {
		
		//because other tests may have made a mess
		foreach (GameObject g in Resources.FindObjectsOfTypeAll<GameObject>()) {
			if (g.name == "Player") {
				g.SetActive (true);
			}
		}
		yield return null;
		SceneManager.LoadScene ("ExplorationTestScene", LoadSceneMode.Single);
		yield return null; //Wait for scene to load
		PlayerData.instance.data = new DataManager(null);
		PlayerData.instance.data.Players [0] = new Player ("George", 1, 100, 30, 5, 5, 5, 5, 5, 0, null,
			new MagicAttack ("hi-jump kicked", "Kick with power 15", 3, 15),
			new RaiseDefence ("buffed up against", "Increase your defence by 10%", 2, 0.1f),
			(Texture2D)Resources.Load ("Character1", typeof(Texture2D)));
		player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerMovement> ();
		sceneLoaded = true;
		player.transform.position = new Vector2 (0, 0);
		yield return null;
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator MG1MinigameScoreIncreases() {
		yield return Setup ();
		SceneChanger sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger> ();
		sceneChanger.loadLevel ("MiniGame", new Vector2 (10000, 10000));
		float time = 0f;
		while (time < 1f) {
			yield return null;
			time += Time.deltaTime;
		}

		GameObject.Find ("StartButton").GetComponent<UnityEngine.UI.Button> ().onClick.Invoke ();

		GameObject button = null;

		while (button == null) {
			yield return null;
			try{
				button = GameObject.Find("GooseButton(Clone)");
			}catch (NullReferenceException) {
			}
		}
		Minigame m = GameObject.Find ("MiniGameCanvas").GetComponent<Minigame> ();
		Assert.Zero (m.score);
		button.GetComponent<UnityEngine.UI.Button> ().onClick.Invoke ();
		Assert.AreEqual (1, m.score);


	}

	[UnityTest]
	public IEnumerator MG2MinigameLosing(){
		for (int i = 0; i < 4; i++) {
			GameObject button = null;

			while (button == null) {
				yield return null;
				try{
					button = GameObject.Find("BunnyButton(Clone)");
				}catch (NullReferenceException) {
				}
			}

			button.GetComponent<UnityEngine.UI.Button> ().onClick.Invoke ();
			yield return null;
		}


		GameObject EndButton = null;
		EndButton = GameObject.Find ("EndButton");
		Assert.NotNull (EndButton);
		EndButton.GetComponent<UnityEngine.UI.Button> ().onClick.Invoke ();
		float time = 0f;
		while (time < 1f) {
			yield return null;
			time += Time.deltaTime;
		}

		Assert.AreEqual (1, PlayerData.instance.data.Money);

	}
}
