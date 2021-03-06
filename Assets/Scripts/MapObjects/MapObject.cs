﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapObject : MonoBehaviour
{
    private Vector2Int pos;
    public Vector2Int Pos
    {
        get
        {
            return pos;
        }
        set
        {
            pos = value;
            transform.position = new Vector3(pos.x * GameController.Current.TileSize, -pos.y * GameController.Current.TileSize, transform.position.z);
        }
    }
    public abstract void Interact(InteractState interactState);
    protected virtual void Start()
    {
        Pos = new Vector2Int((int)transform.position.x, -(int)transform.position.y);
        if (GameController.Current.MapObjects.Contains(this)) // This shouldn't happen - it's becaus of the weird "no Start" bug on units created with CreatePlayerUnit.
        {
            Debug.LogWarning("MapObjects already contains " + name + "!");
            return;
        }
        GameController.Current.MapObjects.Add(this);
    }
    protected virtual void OnDestroy()
    {
        if (!GameController.Current.MapObjects.Contains(this)) // This shouldn't happen - MapObjects are supposed to remove themselves
        {
            Debug.LogWarning("MapObjects doesn't contain " + name + "!");
            return;
        }
        GameController.Current.MapObjects.Remove(this);
    }
}
