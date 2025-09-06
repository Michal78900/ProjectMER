using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using UnityEngine;
namespace ProjectMER.Features.Extensions
{
    public static class ExiledExtensions
    {
        private static Assembly? ExiledLoaderAssembly;

        private static MethodInfo? CustomItemTrySpawnMethodInfo;

        private static MethodInfo? PickupGetBaseMethodInfo;

        public static Pickup? TrySpawn(string customItemName, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (!TrySpawnCustomItem(customItemName, position, out Pickup? pickup))
                return null;

            pickup.Rotation = rotation;
            pickup.Transform.localScale = scale;

            return pickup;
        }

        internal static bool TrySpawnCustomItem(string name, Vector3 position, [NotNullWhen(true)] out Pickup? pickup)
        {
            pickup = null;

            if (CustomItemTrySpawnMethodInfo is null)
                return false;

            object?[] args = [name, position, null];

            CustomItemTrySpawnMethodInfo.Invoke(null, args);

            if (args[2] is null)
                return false;

            pickup = GetPickupFromExiledPickup(args[2]!);

            return pickup is not null;
        }


        internal static Pickup? GetPickupFromExiledPickup(object pickup)
        {
            if (PickupGetBaseMethodInfo is null)
                return null;

            object obj = PickupGetBaseMethodInfo.Invoke(pickup, []);

            if (obj is not ItemPickupBase pickupBase)
            {
                Logger.Warn("Failed to get ItemPickupBase from Exiled Pickup");
                return null;
            }

            return Pickup.Get(pickupBase);
        }

        internal static void TryInitialize()
        {
            const string ErrorMessage = "Failed to find a component of Exiled! This can be ignored if you do not use Exiled Custom Items in ProjectMER. Cause: ";

            KeyValuePair<Plugin, Assembly> kvp = PluginLoader.Plugins.FirstOrDefault(kvp => kvp.Key.Name is "Exiled Loader");

            if (kvp.Value is null)
            {
                // No debug in config :(
                // not printed because ppl can just not use Exiled, but having Exiled.Loader and not API / CI is suspicious
                // Logger.Debug(ErrorMessage + "Failed to get ExiledLoader Plugin");
                return;
            }

            ExiledLoaderAssembly = kvp.Value;

            object? plugin = ExiledLoaderAssembly.GetType("Exiled.Loader.Loader")?.GetMethod("GetPlugin")?.Invoke(null, ["exiled_custom_items"]);

            if (plugin is null)
            {
                // It's possible people use Exiled without CI, so I'll leave this out too
                // Logger.Debug(ErrorMessage + "Failed to get CustomItems Plugin");
                return;
            }

            object? nullableAssembly = plugin.GetType().GetProperty("Assembly")?.GetValue(plugin);

            if (nullableAssembly is not Assembly customItemAssembly)
            {
                Logger.Warn(ErrorMessage + "Failed to get CustomItems Assembly");
                return;
            }

            Type? customItem = customItemAssembly.GetType("Exiled.CustomItems.API.Features.CustomItem");

            if (customItem is null)
            {
                Logger.Warn(ErrorMessage + "Failed to get CustomItem Type");
                return;
            }

            CustomItemTrySpawnMethodInfo = customItem.GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault(method => method.Name is "TrySpawn" && method.GetParameters().Any(parameter => parameter.ParameterType == typeof(string)));

            if (CustomItemTrySpawnMethodInfo is null)
                Logger.Warn(ErrorMessage + "Failed to get CustomItem.TrySpawn method");

            Assembly? apiAssembly = PluginLoader.Dependencies.FirstOrDefault(assembly => assembly.GetName().Name is "Exiled.API");

            if (apiAssembly is null)
            {
                Logger.Warn(ErrorMessage + "Failed to get Exiled.API Assembly");
                return;
            }

            Type? pickupType = apiAssembly.GetType("Exiled.API.Features.Pickups.Pickup");

            if (pickupType is null)
            {
                Logger.Warn(ErrorMessage + "Failed to get Pickup Type");
                return;
            }

            PickupGetBaseMethodInfo = pickupType.GetProperty("Base")?.GetGetMethod();

            if (PickupGetBaseMethodInfo is null)
            {
                Logger.Warn(ErrorMessage + "Failed to get Pickup::Base getter method");
            }
        }
    }
}