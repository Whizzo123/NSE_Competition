using System.Collections;
using UnityEngine;


public class ClueInterpretator : Powerup
{

    private GameObject particle;

    public ClueInterpretator() : base("Clue Interpretator", "Gain insight to where your nearby artefacts works", 5, AbilityUseTypes.RECHARGE, 20.0f, 10.0f )
    {

    }

    public override void Use()
    {
        //do stuff
        ArtefactBehaviour artefact = FindRarestArtefact();
        if(artefact == null)
        {
            Debug.LogError("NO ARTEFACTS AROUND PLAYER MUST BE HANDLED");
            GameObject.FindObjectOfType<CanvasUIManager>().PopupMessage("NO ARTEFACTS IN CURRENT VICINITY");
        }
        else
        {
            //Spawn in particle effect
            switch(artefact.GetRarity())
            {
                case ArtefactRarity.Common:
                    particle = GameObject.Instantiate(Resources.Load("Effects/CommonParticle", typeof(GameObject))) as GameObject;
                    break;
                case ArtefactRarity.Exotic:
                    particle = GameObject.Instantiate(Resources.Load("Effects/ExoticParticle", typeof(GameObject))) as GameObject;
                    break;
                case ArtefactRarity.Rare:
                    particle = GameObject.Instantiate(Resources.Load("Effects/RareParticle", typeof(GameObject))) as GameObject;
                    break;
            }
            particle.transform.position = artefact.transform.position;
        }
        //-----
        inUse = true;
    }

    protected override void EndEffect()
    {
        //Do end effect
        GameObject.Destroy(particle);
        //-------------
        base.EndEffect();
    }

    private ArtefactBehaviour FindRarestArtefact()
    {
        float maxDistance = 20.0f;
        ArtefactBehaviour rarestArtefact = null;
        ArtefactRarity highestRaritySoFar = ArtefactRarity.Common;
        foreach (BoltEntity entity in BoltNetwork.Entities)
        {
            if (entity.GetComponent<ArtefactBehaviour>() == false) continue;
            if (Vector3.Distance(GetPlayerToEmpower().transform.position, entity.transform.position) <= maxDistance)
            {
                if (entity.GetComponent<ArtefactBehaviour>().GetRarity() >= highestRaritySoFar)
                {
                    highestRaritySoFar = entity.GetComponent<ArtefactBehaviour>().GetRarity();
                    rarestArtefact = entity.GetComponent<ArtefactBehaviour>();
                }
            }
        }
        if(rarestArtefact != null)
        {
            return rarestArtefact;
        }
        else
        {
            return null;
        }
    }

    public override Ability Clone()
    {
        return new ClueInterpretator();
    }
}
