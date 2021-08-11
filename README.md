# The Dominion
A Grasshopper plugin for modelling conics and mathematical curves accurately from equations.

## Getting Started
### Prerequisites
* [Rhino 6 or 7](https://www.rhino3d.com/)  

And the matching Visual Studio Template for your version of Rhino.  
* [Grasshopper templates for Rhino 6](https://marketplace.visualstudio.com/items?itemName=McNeel.GrasshopperAssemblyforv6) for Visual Studio (Rhino 6)
* [RhinoCommon and Grasshopper templates for Rhino 7](https://marketplace.visualstudio.com/items?itemName=McNeel.Rhino7Templates)

### Steps
1. Clone the repository.
2. Build the Application in Visual Studio.
3. Open Rhino,
4. Run `_GrasshopperDeveloperSettings` from the Rhino Command Line.
5. Add a Folder in the developer settings window, and point the explorer to the 'bin' folder of 'the_Dominion' clone on your local machine, e.g. C:\Users\\($user)\source\repos\bulgos\the_Dominion\src\bin
6. Run `_Grasshopper` from the Rhino Command Line and navigate to the 'Dominion' tab.

## Authors
* Daniel Christev  
* Michael Wickerson

## License
MIT License. Copyright 2021 Daniel Christev, Michael Wickerson.  
See [LICENSE](./LICENSE) for details.

## Useful Links
[Conic Linear Programming](https://web.stanford.edu/class/msande314/sdpmain.pdf)  
[Rotation of axes](https://en.wikipedia.org/wiki/Rotation_of_axes)  
[Wikipedia Conics](https://en.wikipedia.org/wiki/Conic_section#Conversion_to_canonical_form)  
[Conics and their Duals](https://www-m10.ma.tum.de/foswiki/pub/Lehre/ProjektiveGeometrieWS0607/chap9.pdf)  

### Other Conic and NURBS Tools
[opennurbs](https://github.com/mcneel/opennurbs)  
[SCS (fast conic optimizer)](https://github.com/kul-optec/scs#superscs)  
[Conic Solver by HackerPoet](https://github.com/HackerPoet/Conics)  

[Plotting Conics](https://mmas.github.io/conics-matplotlib) for use in conjunction with below link:  
[matplotlib online python](https://trinket.io/embed/python3/a5bd54189b)  
