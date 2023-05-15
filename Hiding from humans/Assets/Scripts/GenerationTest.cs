using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerationTest : MonoBehaviour
{
    public int width = 30;
    public int height = 20;
    public Tilemap tilemap;
    public TileBase platformTile;

    private Vector3Int currentPosition;
    private Vector3Int direction;
    private int distance;

    private void Start()
    {
        // Set starting position at the bottom-left corner of the map
        currentPosition = new Vector3Int(0, 0, 0);

        // Set random starting direction and distance
        direction = Random.value < 0.5f ? Vector3Int.up : Vector3Int.right;
        distance = Random.Range(5, 10);

        // Generate platform tiles in a random walk pattern
        for (int i = 0; i < width * height; i++)
        {
            // Place a platform tile at the current position
            tilemap.SetTile(currentPosition, platformTile);

            // Update the current position
            currentPosition += direction;

            // Check if the current position is outside the bounds of the map
            if (currentPosition.x < 0 || currentPosition.x >= width || currentPosition.y < 0 || currentPosition.y >= height)
            {
                // Reverse the direction and move back to the last valid position
                currentPosition -= direction;
                direction *= -1;

                // Reset the distance to a new random value
                distance = Random.Range(5, 10);
            }
            else
            {
                // Decrease the remaining distance in the current direction
                distance--;

                // If the remaining distance is 0, randomly change direction and reset the distance
                if (distance == 0)
                {
                    direction = Random.value < 0.5f ? Vector3Int.up : Vector3Int.right;
                    distance = Random.Range(5, 10);
                }
            }
        }
    }
}
