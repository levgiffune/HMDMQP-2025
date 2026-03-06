using System;
using UnityEngine;
using TMPro;
using Meta.WitAi.TTS.Utilities;

/// <summary>
/// Reusable popup card for tour transitions and announcements.
/// World-space Canvas that positions in front of the camera.
/// </summary>
public class PopupCard : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public GameObject aButtonPrompt;
    public TextMeshProUGUI aButtonText;
    public TTSSpeaker speaker;

    [Header("Positioning")]
    public float distanceFromCamera = 1.5f;
    public float heightOffset = 0f;

    private Transform cameraTransform;
    private Canvas canvas;
    private Action onACallback;
    private bool isVisible = false;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
        }

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
        }

        // Start hidden
        SetVisible(false);
    }

    void Update()
    {
        if (!isVisible) return;

        // Position in front of camera
        if (cameraTransform != null)
        {
            Vector3 targetPos = cameraTransform.position +
                cameraTransform.forward * distanceFromCamera +
                Vector3.up * heightOffset;
            transform.position = targetPos;
            transform.LookAt(cameraTransform);
            transform.Rotate(0, 180, 0);
        }

        // B to close (only for informational popups — action popups require A)
        if (onACallback == null && OVRInput.GetDown(OVRInput.Button.Two))
        {
            Close();
        }

        // A for action (if callback set)
        if (onACallback != null && OVRInput.GetDown(OVRInput.Button.One))
        {
            Action callback = onACallback;
            Close();
            callback.Invoke();
        }
    }

    /// <summary>
    /// Show the popup with title and message. Optional A-button action.
    /// </summary>
    public void Show(string title, string message, Action onA = null, string aText = "Press A to continue")
    {
        if (titleText != null) titleText.text = title;
        if (messageText != null) messageText.text = message;

        onACallback = onA;

        if (aButtonPrompt != null)
        {
            aButtonPrompt.SetActive(onA != null);
        }
        if (aButtonText != null && onA != null)
        {
            aButtonText.text = aText;
        }

        SetVisible(true);
        speaker.Speak(message);
    }

    public void Close()
    {
        onACallback = null;
        SetVisible(false);
        speaker.Stop();
    }

    private void SetVisible(bool visible)
    {
        isVisible = visible;
        if (canvas != null) canvas.enabled = visible;
    }
}
