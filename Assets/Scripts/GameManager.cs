using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private LevelManager levelManager;

    [HideInInspector] public int Score;
    public string txtScore_Prefix = "Score: ";
    public Text txtScore;

    private void Awake()
    {
        instance = this;
        levelManager = GetComponent<LevelManager>();
    }

    #region Score
    public void AddToScore(int addition)
    {
        Score += addition;
        UpdateUI_Score();
    }

    private void UpdateUI_Score()
    {
        if (txtScore == null)
        {
            Debug.LogError("GameManager: Score texti referanslanmamış!");
            return;
        }

        txtScore.text = txtScore_Prefix + Score;
    }

    #endregion

    public void CheckPart1Finished()
    {
        for (int i = 3 * levelManager.currentTripletIndex; i < levelManager.bottleSpawned_Count; i++)
        {
            StartCoroutine(levelManager.MoveBottles(levelManager.bottles[i], levelManager.conveyor_Part1toPart2, i));
        }

        if (levelManager.currentTripletIndex + 1 >= levelManager.tripletCount)
        {
            levelManager.currentTripletIndex = 0;
            levelManager.bottleSpawned_Count = 0;
            levelManager.CheckBottleSpawned();
            levelManager.SetupPart2();
        }
        else
        {
            levelManager.currentTripletIndex++;

            levelManager.SetupPart1();
        }
    }

    public void Part1TOPart2()
    {
        int labeledCount = 0;

        for (int i = 3 * levelManager.currentTripletIndex; i < levelManager.bottleSpawned_Count; i++)
        {
            if (levelManager.bottles[i].GetComponent<Bottle>().glassColorMat.color == levelManager.bottles[i].transform.GetChild(1).GetComponent<MeshRenderer>().material.color)
                labeledCount++;
        }

        if (labeledCount == levelManager.bottleSpawned_Count - 3 * levelManager.currentTripletIndex)
            CheckPart1Finished();
    }

    public void CheckPart2Finished()
    {
        for (int i = 3 * levelManager.currentTripletIndex; i < levelManager.bottleSpawned_Count; i++)
        {
            StartCoroutine(levelManager.MoveBottles(levelManager.bottles[i], levelManager.conveyor_Part2toPart3, i));
        }

        ClearLeftoverPills();

        if (levelManager.currentTripletIndex + 1 >= levelManager.tripletCount)
        {
            levelManager.currentTripletIndex = 0;
            levelManager.bottleSpawned_Count = 0;
            levelManager.CheckBottleSpawned();
            levelManager.SetupPart3();
        }
        else
        {
            levelManager.currentTripletIndex++;
            levelManager.CheckBottleSpawned();
            levelManager.SetupPart2();
        }
    }

    public void Part2TOPart3()
    {
        int fullBottles = 0;

        for (int i = 3 * levelManager.currentTripletIndex; i < levelManager.bottleSpawned_Count; i++)
        {
            if (levelManager.bottles[i].transform.GetChild(0).gameObject.activeSelf == true)
                fullBottles++;
        }

        if (fullBottles == levelManager.bottleSpawned_Count - 3 * levelManager.currentTripletIndex)
            CheckPart2Finished();
    }

    private void ClearLeftoverPills()
    {
        GameObject[] leftovers = GameObject.FindGameObjectsWithTag("Pills");
        for (int i = 0; i < leftovers.Length; i++)
        {
            if (leftovers[i].transform.parent == null)
            {
                Destroy(leftovers[i].gameObject);
            }
        }
    }

    public void CheckPart3Finished()
    {
        if (levelManager.currentTripletIndex + 1 >= levelManager.tripletCount)
        {
            levelManager.currentTripletIndex = 0;
            levelManager.bottleSpawned_Count = 0;
            levelManager.CheckBottleSpawned();

            StartCoroutine(levelManager.LoadNextLevel());
        }
        else
        {
            levelManager.currentTripletIndex++;
            levelManager.CheckBottleSpawned();
            levelManager.SetupPart3();
        }
    }

    public void Part3TOFinish()
    {
        int bottlesPlaced = 0;
        for (int i = 3 * levelManager.currentTripletIndex; i < levelManager.bottleSpawned_Count; i++)
        {
            if (levelManager.bottles[i].transform.parent != null)
                bottlesPlaced++;
        }

        if (bottlesPlaced == levelManager.bottleSpawned_Count - 3 * levelManager.currentTripletIndex)
            CheckPart3Finished();
    }
}