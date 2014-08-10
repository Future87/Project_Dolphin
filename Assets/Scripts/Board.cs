﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {

	public List<Gem> gems = new List<Gem>();
	public int GridWidth;
	public int GridHeight;
	public GameObject gemPrefab;
	public Gem lastGem;
	public Vector3 gem1Start,gem1End,gem2Start,gem2End;
	public bool SwapBack = false;
	public bool isSwapping = false;
	public Gem gem1, gem2;
	public float startTime;
	public float SwapRate=2;
	public int AmountToMatch = 3;
	public bool isMatched = false;

	// Use this for initialization
	void Start () 
	{
		for (int y=0;y<GridHeight;y++)
		{
			for(int x=0;x<GridWidth;x++)
			{
				GameObject g = Instantiate (gemPrefab,new Vector3(x,y,0),Quaternion.identity)as GameObject;
				g.transform.parent = gameObject.transform;
				gems.Add(g.GetComponent<Gem>());
			}
		}

		gameObject.transform.position = new Vector3(-6f,-6f,0);


	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isMatched)
		{
			for(int i=0;i<gems.Count;i++)
			{
				if(gems[i].isMatched)
				{
					gems[i].CreateGem();
					gems[i].transform.position = new Vector3(gems[i].transform.position.x, gems[i].transform.position.y+6, gems[i].transform.position.z);
				}
			}
			isMatched = false;
			
		}
		if(isSwapping)
		{
			MoveGem(gem1,gem1End,gem1Start,false);
			MoveGem(gem2,gem2End,gem2Start,true);
			if(Vector3.Distance(gem1.transform.position,gem1End)<.1f || Vector3.Distance(gem2.transform.position,gem2End)<.1f)
			{
				gem1.transform.position = gem1End;
				gem2.transform.position = gem2End;
				lastGem = null;
				
				isSwapping = false;
				TogglePhysics(false);
				if(!SwapBack)
				{
					gem1.ToggleSelector();
					gem2.ToggleSelector();
					CheckMatch();
				}
				else
				{
					SwapBack = false;
				}
			}
		}
		else if(!DetermineBoardState())
		{
			for (int i =0; i < gems.Count; i++)
			{
				CheckForNearbyMatches(gems[i]);
			}
			
			if (!DoesBoardContainMathces())
			{
				isMatched = true;
				for (int i =0; i <gems.Count;i++)
				{
					gems[i].isMatched = true;
				}
			}
		}
	}
	
	public bool DetermineBoardState()
	{
		for (int i = 0; i < gems.Count;i++)
		{
			if(gems[i].transform.localPosition.y > 15.5f)
				return true;
			else if(gems[i].rigidbody.velocity.y >.1f)
				return true;
		}
		
		return false;
	}
	
	public void CheckMatch()
	{
		List<Gem> gem1List = new List<Gem>();
		List<Gem> gem2List = new List<Gem>();
		ConstructMatchList(gem1.color, gem1, gem1.XCoord, gem1.YCoord,ref gem1List);
		FixMatchList(gem1,gem1List);
		ConstructMatchList(gem2.color, gem2, gem2.XCoord, gem2.YCoord,ref gem2List);
		FixMatchList(gem2, gem2List);
		if(!isMatched)
		{
			SwapBack = true;
			ResetGems();
		}
	}
	
	public void ResetGems()
	{
		gem1Start = gem1.transform.position;
		gem1End = gem2.transform.position;
		
		gem2Start = gem2.transform.position;
		gem2End = gem1.transform.position;
		
		startTime = Time.time;
		TogglePhysics(true);
		
		isSwapping = true;
	}
	public void CheckForNearbyMatches(Gem g)
	{
		List<Gem> gemList = new List<Gem>();
		ConstructMatchList(g.color,g,g.XCoord,g.YCoord, ref gemList);
		FixMatchList(g, gemList);
	}
	
	
	public void ConstructMatchList(string color, Gem gem, int XCord, int YCord, ref List<Gem> MatchList)
	{
		if (gem == null)
		{
			return;
		}
		
		else if (gem.color != color)
		{
			return;
		}
		
		else if (MatchList.Contains(gem))
		{
			return;
		}
		
		else
		{
			MatchList.Add(gem);
			if(XCord == gem.XCoord || YCord == gem.YCoord)
			{
				foreach(Gem g in gem.Neighbors)
				{
					ConstructMatchList(color, g, XCord, YCord, ref MatchList);
				}
			}
		}
	}
	
	public bool DoesBoardContainMathces()
	{
		TogglePhysics(true);
		for (int i =0; i < gems.Count; i++)
		{
			for (int j = 0; j < gems.Count; j++)
			{
				if(gems[i].IsNeighborWith(gems[j]))
				{
					Gem g = gems[i];
					Gem f = gems[j];
					
					Vector3 GTemp = g.transform.position;
					Vector3 FTemp = f.transform.position;
					List<Gem> tempNeighbors = new List<Gem>(g.Neighbors);
					
					g.transform.position = FTemp;
					f.transform.position = GTemp;
					g.Neighbors = f.Neighbors;
					f.Neighbors = tempNeighbors;
					List<Gem> testListG = new List<Gem>();
					ConstructMatchList(g.color,g,g.XCoord,g.YCoord, ref testListG);
					if(TestMatchList(g,testListG))
					{
						g.transform.position = GTemp;
						f.transform.position = FTemp;
						f.Neighbors = g.Neighbors;
						g.Neighbors = tempNeighbors;
						TogglePhysics(false);
						return true;
					}
					List<Gem> testListF = new List<Gem>();
					ConstructMatchList(f.color,f,f.XCoord,f.YCoord, ref testListF);
					if (TestMatchList(f, testListF))
					{
						g.transform.position = GTemp;
						f.transform.position = FTemp;
						f.Neighbors = g.Neighbors;
						g.Neighbors = tempNeighbors;
						TogglePhysics(false);
						return true;
					}
					
					g.transform.position = GTemp;
					f.transform.position = FTemp;
					f.Neighbors = g.Neighbors;
					g.Neighbors = tempNeighbors;
					TogglePhysics(true);
				}
			}
		}
		return false;
	}
	
	public void FixMatchList(Gem gem, List<Gem> ListToFix)
	{
		List<Gem> rows = new List<Gem>();
		List<Gem> columns = new List<Gem>();
		
		for (int i=0; i < ListToFix.Count;i++)
		{
			if(gem.XCoord == ListToFix[i].XCoord)
			{
				rows.Add(ListToFix[i]);
			}
			
			if(gem.YCoord == ListToFix[i].YCoord)
			{
				columns.Add(ListToFix[i]);
			}
		}
		
		if(rows.Count >= AmountToMatch)
		{
			isMatched = true;
			for(int i=0; i<rows.Count;i++)
			{
				rows[i].isMatched = true;
			}
		}
		
		if(columns.Count >= AmountToMatch)
		{
			isMatched = true;
			for (int i=0; i<columns.Count;i++)
			{
				columns[i].isMatched = true;
			}
		}
	}
	
	public bool TestMatchList(Gem gem, List<Gem> ListToFix)
	{
		List<Gem> rows = new List<Gem>();
		List<Gem> columns = new List<Gem>();
		
		for (int i=0; i < ListToFix.Count;i++)
		{
			if(gem.XCoord == ListToFix[i].XCoord)
			{
				rows.Add(ListToFix[i]);
			}
			
			if(gem.YCoord == ListToFix[i].YCoord)
			{
				columns.Add(ListToFix[i]);
			}
		}
		
		if(rows.Count >= AmountToMatch)
		{
			return true;
		}
		
		if(columns.Count >= AmountToMatch)
		{
			return true;
		}
		
		return false;
	}
	
	// add in additional parameter over creating a new function, isNegGem should be passed as false when gem is not neg, and true if it is neg.
	public void MoveGem(Gem gemToMove,Vector3 toPos, Vector3 fromPos, bool isNegGem)
	{
		float negCoefficient = 0;
		
		if (isNegGem)
		{
			negCoefficient = -1.0f;
		}
		
		else if (!isNegGem)
		{
			negCoefficient = 1.0f;
		}
			
		Vector3 center = (fromPos + toPos) *.5f;
		center -= new Vector3(0,0,0.1f * negCoefficient);
		Vector3 riseRelCenter = fromPos - center;
		Vector3 setRelCenter = toPos - center;
		float fracComplete = (Time.time - startTime)/SwapRate;
		gemToMove.transform.position = Vector3.Slerp(riseRelCenter,setRelCenter,fracComplete);
		gemToMove.transform.position += center;
	}
	
	public void TogglePhysics(bool isOn)
	{
		for(int i=0; i <gems.Count;i++)
		{
			gems[i].rigidbody.isKinematic = isOn; //kinematics only work between forces
		}
	}
	public void SwapGems(Gem currentGem)
	{
		// no gem selected at all
		if (lastGem == null) 
		{
			lastGem = currentGem;
		}

		// 
		else if (lastGem == currentGem) 
		{
			lastGem = null;
		}

		// We've clicked a gem once, and we know it's not the gem we selected last time
		else 
		{
			if(lastGem.IsNeighborWith(currentGem))
			{
				gem1Start = lastGem.transform.position;
				gem1End = currentGem.transform.position;
				
				gem2Start = currentGem.transform.position;
				gem2End = lastGem.transform.position;
				
				startTime = Time.time;
				TogglePhysics(true);
				
				gem1 = lastGem;
				gem2 = currentGem;
				isSwapping = true;
			}
			else
			{
				lastGem.ToggleSelector();
				lastGem = currentGem;
			}
		}
	}
}
