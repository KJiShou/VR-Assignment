using UnityEngine;
using System.Collections.Generic;

public class FallingObjectSpawner : MonoBehaviour
{
    public static FallingObjectSpawner Instance { get; private set; }

    [Header("Prefab List")]
    public List<GameObject> fallingObjectPrefabs = new();

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float spawnHeightAbovePlayer = 8f;
    public float spawnRadius = 3f;
    public int poolSize = 10;
    public float objectLifetime = 10f;

    [Header("Target")]
    public Transform playerTransform;
    public Player player;
    public StaminaManager staminaManager;

    [Header("Auto Start")]
    public bool autoStartOnAwake = true;

    private float _spawnTimer = 0f;
    private bool _isSpawning = false;
    private readonly Queue<GameObject> _pool = new();
    private readonly List<GameObject> _activeObjects = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePool();

        if (autoStartOnAwake)
        {
            StartSpawning();
        }
    }

    void Start()
    {
        FindStaminaManager();
    }

    void Update()
    {
        if (!_isSpawning) return;

        if (player != null && player.IsWin)
        {
            StopSpawning();
            return;
        }

        if (playerTransform == null)
        {
            FindPlayer();
        }

        if (staminaManager == null)
        {
            FindStaminaManager();
        }

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= spawnInterval && IsPlayerClimbing())
        {
            _spawnTimer = 0f;
            SpawnObject();
        }

        UpdateActiveObjects();
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            CharacterController cc = FindFirstObjectByType<CharacterController>();
            if (cc != null)
            {
                playerObj = cc.gameObject;
            }
        }
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            player = playerObj.GetComponent<Player>();
            Debug.Log("[FallingObjectSpawner] Player found: " + playerObj.name);
        }
    }

    private void FindStaminaManager()
    {
        if (staminaManager == null)
        {
            staminaManager = FindFirstObjectByType<StaminaManager>();
        }
    }

    private bool IsPlayerClimbing()
    {
        return staminaManager != null
            && (staminaManager.isLeftHolding || staminaManager.isRightHolding);
    }

    private void InitializePool()
    {
        if (fallingObjectPrefabs == null || fallingObjectPrefabs.Count == 0)
        {
            Debug.LogWarning("[FallingObjectSpawner] No falling object prefabs assigned!");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = CreateNewObject();
            if (obj != null)
            {
                obj.SetActive(false);
                _pool.Enqueue(obj);
            }
        }
    }

    private GameObject CreateNewObject()
    {
        if (fallingObjectPrefabs == null || fallingObjectPrefabs.Count == 0)
        {
            Debug.LogWarning("[FallingObjectSpawner] No falling object prefabs assigned!");
            return null;
        }

        GameObject prefab = fallingObjectPrefabs[Random.Range(0, fallingObjectPrefabs.Count)];
        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        obj.name = "FallingObject_Pooled";
        return obj;
    }

    private void SpawnObject()
    {
        GameObject obj = GetFromPool();
        if (obj == null) return;

        Vector3 spawnPos = GetSpawnPosition();
        obj.transform.position = spawnPos;
        obj.SetActive(true);

        FallingObject fo = obj.GetComponent<FallingObject>();
        fo?.Reset();
    }

    private GameObject GetFromPool()
    {
        if (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue();
            _activeObjects.Add(obj);
            return obj;
        }

        if (_activeObjects.Count >= poolSize)
        {
            GameObject oldest = _activeObjects[0];
            _activeObjects.RemoveAt(0);
            oldest.SetActive(false);
            _activeObjects.Add(oldest);
            return oldest;
        }

        GameObject newObj = CreateNewObject();
        _activeObjects.Add(newObj);
        return newObj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        _activeObjects.Remove(obj);
        _pool.Enqueue(obj);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 playerPos = playerTransform.position;

        float randomX = Random.Range(-spawnRadius, spawnRadius);
        float randomZ = Random.Range(-spawnRadius, spawnRadius);

        return new Vector3(
            playerPos.x + randomX,
            playerPos.y + spawnHeightAbovePlayer,
            playerPos.z + randomZ
        );
    }

    private void UpdateActiveObjects()
    {
        for (int i = _activeObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = _activeObjects[i];
            if (obj == null)
            {
                _activeObjects.RemoveAt(i);
                continue;
            }

            if (!obj.activeSelf)
            {
                _activeObjects.RemoveAt(i);
                _pool.Enqueue(obj);
            }
        }
    }

    public void StartSpawning()
    {
        _isSpawning = true;
        _spawnTimer = 0f;
    }

    public void StopSpawning()
    {
        _isSpawning = false;
    }

    public bool IsSpawning => _isSpawning;
}
