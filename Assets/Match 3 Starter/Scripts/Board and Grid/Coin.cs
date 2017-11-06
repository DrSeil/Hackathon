using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Tile {

	// Use this for initialization
    public override void OnClear()
    {

        GUIManager.instance.Score += 1;
    }
}
