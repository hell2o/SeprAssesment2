using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base abstract class for Enemy and Player to inherit from.
/// Defines all shared variables, their constructor and their setters and getters
/// </summary>
[System.Serializable]
public abstract class Character {

	[SerializeField]
	protected string name;
	[SerializeField]
	protected int level;
	[SerializeField]
	protected int health;
	[SerializeField]
	protected int attack;
	[SerializeField]
	protected int defence;
	[SerializeField]
	protected int maximumMagic;
	[SerializeField]
	protected int magic;
	[SerializeField]
	protected int luck;
	[SerializeField]
	protected int speed;
	[SerializeField]
	protected SpecialMove special1;
	[SerializeField]
	protected SpecialMove special2;
	[System.NonSerialized]
	protected Texture2D image;

	uint[] colorData;
	int imageWidth;
	int imageHeight;

	protected Character (string name, int level, int health, int attack, int defence, int maximumMagic,
		int magic, int luck, int speed, SpecialMove special1, SpecialMove special2, Texture2D image = null)
	{
		this.name = name;
		this.level = level;
		this.health = health;
		this.attack = attack;
		this.defence = defence;
		this.maximumMagic = maximumMagic;
		this.magic = magic;
		this.luck = luck;
		this.speed = speed;
		this.special1 = special1;
		this.special2 = special2;
		this.image = image;
	}		

	public string Name {
		get {
			return this.name;
		}
		set {
			name = value;
		}
	}

	public int Level {
		get {
			return this.level;
		}
		set {
			level = value;
		}
	}

	public int Health {
		get {
			return this.health;
		}
		set {
			health = value;
		}
	}

	public int Attack {
		get {
			return this.attack;
		}
		set {
			attack = value;
		}
	}

	public int Defence {
		get {
			return this.defence;
		}
		set {
			defence = value;
		}
	}

	public int MaximumMagic {
		get {
			return this.maximumMagic;
		}
		set {
			maximumMagic = value;
		}
	}

	public int Magic {
		get {
			return this.magic;
		}
		set {
			magic = value;
		}
	}

	public int Luck {
		get {
			return this.luck;
		}
		set {
			luck = value;
		}
	}

	public int Speed {
		get {
			return this.speed;
		}
		set {
			speed = value;
		}
	}

	public SpecialMove Special1 {
		get {
			return this.special1;
		}
		set {
			special1 = value;
		}
	}

	public SpecialMove Special2 {
		get {
			return this.special2;
		}
		set {
			special2 = value;
		}
	}

	public Texture2D Image {
		get {
			return this.image;
		}
	}

	/// <summary>
	/// Prepares hard to encode data for saving.
	/// </summary>
	public void PrepareSave (){
		if (image != null) {
			Color32[] colors = image.GetPixels32 ();
			colorData = new uint[colors.Length];
			for (int i = 0; i < colors.Length; i++) {
				colorData [i] = (uint) colors [i].r;
				colorData [i] += ((uint)colors [i].g) << 8;
				colorData [i] += ((uint)colors [i].b) << 16;
				colorData [i] += ((uint)colors [i].a) << 24;
			}
			imageWidth = image.width;
			imageHeight = image.height;
		}
	}
	/// <summary>
	/// Recovers data from saving format.
	/// </summary>
	public void RecoverSave(){
		image = new Texture2D (imageWidth, imageHeight);
		Color32[] colors = new Color32[colorData.Length];
		for (int i = 0; i < colorData.Length; i++){
			Color32 c = new Color32 ();
			c.r = (byte) (colorData [i] % 256);
			c.g = (byte)((colorData [i] >> 8) % 256);
			c.b = (byte)((colorData [i] >> 16) % 256);
			c.a = (byte)((colorData [i] >> 24) % 256) ;
			colors [i] = c;
		}
		image.SetPixels32 (colors);
		image.filterMode = FilterMode.Point;
		image.Apply (true);
	}
}
