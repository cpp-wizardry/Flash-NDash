using System.Collections.Generic;
using UnityEngine;

public class PhotoValidator : MonoBehaviour
{
    public Camera photoCamera;
    public float maxDistance = 30f;

    void Start()
    {
        if (photoCamera == null)
            photoCamera = Camera.main;
    }

    public PhotoFrameData Evaluate()
    {
        var data = new PhotoFrameData();
        Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(photoCamera);

        foreach (var col in Physics.OverlapSphere(photoCamera.transform.position, maxDistance))
        {
            if (!GeometryUtility.TestPlanesAABB(frustum, col.bounds))
                continue;

            if (!IsVisible(col))
                continue;

            NPCReaction reaction = col.GetComponentInParent<NPCReaction>();
            if (reaction != null)
            {
                data.npcTags.Add(col.tag);
                if (reaction.IsReacting)
                    data.reactionStates.Add(reaction.CurrentReaction);
                continue;
            }

            POIPoint poi = col.GetComponentInParent<POIPoint>();
            if (poi != null)
                data.poiTags.Add(col.tag);
        }

        return data;
    }

    private bool IsVisible(Collider col)
    {
        Vector3 dir = col.bounds.center - photoCamera.transform.position;
        if (Physics.Raycast(photoCamera.transform.position, dir.normalized, out RaycastHit hit, maxDistance))
            return hit.collider == col || hit.collider.GetComponentInParent<NPCReaction>() != null;
        return false;
    }
}

public class PhotoFrameData
{
    public List<string> npcTags = new();
    public List<string> poiTags = new();
    public List<ReactionType> reactionStates = new();
}