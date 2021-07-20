using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace the_Dominion
{
    public class the_DominionInfo : GH_AssemblyInfo
    {
        public override string Name => "The Dominion";

        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }

        public override string Description => "A library of tools for Calculus functions inside the Grasshopper platform";

        public override Guid Id => new Guid("d540efd4-25cd-447f-87fc-7b187f033680");

        public override string AuthorName => "Daniel Christev and Michael Wickerson";

        public override string AuthorContact => "dchristev@gmail.com" + "\n" + "mike@wickersonstudios.com";
    }
}
