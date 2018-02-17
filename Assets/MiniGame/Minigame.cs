using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// new for assessment 3
/// handles Minigame interactions.
/// </summary>
public class Minigame : MonoBehaviour {
	public GameObject bunnyButtonTemplate;
	public GameObject gooseButtonTemplate;

	List<GameObject> bunnyButtons;
	List<GameObject> gooseButtons;
	List<float> bunnyButtonsTimeActive;
	List<float> gooseButtonsTimeActive;
	Text scoreText;
	Text livesText;
	GameObject startPanel;
	GameObject endPanel;

	PlayerMovement move;

	bool inGame = false;

	float gameTime = 0f;
	float bunnyTime = 0f; // time untill the next bunny appears (in seconds)
	float gooseTime = 0f; // time untill the next goose appears (in seconds)

	const float maxTimeBetweenPopups = 1f;
	const float maxActiveTime = 2f;
	float activeTime = 2f;// how long the popups can currently be active for (in seconds)
	const float minActiveTime = 0.4f;

	public int score = 0;
	public int lives = 3;

	// Use this for initialization
	void Start () {
		move = GameObject.FindObjectOfType<PlayerMovement> ();
		move.setCanMove (false);
		bunnyButtons = new List<GameObject> ();
		gooseButtons = new List<GameObject> ();
		bunnyButtonsTimeActive = new List<float> ();
		gooseButtonsTimeActive = new List<float> ();

		scoreText = GameObject.Find ("ScoreText").GetComponent<Text> ();
		livesText = GameObject.Find ("LivesText").GetComponent<Text> ();

		startPanel = GameObject.Find ("StartPanel");
		endPanel = GameObject.Find ("EndPanel");

		int pos = 0;
		for (int x = -2; x <= 2; x += 2) {
			for (int y = -2; y <= 2; y += 2) {
				int temp = pos;
				GameObject newButton;
				newButton = Instantiate (bunnyButtonTemplate, this.transform);
				newButton.transform.position += new Vector3((float) x, (float) y);
				newButton.SetActive (false);
				newButton.GetComponent<Button> ().onClick.AddListener (delegate {
					hitBunny(temp);	
				});
				bunnyButtons.Add(newButton);
				bunnyButtonsTimeActive.Add (0f);



				newButton = Instantiate (gooseButtonTemplate, this.transform);
				newButton.transform.position += new Vector3((float) x, (float) y);
				newButton.SetActive (false);
				newButton.GetComponent<Button> ().onClick.AddListener (delegate {
					hitGoose(temp);	
				});
				gooseButtons.Add(newButton);
				gooseButtonsTimeActive.Add (0f);
				pos++;
			}
		}
		bunnyButtonTemplate.SetActive(false);
		gooseButtonTemplate.SetActive(false);

		endPanel.SetActive (false);
		updateText ();
	}
	
	// Update is called once per frame
	void Update () {
		if (inGame) {
			gameTime += Time.deltaTime;

			float tempActiveTime =  (maxActiveTime / Mathf.Pow(gameTime, 0.5f)); // function that defines how long buttons are visible for
			activeTime = Mathf.Clamp (tempActiveTime, minActiveTime, maxActiveTime); //clamp the above into a reasonable range
			for (int i = 0; i < bunnyButtons.Count; i++) {
				if (bunnyButtons [i].activeInHierarchy) {
					bunnyButtonsTimeActive[i] += Time.deltaTime;

					if (bunnyButtonsTimeActive [i] >= activeTime) {
						bunnyButtons [i].SetActive (false);
						bunnyButtonsTimeActive [i] = 0f;
					}
				}
			}

			for (int i = 0; i < gooseButtons.Count; i++) {
				if (gooseButtons [i].activeInHierarchy) {
					gooseButtonsTimeActive[i] += Time.deltaTime;

					if (gooseButtonsTimeActive [i] >= activeTime) {
						gooseButtons [i].SetActive (false);
						gooseButtonsTimeActive [i] = 0f;
					}
				}
			} 

			if (bunnyTime <= gameTime) {
				bunnyTime = gameTime + Random.Range (0.5f, maxTimeBetweenPopups);
				int pos = Random.Range (0, bunnyButtons.Count);
				bool inactiveButtonExists = false;
				for (int i = 0; i < bunnyButtons.Count; i++) {
					if (!bunnyButtons [i].activeInHierarchy && !gooseButtons[i].activeInHierarchy) {
						inactiveButtonExists = true;
					}
				}
				if (inactiveButtonExists) {
					while (bunnyButtons [pos].activeInHierarchy || gooseButtons [pos].activeInHierarchy) {
						pos = Random.Range (0, bunnyButtons.Count);
					}
					bunnyButtons [pos].SetActive (true);
					bunnyButtonsTimeActive [pos] = 0f;
				}
			}
		
			if (gooseTime <= gameTime) {
				gooseTime = gameTime + Random.Range (0.5f, maxTimeBetweenPopups);
				int pos = Random.Range (0, gooseButtons.Count);
				bool inactiveButtonExists = false;
				for (int i = 0; i < bunnyButtons.Count; i++) {
					if (!bunnyButtons [i].activeInHierarchy && !gooseButtons[i].activeInHierarchy) {
						inactiveButtonExists = true;
					}
				}
				if (inactiveButtonExists) {
					while (bunnyButtons [pos].activeInHierarchy || gooseButtons [pos].activeInHierarchy) {
						pos = Random.Range (0, gooseButtons.Count);
					}
					gooseButtons [pos].SetActive (true);
					gooseButtonsTimeActive [pos] = 0f;
				}
			}


		}
		
	}

	public void startGame(){
		startPanel.SetActive (false);
		inGame = true;
		gameTime = 0f;
		bunnyTime = gameTime + Random.Range (0.5f, maxTimeBetweenPopups);
		gooseTime = gameTime + Random.Range (0.5f, maxTimeBetweenPopups);
		
	}

	void hitGoose(int pos){
		gooseButtons [pos].SetActive (false);
		score += 1;
		updateText ();
	}

	void hitBunny(int pos){
		bunnyButtons [pos].SetActive (false);
		if (lives > 0) {
			lives -= 1;
			updateText ();
		} else {
			endGame ();
		}
	}

	void updateText(){
		scoreText.text = "Score: " + score.ToString ();
		livesText.text = lives.ToString () + " Lives Left";
	}
		
	void endGame(){
		endPanel.SetActive (true);
		Debug.Log ("GAME OVER");
		inGame = false;
		foreach (GameObject g in bunnyButtons) {
			g.SetActive (false);
		}
		foreach (GameObject g in gooseButtons) {
			g.SetActive (false);
		}
	}

	public void quitGame(){
		SceneManager.UnloadSceneAsync (this.gameObject.scene);
		move.setCanMove (true);
		PlayerData.instance.data.Money += score;
		SceneChanger sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger> ();
		SoundManager.instance.playSFX ("transition");
		sceneChanger.loadLevel ("WorldMap", new Vector2(-6,4));
	}
}
