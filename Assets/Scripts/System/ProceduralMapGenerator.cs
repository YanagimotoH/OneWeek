using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralMapGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int size = new Vector2Int(41, 41);
    [SerializeField] Vector2Int startCell = new Vector2Int(1, 1);
    [SerializeField] Vector2Int origin;
    [SerializeField] bool randomSeed = true;
    [SerializeField] int seed;
    [SerializeField] int extraOpenings;
    [SerializeField] bool generateOnStart = true;

    [Header("Room")]
    [SerializeField] Vector2Int roomSizeMin = new Vector2Int(7, 7);
    [SerializeField] Vector2Int roomSizeMax = new Vector2Int(13, 13);
    [SerializeField] int roomEntranceCount = 2;

    [Header("Front Stage")]
    [SerializeField] Tilemap frontFloorTilemap;
    [SerializeField] Tilemap frontWallTilemap;
    [SerializeField] TileBase frontFloorTile;
    [SerializeField] TileBase frontWallTile;

    [Header("Back Stage")]
    [SerializeField] Tilemap backFloorTilemap;
    [SerializeField] Tilemap backWallTilemap;
    [SerializeField] TileBase backFloorTile;
    [SerializeField] TileBase backWallTile;

    [Header("Start")]
    [SerializeField] TileBase frontStartTile;
    [SerializeField] TileBase backStartTile;
    [SerializeField] Transform player;

    [Header("Goal")]
    [SerializeField] TileBase frontGoalTile;
    [SerializeField] TileBase backGoalTile;

    [Header("Enemies")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform enemyParent;
    [SerializeField] int enemyCount = 5;

    void Start()
    {
        if (generateOnStart)
        {
            Generate();
        }
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        int width = MakeOdd(size.x);
        int height = MakeOdd(size.y);
        Vector2Int start = SanitizeStart(startCell, width, height);

        System.Random rng = randomSeed ? new System.Random(Environment.TickCount) : new System.Random(seed);
        bool[,] floor = GenerateMaze(width, height, start, rng);
        if (extraOpenings > 0)
        {
            AddExtraOpenings(floor, width, height, rng, extraOpenings);
        }

        CreateRoom(floor, width, height, rng);

        ApplyTiles(floor, width, height);
        PlaceStart(start);
        Vector2Int goal = PlaceGoal(floor, width, height, start);
        PlaceEnemies(floor, width, height, start, goal, rng);
    }

    int MakeOdd(int value)
    {
        if (value < 3)
        {
            return 3;
        }

        return value % 2 == 0 ? value + 1 : value;
    }

    Vector2Int SanitizeStart(Vector2Int start, int width, int height)
    {
        int x = Mathf.Clamp(start.x, 1, width - 2);
        int y = Mathf.Clamp(start.y, 1, height - 2);
        if (x % 2 == 0)
        {
            x += x == width - 2 ? -1 : 1;
        }

        if (y % 2 == 0)
        {
            y += y == height - 2 ? -1 : 1;
        }

        return new Vector2Int(x, y);
    }

    bool[,] GenerateMaze(int width, int height, Vector2Int start, System.Random rng)
    {
        bool[,] floor = new bool[width, height];
        bool[,] visited = new bool[width, height];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(start);
        visited[start.x, start.y] = true;
        floor[start.x, start.y] = true;

        Vector2Int[] directions = new[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            List<Vector2Int> neighbors = new List<Vector2Int>();

            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction * 2;
                if (next.x <= 0 || next.y <= 0 || next.x >= width - 1 || next.y >= height - 1)
                {
                    continue;
                }

                if (!visited[next.x, next.y])
                {
                    neighbors.Add(next);
                }
            }

            if (neighbors.Count == 0)
            {
                stack.Pop();
                continue;
            }

            Vector2Int chosen = neighbors[rng.Next(neighbors.Count)];
            Vector2Int between = (current + chosen) / 2;

            visited[chosen.x, chosen.y] = true;
            floor[chosen.x, chosen.y] = true;
            floor[between.x, between.y] = true;
            stack.Push(chosen);
        }

        return floor;
    }

    void AddExtraOpenings(bool[,] floor, int width, int height, System.Random rng, int count)
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (floor[x, y])
                {
                    continue;
                }

                int adjacent = 0;
                if (floor[x + 1, y]) adjacent++;
                if (floor[x - 1, y]) adjacent++;
                if (floor[x, y + 1]) adjacent++;
                if (floor[x, y - 1]) adjacent++;

                if (adjacent >= 2)
                {
                    candidates.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int i = 0; i < count && candidates.Count > 0; i++)
        {
            int index = rng.Next(candidates.Count);
            Vector2Int cell = candidates[index];
            candidates.RemoveAt(index);
            floor[cell.x, cell.y] = true;
        }
    }

    void CreateRoom(bool[,] floor, int width, int height, System.Random rng)
    {
        int roomWidth = ClampOdd(rng.Next(roomSizeMin.x, roomSizeMax.x + 1));
        int roomHeight = ClampOdd(rng.Next(roomSizeMin.y, roomSizeMax.y + 1));
        roomWidth = Mathf.Min(roomWidth, width - 4);
        roomHeight = Mathf.Min(roomHeight, height - 4);

        int xMin = rng.Next(2, width - roomWidth - 2);
        int yMin = rng.Next(2, height - roomHeight - 2);
        RectInt room = new RectInt(xMin, yMin, roomWidth, roomHeight);

        for (int x = room.xMin; x < room.xMax; x++)
        {
            for (int y = room.yMin; y < room.yMax; y++)
            {
                floor[x, y] = true;
            }
        }

        for (int x = room.xMin - 1; x <= room.xMax; x++)
        {
            for (int y = room.yMin - 1; y <= room.yMax; y++)
            {
                if (x < 0 || y < 0 || x >= width || y >= height)
                {
                    continue;
                }

                if (x >= room.xMin && x < room.xMax && y >= room.yMin && y < room.yMax)
                {
                    continue;
                }

                floor[x, y] = false;
            }
        }

        List<(Vector2Int cell, Vector2Int dir)> entrances = new List<(Vector2Int, Vector2Int)>
        {
            (new Vector2Int(rng.Next(room.xMin, room.xMax), room.yMax - 1), Vector2Int.up),
            (new Vector2Int(rng.Next(room.xMin, room.xMax), room.yMin), Vector2Int.down),
            (new Vector2Int(room.xMin, rng.Next(room.yMin, room.yMax)), Vector2Int.left),
            (new Vector2Int(room.xMax - 1, rng.Next(room.yMin, room.yMax)), Vector2Int.right)
        };

        int entranceCount = Mathf.Clamp(roomEntranceCount, 1, entrances.Count);
        for (int i = 0; i < entranceCount; i++)
        {
            int index = rng.Next(entrances.Count);
            (Vector2Int cell, Vector2Int dir) entrance = entrances[index];
            entrances.RemoveAt(index);
            CarveEntrance(floor, width, height, entrance.cell, entrance.dir);
        }
    }

    void CarveEntrance(bool[,] floor, int width, int height, Vector2Int start, Vector2Int dir)
    {
        Vector2Int current = start + dir;
        int safety = width * height;
        while (safety-- > 0)
        {
            if (current.x <= 0 || current.y <= 0 || current.x >= width - 1 || current.y >= height - 1)
            {
                break;
            }

            floor[current.x, current.y] = true;

            Vector2Int next = current + dir;
            if (next.x < 0 || next.y < 0 || next.x >= width || next.y >= height)
            {
                break;
            }

            if (floor[next.x, next.y])
            {
                break;
            }

            current = next;
        }
    }

    void ApplyTiles(bool[,] floor, int width, int height)
    {
        if (frontFloorTilemap != null) frontFloorTilemap.ClearAllTiles();
        if (frontWallTilemap != null) frontWallTilemap.ClearAllTiles();
        if (backFloorTilemap != null) backFloorTilemap.ClearAllTiles();
        if (backWallTilemap != null) backWallTilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cell = new Vector3Int(x + origin.x, y + origin.y, 0);
                if (floor[x, y])
                {
                    if (frontFloorTilemap != null && frontFloorTile != null)
                    {
                        frontFloorTilemap.SetTile(cell, frontFloorTile);
                    }

                    if (backFloorTilemap != null && backFloorTile != null)
                    {
                        backFloorTilemap.SetTile(cell, backFloorTile);
                    }
                }
                else
                {
                    if (frontWallTilemap != null && frontWallTile != null)
                    {
                        frontWallTilemap.SetTile(cell, frontWallTile);
                    }

                    if (backWallTilemap != null && backWallTile != null)
                    {
                        backWallTilemap.SetTile(cell, backWallTile);
                    }
                }
            }
        }
    }

    void PlaceStart(Vector2Int start)
    {
        if (frontFloorTilemap == null)
        {
            return;
        }

        Vector3Int cell = new Vector3Int(start.x + origin.x, start.y + origin.y, 0);
        Vector3 world = frontFloorTilemap.GetCellCenterWorld(cell);

        if (frontStartTile != null)
        {
            frontFloorTilemap.SetTile(cell, frontStartTile);
        }

        if (backFloorTilemap != null && backStartTile != null)
        {
            backFloorTilemap.SetTile(cell, backStartTile);
        }

        if (player != null)
        {
            Vector3 playerPosition = world;
            playerPosition.z = player.position.z;
            player.SetPositionAndRotation(playerPosition, player.rotation);
        }
    }

    Vector2Int PlaceGoal(bool[,] floor, int width, int height, Vector2Int start)
    {
        if (frontFloorTilemap == null)
        {
            return start;
        }

        Vector2Int farthest = start;
        int[,] distance = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                distance[x, y] = -1;
            }
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        distance[start.x, start.y] = 0;

        Vector2Int[] directions = new[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int direction in directions)
            {
                Vector2Int next = current + direction;
                if (next.x < 0 || next.y < 0 || next.x >= width || next.y >= height)
                {
                    continue;
                }

                if (!floor[next.x, next.y] || distance[next.x, next.y] >= 0)
                {
                    continue;
                }

                distance[next.x, next.y] = distance[current.x, current.y] + 1;
                queue.Enqueue(next);
                if (distance[next.x, next.y] > distance[farthest.x, farthest.y])
                {
                    farthest = next;
                }
            }
        }

        Vector3Int cell = new Vector3Int(farthest.x + origin.x, farthest.y + origin.y, 0);

        if (frontGoalTile != null)
        {
            frontFloorTilemap.SetTile(cell, frontGoalTile);
        }

        if (backFloorTilemap != null && backGoalTile != null)
        {
            backFloorTilemap.SetTile(cell, backGoalTile);
        }

        return farthest;
    }

    void PlaceEnemies(bool[,] floor, int width, int height, Vector2Int start, Vector2Int goal, System.Random rng)
    {
        if (enemyPrefab == null || frontFloorTilemap == null || enemyCount <= 0)
        {
            return;
        }

        if (enemyParent != null)
        {
            for (int i = enemyParent.childCount - 1; i >= 0; i--)
            {
                Destroy(enemyParent.GetChild(i).gameObject);
            }
        }

        List<Vector2Int> candidates = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!floor[x, y])
                {
                    continue;
                }

                Vector2Int cell = new Vector2Int(x, y);
                if (cell == start || cell == goal)
                {
                    continue;
                }

                candidates.Add(cell);
            }
        }

        int spawnCount = Mathf.Min(enemyCount, candidates.Count);
        for (int i = 0; i < spawnCount; i++)
        {
            int index = rng.Next(candidates.Count);
            Vector2Int chosen = candidates[index];
            candidates.RemoveAt(index);

            Vector3Int cell = new Vector3Int(chosen.x + origin.x, chosen.y + origin.y, 0);
            Vector3 world = frontFloorTilemap.GetCellCenterWorld(cell);
            Instantiate(enemyPrefab, world, Quaternion.identity, enemyParent);
        }
    }

    int ClampOdd(int value)
    {
        if (value < 3)
        {
            return 3;
        }

        return value % 2 == 0 ? value - 1 : value;
    }
}
