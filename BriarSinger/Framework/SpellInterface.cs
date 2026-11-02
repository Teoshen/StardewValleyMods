using System.Linq;
using SpaceCore;
using StardewValley;
using StardewValley.Menus;

namespace BriarSinger.Framework

public class SpellInterface
{
	internal class SpellInterface : IClickableMenu
	{
		//Sizes of fields
		private const int WindowWidth = 800;
		private const int WindowHeight = 600;
		private const int SpellIconSize = 64;
		private const int SelectIconSize = 192;

		private Spell SelectedSpell;

		private bool JustLeftClicked;
		

		public SpellInterface()
			: base((Game1.viewport.Size.Width - SpellInterface.WindowWidth) / 2, 
			(Game1.viewport.Size.Height - SpellInterface.WindowHeight) / 2, 
			SpellInterface.WindowWidth, 
			SpellInterface.WindowHeight, 
			true)
			{
			//default spell?
			}

		public override bool overrideSnappyMenuCursorMovementBan()
		{
			return true;
		}

		public override void draw(Spritebatch b)
		{
			//get the spell info
			Spellbook spellbook = Game1.player.GetSpellbook(); //TODO find out how to add 'memory' of spells
			string hoverText = null;

			//main window
			IClickableMenu.drawTextureBox(b,
				this.xPositionOnScreen,
				this.yPositionOnScreen,
				SpellInterface.WindowWidth,
				SpellInterface.WindowHeight,
				Color.White);
			IClickableMenu.drawTextureBox(b,
				this.xPositionOnScreen,
				this.yPositionOnScreen,
				SpellInterface.WindowWidth / 2,
				SpellInterface.WindowHeight,
				Color.White);

			//draw spell icon area
			{
				//figure out how to do this TODO
			}

			//draw spell icons
			{
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

		public override void recieveLeftClick(int x, int y, bool playSound = true)
		{
			base.recieveLeftClick(x, y, playSound);
			this.JustLeftClicked = true;
		}
	}
}
