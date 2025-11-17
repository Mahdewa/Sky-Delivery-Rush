using System.Collections;
using UnityEngine;

public class ResupplySpawner : MonoBehaviour
{
    public GameObject resupplyPrefab;
    public float spawnInterval = 30f; // Interval tetap 30 detik
    public float spawnYPosition = -3f; // Sesuaikan dengan tinggi menara

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        // Tunggu 30 detik *sebelum* spawn pertama
        yield return new WaitForSeconds(spawnInterval);

        while (true)
        {
            // 1. Tentukan posisi spawn
            Vector3 spawnPosition = new Vector3(transform.position.x, spawnYPosition, 0);

            // 2. Buat (Instantiate) menara
            Instantiate(resupplyPrefab, spawnPosition, Quaternion.identity);

            // 3. Tunggu 30 detik lagi
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}