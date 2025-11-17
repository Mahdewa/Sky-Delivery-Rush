using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Tooltip("Pengali kecepatan. 1 = sama cepat dgn game. 0.5 = setengah kecepatan (parallax)")]
    public float parallaxMultiplier = 1f; // <-- VARIABEL BARU

    private float length;
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        // 1. Ambil kecepatan dasar dari GameManager
        float baseSpeed = GameManager.Instance.currentScrollSpeed;

        // 2. Hitung kecepatan layer ini (ini adalah permintaan Anda)
        float speed = baseSpeed * parallaxMultiplier; 

        // 3. Gerakkan layer ini (logika Anda)
        transform.position += Vector3.left * speed * Time.deltaTime;

        // 4. Looping (logika Anda)
        if (cam.position.x - transform.position.x >= length)
        {
            transform.position += Vector3.right * length * 2f;
        }
    }
}