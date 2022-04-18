using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class ResultGame : MonoBehaviourPun
{
    public GameObject[] tiles;
    public GameObject[] players;

    public GameObject gameOverText;
    public GameObject winnerText;

    public GameObject resultPrefab;
    public GameObject resultPanel;
    public GameObject background;
    public GameObject statsPanel;
    public GameSettings GS;
    public Power power;

    public TMP_Text position, details;

    public Dictionary<string, int> resultOfGame = new Dictionary<string, int>();
    public Dictionary<string, int> resultOfGamePercentage = new Dictionary<string, int>();
    public SortedDictionary<int, string> positions = new SortedDictionary<int, string>();

    public GameServerManager GSM;
    public TeamsChangeColor TCC;

    public GameObject cameraMain;

    public AudioSource audioSource;
    public AudioClip clip;

    public Animator anim;

    public bool canOpenChat = true;

    bool showResult;
    int myPosition;

    //My stats :
    int tilesITouched = 0;
    int PercentageIDid = 0;
    int totalNumberOfTiles = 0;
    int numberOfPowerIUsed = 0;
    int timeIDied = 0;

    private void Start()
    {
        showResult = false;
        winnerText.SetActive(false);
        statsPanel.SetActive(false);
    }

    public void CalculResult()
    {
        resultPanel.SetActive(true);
        canOpenChat = false;
        tiles = GameObject.FindGameObjectsWithTag("floor");
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            int numberOfTilesWhichThisPlayerTouched = 0;

            foreach (GameObject tile in tiles)
            {
                if (tile.GetComponent<ChangeColor>().playerWhoTouchedThisTile == player.GetComponent<PhotonView>().Owner.UserId)
                {
                    numberOfTilesWhichThisPlayerTouched++;
                }
            }

            float p = (float.Parse(numberOfTilesWhichThisPlayerTouched.ToString()) / float.Parse(tiles.Length.ToString())) * 100;

            photonView.RPC("CreateResult", RpcTarget.All, player.GetComponent<PhotonView>().Owner.NickName, numberOfTilesWhichThisPlayerTouched, Mathf.RoundToInt(p));
            print("tiles touched " + numberOfTilesWhichThisPlayerTouched);
            print("Number of tiles : " + tiles.Length);
        }

        photonView.RPC("seeLeader", RpcTarget.All);
        
    }

    [PunRPC]
    public void CreateResult(string owner, int numberOfTiles, int percent)
    {
        resultOfGame.Add(owner, numberOfTiles);
        positions.Add(numberOfTiles, owner);
        resultOfGamePercentage.Add(owner, percent);
        print("Added " + owner + " in the list. Tiles : " + numberOfTiles + ". Percentage : " + percent + "%");

        if (owner == PhotonNetwork.LocalPlayer.NickName)
        {
            tilesITouched = numberOfTiles;
            PercentageIDid = percent;
            totalNumberOfTiles = GameObject.FindGameObjectsWithTag("floor").Length;
            numberOfPowerIUsed = power.NumberOfPowersIUsed;
            timeIDied = GameObject.Find("Mine").GetComponent<CubeMovement>().timeIDied;
        }
    }

    [PunRPC]
    public void seeLeader()
    {
        resultPanel.SetActive(true);
        foreach (string player in resultOfGamePercentage.Keys)
        {
            int color = 0;
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (go.GetComponent<PhotonView>().Owner.NickName == player)
                {
                    color = go.GetComponent<CubeMovement>().colorOfBody;
                }
            }
            var result = Instantiate(resultPrefab, resultPanel.transform).GetComponent<ShowResult>();
            result.Show(player, resultOfGamePercentage[player], color);
        }
        StartCoroutine("SeeWinners");
    }

    IEnumerator SeeWinners()
    {
        yield return new WaitForSeconds(8.5f);
        int highPercent = 0;
        string winner = "";
        foreach (string player in resultOfGamePercentage.Keys)
        {
            if (highPercent < resultOfGamePercentage[player])
            {
                highPercent = resultOfGamePercentage[player];
                winner = player;
            }
        }
        foreach (Transform trans in resultPanel.transform)
        {
            Destroy(trans.gameObject);
        }
        CalculPosition();
        MakeStats();
        winnerText.SetActive(true);
        winnerText.GetComponent<TMP_Text>().text = winner;
        gameOverText.GetComponent<TMP_Text>().text = "Winner";
        background.SetActive(false);
        audioSource.clip = clip;
        audioSource.Play();
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().Owner.NickName == winner)
            {
                player.GetComponent<CubeMovement>().canMove = true;
                cameraMain.GetComponent<CameraFollow>().target = player.transform;
            }
            else
            {
                player.GetComponent<CubeMovement>().canMove = false;
            }
        }
        canOpenChat = true;
        Invoke("Calcul", 2f);
    }

    void CalculPosition()
    {
        int index = PhotonNetwork.CurrentRoom.PlayerCount + 1;
        foreach (string id in positions.Values)
        {
            print($"Searching position of {id}");
            index--;
            if (id == PhotonNetwork.LocalPlayer.NickName)
            {
                myPosition = index;
                print($"{id} is {myPosition}th");
                break;
            }
        }

        if (myPosition == 1)
        {
            position.text = "<size=18><color=white>Your position :</color></size>\n1st";
            position.color = Color.yellow;
        }
        else if (myPosition == 2)
        {
            position.text = "<size=18><color=white>Your position :</color></size>\n2nd";
            position.color = Color.gray;
        }
        else if (myPosition == 3)
        {
            position.text = "<size=18><color=white>Your position :</color></size>\n3rd";
            position.color = Color.gray;
        }
        else
        {
            position.text = $"<size=18><color=white>Your position :</color></size>\n{myPosition}th";
            position.color = Color.white;
        }

    }

    void MakeStats()
    {
        string result = "";
        result += $"{PhotonNetwork.LocalPlayer.NickName} :\n\n";
        result += $"    Number of tiles touched : {tilesITouched}/{totalNumberOfTiles}\n";
        result += $"    Percentage : {PercentageIDid}%\n";
        if (GS.isGamePowered)
        {
            result += $"    Powers used : {numberOfPowerIUsed}\n";
        }
        result += $"    Time of dead : {timeIDied} times\n";
        details.text = result;
    }

    void Calcul()
    {
        showResult = true;
    }

    private void Update()
    {
        if (showResult)
        {
            if (!anim.GetBool("end"))
            {
                anim.SetBool("end", true);
                statsPanel.SetActive(true);
            }
        }
    }
}
