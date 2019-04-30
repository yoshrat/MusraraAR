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
        public Transform anchorTransform;
    }

    [System.Serializable]
    public class VideoData
    {
        public string name;
        public GameObject prefab;
        public VideoClip clip;
        public bool playImmediately;
    }
    public List<VideoData> allVideoData;
    public Button skipButton;

    Dictionary<string, VideoData> nameToVideoData = new Dictionary<string, VideoData>();
    GameObject currentVideoInstance = null;
    VideoData currentVideoData = null;
    MarkerDetector markerDetector;

    void Awake()
    {
        markerDetector = GetComponent<MarkerDetector>();
        foreach (VideoData videoData in allVideoData)
        {
            nameToVideoData.Add(videoData.name, videoData);
        }
    }

    void Start()
    {
        markerDetector.enabled = true;
        skipButton.gameObject.SetActive(false);
    }

    public void OnMarkerDiscovered(AugmentedImage image)
    {
        if (currentVideoData != null)
            return;

        Anchor anchor = image.CreateAnchor(image.CenterPose);
        MarkerData markerData = new MarkerData() {name = image.Name, anchorTransform = anchor.transform};
        OnMarkerDiscovered(markerData);
    }

    public void OnMarkerDiscovered(MarkerData markerData)
    {
        if (currentVideoData != null)
            return;

        markerDetector.enabled = false;
        VideoData videoData = nameToVideoData[markerData.name];
        currentVideoData = videoData;
        GameObject instance = Instantiate(videoData.prefab, markerData.anchorTransform);
        currentVideoInstance = instance;
        VideoPlayer videoPlayer = instance.GetComponent<VideoPlayer>();
        videoPlayer.clip = videoData.clip;
        videoPlayer.loopPointReached += OnVideoEnded;
        if (videoData.playImmediately == true)
        {
            videoPlayer.Play();
        }
        skipButton.gameObject.SetActive(true);
    }

    private void OnVideoEnded(VideoPlayer source)
    {
        stopCurrentVideo();
    }

    public void OnInterruptCurrentVideo()
    {
        stopCurrentVideo();
    }

    void stopCurrentVideo()
    {
        if (currentVideoInstance == null)
            return;

        Destroy(currentVideoInstance);
        currentVideoInstance = null;
        currentVideoData = null;
        markerDetector.enabled = true;
        skipButton.gameObject.SetActive(false);
    }
}
