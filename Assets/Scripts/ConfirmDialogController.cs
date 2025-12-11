using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ConfirmDialogController : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup canvasGroup; // on ConfirmDialogRoot
    public GameObject dialogRoot;   // same object, used to SetActive
    public Image backgroundDim;     // fullscreen dim image
    public RectTransform dialogBox; // DialogBox rect
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public Button yesButton;
    public Button noButton;

    [Header("Animation Settings")]
    public float fadeDuration = 0.18f;
    public float scaleDuration = 0.12f;
    public Vector3 popScale = new Vector3(1.03f, 1.03f, 1f);

    // internal
    private Action onConfirm;
    private Action onCancel;
    private Coroutine running;

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        dialogRoot.SetActive(false);
        // Wire default callbacks for buttons
        yesButton.onClick.AddListener(OnYesClicked);
        noButton.onClick.AddListener(OnNoClicked);
    }

    void Update()
    {
        // Close on Escape
        if (dialogRoot.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    // Show with title, message, and confirm/cancel actions
    public void Show(string title, string message, Action confirmAction, Action cancelAction = null)
    {
        titleText.text = title;
        messageText.text = message;
        onConfirm = confirmAction;
        onCancel = cancelAction;

        if (running != null) StopCoroutine(running);
        dialogRoot.SetActive(true);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        // set starting states
        canvasGroup.alpha = 0f;
        dialogBox.localScale = Vector3.one * 0.95f;
        // set focus to NoButton initially (or Yes if you prefer)
        EventSystem.current.SetSelectedGameObject(noButton.gameObject);
        running = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        // fade in + scale up (pop)
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = a;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // small pop scale
        t = 0f;
        Vector3 start = dialogBox.localScale;
        Vector3 end = Vector3.one;
        while (t < scaleDuration)
        {
            t += Time.unscaledDeltaTime;
            float s = Mathf.SmoothStep(0f, 1f, t / scaleDuration);
            dialogBox.localScale = Vector3.Lerp(start, end, s);
            yield return null;
        }
        dialogBox.localScale = end;
        running = null;
    }

    public void Hide()
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        // fade out + scale down slightly
        float t = 0f;
        Vector3 startScale = dialogBox.localScale;
        Vector3 targetScale = Vector3.one * 0.97f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float inv = 1f - Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = inv;
            dialogBox.localScale = Vector3.Lerp(targetScale, startScale, inv);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        dialogRoot.SetActive(false);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        running = null;
    }

    // Button hooks
    private void OnYesClicked()
    {
        onConfirm?.Invoke();
        Hide();
    }

    private void OnNoClicked()
    {
        onCancel?.Invoke();
        Hide();
    }

    // Exposed convenience methods
    public void Confirm()
    {
        OnYesClicked();
    }

    public void Cancel()
    {
        OnNoClicked();
    }
}
