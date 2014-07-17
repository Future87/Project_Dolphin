using UnityEngine;
using System.Collections;

public class Feeler : MonoBehaviour {

	public Gem owner;

	void OnTriggerEnter2D(Collider2D c)
	{
		if (c.tag == "Gem") 
		{
			owner.AddNeighbor(c.GetComponent<Gem>());
		}
	}

	void OnTriggerExit2D(Collider2D c)
	{
		if (c.tag == "Gem") 
		{
			owner.RemoveNeighbor(c.GetComponent<Gem>());
		}
	}
}
