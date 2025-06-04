using AdminToys;
using Room = LabApi.Features.Wrappers.Room;
using Mirror;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Interfaces;
using UnityEngine;

namespace ProjectMER.Features.Serializable;

public class SerializableInteractable : SerializableObject
{
    public InvisibleInteractableToy.ColliderShape Shape { get; set; } = InvisibleInteractableToy.ColliderShape.Box;
    public float InteractionDuration { get; set; } = 5f;
    public bool Locked { get; set; } = false;
    
    public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
    {
        InvisibleInteractableToy interactableToy = instance == null ? UnityEngine.Object.Instantiate(PrefabManager.InvisibleInteractableToy) : instance.GetComponent<InvisibleInteractableToy>();
        Vector3 position = room.GetAbsolutePosition(Position);
        Quaternion rotation = room.GetAbsoluteRotation(Rotation);
        _prevIndex = Index;

        interactableToy.transform.SetPositionAndRotation(position, rotation);
        interactableToy.transform.localScale = Scale;
        interactableToy.NetworkMovementSmoothing = 60;
        
        interactableToy.NetworkShape = Shape;
        interactableToy.NetworkInteractionDuration = InteractionDuration;
        interactableToy.NetworkIsLocked = Locked;

        if (instance == null)
            NetworkServer.Spawn(interactableToy.gameObject);

        return interactableToy.gameObject;
    }
}