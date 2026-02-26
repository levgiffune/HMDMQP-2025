using System;
using UnityEngine;
using TMPro;

/// <summary>
/// First-time intro screen. Shows controls overview and lets user choose Tour or Free Roam.
/// </summary>
public class IntroCard : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI controlsText;
    public GameObject tourOption;
    public GameObject freeRoamOption;
    public TextMeshProUGUI tourLabel;
    public TextMeshProUGUI freeRoamLabel;

    [Header("Selection Visuals")]
    public Color highlightColor = Color.yellow;
    public Color normalColor = Color.white;

    [Header("Positioning")]
    public float distanceFromCamera = 1.5f;
    public float heightOffset = 0f;

    private Transform cameraTransform;
    private Canvas canvas;
    private int selectedOption = 0; // 0 = Tour, 1 = Free Roam
    private float thumbstickCooldown = 0f;
    private const float COOLDOWN_TIME = 0.3f;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }
    }

    void Start()
    {
        cameraTransform = Camera.main?.transform;
        UpdateSelectionVisuals();
    }

    void OnEnable()
    {
        if (canvas != null) canvas.enabled = true;
        selectedOption = 0;
        UpdateSelectionVisuals();
    }

    void Update()
    {
        if (canvas == null || !canvas.enabled) return;

        // Position in front of camera
        if (cameraTransform != null)
        {
            Vector3 pos = cameraTransform.position +
                cameraTransform.forward * distanceFromCamera +
                Vector3.up * heightOffset;
            transform.position = pos;
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0);
        }

        // Handle input
        HandleNavigation();

        // A to confirm selection
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            ConfirmSelection();
        }

        // B to close (after selection is made — acts as "dismiss")
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            // B just closes if they've already seen the controls
            gameObject.SetActive(false);
        }
    }

    private void HandleNavigation()
    {
        if (thumbstickCooldown > 0)
        {
            thumbstickCooldown -= Time.deltaTime;
            return;
        }

        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        if (thumbstick.y > 0.5f || thumbstick.y < -0.5f)
        {
            selectedOption = (selectedOption == 0) ? 1 : 0;
            thumbstickCooldown = COOLDOWN_TIME;
            UpdateSelectionVisuals();
        }
    }

    private void UpdateSelectionVisuals()
    {
        if (tourLabel != null) tourLabel.color = (selectedOption == 0) ? highlightColor : normalColor;
        if (freeRoamLabel != null) freeRoamLabel.color = (selectedOption == 1) ? highlightColor : normalColor;
    }

    private void ConfirmSelection()
    {
        if (GameModeManager.Instance == null) return;

        GameMode mode = (selectedOption == 0) ? GameMode.Tour : GameMode.FreeRoam;
        GameModeManager.Instance.SetMode(mode);

        gameObject.SetActive(false);
    }
}
