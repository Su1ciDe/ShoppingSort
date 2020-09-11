using UnityEngine;
using System.Collections;

public class PillSpawner : MonoBehaviour
{
    public IEnumerator SpawnPills(GameObject pill, int maxPills)
    {
        for (int i = 0; i < maxPills; i++)
        {
            GameObject _pill = Instantiate(pill);
            _pill.transform.position = transform.position;

            yield return new WaitForSeconds(.2f);
        }
    }

    public void SpawnPill(GameObject pill, Material colorMat)
    {
        GameObject _pill = Instantiate(pill);
        _pill.transform.GetChild(0).GetComponent<MeshRenderer>().material = colorMat;
        _pill.transform.localScale= new Vector3(3, 3, 3);
        _pill.transform.position = transform.position;
    }

    public void RespawnPill(GameObject pill)
    {
        pill.transform.localScale = new Vector3(3, 3, 3);
        pill.transform.position = transform.position;
    }
}