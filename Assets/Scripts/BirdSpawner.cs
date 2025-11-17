using System.Collections;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject birdPrefab;

    [Header("Spawn Position Y (Random)")]
    public float minY = -2f; // Sesuai 'lowY'
    public float maxY = 1f;  // Sesuai 'highY'

    [Header("Spawn Timing (Detik)")]
    public float minSpawnDelay = 3.0f;
    public float maxSpawnDelay = 6.0f;

    void Start()
    {
        StartCoroutine(SpawnBirdRoutine());
    }

    IEnumerator SpawnBirdRoutine()
    {
        // Tunggu beberapa detik sebelum burung pertama
        yield return new WaitForSeconds(5f); 

        while (true)
        {
            // Ambil level kesulitan saat ini dari GameManager
            int currentDifficulty = 1;
            if (GameManager.Instance != null)
            {
                currentDifficulty = GameManager.Instance.difficultyLevel;
            }

            // Tentukan parameter spawn berdasarkan level kesulitan
            float currentMinDelay = minSpawnDelay;
            float currentMaxDelay = maxSpawnDelay;
            int spawnCount = 1; // Jumlah burung yang akan di-spawn

            if (currentDifficulty == 2) // Mid Game [cite: 64]
            {
                currentMinDelay = 2.0f;
                currentMaxDelay = 4.0f;
            }
            else if (currentDifficulty == 3) // Late Game 
            {
                currentMinDelay = 1.0f;
                currentMaxDelay = 3.0f;
                spawnCount = Random.Range(2, 4); // Burung muncul berkelompok!
            }

            // Tentukan jeda waktu acak
            float randomDelay = Random.Range(currentMinDelay, currentMaxDelay);
            yield return new WaitForSeconds(randomDelay);

            // Spawn burung sebanyak spawnCount
            for (int i = 0; i < spawnCount; i++)
            {
                // Tentukan posisi Y acak
                float randomY = Random.Range(minY, maxY);
                Vector3 spawnPosition = new Vector3(transform.position.x, randomY, 0);

                // Buat (Instantiate) burung
                Instantiate(birdPrefab, spawnPosition, Quaternion.identity);

                if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Bird");

                // Jika berkelompok, beri jeda sedikit antar burung
                if (spawnCount > 1)
                {
                    yield return new WaitForSeconds(0.3f);
                }
            }
        }
    }
}