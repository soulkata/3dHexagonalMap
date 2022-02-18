# 3dHexagonalMap
This is an proof of concept of a hexagonal 3d map generator. Some images of the generated maps: https://puu.sh/IJSGw/f54bd0ac24.png


We have:
* A simple water shader;
* A more complex terrain shader, that switch terrain based on: A) The height of terrain; B) Slopness of the hill; C) A moisture mask; And incuild suport to paint decalcs
* A way to convert tilemaps to textures to be used on the 3d mesh
* A 3d terrain mesh generator based on the height of each mesh
