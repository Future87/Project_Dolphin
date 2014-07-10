using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {

	public List<GameObject> gems = new List<GameObject>();
	public int GridWidth;
	public int GridHeight;
	public GameObject gemPrefab;
	public Gem lastGem;
	public Vector3 gem1Start,gem1End,gem2Start,gem2End;
	public bool isSwapping = false;
	public Gem gem1, gem2;
	public float startTime;
	public float SwapRate=2;

	// Use this for initialization
	void Start () 
	{
		for (int y=0;y<GridHeight;y++)
		{
			for(int x=0;x<GridWidth;x++)
			{
				GameObject g = Instantiate (gemPrefab,new Vector3(x,y,0),Quaternion.identity)as GameObject;
				g.transform.parent = gameObject.transform;
				gems.Add (g);
			}
		}

		gameObject.transform.position = new Vector3(-6f,-6f,0);

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isSwapping)
		{
			MoveGem(gem1,gem1End,gem1Start,false);
			MoveGem(gem2,gem2End,gem2Start,true);
			if(Vector3.Distance(gem1.transform.position,gem1End)<.1f || Vector3.Distance(gem2.transform.position,gem2End)<.1f)
			{
				gem1.transform.position = gem1End;
				gem2.transform.position = gem2End;
				gem1.ToggleSelector();
				gem2.ToggleSelector();
				lastGem = null;
				
				isSwapping = false;
				TogglePhysics(false);
			}
		}
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
			gems[i].rigidbody2D.isKinematic = isOn; //kinematics only work between forces
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
