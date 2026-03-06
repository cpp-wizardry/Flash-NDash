using UnityEngine;

public class POIPoint : MonoBehaviour
{
    [Tooltip("What kind of place is this?")]
    public POIType poiType;

    [Tooltip("How long an NPC will wait here in seconds")]
    public float waitTime = 5f;

    [Tooltip("How close the NPC needs to get before it counts as arrived")]
    public float arrivalRadius = 1.5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = GetGizmoColor();
        Gizmos.DrawSphere(transform.position, 0.4f);
        Gizmos.DrawWireSphere(transform.position, arrivalRadius);
    }

    private Color GetGizmoColor()
    {
        return poiType switch
        {
            POIType.House => Color.yellow,
            POIType.Park => Color.green,
            POIType.Stage => Color.magenta,
            POIType.FoodStall => Color.red,
            POIType.Shop => Color.cyan,
            POIType.BusStop => Color.blue,
            _ => Color.white
        };
    }
}

public enum POIType
{
    House,
    Park,
    Stage,
    FoodStall,
    Shop,
    BusStop,
    Work,
    Accident,
    Illicit,
    Tags,
    CarStop,
    Generic
}