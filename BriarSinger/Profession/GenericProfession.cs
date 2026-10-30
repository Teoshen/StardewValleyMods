using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceCore;

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
    }
}
