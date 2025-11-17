using UnityEngine;

public class Package : MonoBehaviour
{
    // Fungsi ini akan dipanggil saat collider (Is Trigger = true)
    // menyentuh collider LAIN (yang tidak harus Is Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Nanti kita akan beri tag "House" pada rumah
        if (other.CompareTag("House"))
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("DropSuccess");

            Debug.Log("Paket Kena Rumah! +10 Skor"); 
            
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(10); // Tambah 10 poin
            }

            Destroy(gameObject); // Hancurkan paket
        }
    }

    // Fungsi ini dipanggil jika paket keluar dari layar (view) kamera
    private void OnBecameInvisible()
    {
        // Hancurkan paket agar tidak menumpuk
        Destroy(gameObject, 1f); // Diberi jeda 1 detik untuk aman
    }
}