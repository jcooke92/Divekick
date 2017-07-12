using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour 
{

	private bool isPitfall = false;

	public bool IsPitfall 
	{
		get 
		{ return isPitfall; }
		set 
		{ isPitfall = value; }
	}
}
