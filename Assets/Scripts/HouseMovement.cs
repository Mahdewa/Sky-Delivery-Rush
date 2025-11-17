using UnityEngine;

public class HouseMovement : MonoBehaviour
{
    void Update()
    {
        float speed = GameManager.Instance.currentScrollSpeed;

        // Bergerak ke kiri (Vector3.left)
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Hancurkan rumah jika sudah terlalu jauh keluar layar di sebelah kiri
        // (Angka -20f ini bisa disesuaikan)
        if (transform.position.x < -20f)
        {
            Destroy(gameObject);
        }
    }
}