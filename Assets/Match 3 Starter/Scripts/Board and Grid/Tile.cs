/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static Tile previousSelected = null;
    public static List<GameObject> selectedPath = new List<GameObject>();
	public SpriteRenderer render;
	private bool isSelected = false;
    private static bool mousedown;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private Vector2[] adjacentDirectionsVert = new Vector2[] { Vector2.up, Vector2.zero, Vector2.down};
    private Vector2[] adjacentDirectionsHorz = new Vector2[] { Vector2.left, Vector2.zero, Vector2.right};

    public int x;
    public int y;


    void Start() {
		render = GetComponent<SpriteRenderer>();
        //print(render.sprite);
        //print("TEST"+x+ ' '+ y);
    }
    private void StartPath()
    {
        selectedPath.Clear();
        selectedPath.Add(gameObject);
        Select();
    }
	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		//previousSelected = null;
	}
    private void OnMouseDown()
    {
        //print("mouse Down");
        //render = GetComponent<SpriteRenderer>();
        if (!render.enabled  || BoardManager.instance.IsShifting)
        {
            print(render);
            print(BoardManager.instance.IsShifting);
            return;
        }
        //print("Moving ON");
        mousedown = true;
        StartPath();
    }
    private void OnMouseEnter()
    {
        if(mousedown)
        {
            if(previousSelected.render.sprite == render.sprite)
            {
                if(selectedPath.Contains(gameObject))
                {
                    int ind = selectedPath.IndexOf(gameObject);
                    for(int i=selectedPath.Count-1;i>ind;i--)
                    {
                        selectedPath[i].GetComponent<Tile>().Deselect();
                        selectedPath.RemoveAt(i);
                    }
                    previousSelected = gameObject.GetComponent<Tile>();
                }
                else if (isAdjacent())
                {
                    selectedPath.Add(gameObject);
                    Select();
                }
            }

        }
    }
    private void OnMouseUp()
    {
        mousedown = false;
        if (selectedPath.Count < 3)
        {
            for (int i = selectedPath.Count - 1; i >= 0; i--)
            {
                selectedPath[i].GetComponent<Tile>().Deselect();
                selectedPath.Remove(selectedPath[i]);
            }
        }
        else
        {
            GUIManager.instance.MoveCounter--;
            ClearPath();
        }
    }
    public virtual void OnClear()
    {

    }
    private void ClearPath()
    {
        OnClear();
        for(int i =0;i<selectedPath.Count;i++)
        {
            selectedPath[i].GetComponent<Tile>().Deselect();
            selectedPath[i].GetComponent<SpriteRenderer>().enabled =false;
        }
        selectedPath.Clear();
        //StopCoroutine(BoardManager.instance.FindNullTiles());
        StartCoroutine(BoardManager.instance.FindNullTiles());
        //BoardManager.instance.FindNullTiles();

    }
    public virtual bool isAdjacent()
    {
        return System.Math.Abs(previousSelected.y - y) <= 1 && System.Math.Abs(previousSelected.x - x) <= 1;
        //GameObject tempVert;
        //GameObject temp;
        //for(int i =0; i<adjacentDirectionsVert.Length;i++)
        //{
        //    if(adjacentDirectionsVert[i]!=Vector2.zero)
        //    {
        //        tempVert = GetAdjacent(adjacentDirectionsVert[i]);
        //    }
        //    else
        //    {
        //        tempVert = gameObject;
        //    }
        //    for(int j=0;j<adjacentDirectionsHorz.Length;j++)
        //    {
        //        if(adjacentDirectionsHorz[j]!=Vector2.zero&&tempVert)
        //        {
        //            temp = tempVert.GetComponent<Tile>().GetAdjacent(adjacentDirectionsHorz[j]);
        //        }
        //        else
        //        {
        //            temp = tempVert;
        //        }
        //        if(previousSelected.gameObject == temp)
        //        {
        //            return true;
        //        }
        //    }
        //}
        //return false;
    }
    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

}