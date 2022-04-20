using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile
{
    public float gCost;
    public float hCost;
    public Vector3 worldPosition;

    public Vector3Int cellPosition;

    private List<MapTile> myNeighbours;
    public MapTile parent;

    public MapTile(Vector3 _worldPosition, Vector3Int _cellPosition)
    {
        myNeighbours = new List<MapTile>();
        worldPosition = _worldPosition;
        cellPosition = _cellPosition;
    }

    public List<MapTile> GetNeighbours()
    {
        return myNeighbours;
    }


    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public void AddNeighbour(MapTile neighbour)
    {
        myNeighbours.Add(neighbour);
    }


}