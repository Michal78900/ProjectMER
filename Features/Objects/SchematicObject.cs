using AdminToys;
using Mirror;
using ProjectMER.Events.Handlers;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Serializable.Schematics;
using UnityEngine;
using Utf8Json;
using Utils.NonAllocLINQ;
using Object = UnityEngine.Object;

namespace ProjectMER.Features.Objects;

public class SchematicObject : MonoBehaviour
{
	/// <summary>
	/// Gets the schematic name.
	/// </summary>
	public string Name { get; private set; }

	/// <summary>
	/// Gets a schematic directory path.
	/// </summary>
	public string DirectoryPath { get; private set; }

	/// <summary>
	/// Gets or sets the global position of the object.
	/// </summary>
	public Vector3 Position
	{
		get => transform.position;
		set
		{
			transform.position = value;
		}
	}

	/// <summary>
	/// Gets or sets the global rotation of the object.
	/// </summary>
	public Quaternion Rotation
	{
		get => transform.rotation;
		set
		{
			transform.rotation = value;
		}
	}

	/// <summary>
	/// Gets or sets the global euler angles of the object.
	/// </summary>
	public Vector3 EulerAngles
	{
		get => Rotation.eulerAngles;
		set => Rotation = Quaternion.Euler(value);
	}

	/// <summary>
	/// Gets or sets the scale of the object.
	/// </summary>
	public Vector3 Scale
	{
		get => transform.localScale;
		set
		{
			transform.localScale = value;
		}
	}

	public IReadOnlyList<GameObject> AttachedBlocks
	{
		get
		{
			if (_attachedBlocks.Count != 0 && _attachedBlocks.All(x => x != null))
				return _attachedBlocks;

			_attachedBlocks.Clear();
			foreach (Transform transform in GetComponentsInChildren<Transform>())
			{
				if (transform == this.transform)
					continue;

				_attachedBlocks.Add(transform.gameObject);
			}

			return _attachedBlocks;
		}
	}

	public IReadOnlyList<NetworkIdentity> NetworkIdentities
	{
		get
		{
			if (_networkIdentities.Count > 0 && _networkIdentities.All(x => x != null))
				return _networkIdentities;

			_networkIdentities.Clear();
			foreach (GameObject block in AttachedBlocks)
			{
				if (block.TryGetComponent(out NetworkIdentity networkIdentity))
					_networkIdentities.Add(networkIdentity);
			}

			return _networkIdentities;
		}
	}

	public IReadOnlyList<AdminToyBase> AdminToyBases
	{
		get
		{
			if (_adminToyBases.Count > 0 && _adminToyBases.All(x => x != null))
				return _adminToyBases;

			_adminToyBases.Clear();
			foreach (NetworkIdentity netId in NetworkIdentities)
			{
				if (netId.TryGetComponent(out AdminToyBase adminToyBase))
					_adminToyBases.Add(adminToyBase);
			}

			return _adminToyBases;
		}
	}

	public AnimationController AnimationController => AnimationController.Get(this);

	public SchematicObject Init(SchematicObjectDataList data)
	{
		Name = Path.GetFileNameWithoutExtension(data.Path);
		DirectoryPath = data.Path;

		ObjectFromId = new Dictionary<int, Transform>(data.Blocks.Count + 1)
		{
			{ data.RootObjectId, transform },
		};

		CreateRecursiveFromID(data.RootObjectId, data.Blocks, transform);

		AddRigidbodies();
		AddAnimators();

        // Load teleports after creating regular schematic objects,
        // so all potential parent objects are already available.
        AddTeleports();

        Schematic.OnSchematicSpawned(new(this, Name));

		return this;
	}

	private void CreateRecursiveFromID(int id, List<SchematicBlockData> blocks, Transform parentGameObject)
	{
		Transform childGameObjectTransform = CreateObject(blocks.Find(c => c.ObjectId == id), parentGameObject) ?? transform; // Create the object first before creating children.
		int[] parentSchematics = blocks.Where(bl => bl.BlockType == BlockType.Schematic).Select(bl => bl.ObjectId).ToArray();

		// Gets all the ObjectIds of all the schematic blocks inside "blocks" argument.
		foreach (SchematicBlockData block in blocks.FindAll(c => c.ParentId == id))
		{
			if (parentSchematics.Contains(block.ParentId)) // The block is a child of some schematic inside "parentSchematics" array, therefore it will be skipped to avoid spawning it and its children twice.
				continue;

			CreateRecursiveFromID(block.ObjectId, blocks, childGameObjectTransform); // The child now becomes the parent
		}
	}

	private Transform? CreateObject(SchematicBlockData block, Transform parentTransform)
	{
		if (block == null)
			return null;

		GameObject gameObject = block.Create(this, parentTransform);
		NetworkServer.Spawn(gameObject);

		ObjectFromId.Add(block.ObjectId, gameObject.transform);

		if (block.BlockType != BlockType.Light && TryGetAnimatorController(block.AnimatorName, out RuntimeAnimatorController animatorController))
			_animators.Add(gameObject, animatorController);

		return gameObject.transform;
	}

	private bool TryGetAnimatorController(string animatorName, out RuntimeAnimatorController animatorController)
	{
		animatorController = null!;

		if (string.IsNullOrEmpty(animatorName))
			return false;

		Object? animatorObject = AssetBundle.GetAllLoadedAssetBundles().FirstOrDefault(x => x.mainAsset.name == animatorName)?.LoadAllAssets().First(x => x is RuntimeAnimatorController);

		if (animatorObject is null)
		{
			string path = Path.Combine(DirectoryPath, animatorName);

			if (!File.Exists(path))
			{
				Logger.Warn($"{gameObject.name} block of schematic should have a {animatorName} animator attached, but the file does not exist!");
				return false;
			}

			animatorObject = AssetBundle.LoadFromFile(path).LoadAllAssets().First(x => x is RuntimeAnimatorController);
		}

		animatorController = (RuntimeAnimatorController)animatorObject;
		return true;
	}

	private bool AddAnimators()
	{
		bool isAnimated = false;
		if (!_animators.IsEmpty())
		{
			isAnimated = true;
			foreach (KeyValuePair<GameObject, RuntimeAnimatorController> pair in _animators)
				pair.Key.AddComponent<Animator>().runtimeAnimatorController = pair.Value;
		}

		_animators.Clear();
		AssetBundle.UnloadAllAssetBundles(false);
		return isAnimated;
	}

	private bool AddRigidbodies()
	{
		bool hasRigidbodies = false;
		string rigidbodyPath = Path.Combine(DirectoryPath, $"{Name}-Rigidbodies.json");
		if (!File.Exists(rigidbodyPath))
			return false;

		foreach (KeyValuePair<int, SerializableRigidbody> dict in JsonSerializer.Deserialize<Dictionary<int, SerializableRigidbody>>(File.ReadAllText(rigidbodyPath)))
		{
			if (!ObjectFromId.TryGetValue(dict.Key, out Transform transform))
				continue;

			if (!transform.gameObject.TryGetComponent(out Rigidbody rigidbody))
				rigidbody = transform.gameObject.AddComponent<Rigidbody>();

			rigidbody.isKinematic = dict.Value.IsKinematic;
			rigidbody.useGravity = dict.Value.UseGravity;
			rigidbody.constraints = dict.Value.Constraints;
			rigidbody.mass = dict.Value.Mass;

			hasRigidbodies = true;
		}

		return hasRigidbodies;
	}

    /// <summary>
    /// Loads teleport data from a separate schematic file
    /// and creates game objects with trigger components.
    /// </summary>
    private bool AddTeleports()
    {
        // Unity exports teleport data into a separate file.
        string teleportPath = Path.Combine(DirectoryPath, $"{Name}-Teleports.json");

        // Skip teleport loading if the file does not exist.
        if (!File.Exists(teleportPath))
            return false;

        // Read the JSON file and deserialize it into teleport data.
        List<SchematicTeleportData> teleports =
            JsonSerializer.Deserialize<List<SchematicTeleportData>>(File.ReadAllText(teleportPath));

        if (teleports == null || teleports.Count == 0)
            return false;

        // Stores all created teleports.
        // The key is the teleport's ObjectId, used to find target teleporters.
        Dictionary<int, SchematicTeleportObject> teleportObjects = new();

        foreach (SchematicTeleportData data in teleports)
        {
            // Try to find the parent object using ParentId.
            // If it cannot be found, use the schematic root as the parent.
            Transform parent = ObjectFromId.TryGetValue(data.ParentId, out Transform parentTransform)
                ? parentTransform
                : transform;

            GameObject gameObject = new GameObject(data.Name);

            gameObject.transform.SetParent(parent);

            // Restore the teleport's local position, rotation, and scale
            // using the data exported from Unity.
            gameObject.transform.SetLocalPositionAndRotation(
                data.Position,
                Quaternion.Euler(data.Rotation)
            );

            gameObject.transform.localScale = data.Scale;

            // Create a trigger collider to detect players
            // entering the teleport's area.
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;

            // Add the component responsible for teleport functionality
            // and assign the data loaded from the schematic.
            SchematicTeleportObject teleportObject = gameObject.AddComponent<SchematicTeleportObject>();
            teleportObject.Base = data;

            // Store the teleport by its ObjectId so other teleports
            // can find it when selecting a destination.
            teleportObjects.Add(data.ObjectId, teleportObject);
        }

        // Give every teleport access to the dictionary containing all teleports.
        // This allows each teleport to find any other teleport by its ID.
        foreach (SchematicTeleportObject teleportObject in teleportObjects.Values)
            teleportObject.AllTeleports = teleportObjects;

        return true;
    }

    public void Destroy() => Destroy(gameObject);

	private void OnDestroy()
	{
		AnimationController.Dictionary.Remove(this);
		NetworkServer.Destroy(gameObject);
		Schematic.OnSchematicDestroyed(new(this, Name));
	}

	internal Dictionary<int, Transform> ObjectFromId = [];

	private readonly List<GameObject> _attachedBlocks = [];
	private readonly List<NetworkIdentity> _networkIdentities = [];
	private readonly List<AdminToyBase> _adminToyBases = [];
	private readonly Dictionary<GameObject, RuntimeAnimatorController> _animators = [];
}
