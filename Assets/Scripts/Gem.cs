using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gem : MonoBehaviour {

	public GameObject gemHolder;
	public GameObject cube;
	public string[] gemSpriteName = {"RedGem", "BlueGem", "YellowGem", "GreenGem", "PurpleGem"};
	public GameObject selector;
	public string color = "";
	public GameObject gemShade;
	public List<Gem> Neighbors = new List<Gem>();
	public bool isSelected =false;
	public bool isMatched =false;
	
	public int XCoord
	{
		get
		{
			return Mathf.RoundToInt(transform.localPosition.x);
		}
	}
	
	public int YCoord
	{
		get
		{
			return Mathf.RoundToInt(transform.localPosition.y);
		}
	}

	//public int GridWidth;
	//public int GridHeight;
	public GameObject gemPrefab;

	// Use this for initialization
	void Start () 
	{
		CreateGem ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	public void ToggleSelector()
	{
		isSelected = !isSelected;
		selector.SetActive (isSelected);
	}
	public void CreateGem()
	{
		Destroy (cube);
		color = gemSpriteName[Random.Range (0,gemSpriteName.Length)];
		GameObject gemPrefab = Resources.Load ("Prefabs/"+color) as GameObject;
		cube = (GameObject)Instantiate (gemPrefab,Vector3.zero,Quaternion.identity);
		cube.transform.parent= transform;
		cube.transform.localPosition = Vector3.zero;
		isMatched = false;
	}

	public void AddNeighbor(Gem g)
	{
		if(!Neighbors.Contains(g))
			Neighbors.Add(g);
	}

	public bool IsNeighborWith(Gem g)
	{
		if (Neighbors.Contains (g)) 
		{
			return true;
		}

		return false;
	}

	public void RemoveNeighbor(Gem g)
	{
		Neighbors.Remove(g);
	}

	void OnMouseDown()
	{
		if(!GameObject.Find("Board").GetComponent<Board>().isSwapping) //TODO: change selector behavior, we still want to be able to select gems while others are moving
		{
			ToggleSelector();
			GameObject.Find ("Board").GetComponent<Board> ().SwapGems(this);
		}
	}
}
