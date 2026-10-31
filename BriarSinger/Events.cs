using BriarSinger.Framework;
using BriarSinger.Framework;
using BriarSinger.Spells;
using BriarSinger.Spells.Components;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore.Events;
using SpaceShared.APIs;
using SpaceShared.ConsoleCommands;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Weapons;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BriarSinger.Spells.SpellValidation;

namespace BriarSinger
{
    internal sealed class Events
    {
        private readonly ModEntry mod;

        public Events(ModEntry mod)
        {
            this.mod = mod;
        }

        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.mod.ModManifest,
                reset: () => this.mod.Config = new ModConfig(),
                save: () => this.mod.Helper.WriteConfig(this.mod.Config));
            configMenu.AddKeybind(
                mod: this.mod.ModManifest,
                name: () => "Bolt",
                tooltip: () => "The button to cast Bolt",
                getValue: () => this.mod.Config.CastBoltButton,
                setValue: value => this.mod.Config.CastBoltButton = value);
            configMenu.AddKeybind(
                mod: this.mod.ModManifest,
                name: () => "Spell",
                tooltip: () => "The button to cast the currently active spell",
                getValue: () => this.mod.Config.CastSpellButton,
                setValue: value => this.mod.Config.CastSpellButton = value);

            var manaBar = this.mod.Helper.ModRegistry.GetApi<IManaBarApi>("spacechase0.ManaBar");
            if (manaBar == null)
            {
                this.mod.Monitor.Log("No mana bar API", LogLevel.Error);
                return;
            }

            ModEntry.ManaBarApi = manaBar;
            ModEntry.ContentPatcherApi = this.mod.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
        }

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            Bolt.ProjectileTexture = this.mod.Helper.ModContent.Load<Texture2D>("assets/spellcomponents/boltprojectile.png");

            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile2"))
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/starshotprojectile.png", AssetLoadPriority.Medium);
            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile3"))
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/starshottail.png", AssetLoadPriority.Medium);
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Weapons"))
                e.Edit(asset => HarpSword.EditWeaponData(asset.AsDictionary<string, WeaponData>().Data));
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
                e.Edit(asset => Harp.EditObjectData(asset.AsDictionary<string, ObjectData>().Data));
            if (e.NameWithoutLocale.IsEquivalentTo("BriarSinger/weapons"))
                HarpSword.LoadTexture(e);
            if (e.NameWithoutLocale.IsEquivalentTo("BriarSinger/Objects"))
                Harp.LoadTexture(e);
        }

        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var caster = Game1.player;

            if (Game1.activeClickableMenu != null)
                return;

            if (Context.IsPlayerFree)
            {
                if (e.Button == this.mod.Config.CastBoltButton)
                    SpellValidation.TryCastSpell(caster, Spellbook.BoltSpell);
                if (e.Button == this.mod.Config.CastSpellButton)
                    Spellbook.ActiveSpell.TryCast(caster);
              
                if (caster.CurrentTool?.Name == "HarpSword" && e.Button == SButton.MouseLeft)
                {
                   // SwingCharge.StartCharging();
                    this.mod.Helper.Input.Suppress(e.Button);
                }
              
            }

            if (e.Button.IsActionButton() && caster.CurrentTool?.Name == "HarpSword")
            {
                var tile = caster.GetGrabTile();
                if (!IsInteractableTile(Game1.currentLocation, tile) && SpellValidation.CanCast(caster, Spellbook.StarShotSpell))
                {
                    ModEntry.IsActionButtonDown = true;
                    ChargeTime.SetChargeTime(0f);
                    ChargeTime.WhileCharging();
                }
            }

            ModEntry.FixMana(caster);
        }

        public void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button.IsActionButton())
            {
                ModEntry.IsActionButtonDown = false;
                ChargeTime.ReleaseCharge(Game1.player);
            }
            if (e.Button == SButton.MouseLeft && Game1.player.CurrentTool is HarpSword)
                HarpSword.PerformDirectionalSwing(Game1.player);
        }

        public void OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            ModEntry.ManaRegen(Game1.player);
        }

        public void GivingGift(object sender, EventArgsBeforeReceiveObject e)
        {
            if (e.Gift.Name == "Harp")
                e.Cancel = true;
        }

        public void GameLoopUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            var caster = Game1.player;
            if (!Context.IsWorldReady)
                return;

            if (caster.CurrentTool?.Name == "HarpSword" && ModEntry.IsActionButtonDown)
                ChargeTime.TickUpdate(caster);
            CooldownHelper.Tick(Game1.currentGameTime);
            SpellCastAnim.Tick(Game1.currentGameTime);
          //  if (caster.CurrentTool?.Name == "HarpSword") 
            //    SwingCharge.WhileCharging();
        }

        public void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {

        }
       /*
        {
            if (Game1.player.CurrentTool?.Name == "HarpSword" && ModEntry.IsActionButtonDown)
                ChargeTime.DrawReticle(e.SpriteBatch);
        }
       */

        private static bool IsInteractableTile(GameLocation location, Vector2 tile)
        {
            int tx = (int)tile.X;
            int ty = (int)tile.Y;

            // Doors, signs, generic action tiles, plus Objects/TerrainFeatures that set isActionable()
            if (location.isActionableTile(tx, ty, Game1.player))
                return true;

            // Touch-triggered tiles
            if (!string.IsNullOrEmpty(location.doesTileHaveProperty(tx, ty, "TouchAction", "Back")) ||
                !string.IsNullOrEmpty(location.doesTileHaveProperty(tx, ty, "TouchAction", "Buildings")))
                return true;

            // NPCs — isActionableTile doesn't treat talking as an action
            foreach (NPC npc in location.characters)
            {
                if (npc.Tile == tile)
                    return true;
            }

            // Objects (forage, statues, etc.) regardless of their own isActionable() flag
            if (location.Objects.ContainsKey(tile))
                return true;

            // Terrain features — same reasoning as Objects
            if (location.terrainFeatures.ContainsKey(tile))
                return true;

            // Mine ladders/elevator/shaft: hardcoded Buildings-layer tile indices, not properties
            if (location is MineShaft)
            {
                xTile.Tiles.Tile buildingTile = location.Map.GetLayer("Buildings")
                    ?.PickTile(new xTile.Dimensions.Location(tx * 64, ty * 64), Game1.viewport.Size);

                if (buildingTile != null)
                {
                    switch (buildingTile.TileIndex)
                    {
                        case 112: // elevator
                        case 115: // exit ladder ("up")
                        case 173: // ladder down
                        case 174: // shaft
                            return true;
                    }
                }
            }

            return false;
        }

    }
}
