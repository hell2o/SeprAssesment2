using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class ShopTests {
	GameObject player;

	public IEnumerator Setup() {
		SceneManager.LoadScene ("WorldMap");
		yield return null;

		//Change current level then reload
		GlobalFunctions.instance.currentLevel = 1;
		SceneManager.LoadScene ("WorldMap");
		yield return null;
		player = GameObject.Find ("Player");
		player.transform.position = new Vector3 (0,-1);

	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator S1ShopOpens() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return Setup();

		GlobalFunctions.instance.ShopMenu ();
		yield return null;
		GameObject g = null;

		try{
			g = GameObject.Find("ShopMenu");
		}catch (NullReferenceException){
		}
		Assert.NotNull (g);
	}
	[UnityTest]
	public IEnumerator S2ShopBuyWorks(){
		yield return Setup ();
		//setup preconditions
		PlayerData.instance.data.Money = 100;
		PlayerData.instance.data.Items [0] = null;
		//open the shop
		GlobalFunctions.instance.ShopMenu ();
		yield return null;

		GameObject g = null;

		try{
			g = GameObject.Find("HammerButton");
		}catch (NullReferenceException){
		}
		Assert.NotNull (g);

		g.GetComponent<UnityEngine.UI.Button> ().onClick.Invoke ();
		yield return null;

		Assert.AreEqual (30, PlayerData.instance.data.Money);

		Assert.IsInstanceOf<Hammer> (PlayerData.instance.data.Items [0]);
	}

	[UnityTest]
	public IEnumerator S3ShopClose(){
		yield return Setup ();
		//open the shop
		GlobalFunctions.instance.ShopMenu ();
		yield return null;

		GameObject g = null;

		try{
			g = GameObject.Find("ExitButton");
		}catch (NullReferenceException){
		}
		Assert.NotNull (g);
		g.GetComponent<UnityEngine.UI.Button> ().onClick.Invoke ();
		yield return null;

		g = null;

		try{
			g = GameObject.Find("ShopMenu");
		}catch (NullReferenceException){
		}
		Assert.Null (g);
	}
}
