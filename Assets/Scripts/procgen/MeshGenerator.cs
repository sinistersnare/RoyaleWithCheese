using UnityEngine;

public class MeshGenerator {
    private readonly Vector3[] vertices;
    private readonly int[] triangles;
    private readonly Vector2[] uvs;
    private readonly int triangleIndex;

    public float minValue;

    public MeshGenerator(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float xTopLeft = (width - 1) / -2f;
        float yTopLeft = (height - 1) / -2f;
        vertices = new Vector3[width * height];
        uvs = new Vector2[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];

        int vertexIndex = 0;
        float minVal = 30;
        float maxVal = 0;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float curved = heightCurve.Evaluate(heightMap[x, y]);
                vertices[vertexIndex] = new Vector3(xTopLeft + x, curved * heightMultiplier, yTopLeft - y);
                if (curved < minVal) {
                    minVal = curved;
                }
                if (curved > maxVal) {
                    maxVal = curved;
                }
                uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width-1 && y < height-1 ) { // cant add triangles to edges!
                    // Add 2 triangles for mesh
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + width + 1;
                    triangles[triangleIndex + 2] = vertexIndex + width;
                    triangleIndex += 3; // to signify a new triangle!
                    triangles[triangleIndex] = vertexIndex + width + 1;
                    triangles[triangleIndex + 1] = vertexIndex;
                    triangles[triangleIndex  + 2] = vertexIndex + 1;
                    triangleIndex += 3;
                }
                vertexIndex++;
            }
        }
        this.minValue = minVal;
    }

    public Mesh GenerateMesh() {
        Mesh mesh = new Mesh {
            vertices = this.vertices,
            triangles = this.triangles,
            uv = this.uvs
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}
