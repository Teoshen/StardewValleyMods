using BriarSinger.Core;
using SpaceCore;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BriarSinger.Profession
{
    internal class GenericProfession : SpaceCore.Skills.Skill.Profession
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public GenericProfession(Skills skill, string theId)
            : base(skill, theId) { }

        public override string GetName()
        {
            return this.Name;
        }

        public override string GetDescription()
        {
            return this.Description;
        }
    
 public static void AddEXP(Farmer who, int amount)
        {
            if (amount <= 0)
                return;

            Farmer player = Game1.GetPlayer(who.UniqueMultiplayerID);
            SpaceCore.Skills.AddExperience(player, ModEntry.SkillID, amount);

         /*   if (!RollSkillBookDrop())
                return;

            Game1.createMultipleObjectDebris(SkillBookItemId, who.TilePoint.X, who.TilePoint.Y, 1, who.UniqueMultiplayerID, who.currentLocation);
         */
        }
    }
