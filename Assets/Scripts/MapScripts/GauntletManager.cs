using System.Collections.Generic;
using UnityEngine;

public class GauntletManager : MonoBehaviour
{
    public bool hasStarted;
    public bool hasFinished;
    public int currentWave;
    private List<GameObject> currentEnemies;

    public Door[] doors;

    public void StartGauntlet()
    {
        hasStarted = true;
        currentWave = 0;
        ShowWave(currentWave, true);

        foreach (Door d in doors)
            d.TriggerDoor();
    }

    public void ResetGauntlet()
    {
        hasStarted = false;

        for(int i = 0; i < transform.childCount; i++)
            ShowWave(i, false);

        foreach (Door d in doors)
            d.TriggerDoor();
    }

    public void EndGauntlet()
    {
        hasFinished = true;

        foreach (Door d in doors)
            d.TriggerDoor();
    }

    private void Update()
    {
        if(hasStarted && !hasFinished && IsWaveOver())
        {
            currentWave++;
            ShowWave(currentWave, true);
        }
    }

    public void ShowWave(int wave, bool show)
    {
        currentEnemies = new List<GameObject>();

        if(wave >= transform.childCount)
        {
            EndGauntlet();
            return;
        }

        foreach (Transform t in transform.GetChild(wave))
        {
            t.gameObject.SetActive(show);

            if (show)
            {
                t.gameObject.GetComponent<Enemy>().StartEnemy();
                t.gameObject.GetComponent<Enemy>().gauntlet = true;
                currentEnemies.Add(t.gameObject);
            }
        }
    }

    public bool IsWaveOver()
    {
        foreach(GameObject enemy in currentEnemies)
            if(enemy.activeSelf)
                return false;

        return true;
    }
}
