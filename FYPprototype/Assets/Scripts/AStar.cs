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
    private static Vector2Int[] wallPositions;

    public static List<MapTile> SetTiles(Tilemap _ground, Tilemap _walls)
    {
        ground = _ground;
        walls = _walls;
        List<Vector2Int> _wallPositions = new List<Vector2Int>();

        mapTiles = new List<MapTile>();
        ground.CompressBounds();
        var bounds = ground.cellBounds;
        
        //Debug.Log("max bounds x:" + bounds.max.x);
        //Debug.Log("min bounds x:" + bounds.min.x);
        //Debug.Log("max bounds y:" + bounds.max.y);
        //Debug.Log("min bounds y:" + bounds.min.y);
        
        for (int i = bounds.min.x; i <= bounds.max.x - 1; i++)
        {
            for (int j = bounds.min.y; j <= bounds.max.y - 1; j++)
            {
                var cellPosition = new Vector3Int(i, j, 0);
                if(walls.HasTile(cellPosition))
                {
                    _wallPositions.Add(new Vector2Int(cellPosition.x, cellPosition.y));
                }
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

        wallPositions = _wallPositions.ToArray();

        return mapTiles;
    }


    public static Vector2Int[] GetWallPositions()
    {
        return wallPositions;
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
            }
            else
            {
                foreach(MapTile neighbour in lowestFTile.GetNeighbours())
                {
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


    public static List<MapTile> AvoidanceSearch(Vector3 _startPosition, List<Vector3> _enemyPositions)
    {
        List<MapTile> openList = new List<MapTile>();

        List<MapTile> closedList = new List<MapTile>();

        Vector3 startPosition = new Vector3(_startPosition.x, _startPosition.y, 0);
        Vector3Int startPositionInt = ground.WorldToCell(startPosition);
        MapTile startTile = mapTiles.Where(x => x.cellPosition == startPositionInt).First();

        List<MapTile> enemyPositions = new List<MapTile>();

        foreach (Vector3 position in _enemyPositions)
        {
            Vector3 enemyPosition = new Vector3(position.x, position.y, 0);
            Vector3Int enemyPositionInt = ground.WorldToCell(enemyPosition);
            MapTile enemyTile = mapTiles.Where(x => x.cellPosition == enemyPositionInt).First();
            enemyPositions.Add(enemyTile);
        }

        openList.Add(startTile);
        startTile.gCost = 0;
        startTile.hCost = 0;
        startTile.parent = null;

        MapTile lowestFTile = null;
        int loops = 100;
        while (openList.Any() && loops > 0)
        {
            lowestFTile = null;
            foreach (MapTile tile in openList)
            {
                if (lowestFTile == null || tile.fCost < lowestFTile.fCost)
                {
                    lowestFTile = tile;
                }
            }
            openList.Remove(lowestFTile);
            closedList.Add(lowestFTile);

            foreach (MapTile neighbour in lowestFTile.GetNeighbours())
            {
                if(neighbour != startTile)
                {
                    if (!closedList.Contains(neighbour) && !walls.HasTile(neighbour.cellPosition))
                    {
                        SetAvoidanceCosts(neighbour, lowestFTile, enemyPositions);
                        if (!openList.Contains(neighbour)) openList.Add(neighbour);
                    }
                }
                
            }
            loops--;

        }
        List<MapTile> route = FindRoute(lowestFTile, startTile);
        if (route != null)
        {
            return route;
        }
        return null;

    }

    private static void SetAvoidanceCosts(MapTile neighbour, MapTile currentTile, List<MapTile> enemyPositions)
    {
        neighbour.gCost = currentTile.gCost + 1;

        float hCost = 0;
        foreach(MapTile enemy in enemyPositions)
        {
            float dXSq = Mathf.Pow(neighbour.cellPosition.x - enemy.cellPosition.x, 2f);
            float dYSq = Mathf.Pow(neighbour.cellPosition.y - enemy.cellPosition.y, 2f);
            hCost += 2* Mathf.Sqrt(dXSq + dYSq);
        }
        neighbour.hCost = 1/hCost;

        neighbour.parent = currentTile;
    }



}
