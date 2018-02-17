using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// New for assessment 3
/// Shop menu script, handles purchases and drag and drop.
/// </summary>
public class ShopMenuScript : MonoBehaviour {
	GameObject player;
	PlayerMovement move;
	/// <summary> Refer to all the containers from the item inventory </summary>
	DragAndDropCell[] itemContainers;
	DataManager data;
	Item[] items;

	/// <summary>
	/// initializes the shop menu.
	/// </summary>
	void Start () {
		player = GameObject.Find ("Player");
		move = player.GetComponent<PlayerMovement> ();
		move.setCanMove (false);
		itemContainers = new DragAndDropCell[6];
		data = PlayerData.instance.data;
		items = data.Items;
		//Find all cells
		for (int i = 0; i < 6; i++) {
			//Find and store the item containers
			itemContainers [i] = GameObject.Find ("Item" + i).GetComponent<DragAndDropCell>();
			//If there is an item in the item inventory
			if (items[i] != null) {
				//Load an item object in this position to drag and drop
				createItemCell(itemContainers[i], data.Items[i]);
			}

		}
		PrepareButtons ();
	}

	/// <summary>
	/// Creates an item cell, showing the name and description
	/// </summary>
	/// <param name="cell">Cell.</param>
	/// <param name="itemObject">Item object.</param>
	private void createItemCell(DragAndDropCell cell, Item itemObject) {
		GameObject item = Instantiate (Resources.Load ("Item", typeof(GameObject))) as GameObject;
		updateItemCell (item, cell, itemObject);
	}

	private void updateItemCell(GameObject item, DragAndDropCell cell, Item itemObject) {
		item.name = "Item";
		item.transform.Find ("Text").GetComponent<Text> ().text = itemObject.Name + " - " + itemObject.Desc;
		cell.PlaceItem (item);
	}

	/// <summary>
	/// On the event an item is placed, swap the values in the appropiate arrays
	/// </summary>
	/// <param name="desc">The description of the event, containing source and destination cells as well
	/// as item details</param>
	public void OnItemPlace(DragAndDropCell.DropDescriptor desc) {
		ContainerData source = desc.sourceCell.gameObject.GetComponent<ContainerData> ();
		ContainerData dest = desc.destinationCell.gameObject.GetComponent<ContainerData> ();
		Player[] players = data.Players;
		Item temp;
		temp = items [source.Index];
		items [source.Index] = items [dest.Index];
		items [dest.Index] = temp;
		Debug.Log ("Source: " + source.Type);
		Debug.Log ("Destination: " + dest.Type);
	}

	/// <summary>
	/// Prepares the shop buttons.
	/// </summary>
	void PrepareButtons (){
		Text MoneyText = GameObject.Find ("MoneyText").GetComponent<Text> ();
		MoneyText.text = "Money: " + data.Money.ToString () + " Units";

		Button button;

		button = GameObject.Find ("HammerButton").GetComponent<Button>();
		if (data.Money >= 70 && data.countItems () < data.Items.Length) {
			button.onClick.RemoveAllListeners ();
			button.onClick.AddListener (delegate {
				data.addItem (new Hammer ());
				data.Money -= 70;
				PrepareButtons();
				Debug.Log("ping");
			});
		} else {
			button.enabled = false;
		}

		for (int i = 0; i < 6; i++) {
			//If there is an item in the inventory slot
			if (items[i] != null) {
				createItemCell(itemContainers[i], data.Items[i]);
			}

		}

	
	}
	/// <summary>
	/// Exits the shop.
	/// </summary>
	public void ExitShop(){
		Destroy (this.gameObject);
		move.setCanMove (true);
		SceneManager.UnloadSceneAsync (this.gameObject.scene);
	}
}
