﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gem : MonoBehaviour {

	public string[] gemSpriteName = {"RedGem", "BlueGem", "YellowGem", "GreenGem", "PurpleGem"};
	string color = "";
	public GameObject gemShade;
	public List<Gem> Neighbors = new List<Gem>();

	public int GridWidth;
	public int GridHeight;
	public GameObject gemPrefab;

	// Use this for initialization
	void Start () {
		CreateGem ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CreateGem()
	{
		color = gemSpriteName[Random.Range (0,gemSpriteName.Length)];
		Sprite gemColor = Resources.Load<Sprite> ("Sprites/" + color);
		gemShade.AddComponent<SpriteRenderer>();
		gemShade.GetComponent<SpriteRenderer>().sprite = gemColor;
	}

	void OnMouseDown()
	{
		print("Clicked");
	}

	public void AddNeighbor(Gem g)
	{
		Neighbors.Add(g);
	}

	public void RemoveNeighbor(Gem g)
	{
		Neighbors.Remove(g);
	}
}
