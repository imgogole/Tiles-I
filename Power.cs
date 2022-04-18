using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class Power : MonoBehaviourPun
{
    public GameObject PowerPanel, ZawarudoEffectGameObject, Mine, Chat, PaintBombe, CylinderOfIsMine;

    public int TimeBetweenPowers = 10;
    public int TimeDuringPowers = 10;

    public TMP_Text PowerInformation;
    public Image PowerMiddlegroundImage;
    public Image[] PowerImages;
    //public Button PowerButton;
    public string[] PowerType;
    public int[] PowerDuration;
    public int[] PowerProbabilities;

    public GameServerManager GSM;
    public ClientManager CM;
    public GameSettings GS;
    public AudioSource music;

    public GameObject[] Effects;

    public AudioSource ZaWarudoStop, ZaWarudoResume, Klaxon, NaniSound, Chaos;

    public bool TryOnlyToTest;
    public int PowerToTest;

    public int PowerChoosenIndicator;
    string PowerTypeChoosen;
    Image PowerImageChoosen;
    [HideInInspector]
    public float TimeLeft;
    public float blindness = 0;
    public GameObject blindnessGameObject;

    GameObject Player;

    [HideInInspector]
    public int NumberOfPowersIUsed = 0;

    [HideInInspector] public bool isUsingPower = false, isWaitingPower = false, canUsePower = false;

    private void Start()
    {
        PowerChoosenIndicator = 0;
        PowerMiddlegroundImage.fillAmount = 0;
        isWaitingPower = true;
        TimeLeft = TimeBetweenPowers;
        PowerInformation.text = $"{TimeBetweenPowers}";
        foreach (Image image in PowerImages)
        {
            image.gameObject.SetActive(false);
        }
        foreach (GameObject effect in Effects)
        {
            effect.SetActive(false);
        }
    }

    private void Update()
    {
        if (GS.isGamePowered)
        {
            Player = GameObject.Find("Mine");
            if (GSM.isInGame && !GSM.isGameEnded && GSM.isEveryoneReady)
            {
                PowerPanel.SetActive(true);
                //PowerButton.interactable = !CM.isSettingsOpened && canUsePower && !isWaitingPower && !isUsingPower;
                if (isWaitingPower)
                {
                    //Debug.Log($"Time left : {TimeLeft}");
                    TimeLeft -= Time.deltaTime;
                    PowerInformation.text = $"{Mathf.CeilToInt(TimeLeft)}";
                    PowerMiddlegroundImage.fillAmount = 1 - TimeLeft / TimeBetweenPowers;
                    if (TimeLeft <= 0)
                    {
                        ChoosePower();
                    }
                }
                else if (canUsePower)
                {
                    if (Input.GetKeyDown(KeyCode.Space) && !CM.isSettingsOpened && !Player.GetComponent<CubeMovement>().dead && !GSM.isTimeStopped && !Chat.activeSelf)
                    {
                        UsePower();
                        NumberOfPowersIUsed++;
                    }
                }
                else if (isUsingPower)
                {
                    TimeLeft -= Time.deltaTime;
                    PowerInformation.text = $"{Mathf.CeilToInt(TimeLeft)}";
                    CheckPower();
                    PowerMiddlegroundImage.fillAmount = 1 - TimeLeft / TimeDuringPowers;
                    if (TimeLeft <= 0)
                    {
                        StopPower();
                    }
                }
            }
            else
            {
                PowerPanel.SetActive(false);
            }
        }
        else
        {
            PowerPanel.SetActive(false);
        }

        if (blindness > 0)
        {
            blindnessGameObject.SetActive(true);
            blindness -= Time.deltaTime;
        }
        else
        {
            blindnessGameObject.SetActive(false);
        }
    }

    public void ChoosePower()
    {
        isWaitingPower = false;
        canUsePower = true;
        PowerMiddlegroundImage.fillAmount = 0;
        if (TryOnlyToTest)
        {
            PowerChoosenIndicator = PowerToTest;
        }
        else PowerChoosenIndicator = ChoosePowerRandomly();
        PowerTypeChoosen = PowerType[PowerChoosenIndicator];
        PowerImageChoosen = PowerImages[PowerChoosenIndicator];
        PowerImageChoosen.gameObject.SetActive(true);
        Debug.Log($"Power is choosen : {PowerTypeChoosen}");
        PowerInformation.text = $"{PowerTypeChoosen}";
    }

    public int ChoosePowerRandomly()
    {
        int totalProbability = 0;
        int choice = 0;
        int eachProbability = 0;
        foreach (int i in PowerProbabilities) totalProbability += i;
        int probability = Random.Range(0, totalProbability - 1);
        Debug.Log($"Probability choosen : {probability}");
        foreach (int i in PowerProbabilities)
        {
            eachProbability += i;
            if (probability <= eachProbability)
            {
                return choice;
            }
            else
            {
                choice++;
            }
        }
        return 1;
    }

    public void UsePower()
    {
        canUsePower = false;
        isWaitingPower = false;
        isUsingPower = true;
        TimeDuringPowers = PowerDuration[PowerChoosenIndicator];
        TimeLeft = TimeDuringPowers;

        switch (PowerChoosenIndicator)
        {
            case 0:
                CallPowerZaWarudo(true);
                break;
            case 1:
                CallMinePower();
                break;
            case 2:
                CallGasGasGas(true);
                break;
            case 3:
                CallGeant(true);
                break;
            case 4:
                CallSuddenDeath();
                break;
            case 5:
                CallCadenas(true);
                break;
            case 6:
                CallPaintBall();
                break;
            case 7:
                CallEarthIsMine();
                break;
            case 8:
                CallLava(true);
                break;
            case 9:
                CallSnails();
                break;
            case 10:
                CallPet();
                break;
            case 11:
                CallOutOfControl();
                break;
            case 12:
                CallBlindness();
                break;
        }
    }

    public void StopPower()
    {
        canUsePower = false;
        isUsingPower = false;
        isWaitingPower = true;
        PowerImageChoosen.gameObject.SetActive(false);
        TimeLeft = TimeBetweenPowers;
        switch (PowerChoosenIndicator)
        {
            case 0:
                CallPowerZaWarudo(false);
                break;
            case 1:
                break;
            case 2:
                CallGasGasGas(false);
                break;
            case 3:
                CallGeant(false);
                break;
            case 4:
                break;
            case 5:
                CallCadenas(false);
                break;
            case 8:
                CallLava(false);
                break;
        }
    }

    void CheckPower()
    {
        if (PowerChoosenIndicator == 0)
        {
            Player.GetComponent<CubeMovement>().canMove = true;
        }
    }

    public void CallPowerZaWarudo(bool isBegin)
    {
        photonView.RPC("ZaWarudo", RpcTarget.All, isBegin);
    }

    void CallGasGasGas(bool isGoneFast)
    {
        Player.GetComponent<CubeMovement>().isGoingFar = isGoneFast;
        if (isGoneFast) photonView.RPC("GasGas", RpcTarget.All, Player.GetComponent<PhotonView>().Owner.UserId);
    }

    [PunRPC]
    void GasGas(string id)
    {
        GameObject playerS = null;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().Owner.UserId == id)
            {
                playerS = player;
            }
        }
        if (playerS != null) playerS.GetComponentInChildren<GasGasGasAudio>().isGoingFar = true;
    }

    [PunRPC]
    public void ZaWarudo(bool isBegin)
    {
        Debug.Log($"Zawarudo Effect : {isBegin}");
        Player.GetComponent<CubeMovement>().canMove = !isBegin;
        GSM.isTimeStopped = isBegin;
        if (isBegin)
        {
            Debug.Log("Playing Zawarudo.");
            music.Pause();
            ZaWarudoStop.Play();
            Invoke("CallZaWarudoEffect", 1.8f);
        }
        else
        {
            ZaWarudoResume.Play();
            StartCoroutine("EndZawarudo");
            Invoke("UnPause", 2f);
        }
        
    }

    void CallZaWarudoEffect()
    {
        StartCoroutine("ZaWarudoCoroutine");
    }

    IEnumerator ZaWarudoCoroutine()
    {
        ZawarudoEffectGameObject.SetActive(true);
        ZawarudoEffectGameObject.GetComponent<Animator>().SetBool("start", true);
        yield return new WaitForSecondsRealtime(3.8f);
        ZawarudoEffectGameObject.GetComponent<Animator>().SetBool("start", false);
        ZawarudoEffectGameObject.SetActive(false);
    }

    IEnumerator EndZawarudo()
    {
        ZawarudoEffectGameObject.SetActive(true);
        ZawarudoEffectGameObject.GetComponent<Animator>().SetBool("start", true);
        yield return new WaitForSecondsRealtime(2f);
        ZawarudoEffectGameObject.GetComponent<Animator>().SetBool("start", false);
        ZawarudoEffectGameObject.SetActive(false);
    }

    void UnPause()
    {
        music.UnPause();
    }

    void CallMinePower()
    {
        int x = Mathf.FloorToInt(Player.transform.position.x);
        int z = Mathf.FloorToInt(Player.transform.position.z);
        string owner = Player.GetComponent<PhotonView>().Owner.UserId;
        photonView.RPC("InstanciateMine", RpcTarget.All, x, z, owner);
    }

    [PunRPC]
    void InstanciateMine(int x, int z, string owner)
    {
        Vector3 pos = new Vector3(x, 0.7f, z);
        var mine = Instantiate(Mine, pos, Quaternion.identity);
        mine.GetComponent<BipBip>().owner = owner;
    }

    void CallGeant(bool isGeant)
    {
        Player.GetComponent<CubeMovement>().ChangeGeantState(isGeant);
    }

    void CallSuddenDeath()
    {
        photonView.RPC("CallNaniEffect", RpcTarget.Others); 
    }

    [PunRPC]
    public void CallNaniEffect()
    {
        NaniSound.Play();
        music.Pause();
        Invoke("SuddenDeath", 4f);
    }

    public void SuddenDeath()
    {
        Player.GetComponent<ExplosionCube>().DeadWithExplosion();
        music.UnPause();
    }

    void CallCadenas(bool isLocked)
    {
        Player.GetComponent<CubeMovement>().isPoweredByTheWayAsTilesDontChange = isLocked;
    }

    void CallPaintBall()
    {
        CubeMovement CM = Player.GetComponent<CubeMovement>();
        var bomb = PhotonNetwork.Instantiate("bomb", Player.transform.position, Quaternion.identity);
        bomb.GetComponent<PaintBombe>().colorOfBody = CM.colorOfBody;
        bomb.GetComponent<PaintBombe>().nameID = PhotonNetwork.LocalPlayer.UserId;
        //photonView.RPC("InstanciateBombe", RpcTarget.All, CM.colorOfBody, PhotonNetwork.LocalPlayer.UserId, (int)Player.transform.position.x, (int)(Player.transform.position.y + 0.5f), (int)Player.transform.position.z);
    }

    [PunRPC]
    public void InstanciateBombe(int color, string nameID, int x, int y, int z)
    {
        var bomb = GameObject.Instantiate(PaintBombe, new Vector3(x, y, z), Quaternion.identity);
        bomb.GetComponent<PaintBombe>().colorOfBody = color;
        bomb.GetComponent<PaintBombe>().nameID = nameID;
    }

    public void CallEarthIsMine()
    {
        Player.GetComponent<CubeMovement>().isRising = true;
        Player.GetComponent<CubeMovement>().StopMoving();
        photonView.RPC("EarthIsMine", RpcTarget.All, Player.transform.position.x, Player.transform.position.z, Player.GetComponent<CubeMovement>().colorOfBody, Player.GetComponent<PhotonView>().Owner.UserId);
        Invoke("StopRising", 4.7f);
    }

    [PunRPC]
    public void EarthIsMine(float x, float z, int color, string id)
    {
        var cylinder = GameObject.Instantiate(CylinderOfIsMine, new Vector3(x, 0, z), Quaternion.identity);
        music.Pause();
        Chaos.Play();
        GSM.isTimeStopped = true;
        Destroy(cylinder, 4.7f);
        StartCoroutine(ChangeTiles(x, z, color, id));
    }

    IEnumerator ChangeTiles(float x, float z, int color, string id)
    {
        yield return new WaitForSeconds(4.7f);
        GSM.isTimeStopped = false;
        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("floor"))
        {
            ChangeColor c = tile.GetComponent<ChangeColor>();
            c.colorOfThePlayerWhoTouchedThisTile = color;
            c.gameObject.GetComponent<Renderer>().material.color = CM.colorsAvailable[color];
            c.playerWhoTouchedThisTile = id;
        }
        yield return new WaitForSeconds(2f);
        music.UnPause();
    }

    void StopRising()
    {
        Player.GetComponent<CubeMovement>().isRising = false;
        Player.GetComponent<CubeMovement>().canMove = true;
    }

    void CallLava(bool isActive)
    {
        Player.GetComponent<CubeMovement>().lavableTiles = isActive;
    }

    void CallSnails()
    {
        photonView.RPC("SnailsEveryone", RpcTarget.Others);
    }

    [PunRPC]
    void SnailsEveryone()
    {
        Player.GetComponent<CubeMovement>().slowTime = 8;
    }

    void CallPet()
    {
        int choice = Random.Range(0, 21);
        if (choice == 1) choice = 3;
        else choice = 1;
        for (int i = 0; i < choice; i++)
        {
            PhotonNetwork.Instantiate("Pet", new Vector3((int)Player.transform.position.x, 1, (int)Player.transform.position.z), Quaternion.identity);
        }
    }

    void CallOutOfControl()
    {
        photonView.RPC("OutOfControl", RpcTarget.Others);
    }

    [PunRPC]
    void OutOfControl()
    {
        Player.GetComponent<CubeMovement>().outOfControlTime = 10;
    }

    void CallBlindness()
    {
        photonView.RPC("Blindness", RpcTarget.Others);
    }

    [PunRPC]
    void Blindness()
    {
        blindness = 10f;
    }
}
