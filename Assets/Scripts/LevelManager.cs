using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public enum PARTS { PART1, PART2, PART3 };

    public static LevelManager instance;

    public List<LevelData> levels;
    public int levelNo = 0;
    [HideInInspector] public PARTS currentPart = PARTS.PART1;

    [HideInInspector] public int tripletCount = 0;
    [HideInInspector] public int currentTripletIndex = 0;
    [HideInInspector] public int bottleSpawned_Count = 0;

    public GameObject conveyor_Part1;
    public GameObject conveyor_Part2;
    public GameObject conveyor_Part3;
    public GameObject conveyor_Part1toPart2;
    public GameObject conveyor_Part2toPart3;
    [Header("Prefabs")]
    public GameObject conveyor_GlassHolder_Prefab;
    public GameObject labelPanel_Prefab;
    public GameObject UI_RightWrong_Prefab;

    [Space]
    public GameObject shelves;
    public GameObject petri;
    public GameObject bottleSpawnPoint;

    [HideInInspector] public GameObject labelPanel;
    [HideInInspector] public GameObject[] bottles;
    [HideInInspector] public GameObject[] pills;
    [HideInInspector] public GameObject[] labels;

    private GameObject[] glassHolders;

    private void Awake()
    {
        instance = this;

        levels = Levels.Instance.levels;
        levelNo = PlayerPrefs.GetInt("LevelNo");
    }

    private void Start()
    {
        bottles = new GameObject[levels[levelNo].bottles.Count];
        pills = new GameObject[levels[levelNo].bottles.Count];

        tripletCount = Mathf.CeilToInt(levels[levelNo].bottles.Count / 3f);

        SetupPart1();

        glassHolders = new GameObject[levels[levelNo].bottles.Count];
        SetupBottleHolders(conveyor_Part1toPart2);
        SetupBottleHolders(conveyor_Part2toPart3);
    }

    private void CheckLevelNo()
    {
        if (levelNo < 0 || levelNo >= levels.Count)
            levelNo = 0;
    }

    // Test amaçlı 
    // Eğer bir bölüm varsa tekrar başa dönüyor
    public IEnumerator LoadNextLevel()
    {
        levelNo += 1;
        CheckLevelNo();
        PlayerPrefs.SetInt("LevelNo", levelNo);

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Şişelerin aradaki conveyor beltte durması için gereken referans objectlerin yerleştirilimesi
    private void SetupBottleHolders(GameObject conveyor)
    {
        Vector3 size = conveyor.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size;

        float gap = size.x / levels[levelNo].bottles.Count;
        float x = (-size.x / 2) + (gap / 2);
        for (int i = 0; i < levels[levelNo].bottles.Count; i++)
        {
            if (currentPart == PARTS.PART1)
            {
                glassHolders[i] = Instantiate(conveyor_GlassHolder_Prefab, conveyor.transform.GetChild(0));
            }
            else
            {
                glassHolders[i].transform.SetParent(conveyor.transform.GetChild(0));
            }
            glassHolders[i].transform.localPosition = new Vector3(x + gap * i, 0, 0);
        }
    }

    // Şişelerin conveyor belt'in üzerinde hareket edebilmesi için
    public IEnumerator MoveBottles(GameObject bottle, GameObject conveyor, int index)
    {
        Vector3 newPos;
        if (conveyor.transform.GetChild(0).transform.childCount == 3)
            newPos = conveyor.transform.GetChild(0).transform.GetChild(index - (3 * currentTripletIndex)).position;
        else
            newPos = conveyor.transform.GetChild(0).transform.GetChild(index).position;

        while (true)
        {
            bottle.transform.position = Vector3.Lerp(bottle.transform.position, new Vector3(newPos.x, bottle.transform.position.y, bottle.transform.position.z), Time.deltaTime * 3);
            bottle.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (Vector3.Distance(bottle.transform.position, new Vector3(newPos.x, bottle.transform.position.y, bottle.transform.position.z)) <= 0.01f)
            {
                if (index == bottleSpawned_Count - 1)
                    conveyor.GetComponent<Animator>().SetBool("isMoving", false);

                bottle.GetComponent<BottleDragDrop>().startingPos = bottle.transform.position;

                yield break;
            }

            yield return null;
        }
    }

    public void CheckBottleSpawned()
    {
        if (bottleSpawned_Count + 3 < levels[levelNo].bottles.Count)
            bottleSpawned_Count += 3;
        else
            bottleSpawned_Count = levels[levelNo].bottles.Count;
    }

    #region PART1
    public void SetupPart1()
    {
        currentPart = PARTS.PART1;

        // Kamera
        Camera.main.transform.position = new Vector3(conveyor_Part1.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

        conveyor_Part1.GetComponent<Animator>().SetBool("isMoving", true); // Conveyor animasyonunu başlatmak için

        shelves.SetActive(false);
        petri.SetActive(false);

        //Bottles
        StartCoroutine(SpawnBottles());

        //Labels
        SetupLabels();

    }

    //Labels
    void SetupLabels()
    {
        labels = new GameObject[levels[levelNo].bottles.Count];
        GameObject UI = GameObject.FindGameObjectWithTag("UI");
        if (UI != null)
        {
            if (labelPanel != null)
                Destroy(labelPanel);

            labelPanel = Instantiate(labelPanel_Prefab, UI.transform);
            labelPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("UI tag'i belirlenmemiş veya UI yok!");
            return;
        }

        for (int i = 3 * currentTripletIndex; i < bottleSpawned_Count; i++)
        {
            //Labels
            if (labelPanel != null)
            {
                labels[i] = Instantiate(levels[levelNo].bottles[i].labelPrefab, labelPanel.transform);
                labels[i].GetComponent<LabelDrag>().matchingColorMat = levels[levelNo].bottles[i].colorMat;
                labels[i].GetComponent<Image>().color = levels[levelNo].bottles[i].colorMat.color;
            }
        }
    }

    // Şişelerin ilk partta oluşması için...
    public IEnumerator SpawnBottles()
    {
        CheckBottleSpawned();

        for (int i = 3 * currentTripletIndex; i < bottleSpawned_Count; i++)
        {   // Şişe setup
            bottles[i] = Instantiate(levels[levelNo].bottles[i].bottlePrefab);
            bottles[i].GetComponent<Bottle>().maxPills = levels[levelNo].bottles[i].maxPills;
            bottles[i].GetComponent<Bottle>().colorMat = levels[levelNo].bottles[i].colorMat;
            bottles[i].GetComponent<Bottle>().glassColorMat = levels[levelNo].bottles[i].glassColorMat;
            bottles[i].GetComponent<BottleDragDrop>().enabled = false;

            bottles[i].transform.rotation = Quaternion.identity;
            bottles[i].transform.localPosition = bottleSpawnPoint.transform.position + new Vector3(0, bottles[i].GetComponent<Bottle>().offsetY / 2, 0);
            StartCoroutine(MoveBottles(bottles[i], conveyor_Part1, i)); // Şişeleri yerlerine ilerlet

            yield return new WaitForSeconds(.4f);
        }
    }

    #endregion PART1

    #region PART2
    public void SetupPart2()
    {
        currentPart = PARTS.PART2;

        Camera.main.transform.position = new Vector3(conveyor_Part2.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);

        labelPanel.SetActive(false);
        petri.SetActive(true);

        conveyor_Part2.GetComponent<Animator>().SetBool("isMoving", true);
        for (int i = 3 * currentTripletIndex; i < bottleSpawned_Count; i++)
        {   // Kapağı aç
            bottles[i].transform.GetChild(0).gameObject.SetActive(false);
            // Şişeleri yerlerine ilerlet
            StartCoroutine(MoveBottles(bottles[i], conveyor_Part2, i));
            // pills
            pills[i] = levels[levelNo].bottles[i].pillPrefab;
            pills[i].GetComponent<Rigidbody>().isKinematic = false;
            pills[i].transform.GetChild(0).GetComponent<MeshRenderer>().material = levels[levelNo].bottles[i].colorMat;
            StartCoroutine(petri.transform.GetChild(0).GetComponent<PillSpawner>().SpawnPills(pills[i], levels[levelNo].bottles[i].maxPills));
        }
    }

    #endregion PART2

    #region PART3
    public void SetupPart3()
    {
        currentPart = PARTS.PART3;

        Camera.main.transform.position = new Vector3(conveyor_Part3.transform.position.x, .8f, -1.6f);
        Camera.main.transform.rotation = Quaternion.Euler(2f, 0, 0);

        conveyor_Part3.GetComponent<Animator>().SetBool("isMoving", true);

        shelves.SetActive(true);
        petri.SetActive(false);
        petri.transform.GetChild(0).GetComponent<PillSpawner>().enabled = false;

        foreach (GameObject pill in pills)
        {
            pill.GetComponent<Rigidbody>().isKinematic = true;
        }

        for (int i = 3 * currentTripletIndex; i < bottleSpawned_Count; i++)
        {
            bottles[i].GetComponent<BottleDragDrop>().enabled = true;
            // ui varsa silinir
            foreach (Transform child in bottles[i].transform)
            {
                if (child.GetComponent<Canvas>() != null)
                    Destroy(child.gameObject);
            }

            StartCoroutine(MoveBottles(bottles[i], conveyor_Part3, i));
        }
    }

    #endregion PART3
}