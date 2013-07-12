using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Tetromino : MonoBehaviour {
	public enum TetrominoType
	{
		I,
		O,
		Z,
		RZ,
		T,
		L,
		RL
	};
	
	enum Direction
	{
		Left,
		Right
	}
	TetrominoType type;
	float maxFallTimer;
	float fallTimer;
	float maxMoveTimer;
	float moveTimer;
	float maxRotateTimer;
	float rotateTimer;
	List<Vector2> coordinates; //Pivot is always at first index (taking advantage of this in rotate())
	List<GameObject> blocks;
	bool onGround;
	bool[,] gameTiles;
	
	void Start ()
	{
		
	}
	
	//Custom constructor	
	public void setType(TetrominoType type)
	{
		fallTimer=0;
		maxMoveTimer=0.5f;
		moveTimer=0;
		maxRotateTimer=0.5f;
		rotateTimer=0;
		onGround=false;
		coordinates=new List<Vector2>();
		this.type=type;
		switch(type)
		{
		case TetrominoType.I:
			coordinates.Add(new Vector2(4, 0));
			coordinates.Add(new Vector2(3, 0));
			coordinates.Add(new Vector2(5, 0));
			coordinates.Add(new Vector2(6, 0));
			break;
		case TetrominoType.O:
			coordinates.Add(new Vector2(4, 0));
			coordinates.Add(new Vector2(5, 0));
			coordinates.Add(new Vector2(4, 1));
			coordinates.Add(new Vector2(5, 1));
			break;
		case TetrominoType.Z:
			coordinates.Add(new Vector2(4, 0));
			coordinates.Add(new Vector2(3, 1));
			coordinates.Add(new Vector2(4, 1));
			coordinates.Add(new Vector2(5, 0));
			break;
		case TetrominoType.RZ:
			coordinates.Add(new Vector2(4, 0));
			coordinates.Add(new Vector2(3, 0));
			coordinates.Add(new Vector2(4, 1));
			coordinates.Add(new Vector2(5, 1));
			break;
		case TetrominoType.T:
			coordinates.Add(new Vector2(4, 0));
			coordinates.Add(new Vector2(3, 0));
			coordinates.Add(new Vector2(4, 1));
			coordinates.Add(new Vector2(5, 0));
			break;
		case TetrominoType.L:
			coordinates.Add(new Vector2(4, 0));
			coordinates.Add(new Vector2(3, 1));
			coordinates.Add(new Vector2(3, 0));
			coordinates.Add(new Vector2(5, 0));
			break;
		case TetrominoType.RL:
			coordinates.Add(new Vector2(4, 0));
			coordinates.Add(new Vector2(3, 0));
			coordinates.Add(new Vector2(5, 0));
			coordinates.Add(new Vector2(5, 1));
			break;
		default:
			break;
		}
		
		blocks=new List<GameObject>();
		foreach(Vector2 coordinate in coordinates)
		{
			GameObject block=GameObject.CreatePrimitive(PrimitiveType.Cube);
			block.transform.position=new Vector3(coordinate.x, -coordinate.y, 0);
			blocks.Add(block);
		}
	}
	
	public void setMaxFallTimer(float maxFallTimer)
	{
		this.maxFallTimer=maxFallTimer;
	}
	
	public void setGameTiles(bool[,] gameTiles)
	{
		this.gameTiles=gameTiles;
	}
	
	//Checks whether we either are in the bottom row or have tiles below us
	bool isOnGround()
	{
		foreach(Vector2 coordinate in coordinates)
		{
			if(coordinate.y==gameTiles.GetLength(1)-1 || gameTiles[(int)coordinate.x, (int)coordinate.y+1]/* || gameTiles[(int)coordinate.x, (int)coordinate.y]*/)
			{
				onGround=true;
				return true;
			}
		}
		return false;
	}
	
	//direction is [-1;1]. Checks whether we are at the left or right boundary or are blocked by a tile
	bool isHorizontalSpace(int direction)
	{
		foreach(Vector2 coordinate in coordinates)
		{
			if((direction<0 && coordinate.x<=0) || 
				(direction>0 && coordinate.x>=gameTiles.GetLength(0)-1) ||
				gameTiles[(int)coordinate.x+direction, (int)coordinate.y])
			{
				return false;	
			}
		}
		return true;
	}
	
	void fallDown()
	{
		for(int i=0; i<coordinates.Count; i++)
		{
			coordinates[i]=new Vector2(coordinates[i].x, coordinates[i].y+1);	
		}
	}
	
	void moveHorizontally(int direction)
	{
		for(int i=0; i<coordinates.Count; i++)
		{
			coordinates[i]=new Vector2(coordinates[i].x+direction, coordinates[i].y);	
		}
	}
	
	void rotate()
	{
		if(type==TetrominoType.O)
		{
			return;	
		}
		Vector2 offset=coordinates[0];
		List<Vector2> newCoordinates=new List<Vector2>(coordinates);
		double rotation=Math.PI/2;
		if(type==TetrominoType.I)
		{
			rotation=-rotation;	
		}
		for(int i=0; i<newCoordinates.Count; i++)
		{
			//Move to origin
			newCoordinates[i]-=offset;
			//Rotation
			Vector2 temp=newCoordinates[i];
			newCoordinates[i]=new Vector2(	(float)(temp.x*Math.Cos(rotation)+temp.y*-Math.Sin (rotation)),
											(float)(temp.x*Math.Sin(rotation)+temp.y*Math.Cos (rotation)));
			//Back to original position
			newCoordinates[i]+=offset;
		}
		//Collision check
		bool colliding=false;
		foreach(Vector2 coordinate in newCoordinates)
		{
			if(	coordinate.x<0 ||
				coordinate.x>gameTiles.GetLength(0)-1 ||
				coordinate.y<0 ||
				coordinate.y>gameTiles.GetLength(1)-1 ||
				gameTiles[(int)coordinate.x, (int)coordinate.y])	
			{
				colliding=true;	
			}
		}
		if(!colliding)
		{
			//Overwrite coordinates
			coordinates=newCoordinates;
		}
	}
	
	//Interface for Game class
	public bool getOnGround()
	{
		return onGround;
	}
	
	public List<Vector2> getCoordinates()
	{
		return coordinates;	
	}
	
	public List<GameObject> getBlocks()
	{
		return blocks;	
	}
	
	public void printCoordinates()
	{
		foreach(Vector2 coordinate in coordinates)
		{
			Debug.Log (coordinate.x+ " " +coordinate.y);	
		}
	}
	
	void Update ()
	{
		for(int i=0; i<coordinates.Count; i++)
		{
			blocks[i].transform.position=new Vector3(coordinates[i].x, -coordinates[i].y, 0);
		}
		
		if(isOnGround())
		{
				return;	
		}
		
		rotateTimer+=Time.deltaTime;
		if(rotateTimer>=maxRotateTimer)
		{
			if(Input.GetAxisRaw("Vertical")>0)
			{
				rotate();
				
				if(isOnGround())
				{
					return;	
				}
				rotateTimer=0;
			}
		}
		
		moveTimer+=Time.deltaTime;
		if(moveTimer>=maxMoveTimer)
		{
			int direction=0;
			if(Input.GetAxisRaw("Horizontal")<0)
			{
				direction--;
			}
			if(Input.GetAxisRaw("Horizontal")>0)
			{
				direction++;	
			}
			
			if(direction!=0)
			{
				if(isHorizontalSpace(direction))
				{
					moveHorizontally(direction);
				}
				moveTimer=0;
				
				if(isOnGround())
				{
					return;
				}
			}
		}
		
		fallTimer+=Time.deltaTime;
		if(fallTimer>=maxFallTimer)
		{
			fallDown();
			fallTimer=0;
			
			if(isOnGround())
			{
				return;	
			}
		}
	}
}
