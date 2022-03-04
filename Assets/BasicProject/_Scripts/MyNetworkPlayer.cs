using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Renderer rend;
    
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField] [CanBeNull] 
    private string displayName = "Missing Name";
    
    [SyncVar(hook = nameof(HandleDisplayColorUpdated))] [SerializeField]
    private Color color = Color.black;

    #region Server

    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }
    
    [Server]
    public void SetColor(Color newColor)
    {
        color = newColor;
    }

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        if (newDisplayName.Length < 5 || newDisplayName.Length > 20)
        {
            return;
        }
        
        RpcLogNewName(newDisplayName);
         
        SetDisplayName(newDisplayName);
    }

    #endregion
    
    #region Client
    private void HandleDisplayColorUpdated(Color oldColor, Color newColor)
    {
        rend.material.color = newColor;
    }
    
    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        displayNameText.text = newName;
    }

    [ContextMenu("SetMyName")]
    public void SetMyName()
    {
        CmdSetDisplayName("lol");
    }

    [ClientRpc]
    public void RpcLogNewName(string newName)
    {
        Debug.Log(newName);
    }
    
    #endregion
}
