using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }
    private bool isLoweringButtonHeld = false;

    [Header("Altitude Positions")]
    public float highY = 2f;
    public float midY = 0f;
    public float lowY = -2f;

    [Header("Movement Speeds")]
    public float moveSpeed = 5f;       // speed turun-naik

    [Header("Package Spawning")]
    public GameObject packagePrefab; // Untuk menampung prefab Package
    public Transform dropPoint;      // Untuk menampung objek DropPoint
    public KeyCode dropKey = KeyCode.Mouse0; // Mouse Kiri (sesuai GDD "Tap Kanan" [cite: 17])

    private float targetY;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // default position = High
        targetY = highY;
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            HandleAltitudeInput(); // Ini akan menjalankan 'return'
            MoveToTargetAltitude();
            return;
        }

        // Hanya jalankan input keyboard/mouse jika platform Desktop
        if (GameManager.selectedPlatform == Platform.Desktop)
        {
            HandleAltitudeInput();
            HandlePackageDrop();
        }
        else // (Berarti platformnya Mobile)
        {
            // TAMBAHKAN FUNGSI BARU INI:
            HandleMobileAltitudeInput();
        }

        // Pergerakan visual balon (Lerp) harus selalu berjalan
        MoveToTargetAltitude();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah yang kita tabrak punya tag "Bird"
        if (other.CompareTag("Bird"))
        {
            // GAME OVER
            // Kita hancurkan burungnya agar terlihat meledak
            Destroy(other.gameObject); 

            // Panggil Game Over di GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.HandleGameOver("Menabrak Burung!");
            }

            // Hancurkan pemain (atau mainkan animasi hancur)
            // Destroy(gameObject); 
        }
    }

    void HandleAltitudeInput()
    {
        if (Time.timeScale == 0f)
        {
            targetY = highY;
            return; // Langsung keluar
        }

        if (Input.GetKey(KeyCode.Space)) // placeholder untuk tombol
        {
            // Jika lagi di High → turun ke Mid
            // Jika sudah Mid → turun ke Low
            if (Mathf.Approximately(targetY, highY))
                targetY = midY;
            else if (Mathf.Approximately(targetY, midY))
                targetY = lowY;
        }
        else
        {
            // Tidak menekan tombol → kembali ke High
            targetY = highY;
        }
    }

    void MoveToTargetAltitude()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * moveSpeed);
        transform.position = pos;
    }

    void HandlePackageDrop()
    {
        if (Time.timeScale == 0f)
        {
            return; // Langsung keluar dari fungsi ini
        }

        // Cek input untuk drop paket (Tap)
        if (Input.GetKeyDown(dropKey))
        {
            DropPackage();
        }
    }

    void DropPackage()
    {
        if (packagePrefab != null && dropPoint != null)
        {
            // MINTA IZIN DULU KE GAMEMANAGER
            if (GameManager.Instance != null && GameManager.Instance.UsePackage())
            {
                // Jika diizinkan (paket masih ada), baru buat paketnya
                Instantiate(packagePrefab, dropPoint.position, dropPoint.rotation);
            }
            else
            {
                // Tidak bisa drop (paket habis)
                Debug.Log("Tidak bisa drop, paket habis!");
                // Nanti bisa tambahkan SFX "error/kosong" di sini
            }
        }
        else
        {
            Debug.LogWarning("Package Prefab atau Drop Point belum di-set di Inspector!");
        }
    }

    public void OnDropButtonPress()
    {
        if (Time.timeScale == 0f) return;
        DropPackage(); // Panggil fungsi private drop Anda
    }

    // Dipanggil saat tombol 'Lower' DITEKAN (PointerDown)
    public void OnLowerButtonDown()
    {
        if (Time.timeScale == 0f) return;

        isLoweringButtonHeld = true;
    }

    // Dipanggil saat tombol 'Lower' DILEPAS (PointerUp)
    public void OnLowerButtonUp()
    {
        if (Time.timeScale == 0f) return;
        
        isLoweringButtonHeld = false;
    }

    void HandleMobileAltitudeInput()
    {
        // Jika tombol sedang ditahan
        if (isLoweringButtonHeld)
        {
            // Langsung set target ke paling bawah
            targetY = lowY;
        }
        else
        {
            // Jika tombol dilepas, kembali ke atas
            targetY = highY;
        }
    }
}
