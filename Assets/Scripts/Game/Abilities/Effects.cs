using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEditor;

public class Effects : NetworkBehaviour
{
    private static GenericTimer genericTimer;
    private static float boostToSpeed = 7.5f;
    private static List<GameObject> particles;
    private static LayerMask ground;
    private static Material[] playerNormalMats;
    private static  Material[] playerFadeMats;

    private void Start()
    {
        genericTimer = GameObject.Find("GenericObject").GetComponent<GenericTimer>();
        playerNormalMats = new Material[11];
        playerNormalMats[0] = Resources.Load<Material>("Character/Extracted Mats/Player_1_Mat");
        playerNormalMats[1] = Resources.Load<Material>("Character/Extracted Mats/Player_2_Mat");
        playerNormalMats[2] = Resources.Load<Material>("Character/Extracted Mats/Player_3_Mat");
        playerNormalMats[3] = Resources.Load<Material>("Character/Extracted Mats/Player_4_Mat");
        playerNormalMats[4] = Resources.Load<Material>("Character/Extracted Mats/Player_5_Mat");
        playerNormalMats[5] = Resources.Load<Material>("Character/Extracted Mats/Player_6_Mat");
        playerNormalMats[6] = Resources.Load<Material>("Character/Extracted Mats/Player_7_Mat");
        playerNormalMats[7] = Resources.Load<Material>("Character/Extracted Mats/Player_8_Mat");
        playerNormalMats[8] = Resources.Load<Material>("Character/Extracted Mats/Player_9_Mat");
        playerNormalMats[9] = Resources.Load<Material>("Character/Extracted Mats/Player_10_Mat");
        playerNormalMats[10] = Resources.Load<Material>("Character/Extracted Mats/Player_11_Mat");

        playerFadeMats = new Material[11];
        playerFadeMats[0] = Resources.Load<Material>("Character/Extracted Mats/Player_1_FadeMat");
        playerFadeMats[1] = Resources.Load<Material>("Character/Extracted Mats/Player_2_FadeMat");
        playerFadeMats[2] = Resources.Load<Material>("Character/Extracted Mats/Player_3_FadeMat");
        playerFadeMats[3] = Resources.Load<Material>("Character/Extracted Mats/Player_4_FadeMat");
        playerFadeMats[4] = Resources.Load<Material>("Character/Extracted Mats/Player_5_FadeMat");
        playerFadeMats[5] = Resources.Load<Material>("Character/Extracted Mats/Player_6_FadeMat");
        playerFadeMats[6] = Resources.Load<Material>("Character/Extracted Mats/Player_7_FadeMat");
        playerFadeMats[7] = Resources.Load<Material>("Character/Extracted Mats/Player_8_FadeMat");
        playerFadeMats[8] = Resources.Load<Material>("Character/Extracted Mats/Player_9_FadeMat");
        playerFadeMats[9] = Resources.Load<Material>("Character/Extracted Mats/Player_10_FadeMat");
        playerFadeMats[10] = Resources.Load<Material>("Character/Extracted Mats/Player_11_FadeMat");
    }

    #region PowerupEffects

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
    }

    public static void DeactivateCamouflage(Ability ability)
    {
        AlterMaterials(ability, true);
        ability.GetCastingPlayer().CmdToggleCamouflage(true, ability.GetCastingPlayer());
    }

    public static void ActivateClueInterpretator(Ability ability)
    {
        if (particles == null)
            particles = new List<GameObject>();
        foreach(ArtefactBehaviour artefact in FindObjectsOfType<ArtefactBehaviour>())
        {
            float dist = Vector3.Distance(ability.GetCastingPlayer().transform.position, artefact.transform.position);
            Debug.LogError(dist);
            if (dist < 30)
            {
                switch (artefact.GetRarity())
                {
                    case ArtefactRarity.Common:
                        particles.Add(GameObject.Instantiate(Resources.Load("Artefacts/CommonArtefact_PA", typeof(GameObject))) as GameObject);
                        break;
                    case ArtefactRarity.Exotic:
                        particles.Add(GameObject.Instantiate(Resources.Load("Artefacts/ExoticArtefact_PA", typeof(GameObject))) as GameObject);
                        break;
                    case ArtefactRarity.Rare:
                        particles.Add(GameObject.Instantiate(Resources.Load("Artefacts/RareArtefact_PA", typeof(GameObject))) as GameObject);
                        break;
                }
                particles[particles.Count - 1].transform.position = artefact.transform.position;
            }

        }

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

    public static void ActivatePlayerTracker(Ability ability)
    {
        FindObjectOfType<CanvasUIManager>().playerTrackIcon.SetIconTarget(FindHighestPlayerTarget());
        ability.SetInUse(true);
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
        if(playerWithHighestStashOnPerson == null)
        {
            Debug.LogError("COULDN'T FIND A HIGHER STASH THAN 0 IN ANOTHER PLAYER IN THE GAME");
            return null;
        }
        else
        {
            return playerWithHighestStashOnPerson.GetComponent<PlayerController>();
        }
    }

    #endregion

    #region TrapEffects

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

    public static void ThrowStickyBomb(Ability ability)
    {
        if(ability.GetTargetedPlayer() == null)
        {
            FindObjectOfType<CanvasUIManager>().targetIconGO.SetActive(true);
            PlayerController closestPlayer = FindClosestPlayer(ability);
            if (closestPlayer != null)
            {
                FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(closestPlayer.gameObject);
                ability.SetTargetedPlayer(closestPlayer);
                Debug.Log("Setting targeted player");
                genericTimer.SetTimer(3f, () => { Debug.Log("StickyTimer Up"); ability.Use(); });//Will throw bomb after 3 seconds
            }
            else
            {
                FindObjectOfType<CanvasUIManager>().PopupMessage("No players in the vicinity");
            }
        }
        else
        {
            FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(null);
            ability.SetInUse(true);
            Vector3 spawnPos = ability.GetTargetedPlayer().transform.position;
            FindObjectOfType<Effects>().CmdSpawnStickyBombParticles(spawnPos, ability.GetDuration());
            Ability speedBoost = ability.GetTargetedPlayer().abilityInventory.FindAbility("Speed");
            if (speedBoost != null)
                speedBoost.SetOppositeDebuffActivated(true);
            ability.GetTargetedPlayer().CmdModifySpeed(5f);
            Debug.Log("Got player commencing throw");
            
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

    public static void CastMortalSpell(Ability ability)
    {
        if(ability.GetTargetedPlayer() == null)
        {
            FindObjectOfType<CanvasUIManager>().targetIconGO.SetActive(true);
            PlayerController closestPlayer = FindClosestPlayer(ability);
            FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(closestPlayer.gameObject);
            ability.SetTargetedPlayer(closestPlayer);
        }
        else
        {
            ability.SetInUse(true);
            ability.GetTargetedPlayer().CmdSetMortal(true);
            FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(null);
            FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(ability.GetAbilityName(), true);
        }
    }

    public static void EndMortalSpell(Ability ability)
    {
        ability.GetTargetedPlayer().CmdSetMortal(false);
        ability.SetTargetedPlayer(null);
    }

    public static void ThrowParalysisDart(Ability ability)
    {
        if(ability.GetTargetedPlayer() == null)
        {
            FindObjectOfType<CanvasUIManager>().targetIconGO.SetActive(true);
            PlayerController closestPlayer = FindClosestPlayer(ability);
            FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(closestPlayer.gameObject);
            ability.SetTargetedPlayer(closestPlayer);
        }
        else
        {
            ability.SetInUse(true);
            ability.GetTargetedPlayer().CmdSetImmobilized(true);
            FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(null);
            FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(ability.GetAbilityName(), true);
        }
    }

    public static void EndParalysisDartEffect(Ability ability)
    {
        ability.GetTargetedPlayer().CmdSetImmobilized(false);
        ability.SetTargetedPlayer(null);
    }

    #endregion

    #region UtilityFunctions

    private static PlayerController FindClosestPlayer(Ability ability)
    {
        float shortestDistance = float.MaxValue;
        PlayerController closestPlayer = null;
        foreach(PlayerController player in FindObjectsOfType<PlayerController>())
        {
            if (player == ability.GetCastingPlayer()) continue;
            float newDistance = GetDistance(player.transform.position, ability.GetCastingPlayer().transform.position);
            if(newDistance < shortestDistance)
            {
                shortestDistance = newDistance;
                closestPlayer = player;
            }
        }
        return closestPlayer;
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
            Ability.CreateLocalAbilityEffectTimer(abilityName, fullDuration);
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
