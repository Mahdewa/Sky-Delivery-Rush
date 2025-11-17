using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public enum Platform { Desktop, Mobile }

public class GameManager : MonoBehaviour
{
    // Singleton pattern
    public static GameManager Instance { get; private set; }
    public static Platform selectedPlatform;

    [Header("Wind Mechanic")]
    public float minWindDelay = 10f; // Waktu minimal antar angin
    public float maxWindDelay = 20f; // Waktu maksimal antar angin
    public float slowSpeed = 1.5f;   // Kecepatan saat lambat [cite: 47]
    public float fastSpeed = 5f;
    public GameObject windEffectFastVisual; // Visual untuk angin cepat (kiri ke kanan)
    public GameObject windEffectSlowVisual;

    [Header("Difficulty Curve")]
    public float gameTime = 0f;
    public int difficultyLevel = 1; // 1=Early, 2=Mid, 3=Late
    public float midGameTime = 60f; // Kapan masuk Mid Game (detik) 
    public float lateGameTime = 180f;

    public float currentScrollSpeed = 3f; // Kecepatan game saat ini
    private float defaultScrollSpeed = 3f; // Kecepatan normal

    private float defaultMinWindDelay;
    private float defaultMaxWindDelay;

    public int score = 0;
    public int currentPackages = 20;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject platformSelectPanel;
    public GameObject hudPanel; // Panel yang berisi Score, Paket, dll.
    public GameObject mobileControlsPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("Gameplay Objects")]
    public GameObject playerObject; // Prefab PlayerBalloon Anda
    public Transform playerSpawnPoint; // Titik spawn (Empty GameObject)

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI packageText; // Slot untuk UI Text dari Inspector

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Simpan nilai default dari Inspector
        defaultScrollSpeed = currentScrollSpeed;
        defaultMinWindDelay = minWindDelay;
        defaultMaxWindDelay = maxWindDelay;

        // 1. HENTIKAN GAME (PAUSE)
        Time.timeScale = 0f;

        // 2. TAMPILKAN MAIN MENU
        mainMenuPanel.SetActive(true);

        // 3. SEMBUNYIKAN UI LAINNYA
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // if (playerObject != null)
        // {
        //     playerObject.SetActive(false);
        // }

        // Pastikan efek angin mati
        if (windEffectFastVisual != null) windEffectFastVisual.SetActive(false);
        if (windEffectSlowVisual != null) windEffectSlowVisual.SetActive(false);
    }

    public void ShowPlatformSelect()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Button");

        mainMenuPanel.SetActive(false);
        platformSelectPanel.SetActive(true);
    }

    // Fungsi ini akan dipanggil oleh tombol Desktop/Mobile
    public void SelectPlatform(int platformIndex)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Button");
        
        // 0 = Desktop, 1 = Mobile
        selectedPlatform = (Platform)platformIndex; 
        
        // Sembunyikan panel pilihan dan mulai game
        platformSelectPanel.SetActive(false);
        StartGame(); // Panggil fungsi StartGame yang sudah ada
    }

    public void StartGame()
    {
        // 1. JALANKAN GAME
        Time.timeScale = 1f;

        // 2. TUKAR PANEL UI
        mainMenuPanel.SetActive(false);
        hudPanel.SetActive(true);
        gameOverPanel.SetActive(false); // Pastikan panel game over mati

        // 3. RESET SEMUA VARIABEL GAME
        score = 0;
        currentPackages = 20; // Atur ke jumlah paket awal Anda
        gameTime = 0f;
        difficultyLevel = 1;

        // Reset kecepatan & angin
        currentScrollSpeed = defaultScrollSpeed;
        minWindDelay = defaultMinWindDelay;
        maxWindDelay = defaultMaxWindDelay;
        
        // 4. MUNCULKAN PEMAIN
        if (playerObject != null)
        {
            // Pindahkan pemain ke posisi spawn
            playerObject.transform.position = playerSpawnPoint.position;
            // (Opsional) Reset rotasi jika perlu
            // playerObject.transform.rotation = playerSpawnPoint.rotation;
            
            // Aktifkan pemain
            playerObject.SetActive(true);
        }

        // 5. UPDATE UI
        UpdateScoreUI();
        UpdatePackageUI();

        // 6. MULAI GAME LOGIC
        StopAllCoroutines(); // Hentikan coroutine lama (penting untuk Retry)
        StartCoroutine(WindSpawnerRoutine()); // Mulai spawner angin

        if (selectedPlatform == Platform.Mobile)
        {
            mobileControlsPanel.SetActive(true);
        }
        else
        {
            mobileControlsPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // 1. Hitung waktu bermain
        gameTime += Time.deltaTime;

        // 2. Cek untuk menaikkan level kesulitan
        if (difficultyLevel == 1 && gameTime > midGameTime)
        {
            difficultyLevel = 2;
            Debug.Log("DIFFICULTY: Masuk MID GAME");
            
            minWindDelay = 8f;
            maxWindDelay = 15f;
        }
        else if (difficultyLevel == 2 && gameTime > lateGameTime)
        {
            difficultyLevel = 3;
            Debug.Log("DIFFICULTY: Masuk LATE GAME");

            // Angin lebih sering lagi
            minWindDelay = 5f;
            maxWindDelay = 10f;
            
            defaultScrollSpeed = 4.5f; 
            currentScrollSpeed = defaultScrollSpeed;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score " + score;
        }
    }

    public bool UsePackage()
    {
        if (currentPackages > 0)
        {
            currentPackages--;
            UpdatePackageUI();

            if (currentPackages <= 0)
            {
                HandleGameOver("Paket Habis!");
            }
            return true; // Berhasil menggunakan paket
        }
        else
        {
            // Sudah tidak punya paket
            return false; 
        }
    }

    // Fungsi baru untuk update UI Paket
    void UpdatePackageUI()
    {
        if (packageText != null)
        {
            packageText.text = "" + currentPackages;
        }
    }

    public void LosePackages(int amount)
    {
        currentPackages -= amount;
        if (currentPackages < 0)
        {
            currentPackages = 0;
        }
        
        UpdatePackageUI();

        if (currentPackages == 0)
        {
            HandleGameOver("Paket Habis Diterbangkan Angin!");
        }
    }

    void TriggerWindEvent()
    {
        // 1. Mengurangi Paket
        int packagesToLose = Random.Range(1, 4); // Menghasilkan 1, 2, atau 3 [cite: 48, 51]
        LosePackages(packagesToLose);
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Wind");
        Debug.Log("ANGIN! Kehilangan " + packagesToLose + " paket!");



        // 2. Mengubah Kecepatan
        float duration = Random.Range(2f, 4f); // Durasi angin 2-4 detik [cite: 47, 50]

        // 50/50 kemungkinan angin cepat atau lambat
        if (Random.value > 0.5f)
        {
            // Angin Kencang (Kiri ke Kanan)
            currentScrollSpeed = fastSpeed;
            Debug.Log("Angin kencang! Bergerak cepat!");
            
            // AKTIFKAN VISUAL CEPAT
            if (windEffectFastVisual != null) windEffectFastVisual.SetActive(true);
        }
        else
        {
            // Angin Lambat (Kanan ke Kiri)
            currentScrollSpeed = slowSpeed;
            Debug.Log("Angin haluan! Bergerak lambat!");
            
            // AKTIFKAN VISUAL LAMBAT
            if (windEffectSlowVisual != null) windEffectSlowVisual.SetActive(true);
        }

        // 3. Mulai timer untuk mengembalikan kecepatan ke normal
        StartCoroutine(ResetSpeedAfterDelay(duration));
    }

    IEnumerator ResetSpeedAfterDelay(float delay)
    {
        // Tunggu sesuai durasi angin
        yield return new WaitForSeconds(delay);

        // Kembalikan kecepatan ke normal
        currentScrollSpeed = defaultScrollSpeed;
        Debug.Log("Angin reda. Kecepatan normal.");

        if (windEffectFastVisual != null) windEffectFastVisual.SetActive(false);
        if (windEffectSlowVisual != null) windEffectSlowVisual.SetActive(false);
    }

    IEnumerator WindSpawnerRoutine()
    {
        // Tunggu beberapa detik sebelum angin pertama
        yield return new WaitForSeconds(minWindDelay);

        while (true)
        {
            // Picu event angin!
            TriggerWindEvent();

            // Tentukan jeda waktu acak sebelum angin berikutnya
            float randomDelay = Random.Range(minWindDelay, maxWindDelay);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    public void RetryGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Button");

        // 1. Sembunyikan panel game over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // 2. Tampilkan main menu
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        if (playerObject != null)
        {
            // Pindahkan pemain ke posisi spawn agar posisinya benar
            playerObject.transform.position = playerSpawnPoint.position;
            playerObject.SetActive(true);
        }
    }

    // Fungsi baru untuk Game Over
    public void HandleGameOver(string reason)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("GameOver");
        Debug.Log("GAME OVER: " + reason);
        
        // 1. HENTIKAN SEMUA PROSES GAME
        Time.timeScale = 0f; 
        StopAllCoroutines(); // Hentikan spawner angin dll.

        // 2. TUKAR PANEL UI
        hudPanel.SetActive(false); // Sembunyikan Skor/Paket

        if (finalScoreText != null)
        {
            finalScoreText.text = "Your Score " + score;
        }

        gameOverPanel.SetActive(true); 

        // 3. Hide pemain
        if (playerObject != null)
        {
            playerObject.SetActive(false);
        }
    }

    // Fungsi baru untuk menambah paket
    public void AddPackages(int amount)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Resupply");

        currentPackages += amount;
        UpdatePackageUI(); // Langsung update UI
        Debug.Log("PAKET BERTAMBAH! +" + amount);
    }

    public void QuitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("Button");
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }
}