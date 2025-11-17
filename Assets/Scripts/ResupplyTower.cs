using System.Collections; // Penting untuk Coroutine
using UnityEngine;

public class ResupplyTower : MonoBehaviour
{
    public int packagesToAdd = 20;
    public float cooldownDuration = 30f; // Durasi cooldown 30 detik
    
    // "Saklar" untuk mengecek apakah kita bisa resupply
    private bool canResupply = true; 

    // (Opsional) Untuk feedback visual
    private SpriteRenderer towerSprite; 

    private void Start()
    {
        // Ambil komponen SpriteRenderer jika ada, untuk ganti warna
        towerSprite = GetComponent<SpriteRenderer>(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canResupply)
        {
            int actualPackagesToAdd = packagesToAdd; // Ambil nilai default

            // Cek kesulitan dari GameManager
            if (GameManager.Instance != null)
            {
                int currentDifficulty = GameManager.Instance.difficultyLevel;
                
                if (currentDifficulty == 2) // Mid Game
                {
                    actualPackagesToAdd = 15; // Kurangi jumlah paket
                }
                else if (currentDifficulty == 3) // Late Game
                {
                    actualPackagesToAdd = 10; // Kurangi lebih banyak lagi
                }
            }

            // Panggil GameManager untuk menambah paket (dengan jumlah yg disesuaikan)
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddPackages(actualPackagesToAdd);
            }

            StartCoroutine(StartCooldown());
        }
    }

    private IEnumerator StartCooldown()
    {
        // 1. Matikan "saklar"
        canResupply = false;

        // 2. Beri feedback visual (opsional, tapi sangat disarankan)
        //    Kita buat tower jadi sedikit transparan/merah untuk menandakan non-aktif
        // if (towerSprite != null)
        // {
        //     towerSprite.color = Color.gray; // Ganti jadi abu-abu
        // }

        // 3. Tunggu selama durasi cooldown
        yield return new WaitForSeconds(cooldownDuration);

        // 4. Setelah 30 detik, nyalakan "saklar" lagi
        canResupply = true;

        // 5. Kembalikan visual ke normal (opsional)
        if (towerSprite != null)
        {
            towerSprite.color = Color.white; // Ganti kembali ke warna normal
        }
    }
}