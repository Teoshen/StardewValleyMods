using BriarSinger.Spells.Spell_Tools;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BriarSinger.Spells.SpellValidation;
using Microsoft.Xna.Framework;
using BriarSinger.Spells;

namespace BriarSinger.Framework
{
    internal class CoolDownToolBar
    {
        //finds the toolbar menu so I can draw above it
        public static Toolbar GetToolbar() => Game1.onScreenMenus.OfType<Toolbar>().FirstOrDefault();

        //grabs all the spells in the spellbook
        private static Dictionary<string, ISpellData> spellsById; 
        //TODO when the spellbook changes, set spellsById to null so it will rebuilds. Probably on day start after doing profession perks

        //adds those spells on cooldown to the list
        public static bool TryGetSpell(string id, out ISpellData spell)
        {
            spellsById ??= BuildLookup();
            return spellsById.TryGetValue(id, out spell );
        }
             
        private static Dictionary<string, ISpellData> BuildLookup()
        {
            var dict = new Dictionary<string, ISpellData>();
            if (Spellbook.BoltSpell != null)
                dict[Spellbook.BoltSpell.Id] = Spellbook.BoltSpell;
            foreach (var s in Spellbook.AllSpells)
                if (s != null) dict[s.Id] = s;
            return dict;
        }

        public static void DrawSpellCooldownIcon(SpriteBatch b, ISpellData spell, float remaining, Vector2 pos)
        {
            if (spell?.Icon == null)
            {
                return;
            }

            int size = 48;
            b.Draw(spell.Icon, new Rectangle((int)pos.X, (int)pos.Y, size, size), null, Color.White);

            float pct = remaining / spell.CooldownSeconds;
            int overlayHeight = (int)(size * pct);
            
            //vertical drawdown 
                b.Draw(Game1.staminaRect,
                    new Rectangle((int)pos.X, (int)pos.Y, size, overlayHeight), Color.Black * 0.6f);

                Utility.drawTinyDigits((int)Math.Ceiling(remaining), b,
                    pos + new Vector2(size / 2 - 6, size - 14), 3f, 1f, Color.White);           
        }
    }
}
