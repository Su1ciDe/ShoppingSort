using System.Collections.Generic;
using UnityEngine;

public class Shelves : MonoBehaviour
{
    private LevelManager levelManager;

    public int shelfCount;
    public GameObject[] shelves;
    public Material[] shelfColorMat;
    public Material[] shelfGlassColorMat;

    private List<int> randomList = new List<int>();

    void Start()
    {
        levelManager = LevelManager.instance;

        shelfCount = levelManager.bottles.Length;
        shelves = new GameObject[shelfCount];
        shelfColorMat = new Material[shelfCount];
        shelfGlassColorMat = new Material[shelfCount];

        for (int i = 0; i < shelfCount; i++)
        {
            randomList.Add(i);

            shelves[i] = transform.GetChild(0).transform.GetChild(i).transform.GetChild(0).gameObject;
            shelfColorMat[i] = levelManager.bottles[i].GetComponent<Bottle>().colorMat;
            shelfGlassColorMat[i] = levelManager.bottles[i].GetComponent<Bottle>().glassColorMat;
        }

        SetupShelves();
    }

    private void OnBecameVisible()
    {
        SetupShelves();
    }

    private void OnEnable()
    {
        SetupShelves();
    }

    public void SetupShelves()
    {
        for (int i = 0; i < shelfCount; i++)
        {
            int randomShelf = Random.Range(0, randomList.Count);
            int randomPlace = Random.Range(0, shelves[randomList[randomShelf]].transform.childCount);

            GameObject bottle = Instantiate(levelManager.levels[levelManager.levelNo].bottles[i].bottlePrefab, shelves[randomList[randomShelf]].transform.GetChild(randomPlace).transform);
            bottle.GetComponent<BottleDragDrop>().enabled = false;
            bottle.GetComponent<BoxCollider>().enabled = false;
            bottle.transform.position = shelves[randomList[randomShelf]].transform.GetChild(randomPlace).position + new Vector3(0, bottle.GetComponent<Bottle>().offsetY / 2, 0);
            bottle.transform.localScale = new Vector3(2 / 2.25f, 2 / 2.25f, 2 / 2.25f);
            bottle.GetComponent<Bottle>().colorMat = shelfColorMat[i];

            bottle.transform.GetChild(1).GetComponent<MeshRenderer>().material = shelfGlassColorMat[i];

            shelves[randomList[randomShelf]].GetComponent<ShelfHolders>().colorMat = shelfColorMat[i];
            shelves[randomList[randomShelf]].transform.GetChild(randomPlace).GetComponent<BottleHolder>().isEmpty = false;

            for (int j = 0; j < bottle.GetComponent<Bottle>().maxPills; j++)
            {
                GameObject pill = Instantiate(levelManager.levels[levelManager.levelNo].bottles[i].pillPrefab, bottle.transform);
                pill.transform.GetChild(0).GetComponent<MeshRenderer>().material = shelfColorMat[i];
                pill.GetComponent<PillDragDrop>().canDrag = false;
                pill.GetComponent<PillDragDrop>().enabled = false;
                pill.GetComponent<Rigidbody>().isKinematic = false;
                pill.transform.localPosition = Vector3.zero;
                pill.transform.localScale = new Vector3(1 / 2.25f, 1 / 2.25f, 1 / 2.25f);
            }

            randomList.RemoveAt(randomShelf);
        }
    }
}