using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using zombie_survival;

public class GridVisualizer : MonoBehaviour
{
    [Header("Unity Prefabs (The Visuals)")]
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    public GameObject zombiePrefab;
    public GameObject ammoPrefab;
    public GameObject keyPrefab;
    public GameObject exitPrefab;
    public GameObject p1grave;
    public GameObject p2grave;
    public GameObject deadZombie;
    public GameObject shotgunFlashPrefab;

    private Dictionary<GridEntity, GameObject> visualMap = new Dictionary<GridEntity, GameObject>();

    [Header("Environment Setup")]
    public Tilemap floorTilemap;
    public TileBase[] floorTiles;

    public void InitializeGrid(GameGrid logicalGrid)
    {
        floorTilemap.ClearAllTiles();

        if (floorTiles == null || floorTiles.Length == 0)
        {
            Debug.LogError("Please assign your floor in the Inspector!");
        }

        for (int x = 0; x < logicalGrid.X; x++)
        {
            for (int y = 0; y < logicalGrid.Y; y++)
            {
                int randomIndex = Random.Range(0, floorTiles.Length);
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTiles[randomIndex]);
            }
        }
    }

    public void UpdateVisuals(GameGrid grid)
    {
        if (!grid.Player.HasEscaped)
        {
            UpdateEntityVisual(grid.Player, player1Prefab);
        }

        if (grid.Player2 != null && !grid.Player2.HasEscaped)
        {
             UpdateEntityVisual(grid.Player2, player2Prefab);
        }

        foreach (var zombie in grid.Zombies)
        {
            UpdateEntityVisual(zombie, zombiePrefab);
        }

        foreach (var collectable in grid.Collectables)
        {
            if (collectable is SkeletonKey) continue;

            if (collectable is GridEntity entity)
            {
                GameObject prefabToUse = null;
                if (collectable is AmmoCrate) prefabToUse = ammoPrefab;
                else if (collectable is ExitPortal) prefabToUse = exitPrefab;

                UpdateEntityVisual(entity, prefabToUse);
            }
        }

        CleanupDeadEntities(grid);
    }

    private void UpdateEntityVisual(GridEntity entity, GameObject prefab)
    {
        if (entity == null || prefab == null) return;

        if (!visualMap.ContainsKey(entity))
        {
            GameObject newVisual = Instantiate(prefab);
            visualMap.Add(entity, newVisual);
        }

        if (entity is Player p && p.IsDead)
        {
            GameObject correctGrave = (p == GameManager.Instance.Grid.Player) ? p1grave : p2grave;

            if (!visualMap[entity].name.Contains(correctGrave.name))
            {
                Destroy(visualMap[entity]);
                visualMap.Remove(entity);

                GameObject newVisual = Instantiate(correctGrave);
                visualMap.Add(entity, newVisual);
            }
        }
        visualMap[entity].transform.position = new Vector3(entity.Location.Y + 0.5f, ((GameManager.Instance.Settings.Size - 1) - entity.Location.X + 0.5f), 0);
        
        if (entity is Zombie z)
        {
            if (z.FacingDirection == MoveDirection.Left)
            {
                visualMap[entity].transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (z.FacingDirection == MoveDirection.Right)
            {
                visualMap[entity].transform.localScale = new Vector3(1, 1, 1);
            }
        }

        if (entity is Player alivePlayer && !alivePlayer.IsDead)
        {
            if (alivePlayer.FacingDirection == MoveDirection.Left)
            {
                visualMap[entity].transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (alivePlayer.FacingDirection == MoveDirection.Right)
            {
                visualMap[entity].transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void CleanupDeadEntities(GameGrid grid)
    {
        List<GridEntity> toRemove = new List<GridEntity>();

        foreach (var kvp in visualMap)
        {
            GridEntity entity = kvp.Key;
            bool stillExists = false;

            if (entity == grid.Player && !grid.Player.HasEscaped) stillExists = true;
            else if (grid.Player2 != null && entity == grid.Player2 && !grid.Player2.HasEscaped) stillExists = true;
            else if (entity is Zombie z && grid.Zombies.Contains(z)) stillExists = true;
            else if (entity is ICollectable c && grid.Collectables.Contains(c)) stillExists = true;

            if (!stillExists)
            {
                if (entity is Zombie)
                {
                    Instantiate(deadZombie, kvp.Value.transform.position, Quaternion.identity);
                }

                Destroy(kvp.Value);
                toRemove.Add(entity);
            }
        }

        foreach (var entity in toRemove)
        {
            visualMap.Remove(entity);
        }
    }

    public void SpawnShotEffects(Player player)
    {
        if (player == null || shotgunFlashPrefab == null) return;

        if (player.FacingDirection == MoveDirection.Up || player.FacingDirection == MoveDirection.Down)
        {
            return;
        }

        Vector3 PlayerPos = new Vector3(player.Location.Y + 0.5f, ((GameManager.Instance.Settings.Size - 1) - player.Location.X) + 0.5f, -1f);

        Vector3 offset = Vector3.zero;
        float offsetDistance = 0.6f;
        float verticalOffset = -0.07f;

        bool isFacingLeft = (player.FacingDirection == MoveDirection.Left);

        if (isFacingLeft)
        {
            offset = new Vector3(-offsetDistance, verticalOffset, 0f);            
        }
        else
        {
            offset = new Vector3(offsetDistance, verticalOffset, 0f);
        }
        
        GameObject flash = Instantiate(shotgunFlashPrefab, PlayerPos + offset, Quaternion.identity);
        
        float flashSize = 4f;

        if (isFacingLeft)
        {
            flash.transform.localScale = new Vector3(-flashSize, flashSize, 1f);
        }
        else
        {
            flash.transform.localScale = new Vector3(flashSize, flashSize, 1f);
        }
    }
}