using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Henge3D;

namespace itp380.Objects
{
    class SkinBuilder
    {
        public static PolyhedronPart Square()
        {
            
            /*
             * var vertices = [
    // Front face
    -1.0, -1.0,  1.0,
     1.0, -1.0,  1.0,
     1.0,  1.0,  1.0,
    -1.0,  1.0,  1.0,
    
    // Back face
    -1.0, -1.0, -1.0,
    -1.0,  1.0, -1.0,
     1.0,  1.0, -1.0,
     1.0, -1.0, -1.0,
    
    // Top face
    -1.0,  1.0, -1.0,
    -1.0,  1.0,  1.0,
     1.0,  1.0,  1.0,
     1.0,  1.0, -1.0,
    
    // Bottom face
    -1.0, -1.0, -1.0,
     1.0, -1.0, -1.0,
     1.0, -1.0,  1.0,
    -1.0, -1.0,  1.0,
    
    // Right face
     1.0, -1.0, -1.0,
     1.0,  1.0, -1.0,
     1.0,  1.0,  1.0,
     1.0, -1.0,  1.0,
    
    // Left face
    -1.0, -1.0, -1.0,
    -1.0, -1.0,  1.0,
    -1.0,  1.0,  1.0,
    -1.0,  1.0, -1.0
  ];*/
            Vector3[] vertices = new Vector3[8];
            vertices[0] = new Vector3(1, 1, 1);
            vertices[1] = new Vector3(1, 1, -1);
            vertices[2] = new Vector3(1, -1, 1);
            vertices[3] = new Vector3(1, -1, -1);
            vertices[4] = new Vector3(-1, 1, 1);
            vertices[5] = new Vector3(-1, 1, -1);
            vertices[6] = new Vector3(-1, -1, 1);
            vertices[7] = new Vector3(-1, -1, -1);
            int[][] faces = new int[6][];
            for (int i = 0; i != 6; i++)
            {
                faces[i] = new int[4];
            }
            /*front*/
            faces[0][0] = 0;
            faces[0][1] = 2;
            faces[0][2] = 4;
            faces[0][3] = 6;
            /*back*/
            faces[1][0] = 7;
            faces[1][1] = 5;
            faces[1][2] = 3;
            faces[1][3] = 1;
            /*top*/
            faces[2][0] = 5;
            faces[2][1] = 4;
            faces[2][2] = 0;
            faces[2][3] = 1;
            /*bottom*/
            faces[3][0] = 7;
            faces[3][1] = 3;
            faces[3][2] = 2;
            faces[3][3] = 4;
            /*right*/
            faces[4][0] = 3;
            faces[4][1] = 1;
            faces[4][2] = 0;
            faces[4][3] = 2;
            /*left*/
            faces[5][0] = 7;
            faces[5][1] = 2;
            faces[5][2] = 4;
            faces[5][3] = 5;

            return new PolyhedronPart(vertices, faces);
        }
    }
}
