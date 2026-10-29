using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger
{
    class Events
    {
        ///<summary>Event called when the game launches to load content packs.</summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API if it's installed
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
                return;
            {
                configMenu.Register(
                    mod: this.ModManifest,
                    reset: () => this.Config = new ModConfig(),
                    save: () => this.Helper.WriteConfig(this.Config)
                    );
                configMenu.AddKeybind(
                      mod: this.ModManifest,
                      name: () => "Bolt",
                      tooltip: () => "The button to cast Bolt",
                      getValue: () => this.Config.CastBoltButton,
                      setValue: value => this.Config.CastBoltButton = value
                  );
                configMenu.AddKeybind(
                    mod: this.ModManifest,
                    name: () => "Spell",
                    tooltip: () => "The button to cast the currently active spell",
                    getValue: () => this.Config.CastSpellButton,
                    setValue: value => this.Config.CastSpellButton = value
                    );
            }

            //hook Mana Bar
            {
                var manaBar = this.Helper.ModRegistry.GetApi<IManaBarApi>("spacechase0.ManaBar");

                if (manaBar == null)
                {
                    this.Monitor.Log("No mana bar API", LogLevel.Error);
                    return;
                }

                ModEntry.ManaBarApi = manaBar;

            }

            //hook Content Patcher
            {
                var api = this.Helper.ModRegistry.GetApi<ContentPatcher.IContentPatcherAPI>("Pathoschild.ContentPatcher");
                ModEntry.ContentPatcherApi = api;
            }

        }

        /// <summary>
        /// Event for when the mod requests assets to be loaded.
        /// </summary>

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {

            // Spell Textures
            Bolt.ProjectileTexture = helper.ModContent.Load<Texture2D>("assets/MyProjectile.png");

            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile2"))
            {
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/starshotprojectile.png", AssetLoadPriority.Medium);
            }
            if (e.Name.IsEquivalentTo("BriarSinger/MyProjectile3"))
            {
                e.LoadFromModFile<Texture2D>("assets/spellcomponents/starshottail.png", AssetLoadPriority.Medium);
            }            //don't judge me I promise I'll fix this when I have the tilesheet made

            // Adds the Harpsword weapon to the game.
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Weapons"))
            {
                e.Edit(asset => HarpSword.EditWeaponData(asset.AsDictionary<string, WeaponData>().Data));
            }
            // Adds the Harp object to the game. 
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset => Harp.EditObjectData(asset.AsDictionary<string, ObjectData>().Data));
            }

            //Add the harpsword weapon icon to the weapon sprite sheet.
            if (e.NameWithoutLocale.IsEquivalentTo("BriarSinger/weapons"))
            {
                HarpSword.LoadTexture(e);
            }
            //Add the harp object to the objects sprite sheet.
            if (e.NameWithoutLocale.IsEquivalentTo("BriarSinger/Objects"))
            {
                Harp.LoadTexture(e);
            }

        }

        ///<summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        public void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            var caster = Game1.player;

            if (Game1.activeClickableMenu != null)
                return;

            if (Context.IsPlayerFree)
            {
                if (e.Button == this.Config.CastBoltButton)
                {
                    SpellValidation.TryCastSpell(caster, Spellbook.BoltSpell);
                }
                if (e.Button == this.Config.CastSpellButton)
                {
                    Spellbook.ActiveSpell.TryCast(caster);
                }
            }


            ///<summary>Add a section here to do the spell swap mechanic. Should be shift-right click while holding the HarpSword.</summary>

            //This is what turns the harpsword into a bow, links to ChargeTime.cs for the draw
            if (e.Button.IsActionButton() && caster.CurrentTool?.Name == "HarpSword")
            {
                var tile = Game1.currentCursorTile;

                if (!IsInteractiableTile(Game1.currentLocation, tile) &&
                    SpellValidation.CanCast(caster, StarshotSpell.Data))
                {
                    helper.Input.Suppress(e.Button); //should stop the vanilla special action
                    ModEntry.isActionButtonDown = true;
                    Game1.playSound("slingshot");
                    ChargeTime.SetChargeTime(0f);
                    ChargeTime.WhileCharging();
                }
            }

            ModEntry.FixMana(Game1.player); //change this after everything is done to be the OnDayStarted event that sets mana to full
        }

        public void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            //Release charge will try to cast starshot if the chargetime is at 1f.
            if (e.Button.IsActionButton())
            {
                ModEntry.isActionButtonDown = false;
                ChargeTime.ReleaseCharge(Game1.player);
            }
        }

        // Event run each second
        public void OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            {
                ManaRegen(Game1.player);
            }
        }

        //Event to stop the player from giving away their harp object
        public void GivingGift(object sender, EventArgsBeforeReceiveObject e)
        {
            if (e.Gift.Name == "Harp")
            {
                e.Cancel = true;
                //I know that there's base game function to do this but I already wrote it before I figured it out and it works.
            }
        }

        /// <summary>
        /// While holding the action button and holding the harpsword, does the per tick math for the charging of starshot.
        /// </summary>
        private void GameLoopUpdateTicking(object sender, UpdateTickingEventArgs e)
        {
            var caster = Game1.player;

            if (caster.CurrentTool?.Name == "HarpSword" && ModEntry.isActionButtonDown)
            {
                ChargeTime.TickUpdate(caster);
            }
            CooldownHelper.Tick(Game1.currentGameTime);
            SpellCastState.Tick(e.Time);

        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            //While charging starshot, turns the mouse cursor into a reticle.
            if (Game1.player.CurrentTool?.Name == "HarpSword" && ModEntry.isActionButtonDown)
            {
                ChargeTime.DrawReticle(e.SpriteBatch);
            }
        }

    }
}
