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


public class BoardManager : MonoBehaviour {
	public static BoardManager instance;
	public List<Sprite> characters = new List<Sprite>();
    public List<GameObject> tileList = new List<GameObject>();
	public GameObject tile;
	public int xSize, ySize;

	private GameObject[,] tiles;
    private List<GameObject> path;
	public bool IsShifting { get; set; }
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int lengthOfLineRenderer = 20;
    void Start () {
		instance = GetComponent<BoardManager>();

		Vector2 offset = tile.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y);
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.positionCount = lengthOfLineRenderer;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        lineRenderer.colorGradient = gradient;
        lineRenderer.loop = false;
    }

	private void CreateBoard (float xOffset, float yOffset) {
		tiles = new GameObject[xSize, ySize];

        float startX = transform.position.x;
		float startY = transform.position.y;

		for (int x = 0; x < xSize; x++) {
			for (int y = 0; y < ySize; y++) {
				GameObject newTile = Instantiate(tileList[Random.Range(0,tileList.Count)], new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
                newTile.GetComponent<Tile>().x = x;
                newTile.GetComponent<Tile>().y = y;
				tiles[x, y] = newTile;
                newTile.transform.parent = transform; // 1
                //Sprite newSprite = characters[Random.Range(0, characters.Count)]; // 2
                //newTile.GetComponent<SpriteRenderer>().sprite = newSprite; // 3
            }
        }
    }
    private void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3[] positions = new Vector3[Tile.selectedPath.Count];
        //if (Tile.selectedPath.Count>0)
        //{
        //    temp1 = Tile.selectedPath[0];
        //    for (int i=1;i<Tile.selectedPath.Count;i++)
        //    {
        //        temp2 = Tile.selectedPath[i];
        //        LineRenderer.SetPositions()
        //    }
        //}
        for(int i =0;i<Tile.selectedPath.Count;i++)
        {
            positions[i] = Tile.selectedPath[i].transform.position;
        }
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }
    public IEnumerator FindNullTiles()
    {
        float shiftDelay = .15f;
        bool foundnull = true;
       
        foundnull = false;
        //print("DROPPING" + xSize +' ' +ySize);
        //print("Ill be back");
        yield return new WaitForSeconds(shiftDelay);// 4
        //print("im back");
        for (int x = 0; x < xSize; x++)
        {

            for (int y = 0; y < ySize - 1; y++)
            {
                if (!tiles[x,y].GetComponent<SpriteRenderer>().enabled)
                {
                    //if(tiles[x, y+1].GetComponent<SpriteRenderer>().sprite == null)
                    foundnull = true;
                    Vector3 temp = tiles[x, y].transform.position;
                    GameObject tempGO = tiles[x, y];
                    tempGO.transform.position = tiles[x, y + 1].transform.position;
                    tiles[x, y] = tiles[x, y + 1];
                    tiles[x, y].GetComponent<Tile>().y = y;
                    tiles[x, y].transform.position = temp;
                    tiles[x, y + 1] = tempGO;

                }
                //print(x + ' ' + y);
            }
            if (!tiles[x, ySize - 1].GetComponent<SpriteRenderer>().enabled)
            {
                try
                {
                    GameObject tempGO = tiles[x, ySize - 1];
                    Destroy(tempGO);
                }
                catch (System.Exception e) { print(e); Debug.LogError(e); print(e); }
                tiles[x, ySize - 1] = Instantiate(tileList[Random.Range(0, tileList.Count)], tiles[x, ySize - 1].transform.position, tiles[x, ySize - 1].transform.rotation);
                //if(tiles[x, ySize - 1].GetComponent<Tile>().render.sprite ==null)
                //{
                //    print("WHAT");
                //}
                tiles[x, ySize - 1].GetComponent<Tile>().y = ySize - 1;
                tiles[x, ySize - 1].GetComponent<Tile>().x = x;

            }
            
        //for (int x = 0; x < xSize; x++)
        //{
        //    if (!tiles[x, ySize - 1].GetComponent<SpriteRenderer>().enabled)
        //    {
        //        print("really?");
        //    }
        //}
        }
        if(foundnull)
        {
            StartCoroutine(FindNullTiles());
        }
    }
    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .0f)
    {
        IsShifting = true;
        List<int> renders = new List<int>();
        int nullCount = 0;
        bool nullfound = false;
        for (int y = yStart; y < ySize; y++)
        {  // 1
            SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
            if (!render.enabled)
            { // 2
                nullCount++;
                nullfound = true;
            }
            if (nullfound) 
                renders.Add(y);
        }

        for (int i = 0; i < nullCount; i++)
        { // 3
            yield return new WaitForSeconds(shiftDelay);// 4
            if (renders.Count == 1)
            {
                tiles[x,renders[0]] = GetNewTile(x, ySize - 1);
            }
            int k;

            for (k = 0; k < renders.Count - 1; k++)
            {
                if (!tiles[x, renders[k]].GetComponent < SpriteRenderer>().enabled)
                    break;
            }
            for (; k < renders.Count - 1; k++)
            { // 5

                tiles[x, renders[k]] = tiles[x, renders[k+1]];
                tiles[x, renders[k+1]] = GetNewTile(x, ySize - 1); // 6
            }
            
        }
        IsShifting = false;
    }
    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(characters);

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < xSize - 1)
        {
            possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }
    private GameObject GetNewTile(int x, int y)
    {
        List<GameObject> possibleTiles = new List<GameObject>();
        possibleTiles.AddRange(tileList);

        //if (x > 0)
        //{
        //    possibleTiles.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        //}
        //if (x < xSize - 1)
        //{
        //    possibleTiles.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        //}
        //if (y > 0)
        //{
        //    possibleTiles.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        //}
        //return Instantiate(possibleTiles[Random.Range(0, possibleTiles.Count)], new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);
        return possibleTiles[Random.Range(0, possibleTiles.Count)];
    }
}
