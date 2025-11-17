using UnityEngine;

public class HouseMovement : MonoBehaviour
{
    [Tooltip("Kecepatan saat di Main Menu/Game Over. HARUS SAMA dengan idleScrollSpeed di Background tanah.")]
    public float idleScrollSpeed = 1f; // Atur kecepatan idle di sini

    [Tooltip("Batas hancur di kiri layar")]
    public float destroyPositionX = -20f;

    void Update()
    {
        float speedToUse;
        float timeToUse;

        // Cek apakah game sedang di-pause (MainMenu ATAU GameOver)
        if (Time.timeScale == 0f)
        {
            // Game sedang di-pause: Gunakan kecepatan IDLE
            speedToUse = idleScrollSpeed;
            timeToUse = Time.unscaledDeltaTime; // Waktu yang mengabaikan pause
        }
        else // (Berarti Time.timeScale adalah 1, game sedang Playing)
        {
            // Game sedang berjalan: Gunakan kecepatan dari GameManager
            speedToUse = GameManager.Instance.currentScrollSpeed;
            timeToUse = Time.deltaTime; // Waktu normal
        }

        // Bergerak ke kiri (Vector3.left)
        transform.Translate(Vector3.left * speedToUse * timeToUse);

        // Hancurkan rumah jika sudah terlalu jauh
        if (transform.position.x < destroyPositionX)
        {
            Destroy(gameObject);
        }
    }
}