using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxLeftStamina = 100f;
    public float maxRightStamina = 100f;

    [Header("Current Stamina")]
    public float currentLeftStamina;
    public float currentRightStamina;

    [Header("Holding State")]
    public bool isLeftHolding;
    public bool isRightHolding;

    private float leftDrainRate;
    private float rightDrainRate;
    private float increaseRate = 1;

    private ClimbObject currentLeftClimbObject;
    private ClimbObject currentRightClimbObject;

    void Start()
    {
        currentLeftStamina = maxLeftStamina;
        currentRightStamina = maxRightStamina;
    }

    void Update()
    {
        DrainStaminaOverTime();
        CheckAutoDrop();
        Debug.Log(currentLeftStamina);
    }

    private void DrainStaminaOverTime()
    {
        if (isLeftHolding)
        {
            currentLeftStamina -= leftDrainRate * Time.deltaTime;
            currentLeftStamina = Mathf.Clamp(currentLeftStamina, 0f, maxLeftStamina);
        } 
        else
        {
            currentLeftStamina += increaseRate * Time.deltaTime;
            currentLeftStamina = Mathf.Clamp(currentLeftStamina, 0f, maxLeftStamina);
        }

        if (isRightHolding)
        {
            currentRightStamina -= rightDrainRate * Time.deltaTime;
            currentRightStamina = Mathf.Clamp(currentRightStamina, 0f, maxRightStamina);
        }
        else
        {
            currentRightStamina += increaseRate * Time.deltaTime;
            currentRightStamina = Mathf.Clamp(currentRightStamina, 0f, maxLeftStamina);
        }
    }

    private void CheckAutoDrop()
    {
        if (isLeftHolding && currentLeftStamina <= 0f)
        {
            currentLeftClimbObject?.ForceReleaseLeftHand();
        }

        if (isRightHolding && currentRightStamina <= 0f)
        {
            currentRightClimbObject?.ForceReleaseRightHand();
        }
    }

    public void StartHoldingLeft(float drainRate, ClimbObject climbObject)
    {
        isLeftHolding = true;
        leftDrainRate = drainRate;
        currentLeftClimbObject = climbObject;
    }

    public void StopHoldingLeft(ClimbObject climbObject)
    {
        if (currentLeftClimbObject == climbObject)
        {
            isLeftHolding = false;
            leftDrainRate = 0f;
            currentLeftClimbObject = null;
        }
    }

    public void StartHoldingRight(float drainRate, ClimbObject climbObject)
    {
        isRightHolding = true;
        rightDrainRate = drainRate;
        currentRightClimbObject = climbObject;
    }

    public void StopHoldingRight(ClimbObject climbObject)
    {
        if (currentRightClimbObject == climbObject)
        {
            isRightHolding = false;
            rightDrainRate = 0f;
            currentRightClimbObject = null;
        }
    }

    public void AddLeftStamina(float amount)
    {
        currentLeftStamina = Mathf.Clamp(currentLeftStamina + amount, 0f, maxLeftStamina);
    }

    public void AddRightStamina(float amount)
    {
        currentRightStamina = Mathf.Clamp(currentRightStamina + amount, 0f, maxRightStamina);
    }
}