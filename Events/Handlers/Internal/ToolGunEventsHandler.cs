using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MEC;
using ProjectMER.Features;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using ProjectMER.Features.ToolGun;
using UserSettings.ServerSpecific;

namespace ProjectMER.Events.Handlers.Internal;

public class ToolGunEventsHandler : CustomEventsHandler
{
    private string lastLoadedMap = "None";
    private bool? lastIndicatorState = null;

    public override void OnServerRoundStarted()
	{
		Timing.RunCoroutine(ToolGunGUI());
        Timing.RunCoroutine(ToolGunMapController());
    }

	private static IEnumerator<float> ToolGunGUI()
	{
		while (true)
		{
			yield return Timing.WaitForSeconds(0.1f);

			foreach (Player player in Player.List)
			{
				if (!player.CurrentItem.IsToolGun(out ToolGunItem _) && !ToolGunHandler.TryGetSelectedMapObject(player, out MapEditorObject _))
					continue;

				string hud;
				try
				{
					hud = ToolGunUI.GetHintHUD(player);
				}
				catch (Exception e)
				{
					Logger.Error(e);
					hud = "ERROR: Check server console";
				}

				player.SendHint(hud, 0.25f);
			}
		}
	}

    private IEnumerator<float> ToolGunMapController()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(0.5f);

            foreach (Player player in Player.List)
            {
                if (!player.CurrentItem.IsToolGun(out ToolGunItem toolGun))
                    continue;

                if (ServerSpecificSettingsSync.TryGetSettingOfUser(player.ReferenceHub, 4, out SSTwoButtonsSetting indicatorToggle))
                {
                    bool currentState = indicatorToggle.SyncIsA;

                    if (lastIndicatorState == null || currentState != lastIndicatorState.Value)
                    {
                        if (currentState)
                        {
                            IndicatorObject.ClearIndicators();
                        }
                        else
                        {
                            IndicatorObject.RefreshIndicators();
                        }

                        lastIndicatorState = currentState;
                    }
                }

                if (!ServerSpecificSettingsSync.TryGetSettingOfUser(player.ReferenceHub, 1, out SSDropdownSetting dropdown))
                    continue;

                string selectedMap = dropdown.TryGetSyncSelectionText(out string mapName) ? mapName : "None";

                if (ServerSpecificSettingsSync.TryGetSettingOfUser(player.ReferenceHub, 2, out SSButton loadButton) && loadButton.SyncLastPress.IsRunning)
                {
                    if (!string.IsNullOrEmpty(selectedMap) && selectedMap != "None")
                    {
                        if (lastLoadedMap != "None")
                        {
                            try
                            {
                                MapUtils.UnloadMap(lastLoadedMap);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"Failed to Unload Map {lastLoadedMap}: {ex}");
                                lastLoadedMap = "None";
                            }
                        }

                        try
                        {
                            MapUtils.LoadMap(selectedMap);
                            lastLoadedMap = selectedMap;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Failed to Load Map {selectedMap}: {ex}");
                            lastLoadedMap = "None";
                        }
                    }

                    loadButton.SyncLastPress.Reset();
                }

                if (ServerSpecificSettingsSync.TryGetSettingOfUser(player.ReferenceHub, 3, out SSButton unloadButton) && unloadButton.SyncLastPress.IsRunning)
                {
                    if (lastLoadedMap != "None")
                    {
                        try
                        {
                            MapUtils.UnloadMap(lastLoadedMap);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error($"Failed to Unload Map {lastLoadedMap}: {ex}");
                        }
                        lastLoadedMap = "None";
                    }

                    unloadButton.SyncLastPress.Reset();
                }
            }
        }
    }

    public override void OnPlayerDryFiringWeapon(PlayerDryFiringWeaponEventArgs ev)
	{
		if (!ev.Weapon.IsToolGun(out ToolGunItem toolGun))
			return;

		ev.IsAllowed = false;
		toolGun.Shot(ev.Player);
	}

	public override void OnPlayerReloadingWeapon(PlayerReloadingWeaponEventArgs ev)
	{
		if (!ev.Weapon.IsToolGun(out ToolGunItem toolGun))
			return;

		ev.IsAllowed = false;
		toolGun.SelectedObjectToSpawn--;
	}

	public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev)
	{
		if (!ev.Item.IsToolGun(out ToolGunItem toolGun))
			return;

		ev.IsAllowed = false;
		toolGun.SelectedObjectToSpawn++;
	}
}
