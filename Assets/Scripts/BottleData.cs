using UnityEngine;

[System.Serializable]
public class BottleData
{
    public GameObject bottlePrefab;
    public GameObject pillPrefab;
    public GameObject labelPrefab;

    [Space]
    public Material colorMat;
    public Material glassColorMat;
    public int maxPills;
}