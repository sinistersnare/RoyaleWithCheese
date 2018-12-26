using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelCreator : MonoBehaviour {
	[Header("Set as -1 for a random seed to be chosen.")]
	public int generatorSeed = -1;
	public int bluePlayerSeed = -1;
	public int redPlayerSeed = -1;
    public int itemSeed = -1;
    private System.Random itemRng;
    private System.Random blueRng;
    private System.Random redRng;
    private System.Random rngRng = new System.Random();

    [Range(10, 255)]
    public int mapSize = 150;
	private int mapWidth;
	private int mapHeight;

    public int scoreWinThreshold = 15;
    public int startingItemsInMap = 15;

    public bool twoPlayerMode;

    public UnityEngine.UI.Text unsupportedText;

    /**
     * If you set the detail to 1, the highest resolution, then it will take
     * About a minute to run, and possible a lot of RAM (maybe a gig or two?).
     * The result is a very nice nav mesh, and the player will traverse all
     * traversable terrain. Lowering the resolution will greatly speed up
     * load-times, but will hinder navigation.
    */
    [Header("1 for highest resolution. See source for details.")]
    [Range(1, 4)]
    public int navMeshDetail = 1;

	public float heightMultiplier;
	public AnimationCurve heightCurve;

	public GameObject bluePlayerPrefab;
	public GameObject redPlayerPrefab;
    public GameObject pickupPrefab;

	public Material waterMaterial;

	private float[,] heightMap;
	private float xDiff = -36.5f;
	private float yDiff = -37.5f;

	[System.Serializable]
	public struct PickupInfo {
        public PickupType type;
		public Color pickupColor;
		public GameObject pickupPrefab;
	}
	public PickupInfo[] pickupOptions;

    private List<Vector3> possibleItemSpawns;
    private List<Vector3> possiblePlayerSpawns;

	private void Update() {
        // FIXME: can spawn items on top of eachother, and too close.
        // Maintain a set of currently spawned in items, and make sure that we space them out a little bit
        // when we add new items.
        // PROBLEM: when we do this, how will we know when one is deleted from the map, and then update it in the list?
        // Once we figure that out, this will be a great addition.
        if (this.twoPlayerMode && Random.value < 0.008f) {
            this.PlacePickups(1, -1);
        }

        if (this.twoPlayerMode && XboxCtrlrInput.XCI.GetNumPluggedCtrlrs() < 2) {
            this.unsupportedText.text = "NEED 2 CONTROLLERS FOR 2 PLAYER MODE\nLIMITED FUNCTIONALITY IMPLEMENTED";
        } else if (!this.twoPlayerMode  && XboxCtrlrInput.XCI.GetNumPluggedCtrlrs() == 0) {
            this.unsupportedText.text = "NEED A CONTROLLER FOR 1 PLAYER MODE\nLIMITED FUNCTIONALTY IMPLEMENTED";
        } else {
            this.unsupportedText.text = "";
        }
        Player blue = PlayerManager.instance.bluePlayer;
        Player red = PlayerManager.instance.redPlayer;
        if (blue.Health <= Mathf.Epsilon) {
            int toRedis = blue.Score / 2;
            this.PlacePickups(toRedis, this.pickupOptions.Length - 1);
            this.PlaceBluePlayer();
            PlayerManager.instance.bluePlayer.OnDied();
        }
        if (this.twoPlayerMode && PlayerManager.instance.redPlayer.Health <= Mathf.Epsilon) {
            int toRedis = red.Score / 2;
            this.PlacePickups(toRedis, this.pickupOptions.Length - 1);
            this.PlaceRedPlayer();
            PlayerManager.instance.redPlayer.OnDied();
        }

    }

    private void Start() {

        if (this.redPlayerSeed == -1) {
            this.redRng = new System.Random(this.rngRng.Next());
        } else {
            this.redRng = new System.Random(this.bluePlayerSeed);
        }
        if (this.bluePlayerSeed == -1) {
            this.blueRng = new System.Random(this.rngRng.Next());
        } else {
            this.blueRng = new System.Random(this.bluePlayerSeed);
        }
        if (this.itemSeed == -1) {
            this.rngRng.Next();
            this.itemRng = new System.Random(this.rngRng.Next());
        } else {
            this.itemRng = new System.Random(this.itemSeed);
        }
        PlayerManager.instance.scoreWinThreshold = this.scoreWinThreshold;
        PlayerManager.instance.twoPlayerMode = this.twoPlayerMode;
        this.mapWidth = this.mapHeight = this.mapSize;
        this.yDiff = -(this.mapWidth / 2f);
        this.xDiff = yDiff + 1f;
		this.CreateLevel();
        this.possiblePlayerSpawns = this.AllPointsWithinRange(12, 0.2f, 0.6f);
        this.possibleItemSpawns = this.AllPointsWithinRange(5, 0.2f, 0.8f);
        
        this.PlaceWater();
        this.CreateNavMesh();
        if (this.twoPlayerMode) this.PlaceRedPlayer();
        this.PlaceBluePlayer();
        this.PlacePickups(this.startingItemsInMap, -1);
	}


	private void PlaceWater() {
        Transform waterBoxHolder = this.transform.Find("WaterBoxHolder");
		for (int y = 0; y < this.heightMap.GetLength(1); y++) {
			for (int x = 0; x < this.heightMap.GetLength(0); x++) {
				if (heightMap[x, y] < 0.2) {
					GameObject construct = GameObject.CreatePrimitive(PrimitiveType.Cube);
					construct.name = "waterbox";
                    construct.transform.parent = waterBoxHolder;
                    construct.transform.position = new Vector3(x + this.xDiff, -1, -y + this.yDiff);
					construct.transform.localScale = new Vector3(1, 2f, 1);
					construct.GetComponent<BoxCollider>().enabled = false;
					MeshRenderer constructRenderer = construct.GetComponent<MeshRenderer>();
					constructRenderer.material = this.waterMaterial;
					constructRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					constructRenderer.receiveShadows = false;
					construct.gameObject.AddComponent<NavMeshObstacle>().carving = true;
				}
			}
		}
	}
    

    
    private void PlacePickups(int itemsToPlace, int whichKind) {
        // TODO: make this set of spawns an instance property (see FIXME in Update())
        HashSet<int> spawns = new HashSet<int>();
        for (int i=0; i < itemsToPlace; i++) {
            int listPosition = -1;
            do {
                listPosition = this.itemRng.Next(possibleItemSpawns.Count);
            } while (spawns.Contains(listPosition));
            Vector3 itemPosition = this.possibleItemSpawns[listPosition];
            spawns.Add(listPosition);
            int pickupKind= whichKind == -1 ? this.itemRng.Next(this.pickupOptions.Length) : whichKind;
            this.PlaceSinglePickUp(itemPosition, pickupKind, this.transform.Find("PickupHolder"));
        }   
    }

    private void PlaceSinglePickUp(Vector3 itemPosition, int whichType, Transform parent) {
        PickupInfo pickupInfo = this.pickupOptions[whichType];
        GameObject pickup = Object.Instantiate(this.pickupPrefab, itemPosition, Quaternion.identity, parent);
        pickup.layer = LayerMask.NameToLayer("Pickup");
        MeshRenderer rend = pickup.GetComponent<MeshRenderer>();
        rend.material.color = pickupInfo.pickupColor;
        PickupController holder = pickup.GetComponent<PickupController>();
        holder.pickupPrefab = pickupInfo.pickupPrefab;
        holder.type = pickupInfo.type;
    }

    private void PlaceBluePlayer() {
        Player blue = PlayerManager.instance.bluePlayer;
        if (blue == null) {
            int pos = this.blueRng.Next(this.possiblePlayerSpawns.Count);
            Vector3 which = this.possiblePlayerSpawns[pos];
            GameObject bluePlayer = Instantiate(bluePlayerPrefab, which, Quaternion.identity);
            bluePlayer.name = "Blue Player";
            PlayerManager.instance.bluePlayer = bluePlayer.GetComponent<Player>();
        } else {
            blue.transform.position = this.possiblePlayerSpawns[this.blueRng.Next(this.possiblePlayerSpawns.Count)];
        }
	}
    
	private void PlaceRedPlayer() {
        Player red = PlayerManager.instance.redPlayer;
        if (red == null) {
            List<Vector3> possibleSpawns = this.possiblePlayerSpawns;
            int pos = this.redRng.Next(possibleSpawns.Count);
            Vector3 which = possibleSpawns[pos];
            GameObject redPlayer = Instantiate(this.redPlayerPrefab, which, Quaternion.identity);
            redPlayer.name = "Red Player";
            PlayerManager.instance.redPlayer = redPlayer.GetComponent<Player>();
        } else {
            red.transform.position = this.possiblePlayerSpawns[this.redRng.Next(this.possiblePlayerSpawns.Count)];
        }
	}


    /// <summary>
    /// Returns all points on the heightMap that are within range [min,max).
    /// </summary>
    /// <param name="start">How far into the map to start.</param>
    /// <param name="min">INCLUSIVE</param>
    /// <param name="max">EXCLUSIVE</param>
    /// <returns>List&lt;Vector2&gt; of the points.</returns>
    private List<Vector3> AllPointsWithinRange(int start, float min, float max) {
        List<Vector3> points = new List<Vector3>(100);
        for (int y = start; y < this.mapHeight; y++) {
            for (int x = start; x < this.mapWidth; x++) {
                float posHeight = this.heightMap[x, y];
                if (posHeight >= min && posHeight < max) {
                    points.Add(new Vector3(x + this.xDiff, posHeight + 2,-y + this.yDiff));
                }
            }
        }
        return points;
    }

    private void CreateLevel() {
        int seed = this.generatorSeed;
        if (this.generatorSeed == -1) {
            seed = this.rngRng.Next();
        }
		this.heightMap = MapGenerator.SimpleHeightMapGenerator(this.mapWidth, this.mapHeight, seed);
		Color[] colorMap = MapGenerator.GenerateColorMap(this.heightMap, this.mapWidth, this.mapHeight);
		Texture2D colorMapTexture = this.TextureFromColorMap(colorMap);
		Mesh levelMesh = new MeshGenerator(this.heightMap, this.heightMultiplier, this.heightCurve).GenerateMesh();
		this.gameObject.tag = "Ground";
		this.GetComponent<MeshFilter>().mesh = levelMesh;
		this.GetComponent<MeshRenderer>().material.mainTexture = colorMapTexture;
		this.GetComponent<MeshCollider>().sharedMesh = levelMesh;
    }

	private void CreateNavMesh() {
        // NavMeshComponents to the rescue! Dynamic NavMeshes!
        NavMeshSurface surface = this.GetComponent<NavMeshSurface>();
        surface.overrideVoxelSize = true;
        // Unity said this was a good number. 8 voxels per agent-radius
        surface.voxelSize = 0.0625f;
        // Set the mesh layer, not the NavMeshSurface?
        // surface.layerMask = LayerMask.GetMask("Ground");
        surface.BuildNavMesh();
	}

       
	private Texture2D TextureFromColorMap(Color[] colorMap) {
		Texture2D texture = new Texture2D(mapWidth, mapHeight) {
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
		texture.SetPixels(colorMap);
		texture.Apply();
		return texture;
	}
}
