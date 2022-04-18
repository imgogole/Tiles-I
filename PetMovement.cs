using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PetMovement : MonoBehaviour, IPunObservable
{

    public float tumblingDuration = 0.2f;
    bool isTumbling = false;
    bool hasStarted = false;
    float timeToLive = 15;
    Vector3 lastDirection;
    GameObject[] tiles;
    GameObject myOwner;
    GameServerManager GSM;
    PhotonView PV;
    bool isGrounded;
    int color;
    ClientManager CM;
    Rigidbody rb;

    private void Awake()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Physics.IgnoreCollision(player.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        tiles = GameObject.FindGameObjectsWithTag("floor");
        CM = GameObject.Find("ClientManager").GetComponent<ClientManager>();
        GSM = GameObject.Find("GameServerManager").GetComponent<GameServerManager>();
        if (PV.Owner.UserId != GameObject.Find("Mine").GetComponent<PhotonView>().Owner.UserId)
        {
            Destroy(rb);
        }
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().Owner.UserId == GetComponent<PhotonView>().Owner.UserId)
            {
                myOwner = player;
                break;
            }
        }
        color = myOwner.GetComponent<CubeMovement>().colorOfBody;
        GetComponent<Renderer>().material.color = CM.colorsAvailable[color];
        Move();
        hasStarted = true;
    }
    private void Update()
    {
        tiles = GameObject.FindGameObjectsWithTag("floor");
        timeToLive -= Time.deltaTime;
        if (GSM.isGameEnded || transform.position.y < -2 || timeToLive < 0)
        {
            Destroy(gameObject);
        }
        if (!isTumbling && isGrounded)
        {
            Move();
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Physics.IgnoreCollision(player.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        }
        GameObject[] bots = GameObject.FindGameObjectsWithTag("Bots");
        foreach (GameObject bot in bots)
        {
            Physics.IgnoreCollision(bot.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("lava")) Destroy(gameObject);
        else 
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == collision.gameObject)
            {
                Send(color, i, PV.Owner.UserId);
                break;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "floor") isGrounded = true;
        if (collision.gameObject.tag == "LobbyFloor") isGrounded = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "floor") isGrounded = false;
        if (collision.gameObject.tag == "LobbyFloor") isGrounded = false;
    }

    void Move()
    {
        var dir = Vector3.zero;

        if (!hasStarted)
        {
            dir = ChooseDirection();
            Debug.Log("The bot didn't initialized, choose random location to go.");
            hasStarted = true;
        }
        else
        {
            dir = NextDirectionDecision();
            Debug.Log("The bot was already initialized, so he has 83% to continue in this way.");
        }

        lastDirection = dir;

        if (dir != Vector3.zero && !isTumbling)
        {
            StartCoroutine(Tumble(dir));
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
        yield return new WaitForSeconds(0.05f);
        isTumbling = false;
    }

    Vector3 ChooseDirection(float probLeft = 1, float probRight = 1, float probUp = 1, float probDown = 1)
    {
        Vector3[] vectors = { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        float total = probLeft + probDown + probRight + probUp;
        float[] allProb = { probLeft, probRight, probUp, probDown };
        float random = Random.Range(0, total);
        float sum = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += allProb[i];
            if (random <= sum)
            {
                return vectors[i];
            }
        }
        return vectors[0];
    }

    Vector3 NextDirectionDecision()
    {
        if (lastDirection == Vector3.left)
        {
            return ChooseDirection(10, 1, 2, 2);
        }
        else if(lastDirection == Vector3.right)
        {
            return ChooseDirection(1, 10, 2, 2);
        }
        else if (lastDirection == Vector3.forward)
        {
            return ChooseDirection(2, 2, 10, 1);
        }
        else
        {
            return ChooseDirection(2, 2, 1, 10);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    void Send(int color, int index, string nameID)
    {
        PV.RPC("TilesTouched", RpcTarget.All, color, index, nameID);
    }

    [PunRPC]

    void TilesTouched(int color, int index, string nameID)
    {
        ChangeColor tileCollision = tiles[index].GetComponent<ChangeColor>();

        if (tileCollision.isTouchable)
        {
            tileCollision.GetComponent<Renderer>().material.color = CM.colorsAvailable[color];
            tileCollision.colorOfThePlayerWhoTouchedThisTile = color;
            tileCollision.playerWhoTouchedThisTile = nameID;
            tileCollision.isTouched = true;
        }
    }

}
