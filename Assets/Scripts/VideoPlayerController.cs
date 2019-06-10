using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoPlayerController : MonoBehaviour
{
    
    [System.Serializable]
    public class VideoEvent : UnityEvent<VideoPlayer> {}
    
    public float fadeInDuration = 0.5f;
    public GameController.VideoData videoData;

    public VideoEvent VideoEnded;

    VideoPlayer videoPlayer;
    CanvasGroup canvasGroup;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();   
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    IEnumerator FadeIn(float duration)
    {
        float timeLeft = duration;
        while(timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            canvasGroup.alpha = 1 - (timeLeft / duration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    IEnumerator Start()
    {
        videoPlayer.clip = videoData.clip;
        Graphics.Blit(videoData.firstFrame, videoPlayer.targetTexture);
        videoPlayer.loopPointReached += OnVideoEnded;
        yield return StartCoroutine(FadeIn(fadeInDuration));
        if (videoData.playImmediately == true)
        {
            Debug.Log("Playing video...");
            videoPlayer.Play();
        }
    }

    private void OnVideoEnded(VideoPlayer videoPlayer)
    {
        videoPlayer.loopPointReached -= OnVideoEnded;
        VideoEnded.Invoke(videoPlayer);
    }
}
