using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class SphereSpawner : MonoBehaviour
{
    [SerializeField] GameObject spherePrefab;
    [SerializeField] float spawnInterval = 5f;
    int totalSpheres;
    int ballsHit = 0;
    [SerializeField] float spawnRadius = 2f;
    [Header("Settings")]
    [SerializeField] private int minBallsToHit = 25;
    [SerializeField] private int maxBallsToHit = 30;
    [SerializeField] private GameObject XRRig;
    [SerializeField] private float minHeight = 0.5f;

    void Start()
    {
        XRRig.transform.position = new Vector3(1.8f, 1.6f, 17.145f);
    }

    public void onButtonClicked()
    {
        StartCoroutine(SpawnSpheres());
        GameManager.Instance.StartRecording();
    }

    IEnumerator SpawnSpheres()
    {
        totalSpheres = 27;
        Debug.Log($"Hit {totalSpheres} balls to proceed.");

        for (int i = 0; i < 40; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = Mathf.Clamp(randomPos.y, 1f, 2f); // Keep at comfortable height
            randomPos.y = Mathf.Max(randomPos.y, minHeight);
            Instantiate(spherePrefab, randomPos, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void OnSphereClicked()
    {
        ballsHit++;
        Debug.Log($"Hit {ballsHit} so far.");
        if (ballsHit >= totalSpheres)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadNextScene();
            }
        }
    }
}