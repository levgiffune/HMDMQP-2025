using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using Meta.WitAi.TTS.Utilities;

/// <summary>
/// At-waypoint description card with image carousel and text.
/// Shows "Press A to move on" in Tour Mode.
/// Shown/hidden by ProximityActivator via WaypointVisual.
/// Video clips are treated as the last slide in the carousel.
/// </summary>
public class DescriptionCard : MonoBehaviour
{
    [Header("UI References")]
    public RawImage carouselImage;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI advancePrompt;
    public TextMeshProUGUI titleText;
    public TTSSpeaker speaker;
    
    [Header("Settings")]
    public float autoAdvanceInterval = 3f;

    private Texture2D[] images;
    private int currentMediaIndex = 0;
    private float autoAdvanceTimer = 0f;

    public void StartTTS()
    {
        speaker.SpeakQueued(titleText.text);
        speaker.SpeakQueued(descriptionText.text);
    }
    // Video support
    private VideoPlayer videoPlayer;
    private RenderTexture videoRenderTexture;
    private VideoClip videoClip;
    private int imageCount;
    private int totalMediaCount;

    public void Initialize(Waypoint waypoint)
    {
        images = waypoint.images;
        videoClip = waypoint.videoClip;

        imageCount = (images != null) ? images.Length : 0;
        totalMediaCount = imageCount + (videoClip != null ? 1 : 0);

        if (titleText != null)
        {
            titleText.text = waypoint.name ?? "";
        }

        if (descriptionText != null)
        {
            descriptionText.text = waypoint.desc ?? "";
        }

        if (videoClip != null)
        {
            SetupVideoPlayer();
        }

        currentMediaIndex = 0;
        UpdateCarouselMedia();

        // Tour mode prompt — controlled externally
        if (advancePrompt != null)
        {
            advancePrompt.gameObject.SetActive(false);
        }
    }

    private void SetupVideoPlayer()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
            videoPlayer = gameObject.AddComponent<VideoPlayer>();

        videoRenderTexture = new RenderTexture(1920, 1080, 0);
        videoPlayer.clip = videoClip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = videoRenderTexture;
        videoPlayer.isLooping = true;
        videoPlayer.playOnAwake = false;
        videoPlayer.Stop();
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
        if (totalMediaCount <= 1) return;
        // Don't auto-advance away from the video slide
        if (IsVideoSlide(currentMediaIndex)) return;

        autoAdvanceTimer += Time.deltaTime;
        if (autoAdvanceTimer >= autoAdvanceInterval)
        {
            NextMedia();
            autoAdvanceTimer = 0f;
        }
    }

    private bool IsVideoSlide(int index)
    {
        return videoClip != null && index == imageCount;
    }

    public void NextMedia()
    {
        if (totalMediaCount == 0) return;
        currentMediaIndex = (currentMediaIndex + 1) % totalMediaCount;
        UpdateCarouselMedia();
    }

    public void PreviousMedia()
    {
        if (totalMediaCount == 0) return;
        currentMediaIndex = (currentMediaIndex - 1 + totalMediaCount) % totalMediaCount;
        UpdateCarouselMedia();
    }

    // Keep old names working for any external callers
    public void NextImage() => NextMedia();
    public void PreviousImage() => PreviousMedia();

    private void UpdateCarouselMedia()
    {
        if (carouselImage == null || totalMediaCount == 0)
        {
            if (carouselImage != null) carouselImage.gameObject.SetActive(false);
            StopVideo();
            return;
        }

        if (IsVideoSlide(currentMediaIndex))
        {
            // Show video
            carouselImage.texture = videoRenderTexture;
            carouselImage.gameObject.SetActive(true);
            PlayVideo();
        }
        else if (currentMediaIndex >= 0 && currentMediaIndex < imageCount && images[currentMediaIndex] != null)
        {
            // Show image
            StopVideo();
            carouselImage.texture = images[currentMediaIndex];
            carouselImage.gameObject.SetActive(true);
        }
        else
        {
            StopVideo();
            carouselImage.gameObject.SetActive(false);
        }

        autoAdvanceTimer = 0f;
    }

    private void PlayVideo()
    {
        if (videoPlayer != null && !videoPlayer.isPlaying)
            videoPlayer.Play();
    }

    private void StopVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Stop();
    }

    void OnDestroy()
    {
        if (videoRenderTexture != null)
        {
            videoRenderTexture.Release();
            Destroy(videoRenderTexture);
        }
    }
}
