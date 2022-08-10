using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEditor;
using T_Utils;

public class Effects : NetworkBehaviour
{
    private static float boostToSpeed = 7.5f;
    private static List<GameObject> particles;
    private static LayerMask ground;
    private static Material[] playerNormalMats;
    private static  Material[] playerFadeMats;
    private static string currentTargettingAbilityTimer;

    private void Start()
    {
        playerNormalMats = new Material[11];
        playerNormalMats[0] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_1_Mat");
        playerNormalMats[1] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_2_Mat");
        playerNormalMats[2] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_3_Mat");
        playerNormalMats[3] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_4_Mat");
        playerNormalMats[4] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_5_Mat");
        playerNormalMats[5] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_6_Mat");
        playerNormalMats[6] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_7_Mat");
        playerNormalMats[7] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_8_Mat");
        playerNormalMats[8] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_9_Mat");
        playerNormalMats[9] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_10_Mat");
        playerNormalMats[10] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_11_Mat");

        playerFadeMats = new Material[11];
        playerFadeMats[0] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_1_FadeMat");
        playerFadeMats[1] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_2_FadeMat");
        playerFadeMats[2] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_3_FadeMat");
        playerFadeMats[3] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_4_FadeMat");
        playerFadeMats[4] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_5_FadeMat");
        playerFadeMats[5] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_6_FadeMat");
        playerFadeMats[6] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_7_FadeMat");
        playerFadeMats[7] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_8_FadeMat");
        playerFadeMats[8] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_9_FadeMat");
        playerFadeMats[9] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_10_FadeMat");
        playerFadeMats[10] = Resources.Load<Material>("PlayerAssets/Extracted Mats/Player_11_FadeMat");
    }

    #region PowerupEffects


    /////////Speed
    //Todo: Add speed particles
    public static void SpeedBoost(Ability ability)
    {
        if(ability.GetCastingPlayer().speed != boostToSpeed && !ability.IsOppositeDebuffActivated())
        {
            ability.GetCastingPlayer().speed = ability.GetCastingPlayer().normalSpeed + boostToSpeed;
            ability.SetInUse(true);
        }
    }
    public static void EndSpeedBoost(Ability ability)
    {
        ability.GetCastingPlayer().speed = ability.GetCastingPlayer().normalSpeed;
    }


    /////////Camo
    public static void ActivateCamouflage(Ability ability)
    {
        Vector3 spawnPos = ability.GetCastingPlayer().gameObject.transform.position;
        FindObjectOfType<Effects>().CmdSpawnCamouflageParticles(spawnPos);
        AlterMaterials(ability, false);
        ability.GetCastingPlayer().CmdToggleCamouflage(false, ability.GetCastingPlayer());
        ability.SetInUse(true);
    }
    public static void AlterMaterials(Ability ability, bool toggle)
    {
        Debug.Log("Hitting materials: " + ability.GetCastingPlayer().GetComponentInChildren<SkinnedMeshRenderer>().materials.Length);
        if (toggle)
        {
            ability.GetCastingPlayer().GetComponentInChildren<SkinnedMeshRenderer>().materials = playerNormalMats;
        }
        else
        {
            ability.GetCastingPlayer().GetComponentInChildren<SkinnedMeshRenderer>().materials = playerFadeMats;
        }

    }
    [Command (requiresAuthority = false)]
    public void CmdSpawnCamouflageParticles(Vector3 spawnPos)
    {
        GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "Invisibility_PA"),
            spawnPos, Quaternion.identity);
        NetworkServer.Spawn(go);
        //temp();
    }
    public static void DeactivateCamouflage(Ability ability)
    {
        AlterMaterials(ability, true);
        ability.GetCastingPlayer().CmdToggleCamouflage(true, ability.GetCastingPlayer());
    }


    /////////ClueInterpreter
    public static void ActivateClueInterpretator(Ability ability)
    {
        if (particles == null)
            particles = new List<GameObject>();
        foreach(ArtefactBehaviour artefact in FindObjectsOfType<ArtefactBehaviour>())
        {
            float dist = Vector3.Distance(ability.GetCastingPlayer().transform.position, artefact.transform.position);
            if (dist < 50)
            {
                switch (artefact.GetRarity())
                {
                    case ArtefactRarity.Common:
                        particles.Add(GameObject.Instantiate(Resources.Load("Map/Artefacts/CommonArtefact_PA", typeof(GameObject))) as GameObject);
                        break;
                    case ArtefactRarity.Exotic:
                        particles.Add(GameObject.Instantiate(Resources.Load("Map/Artefacts/ExoticArtefact_PA", typeof(GameObject))) as GameObject);
                        break;
                    case ArtefactRarity.Rare:
                        particles.Add(GameObject.Instantiate(Resources.Load("Map/Artefacts/RareArtefact_PA", typeof(GameObject))) as GameObject);
                        break;
                }
                particles[particles.Count - 1].transform.position = artefact.transform.position;
            }

        }
        Instantiate(Resources.Load("Abilities/ClueInterpreterSphere"), ability.GetCastingPlayer().transform.position, Quaternion.identity);
        ability.SetInUse(true);
    }
    public static void DeactivateClueInterpretator(Ability ability)
    {
        List<GameObject> temp = particles;
        foreach (GameObject gameObject in temp)
        {
            Destroy(gameObject);
        }
        particles.Clear();
    }



    /////////PlayerTracker
    public static void ActivatePlayerTracker(Ability ability)
    {
        PlayerController playerController = FindHighestPlayerTarget();
        Debug.Log(playerController);
        if (playerController == null)
        {
            FindObjectOfType<CanvasUIManager>().PopupMessage("There are no players with artefacts");
            ability.playerTrackerPatch = false;
            return;
        }
        else
        {
            Debug.Log("Passed");
            ability.playerTrackerPatch = true;
            FindObjectOfType<CanvasUIManager>().playerTrackIcon.SetIconTarget(playerController);
            ability.SetInUse(true);
        }


        string uniqueAbilityIdentifier = "ThrowParalysisDartTimer";

        if (TargettingInUse(ability, uniqueAbilityIdentifier))
        {
            TargettingAbilityUse(ability);
            //Particle Effect

            //Paralyse the player
            ability.GetTargetedPlayer().CmdSetParalyzed(true);

        }
        else
        {
            PlayerController closestPlayer = FindClosestPlayer(ability, 30.0f);
            ClosestPlayerUse(ability, closestPlayer, uniqueAbilityIdentifier);
        }
    }
    public static void DeactivatePlayerTracker(Ability ability)
    {
        FindObjectOfType<CanvasUIManager>().playerTrackIcon.SetIconTarget(null);
    }
    public static PlayerController FindHighestPlayerTarget()
    {
        int highestInventoryStash = 0;
        PlayerController playerWithHighestStashOnPerson = null;
        foreach (PlayerController player in FindObjectsOfType<PlayerController>())
        {
            if (player == NetworkClient.localPlayer.gameObject.GetComponent<PlayerController>()) continue;

            List<ItemArtefact> inventory = player.GetComponent<ArtefactInventory>().GetInventory();
            int playerInventoryStash = 0;
            foreach (ItemArtefact item in inventory)
            {
                playerInventoryStash += item.points;
            }
            if(playerInventoryStash > highestInventoryStash)
            {
                highestInventoryStash = playerInventoryStash;
                playerWithHighestStashOnPerson = player;
            }
        }
        return playerWithHighestStashOnPerson;
    }

    #endregion

    #region TrapEffects

    /////////BearTrap
    public static void SpringBearTrap(Ability ability)
    {
        Vector3 spawnPos = ability.GetCastingPlayer().transform.position;
        RaycastHit hit;
        if (Physics.Raycast(spawnPos, Vector3.down, out hit, 10))
            spawnPos = hit.point;
        FindObjectOfType<Effects>().CmdSpawnBearTrap(spawnPos, ability.GetCastingPlayer());
        FindObjectOfType<AudioManager>().PlaySound("BearTrapOpening");
    }
    [Command (requiresAuthority = false)]
    private void CmdSpawnBearTrap(Vector3 spawnPos, PlayerController placingPlayer)
    {
        GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "BearTrap"), spawnPos, Quaternion.identity);
        go.GetComponent<BearTrapBehaviour>().SetPlacingPlayer(placingPlayer);
        NetworkServer.Spawn(go);
    }



    /////////VoodooTrap
    public static void SpringVoodooTrap(Ability ability)
    {
        RaycastHit hit;
        Vector3 spawnPos = Vector3.zero;
        ground = LayerMask.GetMask("ground", "DesertGround", "GrassGround", "SnowGround", "SwampGround", "JungleGround", "SwampWater");
        if (Physics.Raycast(ability.GetCastingPlayer().transform.position, Vector3.down, out hit, 30, ground))
        {
            spawnPos = hit.point;
            Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            spawnRotation *= Quaternion.Euler(-90, 0, 0);

            FindObjectOfType<Effects>().CmdSpawnVoodooTrap(spawnPos, ability.GetCastingPlayer(), spawnRotation);
        }
    }
    [Command (requiresAuthority = false)]
    public void CmdSpawnVoodooTrap(Vector3 spawnPos, PlayerController placingPlayer, Quaternion spawnRotation)
    {
        GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "VoodooPoisonTrap"), spawnPos, spawnRotation);
        go.GetComponent<VoodooPoisonTrapBehaviour>().SetPlacingPlayer(placingPlayer);
        NetworkServer.Spawn(go);
    }

    #endregion

    #region DebuffEffects



    /////////StickyBomb
    public static void ThrowStickyBomb(Ability ability)
    {
        string uniqueAbilityIdentifier = "ThrowStickyBombTimer";


        if(TargettingInUse(ability, uniqueAbilityIdentifier))
        {
            TargettingAbilityUse(ability);
            //Particle Effect
            Vector3 spawnPos = ability.GetTargetedPlayer().transform.position;
            FindObjectOfType<Effects>().CmdSpawnStickyBombParticles(spawnPos, ability.GetDuration());

            //Slow the player
            Ability speedBoost = ability.GetTargetedPlayer().abilityInventory.FindAbility("Speed");
            if (speedBoost != null)
                speedBoost.SetOppositeDebuffActivated(true);//This will make sure that the speed effect does not completely ovveride this effect
            ability.GetTargetedPlayer().CmdModifySpeed(5f);
            
        }
        else
        {
            PlayerController closestPlayer = FindClosestPlayer(ability, 30.0f);
            ClosestPlayerUse(ability, closestPlayer, uniqueAbilityIdentifier);
        }
    }
    public static void EndStickyBombEffect(Ability ability)
    {
        Ability speedBoost = ability.GetTargetedPlayer().abilityInventory.FindAbility("Speed");
        if (speedBoost != null)
            speedBoost.SetOppositeDebuffActivated(false);
        ability.GetTargetedPlayer().CmdModifySpeed(FindObjectOfType<PlayerController>().normalSpeed);
        ability.SetTargetedPlayer(null);
    }
    [Command (requiresAuthority = false)]
    private void CmdSpawnStickyBombParticles(Vector3 spawnPos, float effectDuration)
    {
        GameObject stickyBombParticles = Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefab => spawnPrefab.name == "SlowBombExplosion_PA"), spawnPos, Quaternion.identity);
        stickyBombParticles.GetComponent<StickyBombBehaviour>().effectDuration = effectDuration;
        stickyBombParticles.GetComponent<StickyBombBehaviour>().tick = true;
        NetworkServer.Spawn(stickyBombParticles);
    }



    /////////MortalSpell
    public static void CastMortalSpell(Ability ability)
    {
        string uniqueAbilityIdentifier = "ThrowMortalSpellTimer";
        if(TargettingInUse(ability, uniqueAbilityIdentifier))
        {
            TargettingAbilityUse(ability);

            //Particle Effect

            //Set player to mortal
            ability.GetTargetedPlayer().CmdSetMortal(true);

        }
        else
        {
 
            PlayerController closestPlayer = FindClosestPlayer(ability, 30.0f);
            ClosestPlayerUse(ability, closestPlayer, uniqueAbilityIdentifier);
        }
    }
    public static void EndMortalSpell(Ability ability)
    {
        ability.GetTargetedPlayer().CmdSetMortal(false);
        ability.SetTargetedPlayer(null);
    }



    /////////ParalysisDart
    public static void ThrowParalysisDart(Ability ability)
    {
        string uniqueAbilityIdentifier = "ThrowParalysisDartTimer";

        if (TargettingInUse(ability, uniqueAbilityIdentifier))
        {
            TargettingAbilityUse(ability);
            //Particle Effect

            //Paralyse the player
            ability.GetTargetedPlayer().CmdSetParalyzed(true);

        }
        else
        {
            PlayerController closestPlayer = FindClosestPlayer(ability, 30.0f);
            ClosestPlayerUse(ability, closestPlayer, uniqueAbilityIdentifier);
        }
    }
    public static void EndParalysisDartEffect(Ability ability)
    {
        ability.GetTargetedPlayer().CmdSetParalyzed(false);
        ability.SetTargetedPlayer(null);
    }

    #endregion

    #region UtilityFunctions

    //Now can be given a range to work in
    private static PlayerController FindClosestPlayer(Ability ability, float rangeDistance)
    {
        float shortestDistance = float.MaxValue;
        PlayerController closestPlayer = null;
        foreach(PlayerController player in FindObjectsOfType<PlayerController>())
        {
            if (player == ability.GetCastingPlayer()) continue;
            float newDistance = GetDistance(player.transform.position, ability.GetCastingPlayer().transform.position);
            if(newDistance < shortestDistance && newDistance < rangeDistance)
            {
                shortestDistance = newDistance;
                closestPlayer = player;
            }
        }
        return closestPlayer;
    }
    private static void ClosestPlayerUse(Ability ability, PlayerController closestPlayer, string uniqueAbilityIdentifier)
    {
        if (closestPlayer != null)
        {
            //Set targetting ui
            FindObjectOfType<CanvasUIManager>().targetIconGO.SetActive(true);
            FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(closestPlayer.gameObject);

            ability.SetTargetedPlayer(closestPlayer);
            GenericTimer.Create(() => { if (!ability.IsInUse()) ability.Use(); }, 3.0f, uniqueAbilityIdentifier);//Will throw bomb after 3 seconds, if it hasn't already been used
        }
        else
        {
            FindObjectOfType<CanvasUIManager>().PopupMessage("No players in the vicinity");
        }
    }
    private static void TargettingAbilityUse(Ability ability)
    {
        FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(null);
        FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(ability.GetAbilityName(), true);
        ability.SetInUse(true);
    }
    private static bool TargettingInUse(Ability ability, string uniqueAbilityIdentifier)
    {
        //If we have not used any targetting abilities
        if (ability.GetTargetedPlayer() == null)
        {
            currentTargettingAbilityTimer = uniqueAbilityIdentifier;
            return false;
        }
        //If we used a targetting ability, but we have clicked on a different targetting ability
        if (currentTargettingAbilityTimer != uniqueAbilityIdentifier)
        {
            DestroyAllTimersOfCurrentType();
            currentTargettingAbilityTimer = uniqueAbilityIdentifier;
            return false;
        }

        //We have clicked on the same ability for confirmation
        return true;
    }
    private static void DestroyAllTimersOfCurrentType()
    {
        GenericTimer.StopTimer(currentTargettingAbilityTimer);
    }


    private static float GetDistance(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }

    [Command(requiresAuthority = false)]
    public void CmdCreateAbilityEffectTimer(string abilityName, string targetPlayerName, float fullDuration)
    {
        RpcCreateAbilityEffectTimer(abilityName, targetPlayerName, fullDuration);
    }

    [ClientRpc]
    private void RpcCreateAbilityEffectTimer(string abilityName, string targetPlayerName, float fullDuration)
    {
        if (NetworkClient.localPlayer.GetComponent<PlayerController>().playerName == targetPlayerName)
            Ability.CreateLocalAbilityEffectTimer(abilityName, fullDuration, true);
    }



    //Don't know if I really want this here maybe somewhere else at some point really just proof of concept right now

    [Command(requiresAuthority = false)]
    public void CmdUpdateTargetTimer(string targetPlayerName, string abilityName, float duration)
    {
        RpcUpdateTargetTimer(targetPlayerName, abilityName, duration);
    }

    [ClientRpc]
    private void RpcUpdateTargetTimer(string targetPlayerName, string abilityName, float duration)
    {
        if (NetworkClient.localPlayer.GetComponent<PlayerController>().playerName == targetPlayerName)
            GameObject.FindObjectOfType<AbilityTimerContainer>().UpdateTimer(abilityName, duration);
    }

    #endregion 

}
