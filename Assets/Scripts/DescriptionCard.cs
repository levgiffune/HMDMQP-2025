using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// At-waypoint description card with image carousel and text.
/// Shows "Press A to move on" in Tour Mode.
/// Shown/hidden by ProximityActivator via WaypointVisual.
/// </summary>
public class DescriptionCard : MonoBehaviour
{
    [Header("UI References")]
    public RawImage carouselImage;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI advancePrompt;

    [Header("Settings")]
    public float autoAdvanceInterval = 3f;

    private Texture2D[] images;
    private int currentImageIndex = 0;
    private float autoAdvanceTimer = 0f;

    public void Initialize(Waypoint waypoint)
    {
        images = waypoint.images;

        if (descriptionText != null)
        {
            descriptionText.text = waypoint.desc ?? "";
        }

        currentImageIndex = 0;
        UpdateCarouselImage();

        // Tour mode prompt — controlled externally
        if (advancePrompt != null)
        {
            advancePrompt.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Show or hide the "Press A to move on" prompt (Tour Mode only).
    /// </summary>
    public void SetAdvancePromptVisible(bool visible)
    {
        if (advancePrompt != null)
        {
            advancePrompt.gameObject.SetActive(visible);
        }
    }

    void Update()
    {
        if (images == null || images.Length <= 1) return;

        autoAdvanceTimer += Time.deltaTime;
        if (autoAdvanceTimer >= autoAdvanceInterval)
        {
            NextImage();
            autoAdvanceTimer = 0f;
        }
    }

    public void NextImage()
    {
        if (images == null || images.Length == 0) return;
        currentImageIndex = (currentImageIndex + 1) % images.Length;
        UpdateCarouselImage();
    }

    public void PreviousImage()
    {
        if (images == null || images.Length == 0) return;
        currentImageIndex = (currentImageIndex - 1 + images.Length) % images.Length;
        UpdateCarouselImage();
    }

    private void UpdateCarouselImage()
    {
        if (carouselImage == null || images == null || images.Length == 0)
        {
            if (carouselImage != null) carouselImage.gameObject.SetActive(false);
            return;
        }

        if (currentImageIndex >= 0 && currentImageIndex < images.Length && images[currentImageIndex] != null)
        {
            carouselImage.texture = images[currentImageIndex];
            carouselImage.gameObject.SetActive(true);
        }
        else
        {
            carouselImage.gameObject.SetActive(false);
        }
    }
}
