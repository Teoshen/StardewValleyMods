using BriarSinger.Framework;
using BriarSinger.Spells;
using BriarSinger.Spells.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore.Events;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Weapons;
using StardewValley.Menus;
using StardewValley.Tools;
using System.Diagnostics;
using static BriarSinger.Spells.SpellValidation;

namespace BriarSinger.Core
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
            //create config menu if GMCM is installed
            var configMenu = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
                return;

            configMenu.Register(
                mod: mod.ModManifest,
                reset: () => mod.Config = new ModConfig(),
                save: () => mod.Helper.WriteConfig(mod.Config));
            configMenu.AddKeybind(
                mod: mod.ModManifest,
                name: () => "Leaf Barrage",
                tooltip: () => "The button to cast Leaf Barrage",
                getValue: () => mod.Config.CastLBarrageButton,
                setValue: value => mod.Config.CastLBarrageButton = value);
            configMenu.AddKeybind(
                mod: mod.ModManifest,
                name: () => "Spell",
                tooltip: () => "The button to cast the currently active spell",
                getValue: () => mod.Config.CastSpellButton,
                setValue: value => mod.Config.CastSpellButton = value);

            //grabs the mana bar
            var manaBar = mod.Helper.ModRegistry.GetApi<IManaBarApi>("spacechase0.ManaBar");
            if (manaBar == null)
            {
                mod.Monitor.Log("No mana bar API", LogLevel.Error);
                return;
            }

            //Add Juniper tree tile action
            GameLocation.RegisterTileAction(Buildings.OpenAction, (location, args, who, tile) =>
            {
                Game1.activeClickableMenu = new JuniperMenu();
                return true;
            }); 

            ModEntry.ManaBarApi = manaBar;
            ModEntry.ContentPatcherApi = mod.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
        }

        public void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {

            //Projectiles and spell effects
            LBarrage.ProjectileTexture = mod.Helper.ModContent.Load<Texture2D>("assets/spellcomponents/leafbarrageprojectile.png");
            Roots.RootsTexture = mod.Helper.ModContent.Load<Texture2D>("assets/spellcomponents/rootsprojectile.png");
           // Shockwave.ShockwaveTexture = mod.Helper.ModContent.Load<Texture2D>("assets/spellcomponents/shockwavetexture.png"); //placeholder until I get it on a spritesheet to reference so I don't forget

            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile2"))
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/starshotprojectile.png", AssetLoadPriority.Medium);
            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile3"))
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/starshottail.png", AssetLoadPriority.Medium);

            //Icons
            Spellbook.RootsSpell.Icon = mod.Helper.ModContent.Load<Texture2D>("assets/spellicons/rootsicon.png");
            Spellbook.HealSpell.Icon = mod.Helper.ModContent.Load<Texture2D>("assets/spellicons/healicon.png");
            Spellbook.RejuvenateSpell.Icon = mod.Helper.ModContent.Load<Texture2D>("assets/spellicons/rejuvenateicon.png");
            Spellbook.LBarrageSpell.Icon = mod.Helper.ModContent.Load<Texture2D>("assets/spellicons/leafbarrageicon.png");
            Spellbook.TreeGrowthSpell.Icon = mod.Helper.ModContent.Load<Texture2D>("assets/spellicons/treegrowthicon.png");
            Spellbook.FruitTreeGrowthSpell.Icon = mod.Helper.ModContent.Load<Texture2D>("assets/spellicons/fruittreegrowthicon.png");
            Spellbook.GrassGrowthSpell.Icon = mod.Helper.ModContent.Load<Texture2D>("assets/spellicons/grassgrowthicon.png");
            Spellbook.ShockwaveSpell.Icon = mod.Helper.ModContent.Load<Texture2D>("assets/spellicons/shockwaveicon.png");
            Spellbook.TimberSpell.Icon = mod.Helper.ModContent.Load<Texture2D>("assets/spellicons/timbericon.png");

            //Weapons and Objects
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Weapons"))
                e.Edit(asset => HarpSword.EditWeaponData(asset.AsDictionary<string, WeaponData>().Data));
            if (e.NameWithoutLocale.IsEquivalentTo("BriarSinger/weapons"))
                HarpSword.LoadTexture(e);

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
                e.Edit(asset => Objects.EditObjectData(asset.AsDictionary<string, ObjectData>().Data));            
            if (e.NameWithoutLocale.IsEquivalentTo("BriarSinger/Objects"))
                Objects.LoadTexture(e);

            //Buildings          
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Buildings"))
                e.Edit(asset => Buildings.EditBuildingData(asset.AsDictionary<string,  BuildingData>().Data));
             //   e.Edit(Buildings.Edit);
          //  else Buildings.TryLoadTexture(e);
         

            //I'll convert this to CP or add a tilesheet when it's all done I promise. TODO

            //the building, TODO move this to CP or buildings.cs later
           
        }

        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var caster = Game1.player;

            if (Game1.activeClickableMenu != null ||
                !Context.IsWorldReady)
            {
                return;
            }

            if (Context.IsPlayerFree)
            {
                if (e.Button == mod.Config.CastLBarrageButton)
                {
                    TryCastSpell(caster, Spellbook.LBarrageSpell);
                    Debug.WriteLine("Leaf Barrage button");
                }

                if (e.Button == mod.Config.CastSpellButton)
                {
                    Spellbook.ActiveSpell.TryCast(caster);
                    Debug.WriteLine("Active spell button");
                }

                if (e.Button == SButton.MouseRight 
                    && (mod.Helper.Input.IsDown(SButton.LeftShift) 
                    && caster.CurrentTool is MeleeWeapon hw
                    && hw.ItemId == HarpSword.WeaponId
                    && Context.IsPlayerFree))
                {
                    mod.Helper.Input.Suppress(e.Button); //TODO see if this is needed to suppress charge/special attack
                    Game1.activeClickableMenu = new SpellMenu();
                }
                if (Game1.player.CurrentItem?.QualifiedItemId == Objects.QualifiedSeedId
                    && e.Button.IsActionButton())
                {                   
                    Game1.activeClickableMenu = new CarpenterMenu(Buildings.JuniperBuilder);                    
                }
                
                ModEntry.FixMana(caster); //remove this later after mana is set up TODO
            }
        }

        public void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
          //this might get used someday TODO remove if it's not
        }

        public void OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            ModEntry.ManaRegen(Game1.player);
            StaminaRegenEffects.StaminaOneSecondUpdate();
            HealthRegenEffects.HealthOneSecondUpdate();
        }

        public void GivingGift(object sender, EventArgsBeforeReceiveObject e)
        {
            //I know that base game has this but I already set it up here.
            if (e.Gift.Name == "Harp")
                e.Cancel = true;
        }

        public void GameLoopUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
          if (!Context.IsWorldReady) 
                return;
            if (!Game1.game1.IsActive)
                return;            
            if (!Game1.shouldTimePass())
                return;
            

            CooldownHelper.Tick(Game1.currentGameTime);
            SpellCastAnim.Tick(Game1.currentGameTime);
            Roots.RootEffect.Tick(Game1.currentGameTime);
            GrassGrowth.Tick(Game1.currentGameTime);
        }

        public void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            // Draw the spell cooldown icons on the HUD just above the toolbar

            var active = CooldownHelper.Active;
            if (active.Count == 0 || !Context.IsPlayerFree) return;

            var toolbar = CoolDownToolBar.GetToolbar();
            if (toolbar == null) return;

            int iconSize = 48;
            int spacing = 4;
            int totalWidth = active.Count * iconSize + (active.Count -1) * spacing;

            int baseX = toolbar.xPositionOnScreen + toolbar.width / 2 - totalWidth / 2;
            int baseY = toolbar.yPositionOnScreen - iconSize - 112;

           for (int i = 0; i < active.Count; i++)
            {
                if (!CoolDownToolBar.TryGetSpell(active[i].spellId, out var spell)) continue;

                var pos = new Vector2(baseX + i * (iconSize + spacing), baseY);
                CoolDownToolBar.DrawSpellCooldownIcon(e.SpriteBatch, spell, active[i].Remaining, pos);
            }            
        } //TODO make the daily spells show up for a short period (10sec?) and show how many casts are left for the day

        public void OnDayStarting(object sender, DayStartedEventArgs e)
        {
            SpellValidation.CooldownHelper.OnDayStarted(); //resets daily cast spells
            //TODO add mana fill here
        }
        
        
      
        /* don't think I need this anymore
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

            // NPCs 
            foreach (NPC npc in location.characters)
            {
                if (npc.Tile == tile)
                    return true;
            }

            // Objects 
            if (location.Objects.ContainsKey(tile))
                return true;

            // Terrain features 
            if (location.terrainFeatures.ContainsKey(tile))
                return true;

            // Mine ladders/elevator/shaft: 
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
        private object helper;
        */
    }
}
