using UnityEngine;
using System.Collections;

public static class MapGenerator {
    
    public static float[,] SimpleHeightMapGenerator(int width, int height, int randomSeed) {
        float[,] heightMap = new float[width, height];

        System.Random rng = new System.Random(randomSeed);
        float xOffset = rng.Next(-100000, 100000);
        float yOffset = rng.Next(-100000, 100000);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                
                heightMap[x,y] = Mathf.PerlinNoise(.1f * x + xOffset, .1f * y + yOffset);
                // Debug.Log("x: " + x + ", y: " + y + ", val: " + heightMap[x, y]);
            }
        }
        return heightMap;
    }

    public static Color[] GenerateColorMap(float[,] heightMap, int mapWidth, int mapHeight) {

        Color[] colorMap = new Color[heightMap.GetLength(0) * heightMap.GetLength(1)];

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                float height = heightMap[x, y];
                Color color;
                if (height < 0.2) {
                    color = new Color(0, 41 / 255f, 58 / 255f); // water
                    // color = Color.blue;
                } else if (height < 0.3) {
					color = new Color(219 / 255f, 209 / 255f, 180 / 255f); // sand
                    // color = Color.yellow;
                } else if (height < 0.6) {
                    color = new Color(96 / 255f, 153 / 255f, 25 / 255f); // grass
                    // color = Color.green;
                } else if (height < 0.8) {
                    color = new Color(151 / 255f, 124 / 255f, 83 / 255f); // mountain
                    // color = Color.grey;
                } else if (height <= 1) {
                    color = Color.white; // ice/snow
                } else {
                    // impossibru?
                    color = Color.black;
                }
                colorMap[y * mapWidth + x] = color;
            }
        }
        return colorMap;
    }

}
