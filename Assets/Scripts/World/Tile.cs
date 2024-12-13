using UnityEngine;

public class Tile
{
    public WorldTileData TileData { get; set; } = new WorldTileData();
}

public class WorldTileData
{
    public Sprite RoomLayoutImage;
    public Vector2 pos;

}

// view map command: triggeres map overlay and press any key to continue that disables it
// tile in top right will display the tile the player is on along with its contents (enemies, items, secrets maybe idk yet)
// the rooms will be set up by: multiple prefabs probably