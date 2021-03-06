﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;

public class MapPosition : MonoBehaviour {

	public int originX;
	public int originY;
	public float characterWidth = 0.08f;
	public float characterHeight = 0.16f;
	public MapBlockView mapBlockView;

	public int currentX {
		get { return Mathf.RoundToInt(transform.localPosition.x / characterWidth);}
	}

	public int currentY {
		get { return Mathf.RoundToInt(transform.localPosition.y / -characterHeight);}
	}

	public int screenCurrentX {
		get { 
			int x = this.currentX;
			x += (this.mapBlockView.blockX * this.mapBlockView.MapRows);
			return x;
		}
	}

	public int screenCurrentY {
		get { 
			int y = this.currentY;
			y += (this.mapBlockView.blockY * this.mapBlockView.MapCols);
			return y;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (transform.hasChanged) {
			int curX = currentX;
			int curY = currentY;
			if (originX != curX || originY != curY) {
			// Call Move
				mapBlockView.MoveObject (originX, originY, curX, curY, this.gameObject);
				originX = curX;
				originY = curY;
			}
			transform.hasChanged = false;
		}
	}
		
	public void removeFromMap ()
	{
		mapBlockView.RemoveObject (originX, originY, this.gameObject);
	}
}
