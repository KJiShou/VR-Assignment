using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] CheckPointManager checkPointManager;
    public Transform respawnPoint;

    private CheckPointManager _manager;
    private int _myIndex;
    private bool _isActivated = false;

    // Called by Checkpoint manager when Awake()
    public void Setup(CheckPointManager manager, int index)
    {
        _manager = manager;
        _myIndex = index;

        // Forgot to put respawn point transform, then use its transform
        if (respawnPoint == null)
        {
            respawnPoint = transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isActivated) return;

        // Make sure put Player tag on XR Rig, not hand controller 
        if (other.CompareTag("Player"))
        {
            _isActivated = true;
            _manager.ActivateCheckPoint(_myIndex);
        }
    }
}
