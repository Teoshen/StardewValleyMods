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
}