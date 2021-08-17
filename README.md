# The Dominion
A Grasshopper plugin for modelling conics and mathematical curves accurately from equations.

## Getting Started
If you are only interested in using the application, follow the *Run* instructions.
### Prerequisites
* [Rhino 6 or 7](https://www.rhino3d.com/)  

## Run
Run the sample files once loaded located in the samples folder.

##### Option 1:
1. Download the [latest version](https://www.food4rhino.com/en/app/dominion) from food4Rhino.
2. Copy all unzipped contents into your component library, usually C:\Users\\($user)\AppData\Roaming\Grasshopper\Libraries
3. Unblock all files

##### Option 2:  
1. Navigate to Releases
2. Download and unzip the latest Release, e.g. the_dominion_x.x.x.x.zip.
3. copy all unzipped contents into your component library, usually C:\Users\\($user)\AppData\Roaming\Grasshopper\Libraries
4. Unblock all files

##### Option 3 (Experimental):
1. Open a file from samples folder e.g. dominion_sample_001.gh
2. Install the plugin from the Yak Package Manager

## Build
### Prerequisites
The matching Visual Studio Template for your version of Rhino.  
* [Grasshopper templates for Rhino 6](https://marketplace.visualstudio.com/items?itemName=McNeel.GrasshopperAssemblyforv6) for Visual Studio (Rhino 6)
* [RhinoCommon and Grasshopper templates for Rhino 7](https://marketplace.visualstudio.com/items?itemName=McNeel.Rhino7Templates)

### Steps
##### If you want to build the application from source code on your local machine, follow the steps below:  
1. Clone the repository.
2. Build the Application in Visual Studio.
3. Open Rhino,
4. Run `_GrasshopperDeveloperSettings` from the Rhino Command Line.
5. Add a Folder in the developer settings window, and point the explorer to the 'bin' folder of 'the_Dominion' clone on your local machine, e.g. C:\Users\\($user)\source\repos\bulgos\the_Dominion\src\bin
6. Run `_Grasshopper` from the Rhino Command Line and navigate to the 'Dominion' tab.

## Deploy
##### If you are an owner and want to deploy the application to Yak:  
[Build Yak packages with Rhino 7](https://developer.rhino3d.com/guides/yak/creating-a-grasshopper-plugin-package/)  
1. Set Configuration in Visual Studio to Release.
2. Build Application (this will populate the \dist directory in Release Configuration).
3. Point cmd prompt to root of the repository
4. Change to distribution directory `cd \the_Dominion\dist`
5. Spec the manifest .yml file `"C:\Program Files\Rhino 7\System\Yak.exe" spec`
6. Update the manifest manually if necessary.
7. Build the package (currently windows only) `"C:\Program Files\Rhino 7\System\Yak.exe" build --platform win`

[Push package to Yak server](https://developer.rhino3d.com/guides/yak/pushing-a-package-to-the-server/)  
8. Login to Mcneel account `"C:\Program Files\Rhino 7\System\Yak.exe" login`  
9. Push package to server `"C:\Program Files\Rhino 7\System\Yak.exe" push the-dominion-x.x.x-any-win.yak`  
10. Check package has been successfully pushed `"C:\Program Files\Rhino 7\System\Yak.exe" search --all --prerelease the-dominion`  

##### If it is your first time, try the test server:  
9. Push package to test server `"C:\Program Files\Rhino 7\System\Yak.exe" push --source https://test.yak.rhino3d.com the-dominion-x.x.x-any-win.yak`  
10. Check package has been successfully pushed `"C:\Program Files\Rhino 7\System\Yak.exe" search --source https://test.yak.rhino3d.com --all --prerelease the-dominion`  

[Yak Command Line Tool Reference](https://developer.rhino3d.com/guides/yak/yak-cli-reference/)  

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
[AV Akopyan's Geometry of Conics](https://geometry.ru/books/conic_e.pdf)  

### Other Conic and NURBS Tools
[opennurbs](https://github.com/mcneel/opennurbs)  
[SCS (fast conic optimizer)](https://github.com/kul-optec/scs#superscs)  
[Conic Solver by HackerPoet](https://github.com/HackerPoet/Conics)  

[Plotting Conics](https://mmas.github.io/conics-matplotlib) for use in conjunction with below link:  
[matplotlib online python](https://trinket.io/embed/python3/a5bd54189b)  
