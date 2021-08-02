# Pushes commands to the Rhino Command Line to
# Generate hyperbolae inside the Rhino Dcoument from parameters
import rhinoscriptsyntax as rs
import Rhino.Geometry as rg

generate_set = False

start = rg.Point3d.Origin
end = rg.Point3d(10,0,0)
direction = rg.Vector3d.XAxis


a = 0.536
b = 1

cmd = "_Hyperbola _FromCoefficient %s %s A %s B %s %s " % (start, direction, a, b, end)
print cmd
rs.Command(cmd)



if (generate_set):
    for i in range(0, 10):
            cmd = "_Hyperbola _FromCoefficient %s %s A %s B %s %s " % (start, direction, i+1, b, end)
            rs.Command(cmd)