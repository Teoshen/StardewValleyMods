using BriarSinger.Framework;
using SpaceCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger.Profession : SkillTree
{
    //Mender Track

    internal class MenderProfession
        {
            public MenderProfession(Skills skill, string theId)
                      : base(skill, theId) { }
            //"Heal costs half as much mana and the cooldown is reduced. You can cast daily spells once more per day."
            public override void DoImmediateProfessionPerk()
            {
                Game1.player.setMaxMana(200); //TODO fix this
            }
        }

    internal class SummonerProfession
        {
          {  public SummonerProfession(Skills skill, string theId)
               : base(skill, theId) { }
        //"You can cast daily spells once more per day. Gain the Crop Growth spell."
            public override void DoImmediateProfessionPerk()
            {
                Game1.player.setMaxMana(200); //TODO fix this
            }
        }

    internal class ApothecaryProfession
        {
            public ApothecaryProfession(Skills skill, string theId)
                        : base(skill, theId) { }
            //"Rituals and offerings no longer require reagents. Choose your blessing each day."
            public override void DoImmediateProfessionPerk()
            {
                Game1.player.setMaxMana(200); //TODO fix this
            }
        }


//Seer Track

    internal class SeerProfession
        {
            public SeerProfession(Skills skill, string theId)
                    : base(skill, theId) { }
            //"Bolt costs half as much mana. Starshot has reduced charge time."
            public override void DoImmediateProfessionPerk()
            {
                Game1.player.setMaxMana(200); //TODO fix this
            }
        }

    internal class StarcallerProfession
        {
            public StarcallerProfession(Skills skill, string theId)
                    : base(skill, theId) { }
            //"Star Shot ricochets an additional time off each chained target."
            public override void DoImmediateProfessionPerk()
            {
                Game1.player.setMaxMana(200); //TODO fix this
            }
        }

    internal class HexerProfession
        {
            public HexerProfession(Skills skill, string theId)
                  : base(skill, theId) { }
            //"The Bolt debuff is permanent and includes a damage over time effect."
            public override void DoImmediateProfessionPerk()
            {
                Game1.player.setMaxMana(200); //TODO fix this
            }
        }
}