using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour {

    public Texture2D maze;
    private static Color[] tiles;
    private static Tile[] ground;
    private static Tile[] wall;

    private static int mazeWidth = 64;
    private static int mazeHeight = 64;
    public static Player player;
    public static Monster monsta;
    public static Stash stash;

    static Power tracePower;
    static Power digPower;
    static Power hidePower;

    static readonly Color activeColor = new Color(0.7f,0.7f,0.7f,1);
    static readonly Color inActiveColor = new Color(0.2f, 0.2f, 0.2f, 1);
    static Text traceText;
    static Text digText;
    static Text hideText;

    private static Vector2 origin;
    private static bool gotStash = false;

    private float targetBite;

    static System.Random random = new System.Random();

    // Use this for initialization
    private void Awake()
    {
    }
    

	void Init () {
        tiles = this.maze.GetPixels();
        GenerateGround();
        player = SpawnPlayer();
        Power[] powers = player.gameObject.GetComponents<Power>();
        tracePower = powers[0];
        tracePower.PowerAvailableCallback = () =>
        {
            if (traceText == null)
                traceText = GameObject.Find("Trace").GetComponent<Text>();
            traceText.color = activeColor;
        };
        digPower = powers[1];
        digPower.PowerAvailableCallback = () =>
        {
            if (digText == null)
                digText = GameObject.Find("Dig").GetComponent<Text>();
            digText.color = activeColor;
        };
        hidePower = powers[2];
        hidePower.PowerAvailableCallback = () =>
        {
            if (hideText == null)
                hideText = GameObject.Find("Hide").GetComponent<Text>();
            hideText.color = activeColor;
        };

        CameraController cameraControls = Camera.main.gameObject.AddComponent<CameraController>();
        cameraControls.target = player.transform;
        monsta = SpawnMonster();
        monsta.Roaming = true;
        stash = SpawnStash();
    }

    private void Start()
    {
        Init();
    }

	// Update is called once per frame
	void Update () {
        if(!gotStash && player.TilePosition.x == stash.TilePosition.x && player.TilePosition.y == stash.TilePosition.y)
        {
            gotStash = true;
            Destroy(stash.gameObject);
            TrySniff();
            Instantiate(Resources.Load("Prefabs/Exit"), ground[(int)(origin.x + origin.y * mazeWidth)].transform.position, Quaternion.identity);
        }
        if(gotStash && player.TilePosition.x == origin.x && player.TilePosition.y == origin.y)
        {
            Debug.Log("WON");
        }
        float dist = Vector3.Distance(player.transform.position, monsta.transform.position);
        targetBite = dist;
        float currentBite = targetBite / 50f;
        if(targetBite < 20)
        {
            float oldVal = Camera.main.GetComponent<Dark>().bite;
            currentBite = Mathf.Lerp(oldVal, currentBite, Time.deltaTime*2f);
        }
        Camera.main.GetComponent<Dark>().bite = currentBite;
        Debug.Log("Distance: " + (dist / 10f).ToString());
        Debug.Log("Bite: " + currentBite.ToString());
    }

    public void GenerateGround()
    {
        Generator.ground = new Tile[mazeHeight * mazeHeight];
        Generator.wall = new Tile[mazeHeight * mazeHeight];

        float deltaX = 0;
        float deltaY = 0;
        GameObject ground = Resources.Load("Prefabs/GroundTile") as GameObject;
        GameObject rock = Resources.Load("Prefabs/RockTile") as GameObject;
        for (int i = 0; i < this.maze.height; i++)
        {
            for(int j = 0; j < this.maze.width; j++)
            {
                GameObject tile = GameObject.Instantiate(ground);
                tile.transform.position = new Vector3(deltaX, deltaY, 0f);
                int index = (int)(j + this.maze.width * i);
                Generator.ground[index] = tile.GetComponent<Tile>();
                if (tiles[index].r == 0)
                {
                    GameObject obstacle = GameObject.Instantiate(rock);
                    obstacle.transform.position = new Vector3(deltaX, deltaY, 0f);
                    Generator.wall[index] = obstacle.GetComponent<Tile>();
                    if (Random.Range(0f, 1f) > 0.5f)
                        obstacle.GetComponent<SpriteRenderer>().flipX = true;
                }
                deltaX += 1.28f;
            }
            deltaY += 1.28f;
            deltaX = 0;
        }
    }

    public Player SpawnPlayer()
    {
        GameObject playerPrefab = Resources.Load("Prefabs/Player") as GameObject;
        while (true)
        {
            int randomX = (int)Random.Range(0, 64);
            int randomY = (int)Random.Range(0, 64);

            Vector2 randomPlayerPoint_n = new Vector2(randomX / 64f, randomY / 64f);
            Vector2 randomPlayerPoint = new Vector2(randomPlayerPoint_n.x * this.maze.width, randomPlayerPoint_n.y * this.maze.height);
            int index = (int)Mathf.Round(randomPlayerPoint.x + this.maze.width * randomPlayerPoint.y);
            Debug.Log(index);
            if(tiles[index].r == 1f)
            {
                origin = new Vector2(randomX, randomY);
                GameObject player = Instantiate(playerPrefab);
                player.GetComponent<Player>().TilePosition = new Vector2(randomX, randomY);
                player.transform.position = new Vector3(randomPlayerPoint.x * 1.28f, randomPlayerPoint.y * 1.28f, 0f);
                return player.GetComponent<Player>();
            }
        }
    }

    public Monster SpawnMonster()
    {
        GameObject monsterPrefab = Resources.Load("Prefabs/Monsta") as GameObject;
        int monsterX = (int)(64 - player.TilePosition.x + 2);
        int monsterY = (int)(64 - player.TilePosition.y + 2);
        GameObject monsta = Instantiate(monsterPrefab);
        monsta.GetComponent<Monster>().TilePosition = new Vector2(monsterX, monsterY);
        monsta.transform.position = new Vector3(monsterX * 1.28f, monsterY * 1.28f, 0f);
        return monsta.GetComponent<Monster>();
    }

    public Stash SpawnStash()
    {
        GameObject stashPrefab = Resources.Load("Prefabs/Stash") as GameObject;
        int xSearch = 1;
        int ySearch = 1;
        if (Generator.monsta.TilePosition.x > 32)
            xSearch = -1;
        if (Generator.monsta.TilePosition.y > 32)
            ySearch = -1;

        Vector2 searchVector = new Vector2(xSearch, ySearch);
        Vector2 startVector = Generator.monsta.TilePosition;
        while (true)
        {
            startVector += searchVector;
            int index = (int)Mathf.Round(startVector.x + this.maze.width * startVector.y);
            if (tiles[index].r == 1f)
            {
                GameObject stash = Instantiate(stashPrefab);
                stash.GetComponent<Stash>().TilePosition = new Vector2(startVector.x, startVector.y);
                stash.transform.position = new Vector3(startVector.x * 1.28f, startVector.y * 1.28f, 0f);
                return stash.GetComponent<Stash>();
            }
        }
        return null;
    }

    public static bool CanMove(int x, int y, Vector2 direction)
    {
        var newPos = new Vector2(x + direction.x, y + direction.y);
        int index = (int)Mathf.Round(newPos.x + mazeWidth * newPos.y);
        if (wall[index] == null || wall[index].ShallPass)
            return true;
        return false;
    }

    public static void TryDig()
    {
        if (digPower.Use())
        {
            Vector2 targetTile = player.TilePosition + player.lookDir;
            int index = (int)Mathf.Round(targetTile.x + mazeWidth * targetTile.y);
            if (wall[index] != null)
            {
                player.Busy = true;
                wall[index].TearDown(() =>
                {
                    player.Busy = false;
                });
                digText.color = inActiveColor;
            }
        }
    }

    public static void TrySniff()
    {
        if (tracePower.Use())
        {
            Vector3 target = gotStash ? ground[(int)(origin.x + origin.y * mazeWidth)].transform.position : stash.transform.position;
            Vector3 dir = target - player.transform.position;
            dir.Normalize();
            player.SetArrow(dir);
            traceText.color = inActiveColor;
        }
    }

    public static Vector3 GetDirection()
    {
        float[] dirProbability = new float[] { 0.25f, 0.25f, 0.25f, 0.25f };
        Vector2 randomDir = Vector2.zero;

        double _random = random.NextDouble();
        if (_random > dirProbability[0])
            randomDir += Vector2.up;
        _random = random.NextDouble();
        if (_random > dirProbability[1])
            randomDir += Vector2.left;
        _random = random.NextDouble();
        if (_random > dirProbability[2])
            randomDir += Vector2.down;
        _random = random.NextDouble();
        if (_random > dirProbability[3])
            randomDir += Vector2.right;

        if(randomDir == Vector2.zero)
        {
            if (player.transform.position.x <= monsta.transform.position.x)
                randomDir.x = -1;
            else
                randomDir.x = 1;
            if (player.transform.position.y <= monsta.transform.position.y)
                randomDir.y = -1;
            else
                randomDir.y = 1;
        }
        return randomDir;
    }
}
