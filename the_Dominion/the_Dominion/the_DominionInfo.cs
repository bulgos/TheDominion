using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace the_Dominion
{
    public class the_DominionInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Dominion";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("d540efd4-25cd-447f-87fc-7b187f033680");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
