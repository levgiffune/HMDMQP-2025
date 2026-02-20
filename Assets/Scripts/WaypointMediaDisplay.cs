using UnityEngine;
using UnityEngine.Video;

public class WaypointMediaDisplay : MonoBehaviour
{
    [Header("References")]
    public Transform displayRoot;
    public MeshRenderer imageQuad;
    public VideoPlayer videoPlayer;
    public RenderTexture videoRenderTarget;

    [Header("Visibility")]
    public float showMediaDistance = 0f;
    public bool showOnlyWhenSelected = false;

    [Header("Video")]
    public bool loopVideo = true;

    private Waypoint waypointData;
    private Transform cameraTransform;
    private bool isSelected;
    private bool hasContent;

    public void Initialize(Waypoint waypoint, Transform playerCamera, Transform waypointRoot)
    {
        waypointData = waypoint;
        cameraTransform = playerCamera;
        if (displayRoot == null)
        {
            displayRoot = transform;
        }

        Refresh(waypoint);
    }

    private void Update()
    {
        UpdateVisibility();
    }

    public void Refresh(Waypoint waypoint)
    {
        waypointData = waypoint;
        hasContent = false;

        if (waypoint == null)
        {
            SetRootActive(false);
            return;
        }

        if (waypoint.videoClip != null)
        {
            EnsureVideoPlayer();
            if (videoPlayer != null)
            {
                SetupVideo(waypoint.videoClip);
                hasContent = true;
            }
        }

        if (!hasContent && waypoint.imageRef != null && imageQuad != null)
        {
            SetupImage(waypoint.imageRef);
            hasContent = true;
        }
        else if (!hasContent)
        {
            ClearDisplay();
        }

        UpdateVisibility();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisibility();
    }

    private void SetupVideo(VideoClip clip)
    {
        if (videoPlayer == null)
        {
            return;
        }

        videoPlayer.clip = clip;
        videoPlayer.isLooping = loopVideo;

        if (videoRenderTarget != null)
        {
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = videoRenderTarget;
            ApplyTextureToQuad(videoRenderTarget);
        }
        else if (imageQuad != null)
        {
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.targetMaterialRenderer = imageQuad;
            videoPlayer.targetMaterialProperty = "_MainTex";
        }
        else if (videoPlayer.targetTexture != null)
        {
            ApplyTextureToQuad(videoPlayer.targetTexture);
        }
    }

    private void EnsureVideoPlayer()
    {
        if (videoPlayer != null)
        {
            return;
        }

        GameObject host = displayRoot != null ? displayRoot.gameObject : gameObject;
        videoPlayer = host.GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = host.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
        }
    }

    private void SetupImage(Texture2D texture)
    {
        ApplyTextureToQuad(texture);
    }

    private void ClearDisplay()
    {
        if (imageQuad != null && imageQuad.material != null)
        {
            imageQuad.material.mainTexture = null;
        }
    }

    private void ApplyTextureToQuad(Texture texture)
    {
        if (imageQuad == null || imageQuad.material == null || texture == null)
        {
            return;
        }

        imageQuad.material.mainTexture = texture;
    }

    private void UpdateVisibility()
    {
        if (displayRoot == null)
        {
            return;
        }

        bool shouldShow = hasContent && (isSelected || (!showOnlyWhenSelected && IsWithinDistance(showMediaDistance)));
        SetRootActive(shouldShow);
        UpdateVideoPlayback(shouldShow);
    }

    private void UpdateVideoPlayback(bool shouldShow)
    {
        if (videoPlayer == null || videoPlayer.clip == null)
        {
            return;
        }

        if (shouldShow)
        {
            if (!videoPlayer.isPlaying)
            {
                videoPlayer.Play();
            }
        }
        else
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
        }
    }

    private bool IsWithinDistance(float maxDistance)
    {
        if (maxDistance <= 0f || cameraTransform == null || waypointData == null)
        {
            return false;
        }

        return waypointData.DistanceFrom(cameraTransform.position) <= maxDistance;
    }

    private void SetRootActive(bool active)
    {
        if (displayRoot != null && displayRoot.gameObject.activeSelf != active)
        {
            displayRoot.gameObject.SetActive(active);
        }
    }
}
