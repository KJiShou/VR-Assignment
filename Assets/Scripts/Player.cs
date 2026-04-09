using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

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

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _climbProvider = GetComponentInChildren<ClimbProvider>();

        if(fadeCanvas != null)
        {
            _fadeCanvas = fadeCanvas.GetComponent<FadeCanvas>();
        }
    }

    private void Update()
    {
        //if (leftDirectInteractor != null && leftRayInteractor != null)
        //{
        //    leftRayInteractor.enabled = !leftDirectInteractor.hasSelection;
        //}

        //if (rightDirectInteractor != null && rightRayInteractor != null)
        //{
        //    rightRayInteractor.enabled = !rightDirectInteractor.hasSelection;
        //}

        if (_protectionTimer > 0)
        {
            _protectionTimer -= Time.deltaTime;
            _highestPointDuringFall = transform.position.y;
        }

        // not on floor and not climbing
        bool isClimbing = _climbProvider != null && _climbProvider.locomotionState == LocomotionState.Moving;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TopPoint")
        {
            _isWin = true;
            if (leftRayInteractor != null && rightRayInteractor != null)
            {
                leftRayInteractor.enabled = true;
                rightRayInteractor.enabled = true;
            }
        }
    }

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

            //if (leftLineVisual != null && rightLineVisual != null)
            //{
            //    if (newState == true)
            //    {
            //        leftLineVisual.reticle = null;
            //        leftLineVisual.blockedReticle = null;
            //        rightLineVisual.reticle = null;
            //        rightLineVisual.blockedReticle = null;
            //    }
            //    else
            //    {
            //        leftLineVisual.reticle = reticle;
            //        leftLineVisual.blockedReticle = blockedReticle;
            //        rightLineVisual.reticle = reticle;
            //        rightLineVisual.blockedReticle = blockedReticle;
            //    }
            //}
        }
    }

}
