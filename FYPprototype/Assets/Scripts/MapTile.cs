using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile
{
    public int gCost;
    public int hCost;
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

    public int fCost
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

    public List<MapTile> GetNeightbours()
    {
        return myNeighbours;
    }

}