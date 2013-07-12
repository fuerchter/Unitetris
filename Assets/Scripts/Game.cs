using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Game : MonoBehaviour {
	
	enum RowType
	{
		Empty,
		Filled
	};
	bool[,] gameTiles;
	GameObject[,] gameBlocks;
	GameObject activeTetromino;
	System.Random random;
	
	void Start ()
	{
		gameTiles=new bool[10, 18];
		/*for(int i=0; i<gameTiles.GetLength(0)-1; i++)
		{
			gameTiles[i, 1]=true;	
		}*/
		gameBlocks=new GameObject[gameTiles.GetLength (0), gameTiles.GetLength(1)];
		random=new System.Random();
	}
	
	bool isScreenFilled()
	{
		return !isRowType(0, RowType.Empty);
	}
	
	List<int> getFilledRows()
	{
		List<int> res=new List<int>();
		//Going through all the rows from bottom to top
		for(int y=gameTiles.GetLength(1)-1; y>=0; y--)
		{
			//Once we found an empty row there can be now further filled rows
			if(isRowType(y, RowType.Empty))
			{
				break;
			}
			else
			{
				//Adding the filled row to the list
				if(isRowType(y, RowType.Filled))
				{
					res.Add(y);
				}
			}
		}
		return res;
	}
	
	void removeRow(int row)
	{
		//Setting row false
		for(int x=0; x<gameTiles.GetLength(0); x++)
		{
			gameTiles[x, row]=false;	
		}
		
		//Moving up until empty row is found
		for(int y=row-1; y>=0; y--)
		{
			if(isRowType(y, RowType.Empty))
			{
				break;
			}
			else
			{
				//Moving current row down a tile
				for(int x=0; x<gameTiles.GetLength(0); x++)
				{
					gameTiles[x, y+1]=gameTiles[x, y];
					gameTiles[x, y]=false;
				}
			}
		}
	}
	
	void createTetromino()
	{
		activeTetromino=new GameObject();
		activeTetromino.transform.position=new Vector3(gameTiles.GetLength(0)/2-1, 0, 0);
		activeTetromino.AddComponent("Tetromino");
		Tetromino script=(Tetromino)activeTetromino.GetComponent("Tetromino");
		//Get random Tetromino
		int tetrominoCount=Enum.GetNames(typeof(Tetromino.TetrominoType)).Length;
		script.setType((Tetromino.TetrominoType)random.Next(tetrominoCount));
		script.setMaxFallTimer(0.25f);
		script.setGameTiles(gameTiles);
	}
	
	void destroyTetromino()
	{
		Tetromino script=(Tetromino)activeTetromino.GetComponent("Tetromino");
		List<Vector2> coordinates=script.getCoordinates();
		List<GameObject> blocks=script.getBlocks();
		for(int i=0; i<coordinates.Count; i++)
		{
			gameTiles[(int)coordinates[i].x, (int)coordinates[i].y]=true;	
			GameObject.Destroy(blocks[i]);
		}
		GameObject.Destroy(activeTetromino);
		activeTetromino=null;
	}
	
	bool isRowType(int row, RowType type)
	{
		for(int x=0; x<gameTiles.GetLength(0); x++)
		{
			switch(type)
			{
			case RowType.Empty:
				//We want to check whether a row is empty: break as soon as a block was found
				if(gameTiles[x, row])
				{
					return false;	
				}
				break;
			case RowType.Filled:
				//We want to check whether a row is filled: break as soon as a empty tile was found
				if(!gameTiles[x, row])
				{
					return false;	
				}
				break;
			default:
				break;
			}
			
		}
		return true;
	}
	
	void printTiles()
	{
		for(int y=0; y<gameTiles.GetLength(1); y++)
		{
			string row=""+y+ "\t";
			for(int x=0; x<gameTiles.GetLength(0); x++)
			{
				//Adding Tetromino to printout
				if(!gameTiles[x, y] && activeTetromino!=null)
				{
					bool placed=false;
					Tetromino script=(Tetromino)activeTetromino.GetComponent("Tetromino");
					List<Vector2> coordinates=script.getCoordinates();
					foreach(Vector2 coordinate in coordinates)
					{
						if((int)coordinate.x==x && (int)coordinate.y==y)
						{
							row+=1+ " ";
							placed=true;
							break;
						}
					}
					if(!placed)
					{
						row+=Convert.ToInt32(gameTiles[x, y])+ " ";
					}
				}
				else
				{
					row+=Convert.ToInt32(gameTiles[x, y])+ " ";
				}
			}
			Debug.Log (row);
		}
		Debug.Log ("");
	}
	
	void Update ()
	{
		for(int y=gameTiles.GetLength(1)-1; y>=0; y--)
		{
			for(int x=0; x<gameTiles.GetLength (0); x++)
			{
				if(gameTiles[x, y])
				{
					if(gameBlocks[x, y]==null)
					{
						gameBlocks[x, y]=GameObject.CreatePrimitive(PrimitiveType.Cube);
						gameBlocks[x, y].transform.position=new Vector3(x, -y, 0);
					}
				}
				else
				{
					if(gameBlocks[x, y]!=null)
					{
						GameObject.Destroy(gameBlocks[x, y]);
						gameBlocks[x, y]=null;
					}
				}
			}
		}
		printTiles();
		if(activeTetromino==null)
		{
			if(isScreenFilled())
			{
				Application.LoadLevel(1);
			}
			else
			{
				List<int> filledRows=getFilledRows();
				if(filledRows.Count!=0)
				{
					//Traversing filledRows backwards because it contains the upmost row last (and if we were to remove the bottom row first the filledRows indices won't match
					for(int i=filledRows.Count-1; i>=0; i--)
					{
						removeRow(filledRows[i]);
					}
				}
				createTetromino();
			}
		}
		else
		{
			Tetromino script=(Tetromino)activeTetromino.GetComponent("Tetromino");
			if(script.getOnGround())
			{
				destroyTetromino();	
			}
		}
	}
}
