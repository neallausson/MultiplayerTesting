using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class RTSNetworkingManager : NetworkManager
{
    [SerializeField] private GameObject unitBasePrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    private bool idGameInProgress = false;
    
    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

    #region Server

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!idGameInProgress) { return; }
        
        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        Players.Remove(player);
        
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        idGameInProgress = false;
        
        ServerChangeScene("Scene_Map_01");
    }

    public void StartGame()
    {
        if (Players.Count < 2)
        {
            idGameInProgress = true;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        
        Players.Add(player);
        
        player.SetDisplayName($"Player {Players.Count}");
        
        player.SetTeamColor(new Color(
            Random.Range(0f,1f),
            Random.Range(0f,1f),
            Random.Range(0f,1f)
        ));
        
        player.SetPartyOwner(Players.Count == 1);

        StartCoroutine(DelaySpawnUnitbase(conn));
    }

    private IEnumerator DelaySpawnUnitbase(NetworkConnection conn)
    {
        yield return new WaitForSeconds(0.1f);
        
        GameObject unitSpawnerInstance = Instantiate(
            unitBasePrefab,
            conn.identity.transform.position,
            conn.identity.transform.rotation);
        
        NetworkServer.Spawn(unitSpawnerInstance,conn);
    }

    public override void OnServerSceneChanged(string newSceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);
            
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            foreach (RTSPlayer player in Players)
            {
                GameObject baseInstance = Instantiate(
                    unitBasePrefab,
                    GetStartPosition().position, 
                    Quaternion.identity);
                
                NetworkServer.Spawn(baseInstance, player.connectionToClient);
            }
        }
    }

    
    #endregion
    
    #region Client
    
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion
    
}
