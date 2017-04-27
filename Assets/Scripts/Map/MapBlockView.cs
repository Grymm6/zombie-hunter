﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

namespace AssemblyCSharp {
	public class MapBlockView : MonoBehaviour {

		private int blockX;
		private int blockY;
		public MapBlockData mapBlockData;

		public int MapRows = 20;
		public int MapCols = 20;
		public float characterWidth = 0.08f;
		public float characterHeight = 0.16f;
		protected MapFile mapfile;
		public String mapDataPath = "Assets/Maps/Test/";

		private GameObject prefabWall;
		private bool isInitialized = false;
		//private Map map;

		void Start ()
		{
			mapfile = new MapFile ();
		}

		public void Initialize (int blockX, int blockY, MapBlockData mapBlockData) {
			this.blockX = blockX;
			this.blockY = blockY;
			this.mapBlockData = mapBlockData;				

			if (prefabWall == null) {
				prefabWall = (GameObject)Resources.Load ("Main/Wall", typeof(GameObject));
			}

			if (mapBlockData == null) {
				// Empty map blocks will display as walls
					for (int x = 0; x < MapRows ; x++) {
						for (int y = 0; y < MapCols; y++) {
						CreateMapObject (x, y, prefabWall, this.gameObject, "Main/Wall", 6, MapLayer.Floor);
						}
					}
			} else {
				for (int x = 0; x < mapBlockData.getRows (); x++) {
					for (int y = 0; y < mapBlockData.getCols (); y++) {
						if (x < 0 || x == mapBlockData.getRows () || y < 0 || y == mapBlockData.getCols ()) {
							CreateMapObject (x, y, prefabWall, this.gameObject, "Main/Wall", 6, MapLayer.Floor);
							} else {
							GameObject floorObject = (GameObject)Resources.Load (mapBlockData.getFloorResource (x, y), typeof(GameObject));
								if (floorObject != null) {
								CreateMapObject (x, y, floorObject, this.gameObject, mapBlockData.getFloorResource (x, y), mapBlockData.getFloorInt(x, y), MapLayer.Floor);
								}
							GameObject mainObject = (GameObject)Resources.Load (mapBlockData.getMainResource (x, y), typeof(GameObject));
								if (mainObject != null) {
								CreateMapObject (x, y, mainObject, this.gameObject, mapBlockData.getMainResource (x, y), mapBlockData.getMainInt(x, y), MapLayer.Main);
								}
							}
						}
					}
			}
			isInitialized = true;
		}
	
		public void MoveObject(int x, int y, int newX, int newY, GameObject obj) {
			if (isInitialized) {
				if (mapBlockData != null) {
					if (newX >= 0 && newX <= mapBlockData.getRows () &&
						newY >= 0 && newY <= mapBlockData.getCols ()) {
						AddObject (newX, newY, obj);			
						RemoveObject (x, y, obj);
					} else {
						// move object to a different BlockView
					}
				}
			} else {
				throw new UnityException ("MapBlockView isn't initialized");
			}
		}

		public void RemoveObject(int x, int y, GameObject obj) {
				if (isInitialized) {
				if (mapBlockData != null) {
					if (obj != null) {
						MapValue mapValue = obj.GetComponent<MapValue> ();
						if (mapValue != null) {
							switch (mapValue.layer) {
							case MapLayer.Floor:
								mapBlockData.setFloorInt (x, y, null);
								break;
							case MapLayer.Main:
								mapBlockData.setMainInt (x, y, null);
								break;
							}
						}
					}
				}
			} else {
				throw new UnityException ("MapBlockView isn't initialized");
			}
		}
	
		public void AddObject(int x, int y, GameObject obj) {
				if (isInitialized) {
				if (mapBlockData != null) {
					if (obj != null) {
						obj.transform.parent = this.gameObject.transform;
						MapValue mapValue = obj.GetComponent<MapValue> ();
						if (mapValue != null) {
							switch (mapValue.layer) {
							case MapLayer.Floor:
								mapBlockData.setFloorInt (x, y, mapValue.strValue);
								break;
							case MapLayer.Main:
								mapBlockData.setMainInt (x, y, mapValue.strValue);
								break;
							}
						}
					}
				}
			} else {
				throw new UnityException ("MapBlockView isn't initialized");
			}
		}

		private void CreateMapObject (int x, int y, GameObject mapPrefab, GameObject parent, string resourceName, int resourceInt, MapLayer layer)
		{
			if (mapPrefab != null) {
				if (parent != null) {
					GameObject prefab = (GameObject)Instantiate (mapPrefab,parent.transform.position + calculateTransformPosition(x,y) , Quaternion.identity, parent.transform);
					MapPosition mapPosition = prefab.GetComponent<MapPosition> ();
					if (mapPosition == null) {
						throw new MissingComponentException ("Map Object is missing the MapPosition component");
					}
					mapPosition.originX = x;
					mapPosition.originY = y;
					mapPosition.mapBlockView = this;
					MapValue mapValue =	prefab.AddComponent<MapValue> ();
					mapValue.intValue = resourceInt;
					mapValue.strValue = resourceName;
					mapValue.layer = layer;
				}
			} else {
				throw new MissingReferenceException ("Map Prefab Reference Missing");
			}
		}

		private Vector3 calculateTransformPosition(int x, int y) {
			Vector3 retval;
			retval = new Vector3 ((x * characterWidth), (-y * characterHeight), 0);
			return retval;
		}

		String getMapPath(int x, int y) {
			StringBuilder sb = new StringBuilder (mapDataPath);
			sb.Append ("map");
			sb.Append (x);
			sb.Append ("x");
			sb.Append (y);
			sb.Append(".txt");
			return sb.ToString ();
		}

		void OnDisable() {
			SaveMap(blockX, blockY);
			DestroyObject (this.gameObject);
		}

		void SaveMap(int Worldx, int Worldy) {
			if (this.mapBlockData != null) {
				String mapPath = getMapPath (Worldx, Worldy);
				mapfile.SaveFile (this.mapBlockData, mapPath);
			}
			
		}

		void SaveMapThreaded(int Worldx, int Worldy) {
			if (this.mapBlockData != null) {
				String mapPath = getMapPath (Worldx, Worldy);
				SaveFileJob saveFileJob = new SaveFileJob ();
				saveFileJob.input = this.mapBlockData;
				saveFileJob.path = mapPath;
				saveFileJob.Start ();
			}
		}
	}
}