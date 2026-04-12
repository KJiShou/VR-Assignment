using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;
using static Unity.Burst.Intrinsics.X86.Avx;

public class Player : MonoBehaviour
{
    public XRRayInteractor leftRayInteractor;
    public XRRayInteractor rightRayInteractor;

    public XRInteractorLineVisual leftLineVisual;
    public XRInteractorLineVisual rightLineVisual;
    public GameObject reticle;
    public GameObject blockedReticle;

    public XRDirectInteractor leftDirectInteractor;
    public XRDirectInteractor rightDirectInteractor;

    public CheckPointManager checkPointManager;

    public GameObject fadeCanvas;
    private FadeCanvas _fadeCanvas;
    public float transitionTime = 2.0f;

    public GameObject settingsPanel;

    [Header("Settings")]
    public float deathFallDistance = 5.0f; // Over this distance count as death

    [Tooltip("Respawn interval time")]
    public float respawnProtectionTime = 2.0f;

    private CharacterController _characterController;
    private ClimbProvider _climbProvider;

    private float _highestPointDuringFall;
    private bool _wasFallingLastFrame;
    private float _protectionTimer = 0f;

    private bool _isWin = false;

    [SerializeField] GameObject leftHandController;
    [SerializeField] TextMeshProUGUI winScreenTimeText;
    private GameObject timeText;

    private float currentTime = 0f;
    private bool isTimerRunning = false;

    #region Monobehaviour Methods
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _climbProvider = GetComponentInChildren<ClimbProvider>();

        if(fadeCanvas != null)
        {
            _fadeCanvas = fadeCanvas.GetComponent<FadeCanvas>();
        }
    }

    private void Start()
    {
        StartCoroutine(FindGameObject("ClimbTimer", obj => timeText = obj));
    }

    private void Update()
    {

        if (timeText != null && isTimerRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();
        }

        if (_protectionTimer > 0)
        {
            _protectionTimer -= Time.deltaTime;
            _highestPointDuringFall = transform.position.y;
        }

        // not on floor and not climbing
        bool isClimbing = _climbProvider != null && _climbProvider.locomotionState == LocomotionState.Moving;

        if (isClimbing && !_isWin && !isTimerRunning)
        {
            StartClimbingTimer();
        }

        if (isClimbing && settingsPanel.activeSelf) 
        {
            ToggleMenu();
        }

        if (_climbProvider != null && !isClimbing)
        {
            // if not climbing, force enable gravity, prevent floating
            _characterController.SimpleMove(Vector3.zero);
        }

        bool isGrounded = _characterController.isGrounded;
        bool isFalling = !isGrounded && !isClimbing && _characterController.velocity.y < -0.1f;

        if (isFalling)
        {
            if (!_wasFallingLastFrame)
            {
                // When start falling, record initial falling point
                _highestPointDuringFall = transform.position.y;
            }

            float currentFallDistance = _highestPointDuringFall - transform.position.y;

            if (currentFallDistance > deathFallDistance && _protectionTimer <= 0f)
            {
                TriggerDeath();
                return;
            }
        }
        else
        {
            // fall on the floor or restart climbing
            _wasFallingLastFrame = false;
        }

        _wasFallingLastFrame = isFalling;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TopPoint")
        {
            _isWin = true;
            StopClimbingTimer();
            if (leftRayInteractor != null && rightRayInteractor != null)
            {
                leftRayInteractor.enabled = true;
                rightRayInteractor.enabled = true;
            }
        }
    }
    #endregion

    #region Private Methods
    private void TriggerDeath()
    {
        Debug.Log("<color=red>[DEATH]</color> Fall distance greater than Death distance");
        _wasFallingLastFrame = false;
        _protectionTimer = respawnProtectionTime;

        StartCoroutine(DeathAndRespawnSequence());
    }

    private IEnumerator DeathAndRespawnSequence()
    {
        if (_fadeCanvas != null)
        {
            _fadeCanvas.StartFadeIn();
            yield return new WaitForSeconds(_fadeCanvas.defaultDuration);
        }

        if (checkPointManager != null && checkPointManager.currentCheckPointIndex != -1)
        {
            Transform targetSpawn = checkPointManager.checkPoints[checkPointManager.currentCheckPointIndex].respawnPoint;
            if (_characterController != null)
            {
                _characterController.enabled = false;
            }

            transform.position = targetSpawn.position;
            transform.rotation = targetSpawn.rotation;

            if (_characterController != null)
            {
                _characterController.enabled = true;
            }
        }

        yield return new WaitForSeconds(transitionTime);

        if (_fadeCanvas != null)
        {
            _fadeCanvas.StartFadeOut();
        }
    }

    private GameObject FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName) return child.gameObject;
            GameObject result = FindDeepChild(child, childName);
            if (result != null) return result;
        }
        return null;
    }

    private IEnumerator FindGameObject(string objectName, System.Action<GameObject> saveFoundObject)
    {
        GameObject foundObj = null;
        float timeout = 3.0f; // Maximum wait for 3 sec
        float timer = 0f;

        // time not reach time out, then keep finding
        while (timer < timeout)
        {
            foundObj = FindDeepChild(this.transform, objectName);

            if (foundObj != null)
            {
                break; // if found, directly break the loop
            }

            timer += Time.deltaTime;

            // Wait for next frame
            yield return null;
        }

        if (foundObj == null)
        {
            Debug.LogError($"<color=red>[Time Out Error] Already wait for {timeout} sec, model still haven't instantiate! Can't found {objectName} !" +
                $" Please check the prefab is correctly loaded or not!</color>");
            yield break; // End coroutine
        }

        // Assign value to the variable
        saveFoundObject?.Invoke(foundObj);
    }

    private void StartClimbingTimer()
    {
        currentTime = 0f;
        isTimerRunning = true;
        Debug.Log("<color=cyan>Climbing timer start to count</color>");
    }

    private void StopClimbingTimer()
    {
        isTimerRunning = false;
        Debug.Log($"<color=cyan>Climbing timer stopped! Final use time: {GetFormattedTime()}</color>");
    }

    private void ResumeTimer()
    {
        isTimerRunning = true;
    }

    private void UpdateTimerDisplay()
    {
        if (timeText != null && winScreenTimeText != null)
        {
            string text = GetFormattedTime();
            timeText.GetComponent<TextMeshProUGUI>().text = text;
            winScreenTimeText.text = "Used Time: " + text;
        }
    }

    /// <summary>
    /// Get the format time string
    /// </summary>
    /// <returns>time string 00:00.00</returns>
    private string GetFormattedTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        return time.ToString(@"mm\:ss\.ff");
    }
    #endregion

    #region Public Methods
    public void TeleportToWinScreen(Transform target)
    {
        StartCoroutine(Teleport(target));
    }

    public IEnumerator Teleport(Transform target)
    {
        Debug.Log("TeleportToSpecificPoint()");
        if (_fadeCanvas != null)
        {
            _fadeCanvas.StartFadeIn();
            yield return new WaitForSeconds(_fadeCanvas.defaultDuration);
        }

        if (_characterController != null)
        {
            _characterController.enabled = false;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;

        if (_characterController != null)
        {
            _characterController.enabled = true;
        }

        yield return new WaitForSeconds(transitionTime);

        if (_fadeCanvas != null)
        {
            _fadeCanvas.StartFadeOut();
        }
    }

    public void ToggleMenu()
    {
        Debug.Log("<color=red>Left Hand Menu button pressed!</color>");

        if (settingsPanel == null)
        {
            Debug.LogWarning("ToggleMenu called, but settings panel is empty, please check Inspector.");
            return;
        }

        bool isActive = settingsPanel.activeSelf;
        bool newState = !isActive;

        settingsPanel.SetActive(newState);

        if (!_isWin)
        {
            if (leftRayInteractor != null) leftRayInteractor.enabled = newState;
            if (rightRayInteractor != null) rightRayInteractor.enabled = newState;
        }
    }
    #endregion
}
