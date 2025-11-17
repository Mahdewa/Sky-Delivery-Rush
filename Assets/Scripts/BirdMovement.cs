using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    // Kecepatan burung (HARUS lebih besar dari kecepatan scroll)
    [SerializeField] private float birdSpeed = 5f; 

    void Update()
    {
        // Bergerak ke kiri (Vector3.left)
        transform.Translate(Vector3.left * birdSpeed * Time.deltaTime);

        // Hancurkan burung jika sudah keluar layar di kiri
        if (transform.position.x < -20f)
        {
            Destroy(gameObject);
        }
    }
}