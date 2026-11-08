using BriarSinger.Spells;
using BriarSinger.Spells;
using BriarSinger.Spells.Spell_Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using StardewValley;
using StardewValley.Menus;
using System.Linq;
using static BriarSinger.Spells.SpellValidation;

namespace BriarSinger.Framework;

internal class SpellMenu : IClickableMenu
{
	//Sizes of fields
	private const int WindowWidth = 800;
	private const int WindowHeight = 600;
	private const int SpellIconSize = 64;
	private const int SelectIconSize = 192;

	private List<ClickableTextureComponent> spellIcons = new();

	private ISpellData SelectedSpell;
	private string hoverText = "";

	public SpellMenu()
		: base((Game1.viewport.Size.Width - SpellMenu.WindowWidth) / 2,
		(Game1.viewport.Size.Height - SpellMenu.WindowHeight) / 2,
		SpellMenu.WindowWidth,
		SpellMenu.WindowHeight,
		true)
	{
		int x = this.xPositionOnScreen + WindowWidth / 2 + 32;
		int y = this.yPositionOnScreen + 32;
		int perRow = 4, i = 0;
		foreach (var spell in Game1.player.GetSpellbook())
		{
			Texture2D icon = spell.Icon ?? Game1.mouseCursors; // so the game doesn't crash if the spell doesn't have an icon
			Rectangle sourceRect = spell.Icon != null
				? new Rectangle(0, 0, spell.Icon.Width, spell.Icon.Height)
				: new Rectangle(0, 0, 16, 16);
			float scale = spell.Icon != null
				? (float)SpellIconSize / spell.Icon.Width
				: 4f;

            spellIcons.Add(new ClickableTextureComponent(
				spell.Id,
				new Rectangle(x + (i % perRow) * (SpellIconSize + 16), y + (i / perRow) * (SpellIconSize + 16), SpellIconSize, SpellIconSize),
				null, null, 
				icon,
				sourceRect, 
				scale));
			i++;
		}
	}

	public override bool overrideSnappyMenuCursorMovementBan()
	{
		return true;
	}

	public override void performHoverAction(int x, int y)
	{
		hoverText = "";
		foreach (var icon in spellIcons)
		{
			if (icon.containsPoint(x, y))
			{
				var spell = Spellbook.AllSpells.First(s => s.Id == icon.name);
				hoverText = spell.IsUnlocked(Game1.player)
					? $"{spell.Id}\n{spell.Description}"
					: $"{spell.Id} (Locked - requires level {spell.RequiredLevel})";
				icon.scale = Math.Min(icon.scale + 0.02f, icon.baseScale + 0.1f); //hover-grow, TODO remove if it's broke
				break;
			}
			else
			{
				icon.scale = Math.Max(icon.scale - 0.02f, icon.baseScale);
			}
		}
	}

	public override void draw(SpriteBatch b)
	{
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
			IClickableMenu.drawTextureBox(b,
				xPositionOnScreen + WindowWidth / 2,
				yPositionOnScreen, //TODO check this in game to make sure placement is correct
				WindowWidth / 2,
				WindowHeight,
				Color.White);
		}

		//draw spell icons
		{
			foreach (var icon in spellIcons)
			{
				var spell = Game1.player.GetSpellbook().First(s => s.Id == icon.name);
				bool unlocked = spell.IsUnlocked(Game1.player);
				Color tint = unlocked ? Color.White : Color.Black * 0.5f;
				icon.draw(b, tint, 1f);
			}
		}
		//draw selected spell, it'll be like a purple border around the one you picked
		{
			if (SelectedSpell != null)
			{
				var selIcon = spellIcons.FirstOrDefault(i => i.name == SelectedSpell.Id);
				selIcon?.draw(b, Color.Purple, 1f);
			}
		}
		//draw hover text
		{
			if (!string.IsNullOrEmpty(hoverText))
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

		foreach (var icon in spellIcons)
		{
			if (!icon.containsPoint(x, y)) continue;

			var spell = Spellbook.AllSpells.First(s => s.Id == icon.name);
			if (!spell.IsUnlocked(Game1.player))
			{
				Game1.playSound("cancel"); //TODO maybe add a different sound
				return;
			}

			Spellbook.ActiveSpell = spell;
			Game1.playSound("select"); //TODO maybe add a different sound
			Game1.exitActiveMenu();
			return;
		}
	}

	
}
