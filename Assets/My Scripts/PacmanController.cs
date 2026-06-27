using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    private Transform visualTransform;
    public float chompSpeed = 15f;       // Kecepatan mengunyah
    public float minScaleY = 0.4f;       // Skala minimum saat mulut menutup/mengempis
    
    private Vector3 originalScale;
    private bool isMoving = false;       // Tautkan ini dengan input pergerakanmu

    void Start()
    {
        // Mengambil objek anak ("Visual")
        visualTransform = transform.Find("Visual");
        if (visualTransform != null)
        {
            originalScale = visualTransform.localScale;
        }
    }

    void Update()
    {
        // Contoh logika deteksi gerak (sesuaikan dengan script pergerakan kelompokmu)
        isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

        if (isMoving && visualTransform != null)
        {
            // Membuat efek naik-turun skala menggunakan fungsi Math Sinus (ping-pong)
            float scaleY = Mathf.PingPong(Time.time * chompSpeed, 1f - minScaleY) + minScaleY;
            
            // Terapkan perubahan skala pada sumbu Y objek visual
            visualTransform.localScale = new Vector3(originalScale.x, originalScale.y * scaleY, originalScale.z);
        }
        else if (visualTransform != null)
        {
            // Jika berhenti, kembalikan ke skala lingkaran utuh semula
            visualTransform.localScale = originalScale;
        }
    }
    
    public float speed = 5f;            // Kecepatan gerak Pac-Man
    public LayerMask obstacleLayer;    // Layer khusus untuk Dinding Labirin (Tilemap)
    
    private Vector2 currentDirection;   // Arah gerak Pac-Man saat ini
    private Vector2 nextDirection;      // Antrean arah gerak berikutnya dari input pemain
    private Transform visualTransform;  // Mengambil komponen anak (Visual) untuk dirotasi

    void Start()
    {
        // Secara otomatis mengambil objek anak bernama "Visual" untuk kebutuhan rotasi arah hadap
        visualTransform = transform.Find("Visual");
        
        // Di awal game, Pac-Man langsung bergerak ke kiri
        currentDirection = Vector2.left;
    }

    void Update()
    {
        // 1. MEMBACA INPUT DARI KEYBOARD
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            nextDirection = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            nextDirection = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            nextDirection = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            nextDirection = Vector2.right;
        }

        // 2. CEK ANTREAN ARAH BARU (Apakah bisa langsung berbelok?)
        // Jika arah baru yang diinginkan pemain TIDAK terhalang dinding, ubah arah saat ini
        if (nextDirection != currentDirection)
        {
            if (!CheckWallInDirection(nextDirection))
            {
                currentDirection = nextDirection;
                RotateVisual(currentDirection); // Putar gambar Pac-Man
            }
        }

        // 3. EKSEKUSI PERGERAKAN JIKA JALAN DI DEPAN KOSONG
        if (!CheckWallInDirection(currentDirection))
        {
            transform.Translate(currentDirection * speed * Time.deltaTime);
        }
    }

    // INTERFASIAL FUNGSI SENSOR RAYCAST 2D (Paling Penting!)
    bool CheckWallInDirection(Vector2 direction)
    {
        // Tembakkan garis sensor pendek sejauh 0.55 unit dari pusat Pac-Man ke arah tujuan
        // Gunakan obstacleLayer agar sensor hanya mendeteksi collider milik dinding
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.55f, obstacleLayer);
        
        // Jika hit.collider tidak null, artinya di depan ADA dinding (Return True)
        return hit.collider != null;
    }

    // LOGIKA MEMUTAR ARAH VISUAL PAC-MAN (ANAK HIERARCHY)
    void RotateVisual(Vector2 direction)
    {
        if (visualTransform == null) return;

        float angle = 0f;

        if (direction == Vector2.up) angle = 90f;
        else if (direction == Vector2.left) angle = 180f;
        else if (direction == Vector2.down) angle = 270f;
        else if (direction == Vector2.right) angle = 0f;

        // Hanya memutar objek anak ("Visual"), objek induk tetap lurus tegak
        visualTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}