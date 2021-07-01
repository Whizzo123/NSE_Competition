using System.Collections;
using UnityEngine;
using Mirror;

public class StickyBomb : Debuff 
{

    public GameObject stickyBombParticles;

    public StickyBomb() : base ("StickyBomb", "Stun an opponent of your choosing to change the tides", 1, AbilityUseTypes.RECHARGE, 30.0f, 20.0f)
    {
        
    }

    public override void Use()
    {
        BoltLog.Info("Inside stun use");
        if(target == null)
        {
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.SetActive(true);
            PlayerController closestPlayer = FindClosestPlayer();
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(closestPlayer.gameObject);
            target = closestPlayer;
        }
        else
        {
            inUse = true;

            stickyBombParticles = GameObject.Instantiate(MyNetworkManager.singleton.spawnPrefabs.Find(spawnPrefab =>
                spawnPrefab.name == "SlowBombExplosion_PA"), target.transform.position, Quaternion.identity);
            NetworkServer.Spawn(stickyBombParticles);

            Explode(false);
            GameObject.FindObjectOfType<CanvasUIManager>().targetIconGO.GetComponent<DebuffTargetIcon>().SetTargetIconObject(null);
            GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(name, true);
            base.Use();
        }
    }

    private void Explode(bool finish)
    {
            if (!finish)
            {
                SpeedBoost spd = (SpeedBoost)target.abilityInventory.FindAbility("Speed");
                if (spd != null)
                    spd.SetOppositeDebuffActivated(true);
                //target.speed = 1f;
            }
            else
            {
                SpeedBoost spd = (SpeedBoost)target.abilityInventory.FindAbility("Speed");
                if (spd != null)
                    spd.SetOppositeDebuffActivated(false);
                //target.speed = FindObjectOfType<PlayerController>().normalSpeed;
            }
    }

    public override void EndEffect()
    {
        inUse = false;
        Explode(true);
        target = null;
        GameObject.FindObjectOfType<AbilitySlotBarUI>().SetSlotUseState(name, false);
        base.EndEffect();
        NetworkServer.Destroy(stickyBombParticles);
    }

    public override Ability Clone()
    {
        return new StickyBomb();
    }
}
