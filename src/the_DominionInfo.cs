using Grasshopper.Kernel;
using System;
using System.Diagnostics;
using System.Drawing;
using the_Dominion.Properties;

namespace the_Dominion
{
    public class the_DominionInfo : GH_AssemblyInfo
    {
        public override string Name => "The Dominion";

        public override Bitmap Icon => Resources.the_dominion;

        public override string Description => "Provides components for building Mathematical Curves from Conic Equations";

        public override Guid Id => new Guid("d540efd4-25cd-447f-87fc-7b187f033680");

        public override string AuthorName => "Daniel Christev and Michael Wickerson";

        public override string AuthorContact => "https://github.com/bulgos/the_Dominion";

        public override string AssemblyVersion
        {
            get
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                return FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            }
        }
    }
}
