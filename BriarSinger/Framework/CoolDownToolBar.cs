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
        //finds the toolbar menu so we can draw above it
        public static Toolbar GetToolbar() => Game1.onScreenMenus.OfType<Toolbar>().FirstOrDefault();

        //grabs all the spells in the spellbook
        private static IEnumerable<ISpellData> TrackedSpells()
        {
            yield return Spellbook.BoltSpell;
            foreach (var s in Spellbook.AllSpells)
                yield return s;
        }

        //adds those spells on cooldown to the list
        public static List<ISpellData> GetActiveCooldownSpells(Farmer who)
        {
            var seen = new HashSet<string>();
            var result = new List<ISpellData>();

            foreach (var spell in TrackedSpells())
            {
                if (spell == null) continue;
                if (!seen.Add(spell.Id)) continue;
                if (CooldownHelper.IsOnCooldown(who, spell.Id))
                    result.Add(spell);
            }
            return result;
        } //TODO if this causes performance issues, switch to caching it and adding a trigger on cooldown activation to update it
       
        public static void DrawSpellCooldownIcon(SpriteBatch b, ISpellData spell, Vector2 pos)
        {
            if (spell?.Icon == null)
            {
                return;
            }

            int size = 48;
            b.Draw(spell.Icon, new Rectangle((int)pos.X, (int)pos.Y, size, size), null, Color.White);

            float remaining = CooldownHelper.GetRemainingSeconds(Game1.player, spell.Id);
            if (remaining > 0f)
            {
                float pct = remaining / spell.CooldownSeconds; // 1 = just cast, 0 = ready
                int overlayHeight = (int)(size * pct);

                //vertical drawdown 
                b.Draw(Game1.staminaRect,
                    new Rectangle((int)pos.X, (int)pos.Y, size, overlayHeight), Color.Black * 0.6f);

                Utility.drawTinyDigits((int)Math.Ceiling(remaining), b,
                    pos + new Vector2(size / 2 - 6, size - 14), 3f, 1f, Color.White);
            }
        }
    }
}
