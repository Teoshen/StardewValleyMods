using System;
using BriarSinger.Framework;
using StardewValley;

namespace BriarSinger.Profession
{
    internal class StarcallerProfession : GenericProfession
    {
        public StarcallerProfession(Skills skill, string theId)
            : base(skill, theId) { }

        public override void DoImmediateProfessionPerk()
        {
            Game1.player.setMaxMana(200); //TODO fix this
        }
    }
}
