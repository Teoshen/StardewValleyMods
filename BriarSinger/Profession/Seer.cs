using System;
using BriarSinger.Framework;
using StardewValley;

namespace BriarSinger.Profession
{
    internal class SeerProfession : GenericProfession
    {
        public SeerProfession(Skills skill, string theId)
            : base(skill, theId) { }

        public override void DoImmediateProfessionPerk()
        {
            Game1.player.setMaxMana(200); //TODO fix this
        }
    }
}
