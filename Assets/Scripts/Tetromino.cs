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
		T,
		L
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
	List<Vector2> coordinates;
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
		onGround=false;
		coordinates=new List<Vector2>();
		this.type=type;
		switch(type)
		{
		case TetrominoType.I:
			coordinates.Add(new Vector2(3, 0));
			coordinates.Add(new Vector2(4, 0));
			coordinates.Add(new Vector2(5, 0));
			coordinates.Add(new Vector2(6, 0));
			break;
		case TetrominoType.O:
			coordinates.Add(new Vector2(4, 0));
			coordinates.Add(new Vector2(5, 0));
			coordinates.Add(new Vector2(4, 1));
			coordinates.Add(new Vector2(5, 1));
			break;
		}
	}
	
	public void setGameTiles(bool[,] gameTiles)
	{
		this.gameTiles=gameTiles;
	}
	
	public void setMaxFallTimer(float maxFallTimer)
	{
		this.maxFallTimer=maxFallTimer;	
	}
	
	//Checks whether we either are in the bottom row or have tiles below us
	bool isOnGround()
	{
		foreach(Vector2 coordinate in coordinates)
		{
			if(coordinate.y==gameTiles.GetLength(1)-1 || gameTiles[(int)coordinate.x, (int)coordinate.y+1])
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
	
	//Interface for Game class
	public bool getOnGround()
	{
		return onGround;
	}
	
	public List<Vector2> getCoordinates()
	{
		return coordinates;	
	}
	
	void Update ()
	{
		
		//TODO: Movement
		if(isOnGround())
		{
			return;
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
			}
		}
		
		if(isOnGround())
		{
			return;	
		}
		
		fallTimer+=Time.deltaTime;
		if(fallTimer>=maxFallTimer)
		{
			fallDown();
			fallTimer=0;
		}
	}
}
