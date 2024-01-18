using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class StreamlineManager : PhotonPun.MonoBehaviourPunCallbacks
{
    

    // we need the instance so that we can call NewAnchorWasCreated from another script easily
    public static StreamlineManager Instance;

    private void Awake() {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // anything I want to have run automatically when joined room
    public override void OnJoinedRoom() {
        // test: 
        // var newProperty = new ExitGames.Client.Photon.Hashtable { { "mainObjectCurrentModelName", "Model2" } };
        // PhotonPun.PhotonNetwork.CurrentRoom.SetCustomProperties(newProperty);

        // set the default UI to student mode when a room is joined
        GUIManager.Instance.OnStudentMode();
    }


    // in the future, we can automatically join a room if one exists
    public override void OnJoinedLobby() {
        // wait for room to exist...
    }


    public override void OnRoomListUpdate(List<PhotonRealtime.RoomInfo> roomList)
    {
        SampleController.Instance.Log("Room list was updated! Room list is of length " + roomList.Count);
        if (roomList.Count == 0) {
            SampleController.Instance.Log("Since count is 0, doing nothing");
            return;
        }

        // set nickname if not set
        if (PhotonPun.PhotonNetwork.NickName == "") {
            string testName = "TestUser" + UnityEngine.Random.Range(0, 1000);
            PhotonPun.PhotonNetwork.NickName = testName;
        }

        // consider checking if you are already in a room

        string roomToJoin = roomList[0].Name;
        var roomOptions = new PhotonRealtime.RoomOptions { IsVisible = true, MaxPlayers = 50, EmptyRoomTtl = 0, PlayerTtl = 300000 };
        PhotonPun.PhotonNetwork.JoinOrCreateRoom(roomToJoin, roomOptions, PhotonRealtime.TypedLobby.Default);

        SampleController.Instance.Log("found and joining room called " + roomList[0].Name);


    }



    // this function is called by SharedAnchorLoader.Instance.InstantiateUnboundAnchor (when a new shared anchor is being instantiated)
    public void NewAnchorWasCreated(SharedAnchor anchor) {

        // automatically align with the new shared anchor
        AutoAlignAnchor(anchor);


        // hide the anchor and save it in a list for viewing in admin mode
        // TODO
    }


    public void AutoAlignAnchor(SharedAnchor anchor) {
        anchor.OnAlignButtonPressed();
    }




    
}
