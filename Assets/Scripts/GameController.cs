﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Current;
    public List<Tile> Tiles;
    public Vector2Int MapSize;
    public float TileSize;
    [TextArea(3,10)]
    public List<string> Rooms;
    public GameObject Cursor;
    [Header("UI")]
    public Text TileInfo;
    public RectTransform TileInfoPanel;
    public Text UnitInfo;
    public RectTransform UnitInfoPanel;
    [HideInInspector]
    public Tile[,] Map;
    [HideInInspector]
    public List<MapObject> MapObjects;
    [HideInInspector]
    public InteractState InteractState = InteractState.None;
    private float cursorMoveDelay;
    private Vector2Int previousPos = new Vector2Int(-1, -1);
    private Vector2Int cursorPos
    {
        get
        {
            return new Vector2Int((int)(Cursor.transform.position.x / TileSize), (int)(Cursor.transform.position.y / TileSize));
        }
    }
    private void Start()
    {
        /*
         * I had a few different ideas:
         * -Current
         * -Dungeon: floors and walls, and generate straight lines
         * -Roads: continue random number of steps in a direction, change. Randomaly split/stop.
         */
        Current = this;
        string selectedRoom = Rooms[Random.Range(0, Rooms.Count)];
        string[] lines = selectedRoom.Split(';');
        Map = new Tile[MapSize.x, MapSize.y];
        for (int i = 0; i < MapSize.x; i++)
        {
            string[] line = lines[i].Split('|');
            for (int j = 0; j < MapSize.y; j++)
            {
                int tileID = int.Parse(line[j]);
                Tile newTile = Instantiate(Tiles[tileID].gameObject, transform).GetComponent<Tile>();
                newTile.transform.position = new Vector2(TileSize * i, -TileSize * j);
                newTile.gameObject.SetActive(true);
                Map[i, j] = newTile;
            }
        }
    }
    /// <summary>
    /// Used for player control.
    /// </summary>
    private void Update()
    {
        if (cursorMoveDelay <= 0)
        {
            if (Mathf.Abs(Input.GetAxis("Horizontal")) >= 0.5f || Mathf.Abs(Input.GetAxis("Vertical")) >= 0.5f)
            {
                Cursor.transform.position += new Vector3(
                    Sign(Input.GetAxis("Horizontal")),
                    Sign(Input.GetAxis("Vertical"))) * TileSize;
                cursorMoveDelay = 0.15f;
            }
        }
        else
        {
            cursorMoveDelay -= Time.deltaTime;
        }
        if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.5f && Mathf.Abs(Input.GetAxis("Vertical")) < 0.5f)
        {
            cursorMoveDelay = 0;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            InteractWithTile(cursorPos.x, cursorPos.y);
        }
        if (previousPos != cursorPos)
        {
            TileInfo.text = Map[cursorPos.x, -cursorPos.y].Name + '\n' + (Map[cursorPos.x, -cursorPos.y].MovementCost <= 9 ? (Map[cursorPos.x, -cursorPos.y].MovementCost + "MOV") : "");
            Unit unit = FindUnitAtPos(cursorPos.x, cursorPos.y);
            Vector2Int anchor;
            if (cursorPos.x >= MapSize.x / 2)
            {
                anchor = new Vector2Int(0, 1);
            }
            else
            {
                anchor = new Vector2Int(1, 1);
            }
            if (unit != null)
            {
                UnitInfoPanel.gameObject.SetActive(true);
                UnitInfo.text = unit.Name + "\nHP:" + unit.Health + "/" + unit.MaxHealth;
                UnitInfoPanel.anchorMin = anchor;
                UnitInfoPanel.anchorMax = anchor;
                UnitInfoPanel.pivot = anchor;
                UnitInfoPanel.GetComponent<PalettedSprite>().Palette = unit.gameObject.GetComponent<PalettedSprite>().Palette;
            }
            else
            {
                UnitInfoPanel.gameObject.SetActive(false);
            }
            anchor.y -= 1;
            TileInfoPanel.anchorMin = anchor;
            TileInfoPanel.anchorMax = anchor;
            TileInfoPanel.pivot = anchor;
        }
        previousPos = cursorPos;
    }
    public void InteractWithTile(int x, int y)
    {
        Debug.Log(InteractState);
        MapObjects.FindAll(a => a.Pos.x == x && a.Pos.y == y).ForEach(a => a.Interact(InteractState));
    }
    public void RemoveMarkers()
    {
        MapObjects.FindAll(a => a is Marker).ForEach(a => Destroy(a.gameObject));
        MapObjects.RemoveAll(a => a is Marker);
        previousPos = new Vector2Int(-1, -1);
    }
    public Unit FindUnitAtPos(int x, int y)
    {
        return (Unit)MapObjects.Find(a => a is Unit && a.Pos.x == x && a.Pos.y == y);
    }
    int Sign(float number)
    {
        return number < 0 ? -1 : (number > 0 ? 1 : 0);
    }
}
