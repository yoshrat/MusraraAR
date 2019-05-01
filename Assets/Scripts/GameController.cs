using System;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [System.Serializable]
    public class MarkerData
    {
        public string name;
        public int imageIndex;
        public Transform anchorTransform;
    }

    [System.Serializable]
    public class VideoData
    {
        public string name;
        public Texture image;
        public GameObject prefab;
        public VideoClip clip;
        public bool playImmediately;
    }

    public static GameController Instance {get; private set;}

    public List<VideoData> allVideoData;
    public Button skipButton;
    public GameObject instructionsPanel;
    public GameObject foundPanel;
    public RawImage foundImage;
    public Animator uiAnimator;

    Dictionary<string, VideoData> nameToVideoData = new Dictionary<string, VideoData>();
    GameObject currentVideoInstance = null;
    VideoData currentVideoData = null;
    int currentVideoIndex;
    MarkerDetector markerDetector;

    void Awake()
    {
        Instance = this;
        markerDetector = GetComponent<MarkerDetector>();
        foreach (VideoData videoData in allVideoData)
        {
            nameToVideoData.Add(videoData.name, videoData);
        }
    }

    void Start()
    {
        uiAnimator.SetTrigger("ShowInstructions");
        markerDetector.enabled = true;
        // skipButton.gameObject.SetActive(false);
        // instructionsPanel.gameObject.SetActive(true);
    }

    public void OnMarkerDiscovered(AugmentedImage image)
    {
        Debug.Log("OnMarkerDiscovered. image = " + image.Name);
        if (currentVideoData != null)
            return;

        Anchor anchor = image.CreateAnchor(image.CenterPose);
        MarkerData markerData = new MarkerData() {name = image.Name, imageIndex = image.DatabaseIndex, anchorTransform = anchor.transform};
        OnMarkerDiscovered(markerData);
    }

    public void OnMarkerDiscovered(MarkerData markerData)
    {
        Debug.Log("OnMarkerDiscovered. marker = " + markerData.name);
        if (currentVideoData != null)
            return;

        markerDetector.enabled = false;
        VideoData videoData = nameToVideoData[markerData.name];
        currentVideoData = videoData;
        currentVideoIndex = markerData.imageIndex;
        foundImage.texture = currentVideoData.image;
        // GameObject instance = Instantiate(videoData.prefab, markerData.anchorTransform);
        // currentVideoInstance = instance;
        // VideoPlayer videoPlayer = instance.GetComponent<VideoPlayer>();
        // videoPlayer.clip = videoData.clip;
        // videoPlayer.loopPointReached += OnVideoEnded;
        // skipButton.gameObject.SetActive(true);
        // instructionsPanel.gameObject.SetActive(false);
        StartCoroutine(MarkerDetectedSequence(markerData));
    }

    IEnumerator MarkerDetectedSequence(MarkerData markerData)
    {
        uiAnimator.SetTrigger("FoundMarker");
        yield return new WaitForSeconds(3.0f);
        uiAnimator.SetTrigger("PlayVideo");
        yield return new WaitForSeconds(1.0f);
        GameObject instance = Instantiate(currentVideoData.prefab, markerData.anchorTransform);
        currentVideoInstance = instance;
        VideoPlayer videoPlayer = instance.GetComponent<VideoPlayer>();
        videoPlayer.clip = currentVideoData.clip;
        videoPlayer.loopPointReached += OnVideoEnded;
        if (currentVideoData.playImmediately == true)
        {
            Debug.Log("Playing video...");
            videoPlayer.Play();
        }
    }

    private void OnVideoEnded(VideoPlayer player)
    {
        Debug.Log("OnVideoEnded");
        stopCurrentVideo();
    }

    public void OnInterruptCurrentVideo()
    {
        stopCurrentVideo();
    }

    IEnumerator WaitAndEnableImageDetection()
    {
        yield return new WaitForSeconds(3.0f);
        markerDetector.SetImageDetection(currentVideoIndex, false);
        currentVideoIndex = -1;
        markerDetector.enabled = true;
    }

    void stopCurrentVideo()
    {
        if (currentVideoInstance == null)
            return;

        clearRenderTexture(currentVideoInstance.GetComponent<VideoPlayer>().targetTexture);
        Destroy(currentVideoInstance);
        currentVideoInstance = null;
        currentVideoData = null;
        // skipButton.gameObject.SetActive(false);
        // instructionsPanel.gameObject.SetActive(true);
        uiAnimator.SetTrigger("ShowInstructions");
        StartCoroutine(WaitAndEnableImageDetection());
    }

    void clearRenderTexture(RenderTexture rt)
    {
        RenderTexture tempRt = UnityEngine.RenderTexture.active;
        UnityEngine.RenderTexture.active = rt;
        GL.Clear(true, true, Color.clear);
        UnityEngine.RenderTexture.active = tempRt;
    }
}
