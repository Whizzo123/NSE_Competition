using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ClueInterpretator : Powerup
{

    private List<GameObject> particles;
    private GameObject particle;

    public ClueInterpretator() : base("Clue Interpretator", "Gain insight to where your nearby artefacts works", 5, AbilityUseTypes.RECHARGE, 20.0f, 10.0f )
    {
        particles = new List<GameObject>();
    }

    public override void Use()
    {
        if (particles == null)
            particles = new List<GameObject>();
        //do stuff
        foreach (BoltEntity entity in BoltNetwork.Entities)
        {
            if (entity.GetComponent<ArtefactBehaviour>())
            {
                //ArtefactBehaviour artefact = FindRarestArtefact();
                if (entity == null)
                {
                    Debug.LogError("NO ARTEFACTS AROUND PLAYER MUST BE HANDLED");
                    GameObject.FindObjectOfType<CanvasUIManager>().PopupMessage("NO ARTEFACTS IN CURRENT VICINITY");
                }
                else
                {
                    Debug.Log("Loading in particles");
                    //Spawn in particle effect
                    switch (entity.GetComponent<ArtefactBehaviour>().GetRarity())
                    {
                        case ArtefactRarity.Common:
                            particles.Add(GameObject.Instantiate(Resources.Load("Effects/CommonParticle", typeof(GameObject))) as GameObject);
                            break;
                        case ArtefactRarity.Exotic:
                            particles.Add(GameObject.Instantiate(Resources.Load("Effects/ExoticParticle", typeof(GameObject))) as GameObject);
                            break;
                        case ArtefactRarity.Rare:
                            particles.Add(GameObject.Instantiate(Resources.Load("Effects/RareParticle", typeof(GameObject))) as GameObject);
                            break;
                    }
                    particles[particles.Count - 1].transform.position = entity.transform.position;
                }
            }
        }
        //-----
        inUse = true;
    }

    protected override void EndEffect()
    {
        //Do end effect
        List<GameObject> temp = particles;
        foreach (GameObject gameObject in temp)
        {
            GameObject.Destroy(gameObject);
            //particles.Remove(gameObject);
        }
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
