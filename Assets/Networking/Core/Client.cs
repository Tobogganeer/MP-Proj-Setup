using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Steamworks.Data;

namespace VirtualVoid.Net
{
    public class Client
    {
        public bool IsLocalPlayer => SteamID == SteamManager.SteamID;


        internal Connection connection;

        private SteamId steamID;

        public SteamId SteamID
        {
            get
            {
                if (Authenticated) return steamID;
                else return default;
            }
            private set
            {
                steamID = value;
            }
        }
        public string SteamName
        {
            get
            {
                if (steamID.IsValid) return new Friend(steamID).Name;
                else return Authenticated ? "Invalid ID" : "Unauthenticated";
            }
        }
        public bool sceneLoaded { get; internal set; } = true;
        public byte clientID { get; internal set; }


        private bool destroyed = false;

        public GameObject CurrentPawn { get; set; }
        public VoiceOutput VoiceOutput { get; set; }
        public ClientNetworkTransform NetworkTransform { get; set; }
        //public VoiceOutput voiceOutput { get; private set; }

        // Auth stuff
        public bool Authenticated { get; internal set; }
        public float TimeCreated { get; private set; }
        internal bool connected;


        //internal static Client Create(Connection connection, SteamId steamIDFromConn)
        //{
        //    if (!SteamManager.IsServer)
        //    {
        //        Debug.LogWarning("Calling the server-side Client.Create method on the client!");
        //        return null;
        //    }
        //
        //
        //    Client client = new Client();
        //
        //    client.connection = connection;
        //    client.SteamID = steamIDFromConn;
        //    client.Authenticated = false;
        //    client.TimeCreated = Time.realtimeSinceStartup;
        //
        //    SteamManager.clientsPendingAuth[connection.Id] = client;
        //    return client;
        //}

        internal void OnCreate(Connection connection, SteamId steamIDFromConn)
        {
            if (!SteamManager.IsServer)
            {
                Debug.LogWarning("Calling the server-side Client.OnCreate method on the client!");
            }

            this.connection = connection;
            this.SteamID = steamIDFromConn;
            this.Authenticated = false;
            this.TimeCreated = Time.realtimeSinceStartup;

            SteamManager.clientsPendingAuth[connection.Id] = this;
        }

        internal void Destroy()
        {
            if (!destroyed)
            {
                destroyed = true;

                try
                {
                    OnDisconnect();
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Error calling the OnDisconnect method. " + ex);
                }

                //if (SteamManager.IsServer)
                //{
                //    connection.Close();
                //}

                if (CurrentPawn != null)
                    UnityEngine.Object.Destroy(CurrentPawn);

                connection.Close();

                if (SteamManager.IsServer && Authenticated)
                {
                    SteamUser.EndAuthSession(SteamID);
                    InternalServerSend.SendClientDisconnect(this);
                }

                if (SteamManager.clients.ContainsKey(SteamID)) SteamManager.clients.Remove(SteamID);

                if (SteamManager.connIDToSteamID.ContainsKey(connection.Id)) SteamManager.connIDToSteamID.Remove(connection.Id);
                if (SteamManager.clientIDToSteamID.ContainsKey(clientID)) SteamManager.clientIDToSteamID.Remove(clientID);

                if (SteamManager.clientsPendingAuth.ContainsKey(connection.Id)) SteamManager.clientsPendingAuth.Remove(connection.Id);
                if (SteamManager.unverifiedSteamIDToConnID.ContainsKey(steamID)) SteamManager.unverifiedSteamIDToConnID.Remove(steamID);

                if (Authenticated)
                    SteamManager.OnClientDestroyed(this);
            }
        }


        internal void OnAuthorized(SteamId id)
        {
            if (!SteamManager.IsServer) return;

            steamID = id;
            Authenticated = true;

            if (SteamManager.clientsPendingAuth.ContainsKey(connection.Id))
                SteamManager.clientsPendingAuth.Remove(connection.Id);

            if (SteamManager.unverifiedSteamIDToConnID.ContainsKey(steamID))
                SteamManager.unverifiedSteamIDToConnID.Remove(steamID);

            SteamManager.clients[id] = this;
            SteamManager.connIDToSteamID[connection.Id] = id;

            clientID = SteamManager.GetFreeClientID();
            SteamManager.clientIDToSteamID[clientID] = id;


            InternalServerSend.SendClientConnect(this);


            foreach (Client client in SteamManager.clients.Values)
            {
                if (client != this)
                    InternalServerSend.SendClientConnect(client, id);
            }

            OnConnect();
        }

        internal void OnSceneFinishedLoading()
        {
            //try
            //{
            //    foreach (NetworkID networkID in NetworkID.networkIDs.Values)
            //    {
            //        Debug.Log($"Spawned {networkID.name} on {SteamName}'s client");
            //        SteamManager.SpawnObject(networkID, SteamID);
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    Debug.Log($"Caught error spawning network objects for {SteamName}: {ex}");
            //}

            // Already spawn Network IDs when all clients' scenes are loaded

            OnSceneLoaded();
        }


        internal static void Spawn_ClientSide(byte clientID, SteamId steamID)
        {
            if (SteamManager.IsServer)
            {
                Debug.Log("Tried to spawn client-side version of client, but this machine is the server!");
                return;
            }

            Client client = SteamManager.instance.GetClient();
            client.Spawn_ClientSide_Local(clientID, steamID);

        }

        private void Spawn_ClientSide_Local(byte clientID, SteamId steamID)
        {
            //if (SteamManager.IsServer) return;
            // Un-needed

            this.clientID = clientID;
            this.SteamID = steamID;
            this.Authenticated = true;

            SteamManager.clientIDToSteamID[clientID] = steamID;
            SteamManager.clients[steamID] = this;

            if (steamID == SteamManager.SteamID)
                SteamManager.OnThisClientConnectedToServer();
            SteamManager.OnClientConnected_Message(this);

            OnConnect();
        }

        internal static void Disconnect_ClientSide(byte clientID, SteamId steamID)
        {
            if (SteamManager.IsServer)
            {
                Debug.Log("Tried to disconnect client-side version of client, but this machine is the server!");
                return;
            }

            if (steamID == SteamManager.SteamID)
            {
                SteamManager.Leave();
                return;
            }

            if (!SteamManager.clientIDToSteamID.TryGetValue(clientID, out SteamId dictID))
            {
                Debug.LogWarning("Could not get SteamID of client " + clientID + "!");

                if (!SteamManager.clients.TryGetValue(steamID, out Client client))
                {
                    Debug.LogWarning("Could not get Client from received SteamID!");
                    return;
                }
                else
                {
                    client.Destroy();
                }
            }
            else
            {
                if (steamID != dictID)
                {
                    Debug.LogWarning("The received SteamID did not match the SteamID in the dictionary!");
                }

                if (!SteamManager.clients.TryGetValue(dictID, out Client client))
                {
                    Debug.LogWarning("Could not get Client from dictionary SteamID!");
                    return;
                }
                else
                {
                    client.Destroy();
                }
            }
        }



        /// <summary>
        /// Called once this clients identity is authenticated. No default implementation.
        /// </summary>
        protected virtual void OnConnect() { }

        /// <summary>
        /// Called when this client disconnects. No default implementation.
        /// </summary>
        protected virtual void OnDisconnect() { }

        /// <summary>
        /// Called when this client has loaded a new scene. No default implementation.
        /// </summary>
        protected virtual void OnSceneLoaded() { }


        public string GetDetailedInfo()
        {
            return $"ConnID: {connection.Id} - Steam ID: {steamID} - SceneLoaded: {sceneLoaded} - ClientID: {clientID} - Authenticated: {Authenticated}";
        }

        ///// <summary>
        ///// Use this method to fetch the spawn data added in SteamManager.AddSpawnData().
        ///// </summary>
        ///// <param name="message"></param>
        //protected internal virtual void GetSpawnData(Message message) { }
    }
}
