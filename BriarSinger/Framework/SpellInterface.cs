using BriarSinger.Spells;
using BriarSinger.Spells;
using BriarSinger.Spells.Spell_Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using StardewValley;
using StardewValley.Menus;
using System.Linq;

namespace BriarSinger.Framework;

	internal class SpellMenu : IClickableMenu
	{
		//Sizes of fields
		private const int WindowWidth = 800;
		private const int WindowHeight = 600;
		private const int SpellIconSize = 64;
		private const int SelectIconSize = 192;

		private ISpellData SelectedSpell; //add a definition of spell in BriarSinger.Spells TODO

		private bool JustLeftClicked;
		

		public SpellMenu()
			: base((Game1.viewport.Size.Width - SpellMenu.WindowWidth) / 2, 
			(Game1.viewport.Size.Height - SpellMenu.WindowHeight) / 2, 
			SpellMenu.WindowWidth, 
			SpellMenu.WindowHeight, 
			true)
			{
			//default spell?
			}

		public override bool overrideSnappyMenuCursorMovementBan()
		{
			return true;
		}

		public override void draw(SpriteBatch b)
		{
		//get the spell info
		var spells = Game1.player.GetSpellbook();

        string hoverText = null;

			//main window
			IClickableMenu.drawTextureBox(b,
				this.xPositionOnScreen,
				this.yPositionOnScreen,
				SpellMenu.WindowWidth,
                SpellMenu.WindowHeight,
				Color.White);
			IClickableMenu.drawTextureBox(b,
				this.xPositionOnScreen,
				this.yPositionOnScreen,
                SpellMenu.WindowWidth / 2,
                SpellMenu.WindowHeight,
				Color.White);

			//draw spell icon area
			{
				//figure out how to do this TODO
			}

			//draw spell icons
			{
				foreach (var spell in Spellbook.AllSpells)
				{
					bool unlocked = spell.IsUnlocked(Game1.player);
				Color tint = unlocked ? Color.White : Color.Black * 0.5f; //should be grey if locked
				}
				//figure out how to do this TODO
			}
			//draw selected spell, it'll be like a purple border around the one you picked
			{
				//TODO
			}
			//draw hover text
			{
				if (hoverText != null)
				{
					drawHoverText(b, hoverText, Game1.smallFont);
				}
			}
			//draw base menu
			base.draw(b);

			//draw cursor
			this.drawMouse(b);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			base.receiveLeftClick(x, y, playSound);
			this.JustLeftClicked = true;
			//might need to add a thing here to not let it left click if the spell is not unlocked
		}
	}
