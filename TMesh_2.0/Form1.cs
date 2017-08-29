using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform;


namespace TMesh_2._0
{
    public partial class Form1 : Form
    {
        Mesh mesh;
        Vertex camPos;
        Vertex center;
        Vertex camUp;

        public Form1()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Board.InitializeContexts();
            mesh = new Mesh();

            camPos = new Vertex(0, 0, 2);
            center = new Vertex(0, 0, 0);
            camUp = new Vertex(0, 1, 0);

        }

        private void Settings()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            
            Gl.glLoadIdentity();
            Glu.gluPerspective(45, Board.Width / Board.Height, 1, 1000);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glEnable(Gl.GL_DEPTH);
            Gl.glViewport(0, 0, Board.Width, Board.Height);
            
            Glu.gluLookAt(camPos.X, camPos.Y, camPos.Z,
                            center.X, center.Y, center.Z,
                            camUp.X, camUp.Y, camUp.Z);

            
        }

        private void PaintGL(object sender, PaintEventArgs e)
        {
            Settings();
            //DrawCoords();
            DrawLines();

        }

        private void DrawLines()
        {
            Gl.glLineWidth(2);
            Gl.glColor3f(1, 1, 1);
            Gl.glBegin(Gl.GL_LINES);

            Vertex u, v;

            for (int i = 0; i < mesh.CountEdge; i++)
            {
                u = mesh.VertexesAt(mesh.EdgesAt(i).dst);
                v = mesh.VertexesAt(mesh.EdgesAt(i).mate);

                Gl.glVertex3d(u.X, u.Y, u.Z);
                Gl.glVertex3d(v.X, v.Y, v.Z);
            }

            Gl.glEnd();
        }

        private void DrawCoords()
        {
            Gl.glLineWidth(1);
            Gl.glColor3f(1, 0, 0);
            Gl.glBegin(Gl.GL_LINES);

           

            for (int x  = -100; x < 100; x++)
            {
                Gl.glVertex3d(x, 0, -100);
                Gl.glVertex3d(x, 0, 100);
               
            }

            for (int z = -100; z < 100; z++)
            {                
                Gl.glVertex3d(-100, 0, z);
                Gl.glVertex3d(100, 0, z);
            }

            Gl.glEnd();
        }

        private void OpenOFF(object sender, EventArgs e)
        {
            Initialize();
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "|*.off";


            if (file.ShowDialog() == DialogResult.OK)
            {
                mesh.LoadOFF(file.FileName);
                Board.Invalidate();
            }

            
            

        }

        private void Zoom(double factor)
        {
            camPos = (camPos - center) * factor + center;
            Board.Invalidate();
        }

        private void ZoomOut(object sender, EventArgs e)
        {
            Zoom(2);
            Board.Invalidate();

        }

        private void ZoomIn(object sender, EventArgs e)
        {
            Zoom(0.5);
            Board.Invalidate();
        }


        private static double NormalizeAngle(double angle)
        {
            while(angle < 0) angle += 360;
            while(angle >= 360) angle -= 360;

            return angle;
        }
        
        private void XRotation(double angle)
        {
            angle = NormalizeAngle(angle);
        }

        private void YRotation(double angle)
        {
            angle = NormalizeAngle(angle);
        }

        private void ZRotation(double angle)
        {
            angle = NormalizeAngle(angle);
        }

        private void wheelEvent(object sender, ScrollEventArgs e)
        {

        }

        private void Exit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Board_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void UpdateCamPosX(object sender, EventArgs e)
        {
            camPos.X = (double)camPosX.Value;
            Board.Invalidate();
        }

        private void UpdateCamPosY(object sender, EventArgs e)
        {
            camPos.Y = (double)camPosY.Value;
            Board.Invalidate();
        }

        private void UpdateCamPosZ(object sender, EventArgs e)
        {
            camPos.Z = (double)camPosZ.Value;
            Board.Invalidate();
        }

        private void UpdateCenterX(object sender, EventArgs e)
        {
            center.X = (double)centerX.Value;
            Board.Invalidate();
        }

        private void UpdateCenterY(object sender, EventArgs e)
        {
            center.Z = (double)centerZ.Value;
            Board.Invalidate();
        }

        private void UpdateCenterZ(object sender, EventArgs e)
        {
            center.Z = (double)centerZ.Value;
            Board.Invalidate();
        }

        private void UpdateCamUpX(object sender, EventArgs e)
        {
            camUp.X = (double)camUpX.Value;
            Board.Invalidate();
        }

        private void UpdateCamUpY(object sender, EventArgs e)
        {
            camUp.Y = (double)camUpY.Value;
            Board.Invalidate();
        }

        private void UpdateCamUpZ(object sender, EventArgs e)
        {
            camUp.Z = (double)camUpZ.Value;
            Board.Invalidate();
        }

        

       

        

        

    }
}
