using BriarSinger.Core;
using BriarSinger.Spells;
using BriarSinger.Spells.Spell_Tools;


// Replace this with the actual namespace of the Mana Bar API if different
using SpaceShared.APIs;
using StardewValley;

namespace BriarSinger.Framework
{
    internal static class Extensions
    {
        public static int GetMaxMana(this Farmer player)
        {
            return ModEntry.ManaBarApi.GetMaxMana(player);
        }

        public static void SetMaxMana(this Farmer player, int newCap)
        {
            ModEntry.ManaBarApi.SetMaxMana(player, newCap);
        }

        public static int GetMana(this Farmer player)
        {
            return ModEntry.ManaBarApi.GetMana(player);
        }

        public static void AddMana(this Farmer farmer, int amt)
        {
            ModEntry.ManaBarApi.AddMana(farmer, amt);
        }

    }

    public static class FarmerExtensions
    {
        public static IEnumerable<ISpellData> GetSpellbook(this Farmer farmer)
        {
            return Spellbook.AllSpells; //change this maybe to include the level requirements
        }
    }

    public static class DirectionHelper
    {
        public static Vector2 FacingDirectionToVector(int facingDirection)
        {
            return facingDirection switch
            {
                0 => new Vector2(0, -1),  // up
                1 => new Vector2(1, 0),   // right
                2 => new Vector2(0, 1),   // down
                3 => new Vector2(-1, 0),  // left
                _ => new Vector2(0, 1),
            };
        }
    }
}