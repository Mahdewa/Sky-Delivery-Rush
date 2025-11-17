using System.Collections;
using UnityEngine;

public class HouseSpawner : MonoBehaviour
{
    public GameObject housePrefab; // Slot untuk prefab House

    // Ketinggian Y tempat rumah akan muncul
    // Kita bisa buat acak nanti, untuk sekarang kita paku di -4
    public float spawnYPosition = -4f; 

    [Header("Spawn Timing (Detik)")]
    public float minSpawnDelay = 2.0f;
    public float maxSpawnDelay = 5.0f;

    void Start()
    {
        // Memulai proses spawn
        StartCoroutine(SpawnHouseRoutine());
    }

    // Coroutine adalah fungsi yang bisa dijeda
    IEnumerator SpawnHouseRoutine()
    {
        // Loop ini akan berjalan selamanya
        while (true)
        {
            // 1. Tentukan posisi spawn
            // Posisi X diambil dari posisi objek Spawner ini
            Vector3 spawnPosition = new Vector3(transform.position.x, spawnYPosition, 0);

            // 2. Buat (Instantiate) rumah di posisi tersebut
            Instantiate(housePrefab, spawnPosition, Quaternion.identity);

            // 3. Tentukan jeda waktu acak sebelum spawn berikutnya
            float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);

            // 4. Tunggu selama jeda waktu tersebut
            yield return new WaitForSeconds(randomDelay);
        }
    }
}