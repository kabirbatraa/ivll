
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;
// using PlayerProperties = Photon.Pun.PhotonNetwork.CustomProperties;
// using PlayerProperties = Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties;
// using LocalPlayer = Photon.Pun.PhotonNetwork.LocalPlayer;

public class SharedAnchorControlPanelAdditionalFunctions : MonoBehaviour
{

    [SerializeField]
    private GameObject spherePrefab;


    [SerializeField]
    private GameObject jengaPrefab;

    [SerializeField]
    private GameObject tablePrefab;


    [SerializeField]
    private Transform spawnPoint;


    [SerializeField]
    private GameObject[] adminButtons;

    [SerializeField]
    private GameObject[] studentButtons;




    private bool alignTableMode = false;
    private int countAButton = 0;

    private GameObject mostRecentSphere;

    public void Update() {

        if (alignTableMode) {

            // bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger);
            bool buttonPressed = OVRInput.GetDown(OVRInput.RawButton.A);

            
            if (buttonPressed) {
                SampleController.Instance.Log("The A button was pressed!");

                var controllerType = OVRInput.Controller.RTouch; // the right controller
                Vector3 controllerPosition = OVRInput.GetLocalControllerPosition(controllerType);

                string x = controllerPosition.x.ToString("0.00");
                string y = controllerPosition.y.ToString("0.00");
                string z = controllerPosition.z.ToString("0.00");
                SampleController.Instance.Log(x + " " + y + " " + z);

                countAButton++;
                if (countAButton == 2) {
                    countAButton = 0;
                    alignTableMode = false;
                }
            }
        }


        // if the B button is pressed, then enable or disable the most recent spawned sphere
        bool BButton = OVRInput.GetDown(OVRInput.RawButton.B);
        if (BButton) {
            
            // mostRecentSphere.SetActive(!mostRecentSphere.activeSelf);

            // instead, lets change the group number and see if the code below works 
            // (automatically set inactive if not same group number)
            ObjectData data = mostRecentSphere.GetComponent<ObjectData>();
            data.groupNumber = data.groupNumber == 0 ? 1 : 0;
            SampleController.Instance.Log("set group number to " + data.groupNumber);
        }


        // Object Group Filtering:

        // every frame, scan through all objects that have "object data" component
        // and enable or disable them based on if their group number matches the current 
        // user's group number

        // int currentUserGroupNumber = 0;
        // int currentUserGroupNumber = gameObject.GetComponent<StudentData>().groupNumber;
        int currentUserGroupNumber = GetCurrentGroupNumber();


        // need to include inactive (params are type, includeInactive)
        ObjectData[] allNetworkObjectDatas = (ObjectData[]) FindObjectsOfType(typeof(ObjectData), true);

        foreach (ObjectData objectData in allNetworkObjectDatas) {
            GameObject gameObject = objectData.gameObject;

            // if group 0 or the group numbers match, then set the object to active
            if (currentUserGroupNumber == 0 || objectData.groupNumber == currentUserGroupNumber) {
                gameObject.SetActive(true);
            }
            // if they do not match, disable the object
            else {
                gameObject.SetActive(false);
            }
        }


        // if you press X, then print all spawned object's instance ids
        bool XPressed = OVRInput.GetDown(OVRInput.RawButton.X);
        if (XPressed) {
            foreach (ObjectData objectData in allNetworkObjectDatas) {
                GameObject obj = objectData.gameObject;
                SampleController.Instance.Log(obj.GetInstanceID().ToString());
            }
        }


    }





    public void OnSpawnSphereButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnSphereButtonPressed");

        SpawnSphere();
    }

    private void SpawnSphere()
    {
        var sphereObject = PhotonPun.PhotonNetwork.Instantiate(spherePrefab.name, spawnPoint.position, spawnPoint.rotation);
        var photonGrabbable = sphereObject.GetComponent<PhotonGrabbableObject>();
        photonGrabbable.TransferOwnershipToLocalPlayer();


        int currentUserGroupNumber = GetCurrentGroupNumber();

        // print the sphere's group number if it has an "object data" property
        ObjectData data = sphereObject.GetComponent<ObjectData>();
        data.SetGroupNumber(currentUserGroupNumber); // set the group number to current user's group number
        SampleController.Instance.Log("group number of this sphere is: " + data.groupNumber);

        mostRecentSphere = sphereObject;
    }

    public void OnSpawnJengaButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnJengaButtonPressed");

        SpawnJenga();
    }

    private void SpawnJenga()
    {
        var networkedCube = PhotonPun.PhotonNetwork.Instantiate(jengaPrefab.name, spawnPoint.position, spawnPoint.rotation);
        // var photonGrabbable = networkedCube.GetComponent<PhotonGrabbableObject>();
        // photonGrabbable.TransferOwnershipToLocalPlayer();
    }

    public void OnSpawnTableButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnTableButtonPressed");

        SpawnTable();
    }

    private void SpawnTable()
    {
        var networkedCube = PhotonPun.PhotonNetwork.Instantiate(tablePrefab.name, spawnPoint.position, spawnPoint.rotation);
        // var photonGrabbable = networkedCube.GetComponent<PhotonGrabbableObject>();
        // photonGrabbable.TransferOwnershipToLocalPlayer();
    }



    public void OnSpawnAlignedTableButtonPressed()
    {
        SampleController.Instance.Log("OnSpawnAlignedTableButtonPressed");

        SpawnAlignedTable();
    }

    private void SpawnAlignedTable()
    {

        // var networkedCube = PhotonPun.PhotonNetwork.Instantiate(tablePrefab.name, spawnPoint.position, spawnPoint.rotation);
        // var photonGrabbable = networkedCube.GetComponent<PhotonGrabbableObject>();
        // photonGrabbable.TransferOwnershipToLocalPlayer();



        alignTableMode = true;
        // SampleController.Instance.Log(alignTableMode.ToString());
        
    }



    public void OnSetGroupNumber(int groupNumber) {
        SampleController.Instance.Log("Setting group number to " + groupNumber);
        SetGroupNumber(groupNumber);
    }
    private void SetGroupNumber(int groupNumber) {
        // gameObject.GetComponent<StudentData>().SetGroupNumber(groupNumber);
        // SampleController.Instance.Log("Set group number to " + gameObject.GetComponent<StudentData>().groupNumber);

        PhotonRealtime.Player LocalPlayer = Photon.Pun.PhotonNetwork.LocalPlayer;

        LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });


    }



    private int GetCurrentGroupNumber() {

        ExitGames.Client.Photon.Hashtable PlayerProperties = Photon.Pun.PhotonNetwork.LocalPlayer.CustomProperties;

        bool groupNumberExists = PlayerProperties.ContainsKey("groupNumber");
        int currentUserGroupNumber = groupNumberExists ? (int)PlayerProperties["groupNumber"] : 0;

        return currentUserGroupNumber;

    }

    public void OnLogCurrentGroupNumber() {
        SampleController.Instance.Log("Your group number is " + GetCurrentGroupNumber());
        
    }




    public void OnSetEveryoneElseGroupNumber(int groupNumber) {

        SampleController.Instance.Log("Setting everyone else's group number to " + groupNumber);

        SetEveryoneElseGroupNumber(groupNumber);

    }

    private void SetEveryoneElseGroupNumber(int groupNumber) {

        // value collection (basically list) of PhotonRealtime.Player objects
        var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        foreach (PhotonRealtime.Player player in players) {
            
            // if the player is the current player, then skip
            if (player.Equals(Photon.Pun.PhotonNetwork.LocalPlayer)) {
                SampleController.Instance.Log("(skipping current player)");
                continue;
            }

            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });
            SampleController.Instance.Log("Set player group of nickname: " + player.NickName);
        }

    }



    public void OnSetToAdminMode() {
        SampleController.Instance.Log("Setting to Admin Mode");
        SetToAdminMode();
    }

    private void SetToAdminMode() {

        // for every button in a list of buttons, make them active


        foreach (GameObject b in studentButtons) {
            b.SetActive(false);
        }
        foreach (GameObject b in adminButtons) {
            b.SetActive(true);
        }
        

    }

    public void OnSetToStudentMode() {
        SampleController.Instance.Log("Setting to Admin Mode");
        SetToStudentMode();
    }

    private void SetToStudentMode() {

        // for every button in a list of buttons, make them active
        foreach (GameObject b in adminButtons) {
            b.SetActive(false);
        }
        foreach (GameObject b in studentButtons) {
            b.SetActive(true);
        }

    }


    public void OnSetEveryoneSeparateGroups() {
        SampleController.Instance.Log("Setting everyone to separate groups (except admin)");
        SetEveryoneSeparateGroups();
    }

    private void SetEveryoneSeparateGroups() {

        // value collection (basically list) of PhotonRealtime.Player objects
        var players = PhotonPun.PhotonNetwork.CurrentRoom.Players.Values;

        int groupNumber = 1;
        foreach (PhotonRealtime.Player player in players) {
            
            // if the player is the current player, then skip
            if (player.Equals(Photon.Pun.PhotonNetwork.LocalPlayer)) {
                SampleController.Instance.Log("(skipping current player)");
                continue;
            }

            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "groupNumber", groupNumber } });
            SampleController.Instance.Log("Set player group of nickname " + player.NickName + " to group " + groupNumber);
            groupNumber++;
        }

    }

}