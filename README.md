# NSE_Competition
Open-world easter egg hunting game
Unity Version:- 2020.1.12f1

Network Stuff For Understanding:

//////Remember, this was all called from Update() A [ClientCallback], also remember we testing player position. This is updated from the transform, not necessarily the function
//No tags, if we do something, everyone else sees that
//[Client] If we do somethingg, everyone else sees that
//[ClientRpc] Host can call, everyone sees. Host called RocketMan(), host and client saw host do RocketMan. Client tried RocketMan, didn't do anything for host and client.
//[Command] Anyone can call, only host sees. Client call RocketMan(), client doesn't do RocketMan, but host sees RocketMan on client.

//////Remember, this was all called from Update() A [ClientCallback], we are destroying an object
//No tags, Host can call, only host sees. Client can call, only client sees
//[Client] Host can call, only host sees. Client can call, only client sees
//[ClientRpc] Host can call, everyone sees. 
//[Command] Anyone can call, only host sees
//Error message for 2 below, called when server not active
//[Server] ???KEEP AN EYE, not completely sure. Host can call, only host sees
//[TargetRpc] ???KEEP AN EYE, not completely sure. Host can call, only host sees




//[Server]
//Only a server can call the method(throws a warning or an error when called on a client).
//[ServerCallback]
//Same as Server but does not throw warning when called on client.

//[Client]
//Only a Client can call the method(throws a warning or an error when called on the server).
//[ClientCallback]
//Same as Client but does not throw warning when called on server.

//[ClientRpc]
//The server uses a Remote Procedure Call(RPC) to run that function on clients.See also: Remote Actions
//[TargetRpc]
// This is an attribute that can be put on methods of NetworkBehaviour classes to allow them to be invoked on clients from a server. Unlike the ClientRpc attribute, 
//these functions are invoked on one individual target client, not all of the ready clients. See also: Remote Actions
//[Command]
//Call this from a client to run this function on the server. Make sure to validate input etc. 
//It's not possible to call this from a server. Use this as a wrapper around another function, if you want to call it from the server too. 
