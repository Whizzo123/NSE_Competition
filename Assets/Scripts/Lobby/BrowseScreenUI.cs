using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using UdpKit;
using System;

public class BrowseScreenUI : GlobalEventListener
{

    public GameObject sessionListObjectPrefab;
    public GameObject noServerFoundText;
    public GameObject serverList;

    public event Action<UdpSession> OnClickJoinSession;

    private void ResetUI()
    {
        noServerFoundText.SetActive(true);

        foreach(ServerListRoomUI child in serverList.transform.GetComponentsInChildren<ServerListRoomUI>())
        {
            Destroy(child.gameObject);
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.Log("Recieved session list update");

        ResetUI();

        if(sessionList.Count == 0)
        {
            noServerFoundText.SetActive(true);
            return;
        }

        noServerFoundText.SetActive(false);

        var i = 0;

        foreach (var pair in sessionList)
        {
            var session = pair.Value;
            GameObject serverEntryGO = Instantiate(sessionListObjectPrefab, serverList.transform, false);

            ServerListRoomUI serverEntryUI = serverEntryGO.GetComponent<ServerListRoomUI>();
            serverEntryUI.Populate(session, UnityEngine.Random.ColorHSV(), () => 
            { 
                if (OnClickJoinSession != null) OnClickJoinSession.Invoke(session); 
            });
        }
    }
}
