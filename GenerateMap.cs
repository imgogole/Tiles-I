using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GenerateMap : MonoBehaviourPun
{
    public GameObject tilePrefab, customMap;
    public GameObject[] MapsPrefab;
    public GameSettings GS;
    public int height = 15;
    public int width = 15;
    public bool randomHoles = false;
    public bool checkAloneTiles = false;
    public bool isMapOptimized;
    public int mapInt;
    public string mapName;
    [Range(0, 100)]
    public int holesPercentage;
    public GameServerManager GSM;

    private void Start()
    {
        foreach (GameObject map in MapsPrefab)
        {
            map.SetActive(false);
        }
    }

    public void GeneratingMap()
    {
        height = GS.mapHeight;
        width = GS.mapWedth;
        randomHoles = GS.isMapHoled;
        isMapOptimized = GS.isMapOptimized;
        mapInt = GS.mapOptimized;
        foreach (Transform tile in transform)
        {
            Destroy(tile.gameObject);
        }
        if (isMapOptimized)
        {
            photonView.RPC("EnabledOptimizedMap", RpcTarget.All, mapInt);
        }
        else {
            StartCoroutine(CreateMap(height, width));
            /*
            int startingHeight = 0 - Mathf.FloorToInt(height / 2);
            int startingWidth = 0 - Mathf.FloorToInt(width / 2);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bool isSpawn = startingHeight + i == 0 && startingWidth + j == 0;
                    if (!randomHoles)
                    {
                        photonView.RPC("CreateTile", RpcTarget.AllBuffered, startingHeight, startingWidth, i, j, isSpawn);
                    }
                    else
                    {
                        var random = Random.Range(0, 100);
                        
                        if (random <= 100 - holesPercentage || isSpawn)
                        {
                            photonView.RPC("CreateTile", RpcTarget.AllBuffered, startingHeight, startingWidth, i, j, isSpawn);
                        }
                    }
                }
            }
            photonView.RPC("EveryoneReady", RpcTarget.All);
            */
        }
    }

    [PunRPC]
    public void CreateTile(int startingHeight, int startingWidth, int i, int j, bool isSpawn)
    {
        int x = startingHeight + i;
        int y = startingWidth + j;
        var tile = GameObject.Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
        tile.transform.SetParent(gameObject.transform);
        tile.name = $"[Tile] Position : {x}, {y}";
        tile.tag = "floor";
        tile.GetComponent<ChangeColor>().positionX = x;
        tile.GetComponent<ChangeColor>().positionY = y;
        tile.GetComponent<ChangeColor>().isSpawn = isSpawn;
    }

    [PunRPC]
    void EveryoneReady()
    {
        GSM.OnPlayerReady();
    }

    [PunRPC]
    public void EnabledOptimizedMap(int map)
    {
        MapsPrefab[map].SetActive(true);
        mapName = MapsPrefab[map].name;
        GSM.OnPlayerReady();
    }

    IEnumerator CreateMap(int height, int width)
    {
        int startingHeight = 0 - Mathf.FloorToInt(height / 2);
        int startingWidth = 0 - Mathf.FloorToInt(width / 2);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                bool isSpawn = startingHeight + i == 0 && startingWidth + j == 0;
                if (!randomHoles)
                {
                    photonView.RPC("CreateTile", RpcTarget.AllBuffered, startingHeight, startingWidth, i, j, isSpawn);
                }
                else
                {
                    var random = Random.Range(0, 100);

                    if (random <= 100 - holesPercentage || isSpawn)
                    {
                        photonView.RPC("CreateTile", RpcTarget.AllBuffered, startingHeight, startingWidth, i, j, isSpawn);
                    }
                }
            }
        }
        photonView.RPC("EveryoneReady", RpcTarget.All);
        yield return null;
    }
}
