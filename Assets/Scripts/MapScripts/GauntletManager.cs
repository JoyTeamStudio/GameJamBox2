using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GauntletManager : MonoBehaviour
{
    public string gauntletName;
    public bool hasStarted;
    public bool hasFinished;
    private bool spawning;
    public int currentWave;
    private List<GameObject> currentEnemies;
    private List<GameObject> currentParticles;

    public Door[] doors;
    public GameObject particles;
    public Transform particlesY;

    private void Start()
    {
        for (int i = 0; i < MainManager.Instance.gauntletBossData.Length; i++)
        {
            if (gauntletName.ToLower() == MainManager.Instance.gauntletBossData[i].gaunletName.ToLower())
            {
                hasFinished = MainManager.Instance.gauntletBossData[i].finished;
                hasStarted = hasFinished;
                break;
            }
        }
    }

    public void StartGauntlet()
    {
        FindAnyObjectByType<GameManager>().PlayFightMusic();
        spawning = false;
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
        FindAnyObjectByType<GameManager>().PlayMainMusic();
        hasFinished = true;

        for(int i = 0; i < MainManager.Instance.gauntletBossData.Length; i++)
        {
            if(gauntletName.ToLower() == MainManager.Instance.gauntletBossData[i].gaunletName.ToLower())
            {
                MainManager.Instance.gauntletBossData[i].finished = true;
                break;
            }
        }

        foreach (Door d in doors)
            d.TriggerDoor();
    }

    private void Update()
    {
        if(hasStarted && !hasFinished && !spawning && IsWaveOver())
        {
            currentWave++;
            ShowWave(currentWave, true);
        }
    }

    public void ShowWave(int wave, bool show)
    {
        currentEnemies = new List<GameObject>();
        currentParticles = new List<GameObject>();

        if(wave >= transform.childCount)
        {
            EndGauntlet();
            return;
        }

        spawning = show;

        foreach (Transform t in transform.GetChild(wave))
        {
            if (show)
            {
                GameObject newPart = Instantiate(particles, new Vector3(t.position.x, particlesY.position.y, particles.transform.position.z), particles.transform.rotation);
                currentParticles.Add(newPart);
                currentEnemies.Add(t.gameObject);
            }else
                t.gameObject.SetActive(false);
        }

        if(show)
            StartCoroutine(ShowEnemies());
    }

    public bool IsWaveOver()
    {
        foreach(GameObject enemy in currentEnemies)
            if(enemy.activeSelf)
                return false;

        return true;
    }

    private IEnumerator ShowEnemies()
    {
        yield return new WaitForSeconds(1.1f);

        foreach(GameObject enemy in currentEnemies)
        {
            enemy.SetActive(true);

            enemy.GetComponent<Enemy>().StartEnemy();
            enemy.GetComponent<Enemy>().gauntlet = true;
        }

        foreach(GameObject part in currentParticles)
            Destroy(part);

        currentParticles.Clear();

        spawning = false;
    }
}
