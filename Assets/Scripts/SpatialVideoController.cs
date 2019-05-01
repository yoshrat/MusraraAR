using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SpatialVideoController : MonoBehaviour
{
    public LookAtMonitor lookAtMonitor;
    public VideoPlayer[] videoPlayers;


    List<VideoPlayer> videoPlayersWaitingList = new List<VideoPlayer>();

    void Start()
    {
        GameController.Instance.uiAnimator.SetTrigger("ShowWallInstructions");
    }

    public void OnLookingStarted()
    {
        lookAtMonitor.enabled = false;
        StartCoroutine(LookingStartedSequence());
    }

    IEnumerator LookingStartedSequence()
    {
        GameController.Instance.uiAnimator.SetTrigger("PlayVideo");
        yield return new WaitForSeconds(1.0f);

        videoPlayersWaitingList.Clear();
        foreach (VideoPlayer player in videoPlayers)
        {
            player.Prepare();
            videoPlayersWaitingList.Add(player);
            player.prepareCompleted += OnPrepareComplete;
        }
    }

    private void OnPrepareComplete(VideoPlayer player)
    {
        videoPlayersWaitingList.Remove(player);
        if (videoPlayersWaitingList.Count == 0)
        {
            OnAllPlayersReady();
        }
    }

    private void OnAllPlayersReady()
    {
        foreach (VideoPlayer player in videoPlayers)
        {
            player.Play();
        }
    }
}
