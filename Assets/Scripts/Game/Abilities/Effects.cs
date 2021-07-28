using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class Effects : NetworkBehaviour
{

    private static float boostToSpeed = 7.5f;
    private static List<GameObject> particles;
    private static LayerMask ground;

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
        ability.GetCastingPlayer().CmdToggleCamouflage(false, ability.GetCastingPlayer());
        ability.SetInUse(true);
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
        ability.GetCastingPlayer().CmdToggleCamouflage(true, ability.GetCastingPlayer());
    }

    public static void ActivateClueInterpretator(Ability ability)
    {
        if (particles == null)
            particles = new List<GameObject>();
        foreach(ArtefactBehaviour artefact in FindObjectsOfType<ArtefactBehaviour>())
        {
            switch(artefact.GetRarity())
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
            FindObjectOfType<Effects>().CmdSpawnVoodooTrap(spawnPos, ability.GetCastingPlayer());
        }
    }

    [Command (requiresAuthority = false)]
    public void CmdSpawnVoodooTrap(Vector3 spawnPos, PlayerController placingPlayer)
    {
        GameObject go = Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefabs => spawnPrefabs.name == "VoodooPoisonTrap"), spawnPos, Quaternion.identity);
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
            FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(closestPlayer.gameObject);
            ability.SetTargetedPlayer(closestPlayer);
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

    #endregion 

}
