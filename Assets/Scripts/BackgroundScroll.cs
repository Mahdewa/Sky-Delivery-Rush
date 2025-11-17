using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Tooltip("Pengali kecepatan gameplay. 1 = sama cepat dgn game. 0.5 = setengah kecepatan (parallax).")]
    public float parallaxMultiplier = 1f;

    [Tooltip("Kecepatan saat di Main Menu (idle). Biarkan 0 jika ingin diam.")]
    public float idleScrollSpeed = 0.5f; // <-- VARIABEL BARU

    private float length;
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        // Variabel untuk menampung kecepatan dan waktu
        float speedToUse;
        float timeToUse;

        // Cek apakah game sedang berjalan atau di-pause (Main Menu/Game Over)
        if (Time.timeScale == 0f)
        {
            // Game sedang di-pause: Gunakan kecepatan IDLE
            speedToUse = idleScrollSpeed;
            timeToUse = Time.unscaledDeltaTime; // Waktu yang mengabaikan pause
        }
        else
        {
            // Game sedang berjalan: Gunakan kecepatan dari GameManager
            speedToUse = GameManager.Instance.currentScrollSpeed;
            timeToUse = Time.deltaTime; // Waktu normal
        }

        // 1. Hitung kecepatan akhir (sudah termasuk parallax)
        float finalSpeed = speedToUse * parallaxMultiplier;

        // 2. Gerakkan layer ini (menggunakan variabel yang sudah kita tentukan)
        transform.position += Vector3.left * finalSpeed * timeToUse;

        // 3. Looping (logika Anda)
        if (cam.position.x - transform.position.x >= length)
        {
            transform.position += Vector3.right * length * 2f;
        }
    }
}