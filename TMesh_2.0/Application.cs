﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace TMesh_2._0
{
    public partial class Application : Form
    {
        Mesh mesh;
        private enum Status { selected, segmented, none, Visible };
        private bool showBB;
        private Status act_state;
        private Color[] colors = { Color.Blue, Color.Red,Color.Green,Color.Gray,Color.Gold,Color.Violet,Color.Cyan, Color.Orange, Color.Lime,Color.Maroon,Color.ForestGreen,Color.DarkCyan};
        private double tx, ty, tz, rx, ry, rz, scale; //traslation, rotation and scale factors
        private Point lMouseLoc;
        private List<Tuple<double[], double[]>> bbx;
        private List<Face> visibles;
        private Matrix4 matr;
        private int NUMFACE = 2; //to calculate distance from NUMFACE to all other faces

        public Application()
        {
            InitializeComponent();
            Initialize();
        }
        
        #region OpenTK control methods
        private void SetupViewPort()//Prepare the board to an apropiate image projection
        {
            int w = Board.Width;
            int h = Board.Height;
            GL.Viewport(0, 0, w, h);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 matrix = Matrix4.Perspective(45.0f, w / h, 1.0f, 100.0f);
            this.matr = matrix;
            GL.LoadMatrix(ref matrix);
            GL.MatrixMode(MatrixMode.Modelview);    
            GL.Enable(EnableCap.DepthTest);            
        }
        private void SetupLigthing()//Creates the lights
        {
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);
            GL.Enable(EnableCap.Light2);
            GL.Enable(EnableCap.Light3);
            GL.Enable(EnableCap.Light4);
            GL.Enable(EnableCap.Light5);
            GL.Enable(EnableCap.Light6);
            GL.Enable(EnableCap.Light7);
        }
        private void Settings()//Prepare the board to just paint the triangles and lines
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(tx, ty, tz);
            GL.Rotate(rx, 1, 0, 0);
            GL.Rotate(ry, 0, 1, 0);
            GL.Rotate(rz, 0, 0, 1);
            GL.Scale(scale, scale, scale);            
        }
        private void DrawLines()//to drow lines of the triangles
        {
            GL.LineWidth(1.0f);
            GL.Color3(Color.White);
            GL.Begin(BeginMode.Lines);
            Vertex u, v;

            for (int i = 0; i < mesh.CountEdge; i++)
            {
                u = mesh.VertexesAt(mesh.EdgesAt(i).frm);
                v = mesh.VertexesAt(mesh.EdgesAt(i).dst);
                GL.Vertex3(u.X, u.Y, u.Z);
                GL.Vertex3(v.X, v.Y, v.Z);
            }            
            GL.End();
        }
        private void DrawTriangles()
        {
            GL.Begin(BeginMode.Triangles);
            Face f;
            if (this.chkFarest.Checked)//to show the selected pick of the affinity matrix
            {
                this.act_state = Status.none;
                try
                {
                    for (int i = 0; i < mesh.K; i++)
                    {
                        f = mesh.FacesAt(mesh.nextIndex[i]);
                        GL.Color3(Color.LightGray);
                        Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                        GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                        GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                        GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                        GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                    }
                }
                catch { }
            }
            if (this.act_state == Status.Visible)//to show cone of visibility of a given face
            {
                    f = mesh.FacesAt(NUMFACE);
                    Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                    GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                    GL.Color3(Color.Red);
                    GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                    GL.Color3(Color.Green);
                    GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                    GL.Color3(Color.Blue);
                    GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);                

                foreach (var item in visibles)
                {
                    f = item;
                    GL.Color3(Color.Gold);
                    normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                    GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                    GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                    GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                    GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                }
                if (this.chkTriangles.Checked)
                {
                    for (int i = 0; i < mesh.CountFace; i++)
                    {
                        f = mesh.FacesAt(i);
                        GL.Color3(Color.White);
                        normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                        GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                        GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                        GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                        GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                    }
                }
            }
            else
            {
                for (int i = 0; i < mesh.CountFace; i++)
                {
                    f = mesh.FacesAt(i);
                    if (this.act_state == Status.selected)//to show distances scale to a given Face (in NUMFACE)
                    {
                        if (i == NUMFACE)
                        {
                            Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                            GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                            GL.Color3(Color.Red);
                            GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                            GL.Color3(Color.Green);
                            GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                            GL.Color3(Color.Blue);
                            GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                        }
                        else
                        {
                            double dist = mesh.distancesMatrix[NUMFACE][i];
                            if (dist > 1)
                                GL.Color3(Color.Red);
                            else GL.Color3(dist, 1 - dist, 0);
                            Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                            GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                            GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                            GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                            GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                        }
                    }
                    else
                    {
                        if (this.act_state == Status.segmented)//after segment paint each triangle of the color of its group
                        {
                            GL.Color3(colors[mesh.cluster[i]]);//20
                            Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                            GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                            GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                            GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                            GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                        }
                        else//default style for a triangle
                        {
                            GL.Color3(Color.LightSeaGreen);
                            Vector normal = Vector.normal(new Vector(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z), new Vector(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z), new Vector(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z));
                            GL.Normal3(normal.elements[0], normal.elements[1], normal.elements[2]);
                            GL.Vertex3(mesh.VertexesAt(f.i).X, mesh.VertexesAt(f.i).Y, mesh.VertexesAt(f.i).Z);
                            GL.Vertex3(mesh.VertexesAt(f.j).X, mesh.VertexesAt(f.j).Y, mesh.VertexesAt(f.j).Z);
                            GL.Vertex3(mesh.VertexesAt(f.k).X, mesh.VertexesAt(f.k).Y, mesh.VertexesAt(f.k).Z);
                        }
                    }
                }
            }
            GL.End();
        }
        private void DrawCoords()//to drow coordinate system
        {
            GL.LineWidth(3.0f);
            GL.Color3(Color.Red);
            GL.Begin(BeginMode.Lines);

            GL.Vertex3(-100, 0, 0);
            GL.Vertex3(100, 0, 0);

            GL.Vertex3(0, -100, 0);
            GL.Vertex3(0, 100, 0);

            GL.Vertex3(0, 0,-10);
            GL.Vertex3(0, 0, 10);

            GL.End();

            DrawNormal();
        }
        private void DrawNormal()//to drow normals to all faces
        {
            GL.LineWidth(3.0f);
            GL.Color3(Color.Red);
            GL.Begin(BeginMode.Lines);
            for (int i = 0; i < mesh.CountFace; i++)
            {
                Vector baricenter = mesh.Baricenter(i);
                Vector normal = Vector.normal(new Vector(mesh.VertexesAt(mesh.FacesAt(i).i).X, mesh.VertexesAt(mesh.FacesAt(i).i).Y, mesh.VertexesAt(mesh.FacesAt(i).i).Z), new Vector(mesh.VertexesAt(mesh.FacesAt(i).j).X, mesh.VertexesAt(mesh.FacesAt(i).j).Y, mesh.VertexesAt(mesh.FacesAt(i).j).Z), new Vector(mesh.VertexesAt(mesh.FacesAt(i).k).X, mesh.VertexesAt(mesh.FacesAt(i).k).Y, mesh.VertexesAt(mesh.FacesAt(i).k).Z));
                GL.Vertex3(baricenter.elements[0],baricenter.elements[1],baricenter.elements[2]);
                GL.Vertex3(baricenter.elements[0]+normal.elements[0], baricenter.elements[1] + normal.elements[1], baricenter.elements[2]+normal.elements[2]);
            }
            GL.End();
        }
        #endregion

        #region File handle methods
        private void OpenOFF(object sender, EventArgs e)
        {
            Initialize();
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "|*.off";

            if (file.ShowDialog() == DialogResult.OK)
            {
                mesh.LoadOFF(file.FileName);
                lblInfo.Text = "Vertexes: " + mesh.CountVertex.ToString() + "\n" + "Faces: " + mesh.CountFace.ToString();
                Board.Invalidate();
            }
        }
        private int[] OpenSEG(string fileName,int n)
        {
            int[] clusters = new int[n];
            StreamReader reader = new StreamReader(fileName);
            for (int i = 0; i < n; i++)
                clusters[i] = int.Parse(reader.ReadLine());
            return clusters;
        }
        #endregion

        #region Auxiliar methods
        private void Initialize()//Just for initialize the Form Class
        {
            mesh = new Mesh();
            this.distanceSelector.Text = mesh.Distance.ToString();
            act_state = Status.none;
            this.tx = 0;
            this.ty = 0;
            this.tz = -10;
            this.rx = 0;
            this.ry = 0;
            this.rz = 0;
            this.scale = 1;
            this.showBB = false;
        }
        private void Zoom(double factor)
        {
            tz += factor;
            Board.Invalidate();
        }
        private static double NormalizeAngle(double angle)
        {
            while(angle < 0) angle += 360;
            while(angle >= 360) angle -= 360;
            return angle;
        }
        private void compare()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "|*.seg";

            if (file.ShowDialog() == DialogResult.OK)
            {
                int[] other = OpenSEG(file.FileName, mesh.CountFace);
                MessageBox.Show("Resultado usando Indice de Rand: " + mesh.compareWith(other).ToString() + "\n" + "Resultado usando Indice de Jackard: " + mesh.compareWith(other, false, mesh.partitions));
                compareWith c = new compareWith(other, mesh);
                Thread aux = new Thread(() => c.ShowDialog());
                aux.Start();
                this.lblSim.Text = mesh.compareWith(other).ToString();
                this.lblSim.Visible = true;
                aux.Join();
            }
            this.lblSim.Visible = false;
        }
        #endregion

        #region Forms control methods
        private void Exit(object sender, EventArgs e)
        {
            this.Close();
        }
        private void chBoxShowAxes_CheckedChanged(object sender, EventArgs e)
        {
            Board.Invalidate();
        }
        private void btn_segment_Click(object sender, EventArgs e)
        {
            SegmentationSettings s = new SegmentationSettings();
            CombinationMenu c = new CombinationMenu();

        Menu:
            s.ShowDialog();
            if (s.DialogResult == DialogResult.OK)
            {
                if (this.distanceSelector.SelectedIndex == 4)
                {
                    c.ShowDialog();
                    if (c.DialogResult == DialogResult.Cancel || (c.angular == 0 && c.geodesic == 0))
                        goto Menu;
                }
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                lblFar.Visible = false;
                lblMid.Visible = false;
                lblNear.Visible = false;
                int elapsed = mesh.Segment(this.chkHiddenFaces.Checked, s.numGroups, s.numIter, c.angular, c.geodesic, s.K);
                MessageBox.Show("Segmented in: " + elapsed + " sec");
                this.act_state = Status.segmented;
                Board.Invalidate();
                this.lblK.Visible = true;
                this.lblK.Text = "K: " + mesh.K.ToString();
            }
        }
        private void Board_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                mesh.build_dual_graph(Math.Max((int)((double)mesh.CountFace),3),true);
                this.act_state = Status.selected;
                panel1.BackColor = Color.FromArgb(255, 0, 0);
                panel2.BackColor = Color.FromArgb((int)((double)255*0.5), (int)((double)255 * 0.5), 0);
                panel3.BackColor = Color.FromArgb(0, 255, 0);
                lblFar.Text = "1";
                lblMid.Text = "0.5";
                lblNear.Text = "0";
                panel1.Visible = true;
                panel2.Visible = true;
                panel3.Visible = true;
                lblFar.Visible = true;
                lblMid.Visible = true;
                lblNear.Visible = true;
                Board.Invalidate();
            }            
        }
        private void Board_Scroll(object sender, ScrollEventArgs e)
        {
            int moved = e.OldValue - e.NewValue;
            if (moved > 0)
            {
                Zoom(1*moved);
                Board.Invalidate();
            }
            else
            {
                Zoom(-1*moved);
                Board.Invalidate();
            }
        }
        private void ZoomOut(object sender, EventArgs e)
        {
            Zoom(-3);
            Board.Invalidate();
        }
        private void ZoomIn(object sender, EventArgs e)
        {
            Zoom(2);
            Board.Invalidate();
        }
        private void scrollZoom_Scroll(object sender, ScrollEventArgs e)
        {
            int moved = e.OldValue-e.NewValue;
            if (moved > 0)
            {
                Zoom(1.005);
                Board.Invalidate();
            }
            else
            {
                Zoom(0.95);
                Board.Invalidate();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {}
        private void Board_Load(object sender, EventArgs e)
        {
            SetupViewPort();
            SetupLigthing();
        }
        private void Board_Resize(object sender, EventArgs e)
        {
            SetupViewPort();
            Board.Invalidate();
        }
        private void Board_Paint(object sender, PaintEventArgs e)//Method called when invalidate
        {
            Settings();
            if (chBoxShowAxes.Checked)
            {
                DrawCoords();
            }
            DrawTriangles();
            if(chkLines.Checked)
                DrawLines();            
            if (showBB)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(Color.Azure);
                for (int i = 0; i < bbx.Count; i++)
                {
                    GL.Vertex3(bbx[i].Item1[0], bbx[i].Item1[1], bbx[i].Item1[2]);
                    GL.Vertex3(bbx[i].Item2[0], bbx[i].Item2[1], bbx[i].Item2[2]);
                }
                GL.End();
            }
            if (aabbTest.Checked)
            {
                GL.Begin(BeginMode.Lines);
                GL.Color3(Color.Green);
                double epsilon = 0.1;
                for (int i = 0; i < mesh.aabb.tests.Count; i++)
                {
                    GL.Vertex3(mesh.aabb.tests[i][0]-epsilon, mesh.aabb.tests[i][1], mesh.aabb.tests[i][2]);
                    GL.Vertex3(mesh.aabb.tests[i][0]+epsilon, mesh.aabb.tests[i][1], mesh.aabb.tests[i][2]);

                    GL.Vertex3(mesh.aabb.tests[i][0], mesh.aabb.tests[i][1]-epsilon, mesh.aabb.tests[i][2]);
                    GL.Vertex3(mesh.aabb.tests[i][0], mesh.aabb.tests[i][1]+epsilon, mesh.aabb.tests[i][2]);

                    GL.Vertex3(mesh.aabb.tests[i][0], mesh.aabb.tests[i][1], mesh.aabb.tests[i][2]-epsilon);
                    GL.Vertex3(mesh.aabb.tests[i][0], mesh.aabb.tests[i][1], mesh.aabb.tests[i][2]+epsilon);
                }
                GL.End();
            }
            // DrawNormal();
            Board.SwapBuffers();
        }
        private void Board_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.A:
                    this.tx -= 1;
                    break;
                case Keys.S:
                    this.ty -= 1;
                    break;
                case Keys.D:
                    this.tx += 1;
                    break;
                case Keys.W:
                    this.ty += 1;
                    break;
                case Keys.T:
                    this.tz += 1;
                    break;
                case Keys.B:
                    this.tz -= 1;
                    break;
                case Keys.Q:
                    this.rz -= 10;
                    break;
                case Keys.E:
                    this.rz += 10;
                    break;
                case Keys.Z:
                    this.rx -= 10;
                    break;
                case Keys.C:
                    this.rx += 10;
                    break;
                case Keys.R:
                    this.ry -= 10;
                    break;
                case Keys.V:
                    this.ry += 10;
                    break;
                case Keys.D0:
                    if (GL.IsEnabled(EnableCap.Light0))
                        GL.Disable(EnableCap.Light0);
                    else GL.Enable(EnableCap.Light0);
                    break;
                case Keys.D1:
                    if (GL.IsEnabled(EnableCap.Light1))
                        GL.Disable(EnableCap.Light1);
                    else GL.Enable(EnableCap.Light1);
                    break;
                case Keys.D2:
                    if (GL.IsEnabled(EnableCap.Light2))
                        GL.Disable(EnableCap.Light2);
                    else GL.Enable(EnableCap.Light2);
                    break;
                case Keys.D3:
                    if (GL.IsEnabled(EnableCap.Light3))
                        GL.Disable(EnableCap.Light3);
                    else GL.Enable(EnableCap.Light3);
                    break;
                case Keys.D4:
                    if (GL.IsEnabled(EnableCap.Light4))
                        GL.Disable(EnableCap.Light4);
                    else GL.Enable(EnableCap.Light4);
                    break;
                case Keys.D5:
                    if (GL.IsEnabled(EnableCap.Light5))
                        GL.Disable(EnableCap.Light5);
                    else GL.Enable(EnableCap.Light5);
                    break;
                case Keys.D6:
                    if (GL.IsEnabled(EnableCap.Light6))
                        GL.Disable(EnableCap.Light6);
                    else GL.Enable(EnableCap.Light6);
                    break;

                default:
                    break;
            }
            Board.Invalidate();
        }
        private void Board_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {                
                this.lMouseLoc = e.Location;
            }
            if (e.Button == MouseButtons.Right)
            {
                mesh.build_dual_graph(mesh.CountFace,true,true);
                this.act_state = Status.selected;
                panel1.BackColor = Color.FromArgb(255, 0, 0);
                panel2.BackColor = Color.FromArgb((int)((double)255 * 0.5), (int)((double)255 * 0.5), 0);
                panel3.BackColor = Color.FromArgb(0, 255, 0);
                lblFar.Text = "Far";
                lblMid.Text = "Midle";
                lblNear.Text = "Near";
                panel1.Visible = true;
                panel2.Visible = true;
                panel3.Visible = true;
                lblFar.Visible = true;
                lblMid.Visible = true;
                lblNear.Visible = true;
                Board.Invalidate();
            }
        }
        private void Board_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.ry += (e.Location.X - lMouseLoc.X);
                this.rx += (e.Location.Y - lMouseLoc.Y);
                Board.Invalidate();
                Cursor = Cursors.Default;
            }
        }
        private void visibilityBtn_Click(object sender, EventArgs e)
        {
            this.act_state = Status.Visible;
            this.visibles = mesh.TestVisibility(NUMFACE, this.chkHiddenFaces.Checked, 60);
            this.Board.Invalidate();
        }
        private void btnBbox_Click(object sender, EventArgs e)
        {
            if (this.showBB)
                this.showBB = false;
            else this.showBB = true;
            this.bbx = mesh.Getbbx();
            this.Board.Invalidate();
        }
        private void btnSaveImage(object sender, EventArgs e)
        {
            try
            {
                int width = Board.Width;
                int height = Board.Height;
                byte[] rgbData = new byte[width * height * 3];

                Bitmap bmp = new Bitmap(width, height);
                BitmapData data = bmp.LockBits(Board.ClientRectangle, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                bmp.UnlockBits(data);
                saveFileDialog1.Title = "Choose a folder to save the screen";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog1.FileName != "")
                        bmp.Save(saveFileDialog1.FileName, ImageFormat.Bmp);
                }
            }
            catch
            {
                throw new Exception("There was an error saving file");
            }
        }
        private void btnCmp_Click(object sender, EventArgs e)
        {
            compare();
        }
        private void Board_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.lMouseLoc != null && e.Button == MouseButtons.Left)
            {
                Cursor = Cursors.NoMove2D;
                this.ry += (e.Location.X - lMouseLoc.X);
                this.rx += (e.Location.Y - lMouseLoc.Y);
                Board.Invalidate();
            }
            this.lMouseLoc = e.Location;
        }
        private void aabbTest_CheckedChanged(object sender, EventArgs e)
        {
            Board.Invalidate();
        }
        private void btnSaveSeg(object sender, EventArgs e)
        {
            if (mesh.cluster != null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Segmentation file|*.seg|Binary file|*.bin";
                saveFileDialog1.Title = "Choose a folder to save the segmentation";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog1.FileName != "")
                    {
                        if (saveFileDialog1.FileName.Contains(".seg"))
                        {
                            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                            for (int i = 0; i < mesh.cluster.Length; i++)
                                sw.WriteLine(mesh.cluster[i]);
                            sw.Close();
                        }
                        else
                            mesh.Save(saveFileDialog1.FileName);
                    }
                }
                
            }
        }
        private void cargarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "Segmentation file|*.seg|Binary file|*.bin";

            if (file.ShowDialog() == DialogResult.OK)
            {
                if (file.FileName.Contains(".bin"))
                {
                    IFormatter format = new BinaryFormatter();
                    Stream s = new FileStream(file.FileName, FileMode.Open);
                    this.mesh = (Mesh)format.Deserialize(s);
                    s.Close();
                    if (mesh.cluster != null)
                        this.act_state = Status.segmented;
                    Board.Invalidate();
                }
                else
                {
                    StreamReader sr = new StreamReader(file.FileName);
                    mesh.cluster = new int[mesh.CountFace];
                    for (int i = 0; i < mesh.cluster.Length; i++)
                    {
                        try
                        {
                            mesh.cluster[i] = int.Parse(sr.ReadLine());
                        }
                        catch
                        {
                            throw new Exception("The file is corrupted or not represent the loaded OFF file");
                        }
                    }
                    sr.Close();
                    this.act_state = Status.segmented;
                    Board.Invalidate();
                }
            }
        }
        private void chkTriangles_CheckedChanged(object sender, EventArgs e)
        {
            Board.Invalidate();
        }
        private void VolumetricType_TextChanged(object sender, EventArgs e)
        {
            throw new Exception("Just testing");
        }
        private void chkHiddenFaces_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void chkLines_CheckedChanged(object sender, EventArgs e)
        {
            Board.Invalidate();
        }
        private void distanceSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (distanceSelector.SelectedIndex)
            {
                case 0:
                    distanceSelector.Text = "Angular";
                    this.mesh.Distance = Mesh.DistanceType.Angular;
                    break;
                case 1:
                    distanceSelector.Text = "Geodesic";
                    this.mesh.Distance = Mesh.DistanceType.Geodesic;
                    break;
                case 2:
                    distanceSelector.Text = "Volumetric";
                    this.mesh.Distance = Mesh.DistanceType.Volumetric;
                    break;
                case 3:
                    distanceSelector.Text = "Producto Angular-Geodesica";
                    this.mesh.Distance = Mesh.DistanceType.Combined1;
                    break;
                case 4:
                    distanceSelector.Text = "Combinacion Angular-Geodesica-Volumetrica";
                    this.mesh.Distance = Mesh.DistanceType.Combined2;
                    break;
                default:
                    break;
            }
        }
        private void chkFarest_CheckedChanged(object sender, EventArgs e)
        {
            Board.Invalidate();
        }        
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TestMenu tm = new TestMenu();
            TestResults tr = new TestResults();
            tm.ShowDialog();
            if(tm.DialogResult == DialogResult.OK)
            {
                List<Vertex> verts = new List<Vertex>();
                verts.Add(new Vertex(tm.values[0],tm.values[1],tm.values[2]));
                verts.Add(new Vertex(tm.values[3], tm.values[4], tm.values[5]));
                verts.Add(new Vertex(tm.values[6], tm.values[7], tm.values[8]));
                verts.Add(new Vertex(tm.values[9], tm.values[10], tm.values[11]));
                Face t1 = new Face(0,1,2);
                Face t2 = new Face(0,3,1);
                double result = this.mesh.testGeodesic(t1,t2,verts);
                MessageBox.Show(result.ToString());
            }
        }
        private void chbJustDistances_CheckedChanged(object sender, EventArgs e)
        {
            this.mesh.GetAffinity = !chbJustDistances.Checked;
        }
        #endregion
    }
}
