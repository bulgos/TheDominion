# Plot conic sections using the below code
# Plots implicit equations in a graph
import matplotlib as mpl
import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
import scipy as sp

mpl.rcParams['lines.color'] = 'k'
mpl.rcParams['axes.prop_cycle'] = mpl.cycler('color', ['k'])

x = np.linspace(0, 125, 400)
y = np.linspace(0, 70, 400)
x, y = np.meshgrid(x, y)

def axes():
    plt.axhline(0, alpha=.1)
    plt.axvline(0, alpha=.1)

a, b, c, d, e, f = 0.00117, -0.002175, 0.002211, -0.077139, 0.022882, -1

axes()
plt.contour(x, y,(a*x**2 + b*x*y + c*y**2 + d*x + e*y + f), [0], colors='k')
plt.show()