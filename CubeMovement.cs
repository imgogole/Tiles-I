using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class CubeMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    ClientManager clientManager;
    GameServerManager gameServerManager;
    GameSettings GS;
    AFKManager AFKmanager;
    GenerateMap GM;
    Power power;
    Rigidbody rb;
    Joystick joystick;
    public ExplosionCube explosionCube;
    public DeathManager deathManager;
    public float tumblingDuration = 0.2f;
    bool isGrounded;
    public bool isTumbling = false;
    public bool dead = false;
    public bool isGameStarted = false;
    public int colorOfBody;
    public bool canMove = true;
    public bool isGoingFar = false;
    public bool callDeathFunction = false;
    public bool isPoweredByTheWayAsTilesDontChange = false;
    public bool lavableTiles = false;
    GameObject[] players;
    public bool isRising = false;
    public float outOfControlTime;
    [HideInInspector]
    public int timeIDied = 0;
    [HideInInspector] public GameObject[] tiles;

    public int team;
    public int colorTeam;
    public string nameTeam;

    public float slowTime = 0;

    public float researchTime = 30;

    bool isGeant;

    private void Start()
    {
        clientManager = GameObject.Find("ClientManager").GetComponent<ClientManager>();
        power = clientManager.GetComponent<Power>();
        gameServerManager = GameObject.Find("GameServerManager").GetComponent<GameServerManager>();
        GS = GameObject.Find("UI/GameSettings").GetComponent<GameSettings>();
        GM = GameObject.Find("Map").GetComponent<GenerateMap>();
        AFKmanager = clientManager.gameObject.GetComponent<AFKManager>();
        rb = GetComponent<Rigidbody>();
        if (photonView.IsMine) photonView.RPC("NickName", RpcTarget.OthersBuffered ,PhotonNetwork.LocalPlayer.UserId);
        if (photonView.IsMine)
        {
            if (!PlayerPrefs.HasKey("color"))
            {
                int randomColor = Random.Range(0, clientManager.colorsAvailable.Length);
                PlayerPrefs.SetInt("color", randomColor);
                ChangeColorBody(randomColor);
            }
            else
            {
                int color = PlayerPrefs.GetInt("color");
                ChangeColorBody(color);
            }
            gameObject.AddComponent<AudioListener>();
        }
        players = GameObject.FindGameObjectsWithTag("Player");
        joystick = GameObject.Find("UI/Joystick").GetComponent<Joystick>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "floor")
        {
            if (!photonView.IsMine || gameServerManager.isGameEnded || dead) return;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] == collision.gameObject)
                {
                    TouchTile(colorOfBody, i, PhotonNetwork.LocalPlayer.UserId, isPoweredByTheWayAsTilesDontChange, lavableTiles, power.TimeLeft);
                    break;
                }
            }
        }
    }
    private void OnCollisionStay(Collision collision) { 
        if (collision.gameObject.tag == "floor") isGrounded = true;
        if (collision.gameObject.tag == "LobbyFloor") isGrounded = true;
    }
    private void OnCollisionExit(Collision collision) { 
        if (collision.gameObject.tag == "floor") isGrounded = false;
        if (collision.gameObject.tag == "LobbyFloor") isGrounded = false;
    }

    private void LateUpdate()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        ArrangeColor();
        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
        if (!gameServerManager.isInGame)
        {
            foreach (GameObject player in players)
            {
                Physics.IgnoreCollision(player.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
            }
            foreach (GameObject bot in bots)
            {
                Physics.IgnoreCollision(bot.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
            }
        }
        if (researchTime > 0 && gameServerManager.isInGame)
        {
            researchTime -= Time.deltaTime;
            SearchTiles();
        }
        if (!photonView.IsMine) return;
        if (GS.isGamePowered)
        {
            CheckSpeed();
        }
        if (outOfControlTime > 0)
        {
            outOfControlTime -= Time.deltaTime;
        }
        if (isRising)
        {
            print("rising");
            canMove = false;
            GetComponent<Rigidbody>().useGravity = false;
            transform.Translate(Vector3.up * 2f * Time.deltaTime, Space.World);
            return;
        }
        if (!canMove) GetComponent<Rigidbody>().useGravity = false;
        else GetComponent<Rigidbody>().useGravity = true;
        if (isGrounded && !dead && canMove && !GS.isGameSettingsOpened) Move();
        if (transform.position.y <= -2)
        {
            callDeathFunction = true;
        }
        if (callDeathFunction)
        {
            StopMoving();
            timeIDied++;
            if (gameServerManager.isInGame && !gameServerManager.isGameEnded)
            {
                deathManager.OnDead();
            }
            else gameObject.transform.position = new Vector3(0, 1, 0);
            callDeathFunction = false;
        }
        if (!gameServerManager.isInGame && gameServerManager.isGameEnded)
        {
            transform.localScale = Vector3.one;
            tumblingDuration = 0.2f;
        }

        if (!isTumbling)
        {
            transform.position = new Vector3((int)transform.position.x, transform.position.y, (int)transform.position.z);
        }
    }

    public void StopMoving()
    {
        StopAllCoroutines();
        isTumbling = false;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        gameObject.transform.position.Set((int)transform.position.x, 0.5f, (int)transform.position.z);
    }

    void Move()
    {
        if (clientManager.isSettingsOpened) return;

        var dir = Vector3.zero;

        if (outOfControlTime > 0)
        {
            int choice = Random.Range(0, 4);
            switch (choice)
            {
                case 0:
                    dir = Vector3.forward;
                    break;
                case 1:
                    dir = Vector3.back;
                    break;
                case 2:
                    dir = Vector3.left;
                    break;
                case 3:
                    dir = Vector3.right;
                    break;
            }
        }
        else
        {
            if (clientManager.isMobile)
            {
                if (joystick.Vertical >= 0.3f)
                    dir = Vector3.forward;

                else if (joystick.Vertical <= -0.3f)
                    dir = Vector3.back;

                else if (joystick.Horizontal >= 0.3f)
                    dir = Vector3.right;

                else if (joystick.Horizontal <= -0.3f)
                    dir = Vector3.left;
            }
            else
            {
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Z))
                    dir = Vector3.forward;

                if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                    dir = Vector3.back;

                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q))
                    dir = Vector3.left;

                if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                    dir = Vector3.right;
            }
        }

        if (dir != Vector3.zero && !isTumbling)
        {
            StartCoroutine(Tumble(dir));
            AFKmanager.ResetKey();
        }
    }

    IEnumerator Tumble(Vector3 direction)
    {
        isTumbling = true;
        float scale = transform.localScale.y / 2;

        var rotAxis = Vector3.Cross(Vector3.up, direction);
        var pivot = (transform.position + Vector3.down * scale) + direction * scale;

        var startRotation = transform.rotation;
        var endRotation = Quaternion.AngleAxis(90.0f, rotAxis) * startRotation;

        var startPosition = transform.position;
        var endPosition = transform.position + direction * (scale * 2);

        var rotSpeed = 90.0f / tumblingDuration;
        var t = 0.0f;

        while (t < tumblingDuration)
        {
            t += Time.deltaTime;
            if (t < tumblingDuration)
            {
                transform.RotateAround(pivot, rotAxis, rotSpeed * Time.deltaTime);
                yield return null;
            }
            else
            {
                transform.rotation = endRotation;
                transform.position = endPosition;
            }
        }

        transform.position = new Vector3((int)transform.position.x, transform.position.y, (int)transform.position.z);
        isTumbling = false;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale);
            stream.SendNext(rb.velocity);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            transform.localScale = (Vector3)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            rb.position += rb.velocity * lag;
        }
    }
    [PunRPC]
    void ChooseColor(int color)
    {
        colorOfBody = color;
    }

    public void TouchTile(int color, int index, string nameID, bool isTouchable, bool isLavable, float timeRemaining)
    {
        photonView.RPC("ChangeTile", RpcTarget.All, color, index, nameID, isTouchable, isLavable, timeRemaining);
    }

    [PunRPC]
    void ChangeTile(int color, /*int x,*/ int index, string nameID, bool isTouchable, bool isLavable, float timeRemaining)
    {
        ChangeColor tileCollision = tiles[index].GetComponent<ChangeColor>();
        
        if (tileCollision.isTouchable)
        {
            tileCollision.GetComponent<Renderer>().material.color = clientManager.colorsAvailable[color];
            tileCollision.colorOfThePlayerWhoTouchedThisTile = color;
            tileCollision.playerWhoTouchedThisTile = nameID;
            tileCollision.isTouched = true;
            if (!tileCollision.isSpawn)
            {
                if (isLavable)
                {
                    tileCollision.Lavable(timeRemaining);
                    tileCollision.GetComponentInChildren<Lava>().id = nameID;
                }
                if (isTouchable)
                {
                    tileCollision.isTouchable = false;
                    tileCollision.ChangeTouchable(timeRemaining);
                }
            }
        }
    }

    [PunRPC]
    void NickName(string name)
    {
        gameObject.name = "[Player] " + name;
    }

    public void ChangeColorBody(int color)
    {
        photonView.RPC("ChooseColor", RpcTarget.AllBuffered, color);
    }

    void ArrangeColor()
    {
        if (!isGameStarted)
        {
            GetComponent<Renderer>().material.color = clientManager.colorsAvailable[colorOfBody];
        }
        else
        {
            if (gameServerManager.isInGame && GS.hasTeams) colorOfBody = colorTeam;
            GetComponent<Renderer>().material.color = clientManager.colorsAvailable[colorOfBody];
        }

    }

    public void ChangeGeantState(bool isNewGeant)
    {
        if (isNewGeant)
        {
            StopMoving();
            isGeant = true;
            gameObject.transform.localScale = Vector3.one * 3;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1.5f, gameObject.transform.position.z);
        }
        else
        {
            StopMoving();
            isGeant = false;
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0.5f, gameObject.transform.position.z);
        }
    }

    void SearchTiles()
    {
        tiles = GameObject.FindGameObjectsWithTag("floor");
    }

    void CheckSpeed()
    {
        if (slowTime > 0)
        {
            slowTime -= Time.deltaTime;
        }
        if (!isTumbling)
        {
            if (isGoingFar)
            {
                if (slowTime > 0)
                {
                    tumblingDuration = 0.2f;
                }
                else
                {
                    tumblingDuration = 0.08f;
                }
            }
            else if (isGeant)
            {
                tumblingDuration = 0.4f;
            }
            else if (slowTime > 0)
            {
                tumblingDuration = 1f;
            }
            else
            {
                tumblingDuration = 0.2f;
            }
        }
    }
}

        
