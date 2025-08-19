using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private PlayerChat playerChat;
        public float moveSpeed = 5f;
        public NetworkVariable<FixedString32Bytes> playerName =
            new NetworkVariable<FixedString32Bytes>(
                value: "",
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Owner
            );

        //public string PlayerName {get => playerName.Value.ToString(); private set;}

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();  //it check own functionality and then we implement our logic

            if (!IsOwner) return;

            if (PlayerSettings.PlayerName.Length <= 0)
            {
                Debug.Log("Cannot assign empty player name to player");
                return;
            }
            playerName.Value = PlayerSettings.PlayerName;
            Debug.Log($"This player name is {playerName.Value}");
        }

        private void Update()
        {
            // Only process input for the local player
            if (!IsOwner) return;

            Vector3 input = new Vector3(
                Input.GetAxis("Horizontal"),
                0f,
                Input.GetAxis("Vertical")
            );


            Vector3 move = input * moveSpeed * Time.deltaTime;

            // Send the movement to the server
            MoveServerRpc(move);
        }

        [ServerRpc]
        private void MoveServerRpc(Vector3 move, ServerRpcParams rpcParams = default)
        {
            // Apply movement on the server
            transform.position += move;
        }
    }
}