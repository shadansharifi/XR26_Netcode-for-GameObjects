using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class PlayerChat : NetworkBehaviour
{
    [SerializeField] Player.Player player;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;

        //This is an ingerited design issue. We have to wait for the UI Chat Singleton 
        //to initialize before we can start listening to when message is submitted
        StartCoroutine(OnStart());
    }

    private IEnumerator OnStart()
    {
        yield return new WaitForSeconds(2);
        ChatUI.Instance.OnMessageSubmit.AddListener(SendChatMessage);
        yield return null;
    }


    /*
	public PlayerChat(object sendChatMessage)
	{
		SendChatMessage = sendChatMessage;
	}*/

    //public object SendChatMessage { get; set; }

    public void SendChatMessage()
    {
        SendMessageServerRpc(ChatUI.Instance.chatInput.text);
    }

    [ServerRpc]
    //The client calls a server function below and pass a string of the message
    //This function then calls "ReceiveMessageClientRpc" to ALL clients
    //Which means every connected client wil call this on THEIR computer
    public void SendMessageServerRpc(string text)
    {
        ReceiveMessageClientRpc(player.playerName.Value.ToString(), text);
    }
    [ClientRpc]
    public void ReceiveMessageClientRpc(string name, string text)
    {
        ChatUI.Instance.CreateChatMessage(name, text);
    }
}