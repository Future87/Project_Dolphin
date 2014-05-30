using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gem : MonoBehaviour {


	public List<Gem> Neighbors = new List<Gem>();

	public int GridWidth;
	public int GridHeight;
	public GameObject gemPrefab;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
