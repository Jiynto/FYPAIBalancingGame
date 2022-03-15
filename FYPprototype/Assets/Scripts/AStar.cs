using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public static class AStar
{

    private static List<MapTile> mapTiles;

    private static Tilemap ground = null;
    private static Tilemap walls = null;


    public static List<MapTile> SetTiles(Tilemap _ground, Tilemap _walls)
    {
        ground = _ground;
        walls = _walls;

        mapTiles = new List<MapTile>();
        var bounds = ground.cellBounds;
        for(int i = bounds.min.x; i <= bounds.max.x; i++)
        {
            for (int j = bounds.min.y; j <= bounds.max.y; j++)
            {
                var cellPosition = new Vector3Int(i, j, 0);
                var worldPosition = ground.GetCellCenterWorld(cellPosition);
                worldPosition.z = -1;
                MapTile newMapTile = new MapTile(worldPosition, cellPosition);
                mapTiles.Add(newMapTile);
            }
            
        }

        foreach(MapTile mapTile in mapTiles)
        {
            
            for(int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == 0 && j == 0))
                    {
                        var possibleNeighbour = mapTiles.Where(x => x.cellPosition.x == mapTile.cellPosition.x + i && x.cellPosition.y == mapTile.cellPosition.y + j) ;
                        if (possibleNeighbour.Any()) mapTile.AddNeighbour(possibleNeighbour.First());
                    }
                }

            }
        }

        return mapTiles;
    }


    public static List<MapTile> Search(Vector3 _startPosition, Vector3 _endPosition)
    {

        List<MapTile> openList = new List<MapTile>();

        List<MapTile> closedList = new List<MapTile>();

        Vector3 startPosition = new Vector3(_startPosition.x, _startPosition.y, 0);
        Vector3 endPosition = new Vector3(_endPosition.x, _endPosition.y, 0);

        Vector3Int endPositionInt = ground.WorldToCell(endPosition);
        Vector3Int startPositionInt = ground.WorldToCell(startPosition);



        MapTile startTile = mapTiles.Where(x => x.cellPosition == startPositionInt).First();
        MapTile endTile = mapTiles.Where(x => x.cellPosition == endPositionInt).First();

        openList.Add(startTile);
        startTile.gCost = 0;
        startTile.hCost = 0;
        startTile.parent = null;


        while (openList.Any())
        {
            MapTile lowestFTile = null;
            foreach (MapTile tile in openList)
            {
                if (lowestFTile == null || tile.fCost < lowestFTile.fCost)
                {
                    lowestFTile = tile;
                }
            }
            openList.Remove(lowestFTile);
            closedList.Add(lowestFTile);

            if(lowestFTile == endTile)
            {
                List<MapTile> route = FindRoute(endTile, startTile);
                if (route != null)
                {
                    return route;
                }
                else
                {
                    List<MapTile> closed = closedList;
                }
            }
            else
            {
                foreach(MapTile neighbour in lowestFTile.GetNeightbours())
                {
                    Vector3 temp = neighbour.worldPosition;
                    temp.z = 0;
                    if(!closedList.Contains(neighbour) && !walls.HasTile( neighbour.cellPosition))
                    {
                        SetCosts(neighbour, lowestFTile, endTile);
                        if (!openList.Contains(neighbour)) openList.Add(neighbour);
                    }
                }
            }

        }
        return null;
    }



    private static void SetCosts(MapTile neighbour, MapTile currentTile, MapTile endTile)
    {
        neighbour.gCost = currentTile.gCost + 1;
        float dXSq = Mathf.Pow(neighbour.cellPosition.x - endTile.cellPosition.x, 2f);
        float dYSq = Mathf.Pow(neighbour.cellPosition.y - endTile.cellPosition.y, 2f);
        neighbour.hCost = (int)Mathf.Sqrt(dXSq + dYSq);
        neighbour.parent = currentTile;
    }


    private static List<MapTile> FindRoute(MapTile endTile, MapTile startTile)
    {
        List<MapTile> route = new List<MapTile>();
        MapTile currentTile = endTile;
        while (currentTile != startTile)
        {
            route.Add(currentTile);
            currentTile = currentTile.parent;
        }
        return route;
    }

}
