using UnityEngine;
using UnityEngine.SceneManagement; // Penting untuk pindah Scene

public class MainMenu : MonoBehaviour
{
    // Pastikan nama "GameScene" sama dengan nama Scene game Anda
    public string gameSceneName = "GameScene"; 

    // Fungsi ini akan dipanggil oleh tombol "Start"
    public void StartGame()
    {
        // Memuat scene game utama
        // Pastikan Anda sudah menambahkan scene ini ke Build Settings!
        SceneManager.LoadScene(gameSceneName);
    }

    // Fungsi ini akan dipanggil oleh tombol "Settings"
    public void OpenSettings()
    {
        // (Logika untuk menampilkan panel settings)
        Debug.Log("Tombol Settings ditekan!");
    }

    // Fungsi ini akan dipanggil oleh tombol "Exit"
    public void QuitGame()
    {
        // Ini hanya berfungsi di build game (PC/Mobile),
        // tidak akan bekerja di dalam Editor Unity.
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }
}