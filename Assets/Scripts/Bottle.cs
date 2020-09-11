using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Bottle : MonoBehaviour
{
    [Header("Attributes")]
    public int maxPills = 4;

    [Header("Settings")]
    public Material colorMat;
    public Material glassColorMat;
    public float offsetY;

    private Transform cap;
    private Transform glass;

    private Animator anim;

    private int curPills = 0;

    private void Awake()
    {
        //Kapak en üst child olmalı, cam 2. child olmalı
        cap = transform.GetChild(0);
        glass = transform.GetChild(1);

        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (cap != null)
            cap.GetComponent<MeshRenderer>().material = colorMat;
    }

    #region PART1

    public void ChangeGlassColor()
    {
        glass.GetComponent<MeshRenderer>().material = glassColorMat;
    }

    #endregion PART1

    #region PART2

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pills"))
        {
            curPills++;

            other.transform.SetParent(transform);

            Material[] material = other.GetComponentInChildren<MeshRenderer>().materials;
            for (int i = 0; i < material.Length; i++)
            {
                if (material[i].name.ToString().Contains("Default-Material"))
                    continue;
                if (material[i].color == colorMat.color)
                {
                    RightPill();
                }
                else
                {
                    LevelManager.instance.petri.transform.GetChild(0).GetComponent<PillSpawner>().SpawnPill(other.gameObject, other.transform.GetChild(0).GetComponent<MeshRenderer>().material);
                    WrongPill();
                }
            }
        }

        if (curPills >= maxPills)
        {
            CloseCap();
        }
    }

    private IEnumerator UI_RightWrong(bool rightOrWrong, float waitBefore, float waitBeforeDestroy)
    {
        yield return new WaitForSeconds(waitBefore);

        GameObject ui = Instantiate(LevelManager.instance.UI_RightWrong_Prefab, transform);
        ui.GetComponent<Canvas>().worldCamera = Camera.main;
        if (rightOrWrong)
        {
            ui.transform.GetChild(0).GetComponent<Text>().text = "✓";
            ui.transform.GetChild(0).GetComponent<Text>().color = Color.green;
        }
        else
        {
            ui.transform.GetChild(0).GetComponent<Text>().text = "X";
            ui.transform.GetChild(0).GetComponent<Text>().color = Color.red;
        }

        if (waitBeforeDestroy != 0f)
            Destroy(ui.gameObject, waitBeforeDestroy);
    }

    private void CloseCap()
    {
        if (cap.gameObject.activeSelf == false)
        {
            cap.gameObject.SetActive(true);
            GetComponent<BoxCollider>().enabled = false;

            // şişe dolduysa tik animasyonu
            StartCoroutine(UI_RightWrong(true, 1, 0));

            // kapak animasyonu
            anim.SetTrigger("capAnim");

            GameManager.instance.Part2TOPart3();
        }
    }

    private void RightPill()
    {
        Debug.Log("Right Pill");
        GameManager.instance.AddToScore(5);
    }

    private void WrongPill()
    {
        Debug.Log("Wrong Pill");
        GameManager.instance.AddToScore(-3);

        // yalnış pill ise X animasyonu
        StartCoroutine(UI_RightWrong(false, 0, 1));
    }

    #endregion PART2
}