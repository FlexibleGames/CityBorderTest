using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CityBorderTest
{
    struct GridBase
    {
        public int mx;
        public int my;
        public int x;
        public int y;
        public int cityflag;        
        public GridBase(int tx, int ty, int tmx, int tmy, int cflag)
        {
            x = tx;
            y = ty;
            mx = tmx;
            my = tmy;
            cityflag = cflag;
        }
    }
    struct CityEdge
    {
        public int x;
        public int y;
        public byte cityflag;
        public bool processed;
        public CityEdge(int tx, int ty, byte tflag)
        {
            x = tx;
            y = ty;
            cityflag = tflag;
            processed = false;
        }
        public override bool Equals(object obj)
        {
            if (((CityEdge)obj).x == -2 && ((CityEdge)obj).y == -2)
            {
                return cityflag == ((CityEdge)obj).cityflag;
            }
            else
            {
                return (x == ((CityEdge)obj).x && y == ((CityEdge)obj).y && cityflag == ((CityEdge)obj).cityflag);
            }
        }
        public override int GetHashCode()
        {
            return x^y+cityflag;
        }
    }
    public partial class BorderTestForm : Form
    {
        private bool is_running = false;
        private bool is_add_owner = false;
        private bool draw_grid = false;
        private bool show_ownership = false;
        private bool show_edges = false;
        private bool show_strengths = false;
        private bool show_border = false;
        private byte show_city = 0;
        private int m_winXSize;
        private int m_winYSize;
        private int m_panelXSize;
        private int m_panelYSize;
        private Bitmap m_buffer;
        private Bitmap m_borderBuffer;
        private Bitmap m_borderBufferC2;
        private int m_mouseXLoc;
        private int m_mouseYLoc;
        private int m_mouseGridXLoc;
        private int m_mouseGridYLoc;
        private int[][] m_gridValues;
        private int[][] m_gridValuesC2;
        private byte[][] m_gridOwner;
        private ArrayList BorderBases;
        private ArrayList BorderOwners;
        private ArrayList BorderEdges;
        private int city1EdgeCount;
        private int city2EdgeCount;
        

        public BorderTestForm()
        {
            InitializeComponent();
            comboCitySelect.SelectedIndex = 0;
            m_mouseXLoc = 0;
            m_mouseYLoc = 0;
            m_mouseGridXLoc = 0;
            m_mouseGridYLoc = 0;
            show_city = 0;
            BorderBases = new ArrayList();
            BorderOwners = new ArrayList();
            // testing
 //           BorderBases.Add(new GridBase(9,10,185,205,0));
//            BorderBases.Add(new GridBase(17,8,345,165,1));

            BorderEdges = new ArrayList();
            city1EdgeCount = 0;
            city2EdgeCount = 0;
            m_winXSize = 640;
            m_winYSize = 480;
            m_panelXSize = 1040;
            m_panelYSize = 800;
            draw_grid = true;
            show_ownership = false;
            show_edges = false;
            show_strengths = true;
            show_border = false;
            this.ClientSize = new Size(m_winXSize, m_winYSize);
            this.panelMain.Size = new Size(m_panelXSize, m_panelYSize);
            this.panelBorder.Size = new Size(m_panelXSize / 20, m_panelYSize / 20);
            this.panelBorder.Location = new Point(m_panelXSize + 20, panelMain.Location.Y);
            this.panelBorder2.Size = new Size(m_panelXSize / 20, m_panelYSize / 20);
            this.panelBorder2.Location = new Point(m_panelXSize + 20, panelBorder.Location.Y + panelBorder.Size.Height + 5);
            m_buffer = new Bitmap(m_panelXSize, m_panelYSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            m_borderBuffer = new Bitmap(m_panelXSize/20, m_panelYSize/20, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            m_borderBufferC2 = new Bitmap(m_panelXSize / 20, m_panelYSize / 20, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            is_add_owner = false;            
            // grid values, from pixel to coord in jagged array
            m_gridValues = new int[m_panelXSize/20][];
            m_gridValuesC2 = new int[m_panelXSize / 20][];
            m_gridOwner = new byte[m_panelXSize / 20][];
            for (int x = 0; x < m_panelXSize / 20; x++)
            {
                m_gridValues[x] = new int[m_panelYSize / 20];
                m_gridValuesC2[x] = new int[m_panelYSize / 20];
                m_gridOwner[x] = new byte[m_panelYSize / 20];
            }
            // initialize to 0;
            for (int x = 0; x < m_panelXSize / 20; x++)
            {
                for (int y = 0; y < m_panelYSize / 20; y++)
                {
                    m_gridValues[x][y] = 0;
                    m_gridValuesC2[x][y] = 0;
                    m_gridOwner[x][y] = 0;
                }
            }
        }

        // Game loop function for seperate thread.
        private void GameLoop()
        {            
        }

        // render the panel!
        private void RenderPanel()
        {

            Graphics g = Graphics.FromImage(m_buffer);
            Graphics pg = Graphics.FromImage(m_borderBuffer);
            Graphics pgc2 = Graphics.FromImage(m_borderBufferC2);
            cbAddingOwner.Checked = is_add_owner;
            // Clear the panels            
            Brush panelBrush = new SolidBrush(Color.Black);
            g.FillRectangle(panelBrush, 0, 0, m_panelXSize, m_panelYSize);
            panelBrush.Dispose();
            panelBrush = new SolidBrush(Color.FromArgb(255,0,0,0));
            pg.FillRectangle(panelBrush, 0, 0, m_panelXSize / 20, m_panelYSize / 20);
            pgc2.FillRectangle(panelBrush, 0, 0, m_panelXSize / 20, m_panelYSize / 20);
            panelBrush.Dispose();

            if (draw_grid)
            {
                DrawGrid(ref g);
            }

            // do we need to update the border?
            if (BorderBases.Count > 0)
            {
                UpdateBorder(ref pg, ref pgc2);
                CalcStrengths();
                FindEdges();
                if (show_ownership)
                {
                    DrawOwnership(ref g);                    
                }
                if (show_strengths)
                {
                    DrawStrengths(ref g);
                }
                if (show_edges)
                {
                    DrawEdges(ref g);
                }
                if (show_border)
                {
                    if (show_city == 1)
                    {
                        DrawBorders(ref g, 1);
                    }
                    else if (show_city == 2)
                    {
                        DrawBorders(ref g, 2);
                    }
                    else if (show_city == 0)
                    {
                        DrawBorders(ref g, 1);
                        DrawBorders(ref g, 2);
                    }
                }
            }

            panelBrush = new SolidBrush(Color.Green);
            Font panelFont = new Font(FontFamily.GenericSansSerif, 11);
            g.DrawString("X : " + m_mouseXLoc.ToString(), panelFont, panelBrush, 5, 5, StringFormat.GenericDefault);
            g.DrawString("Y : " + m_mouseYLoc.ToString(), panelFont, panelBrush, 5, 20, StringFormat.GenericDefault);
            g.DrawString("X Grid : " + m_mouseGridXLoc.ToString(), panelFont, panelBrush, 5, 35, StringFormat.GenericDefault);
            g.DrawString("Y Grid: " + m_mouseGridYLoc.ToString(), panelFont, panelBrush, 5, 50, StringFormat.GenericDefault);

            panelBrush.Dispose();
            panelFont.Dispose();
            g.Dispose();
            pg.Dispose();

            pg = panelBorder.CreateGraphics();
            pg.DrawImage(m_borderBuffer, 0, 0);
            pgc2 = panelBorder2.CreateGraphics();
            pgc2.DrawImage(m_borderBufferC2, 0, 0);
            g = panelMain.CreateGraphics();
            g.DrawImage(m_buffer, 0, 0);
            g.Dispose();
            pg.Dispose();
            pgc2.Dispose();
        }

        /// <summary>
        /// Draws the Grid
        /// </summary>
        /// <param name="g">Graphics Device for main panel</param>
        private void DrawGrid(ref Graphics g)
        {
//            Graphics g = Graphics.FromImage(m_buffer);

            Pen gridPen = new Pen(Color.FromArgb(128,55,55,55));
            gridPen.Width = 1.0F;
            
            // horizontal lines first
            for (int y = 0;y<m_panelYSize;y+=20)
            {
                g.DrawLine(gridPen, 0, y, m_panelXSize - 1, y);
            }

            // now vertical lines
            for (int x = 0; x < m_panelXSize; x += 20)
            {
                g.DrawLine(gridPen, x, 0, x, m_panelYSize - 1);
            }          
//            g.Dispose();
            gridPen.Dispose();
        }

        /// <summary>
        /// Updates Border panel image
        /// </summary>
        /// <param name="pg">Border Panel Graphics device</param>
        private void UpdateBorder(ref Graphics pg, ref Graphics pgc2)
        {
            //gnarly code goes here                                                
            GraphicsPath gpath = new GraphicsPath();
            foreach (GridBase gb in BorderBases)
            {
                gpath.Reset();
                gpath.AddEllipse(gb.mx/20 - 6, gb.my/20 - 6, 12, 12);

                PathGradientBrush pgBrush = new PathGradientBrush(gpath);
                pgBrush.CenterPoint = new Point(gb.mx/20, gb.my/20);
                pgBrush.CenterColor = Color.FromArgb(254, 255, 255, 255);

                Color[] bcolor = { Color.FromArgb(1, 0, 0, 0) };
                pgBrush.SurroundColors = bcolor;
                if (gb.cityflag == 0)
                {
                    pg.FillEllipse(pgBrush, gb.mx / 20 - 6, gb.my / 20 - 6, 12, 12);
                }
                else
                {
                    pgc2.FillEllipse(pgBrush, gb.mx / 20 - 6, gb.my / 20 - 6, 12, 12);
                }
                pgBrush.Dispose();                
            }
            gpath.Dispose(); 
            // update grid array with new values from image
        }

        /// <summary>
        /// Pulls values from jagged array and displays them on the grid
        /// </summary>
        /// <param name="g">Graphics device for main panel</param>
        private void DrawStrengths(ref Graphics g)
        {
            Brush borderbrush = new SolidBrush(Color.Red);
            Brush borderbrushc2 = new SolidBrush(Color.Yellow);
            Font borderfont = new Font(FontFamily.GenericMonospace, 8);
            for (int x = 0; x < m_panelXSize / 20; x++)
            {
                for (int y = 0; y < m_panelYSize / 20; y++)
                {
                    if (m_gridValues[x][y] == m_gridValuesC2[x][y] &&
                        m_gridValues[x][y] != 0)
                    {
                        m_gridValuesC2[x][y]--;
                    }
                    int bstrength = m_gridValues[x][y];
                    int b2strength = m_gridValuesC2[x][y];
                    if (bstrength > 0 && show_city == 1)
                    {
                        if (bstrength > 99)
                        {
                            g.DrawString(bstrength.ToString(), borderfont, borderbrush, x * 20 + 2, y * 20 + 5, StringFormat.GenericDefault);
                        }
                        else
                        {
                            g.DrawString(bstrength.ToString(), borderfont, borderbrush, x * 20 + 5, y * 20 + 5, StringFormat.GenericDefault);
                        }
                    }
                    if (b2strength > 0 && show_city == 2) 
                    {
                        if (b2strength > 99)
                        {
                            g.DrawString(b2strength.ToString(), borderfont, borderbrushc2, x * 20 + 2, y * 20 + 5, StringFormat.GenericDefault);
                        }
                        else
                        {
                            g.DrawString(b2strength.ToString(), borderfont, borderbrushc2, x * 20 + 5, y * 20 + 5, StringFormat.GenericDefault);
                        }
                    }
                    if (show_city == 0)
                    {
                        if (bstrength > b2strength)
                        {
                            if (bstrength > 0)
                            {
                                if (bstrength > 99)
                                {
                                    g.DrawString(bstrength.ToString(), borderfont, borderbrush, x * 20 + 2, y * 20 + 5, StringFormat.GenericDefault);
                                }
                                else
                                {
                                    g.DrawString(bstrength.ToString(), borderfont, borderbrush, x * 20 + 5, y * 20 + 5, StringFormat.GenericDefault);
                                }
                            }
                        }
                        else 
                        {
                            if (b2strength > 0)
                            {
                                if (b2strength > 99)
                                {
                                    g.DrawString(b2strength.ToString(), borderfont, borderbrushc2, x * 20 + 2, y * 20 + 5, StringFormat.GenericDefault);
                                }
                                else
                                {
                                    g.DrawString(b2strength.ToString(), borderfont, borderbrushc2, x * 20 + 5, y * 20 + 5, StringFormat.GenericDefault);
                                }
                            }
                        }
                    }
                }
            }
            borderbrush.Dispose();
            borderbrushc2.Dispose();
            borderfont.Dispose();
        }

        /// <summary>
        /// Fills the gridValues and gridValuesC2 jagged arrays.
        /// </summary>
        private void CalcStrengths()
        {
            if (BorderBases.Count > 0)
            {
                for (int x = 0; x < m_panelXSize / 20; x++)
                {
                    for (int y = 0; y < m_panelYSize / 20; y++)
                    {
                        m_gridValues[x][y] = m_borderBuffer.GetPixel(x, y).R;
                        m_gridValuesC2[x][y] = m_borderBufferC2.GetPixel(x, y).R;
                        if (m_gridValues[x][y] == m_gridValuesC2[x][y] && m_gridValues[x][y] != 0)
                        {
                            m_gridValuesC2[x][y]--;
                        }
                    }
                }
            }
            if (BorderOwners.Count > 0)
            {
                foreach (GridBase gb in BorderOwners)
                {
                    if (gb.cityflag == 0)
                    {
                        m_borderBuffer.SetPixel(gb.x, gb.y, Color.FromArgb(255, 254, 254, 254));
                        m_gridValues[gb.x][gb.y] = 254;
                    }
                    else
                    {
                        m_borderBufferC2.SetPixel(gb.x,gb.y, Color.FromArgb(255,254,254,254));
                        m_gridValuesC2[gb.x][gb.y] = 254;                        
                    }
                }
            }
        }

        /// <summary>
        /// Draws small circles where city 1 or 2 owns that square.
        /// </summary>
        /// <param name="g">Graphics device for main panel</param>
        private void DrawOwnership(ref Graphics g)
        {
            Brush city1 = new SolidBrush(Color.Red);
            Brush city2 = new SolidBrush(Color.Yellow);

            for (int x = 0; x < m_panelXSize / 20; x++)
            {
                for (int y = 0; y < m_panelYSize / 20; y++)
                {
                    if (m_gridOwner[x][y] != 0)
                    {
                        if (m_gridOwner[x][y] == 1 && (show_city == 1 || show_city ==0))
                        {
                            g.FillEllipse(city1, x * 20 + 6, y * 20 + 6, 10, 10);
                        }
                        else if (m_gridOwner[x][y] == 2 && (show_city == 2 || show_city == 0))
                        {
                            g.FillEllipse(city2, x * 20 + 6, y * 20 + 6, 10, 10);
                        }
                    }
                }
            }
            city1.Dispose();
            city2.Dispose();
        }

        /// <summary>
        /// procedure to find and store the border edges for the two cities
        /// </summary>
        private void FindEdges()
        {
            BorderEdges.Clear();
            city1EdgeCount = 0;
            city2EdgeCount = 0;
            bool isEdge = false;
            for (int x = 0; x < m_panelXSize / 20; x++)
            {
                for (int y = 0; y < m_panelYSize / 20; y++)
                {
                    if ((x - 1 < 0 ||
                        y - 1 < 0 ||
                        x + 1 > m_panelXSize / 20 - 1 ||
                        y + 1 > m_panelYSize / 20 - 1) && 
                        (m_gridValues[x][y] > 0 ||
                        m_gridValuesC2[x][y] > 0))
                    {
                        if (m_gridValues[x][y] > m_gridValuesC2[x][y])
                        {
                            BorderEdges.Add(new CityEdge(x, y, 1));
                            city1EdgeCount++;
                        }
                        else
                        {
                            BorderEdges.Add(new CityEdge(x, y, 2));
                            city2EdgeCount++;
                        }
                        continue;
                    }                    
                    if (m_gridValues[x][y] > 0)
                    {
                        if (m_gridValues[x - 1][y] == 0 ||
                            m_gridValues[x + 1][y] == 0 ||
                            m_gridValues[x][y - 1] == 0 ||
                            m_gridValues[x][y + 1] == 0)
                        {
                            isEdge = true;
                        }
                        if (m_gridValues[x][y] < m_gridValuesC2[x][y])
                        {
                            isEdge = false;
                        }
                        else
                        {
                            if ((m_gridValues[x-1][y] > m_gridValuesC2[x-1][y] &&
                                m_gridValues[x+1][y] < m_gridValuesC2[x+1][y]) ||
                                (m_gridValues[x][y-1] > m_gridValuesC2[x][y-1] &&
                                m_gridValues[x][y+1] < m_gridValuesC2[x][y+1]) ||
                                (m_gridValues[x+1][y] > m_gridValuesC2[x+1][y] &&
                                m_gridValues[x-1][y] < m_gridValuesC2[x-1][y]) ||
                                (m_gridValues[x][y+1] > m_gridValuesC2[x][y+1] &&
                                m_gridValues[x][y-1] < m_gridValuesC2[x][y-1]))
                            {
                                isEdge = true;
                            }
                        }
                        if (isEdge)
                        {
                            BorderEdges.Add(new CityEdge(x, y, 1));
                            city1EdgeCount++;
                            isEdge = false;
                        }
                    }
                    if (m_gridValuesC2[x][y] > 0)
                    {
                        if (m_gridValuesC2[x - 1][y] == 0 ||
                            m_gridValuesC2[x + 1][y] == 0 ||
                            m_gridValuesC2[x][y - 1] == 0 ||
                            m_gridValuesC2[x][y + 1] == 0)
                        {
                            isEdge = true;
                        }
                        if (m_gridValuesC2[x][y] < m_gridValues[x][y])
                        {
                            isEdge = false;
                        }
                        else
                        {
                            if ((m_gridValuesC2[x - 1][y] > m_gridValues[x - 1][y] &&
                                m_gridValuesC2[x + 1][y] < m_gridValues[x + 1][y]) ||
                                (m_gridValuesC2[x][y - 1] > m_gridValues[x][y - 1] &&
                                m_gridValuesC2[x][y + 1] < m_gridValues[x][y + 1]) ||
                                (m_gridValuesC2[x + 1][y] > m_gridValues[x + 1][y] &&
                                m_gridValuesC2[x - 1][y] < m_gridValues[x - 1][y]) ||
                                (m_gridValuesC2[x][y + 1] > m_gridValues[x][y + 1] &&
                                m_gridValuesC2[x][y - 1] < m_gridValues[x][y - 1]))
                            {
                                isEdge = true;
                            }
                        }
                        if (isEdge)
                        {
                            BorderEdges.Add(new CityEdge(x, y, 2));
                            city2EdgeCount++;
                            isEdge = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw edges held in BorderEdges
        /// </summary>
        /// <param name="g">Graphics device for main panel</param>
        private void DrawEdges(ref Graphics g)
        {
            Pen city1 = new Pen(Color.Red, 1);
            Pen city2 = new Pen(Color.Yellow, 1);
            Pen bothcities = new Pen(Color.Purple, 1);

            foreach (CityEdge ce in BorderEdges)
            {
                if (show_city == 0)
                {
                    if (ce.cityflag == 1)
                    {
                        g.DrawEllipse(city1, ce.x * 20 + 8, ce.y * 20 + 8, 8, 8);
                    }
                    else
                    {
                        g.DrawEllipse(city2, ce.x * 20 + 8, ce.y * 20 + 8, 8, 8);
                    }
                }
                else if (show_city == 1 && ce.cityflag == 1)
                {
                    g.DrawEllipse(city1, ce.x * 20 + 8, ce.y * 20 + 8, 8, 8);
                }
                else if (show_city == 2 && ce.cityflag == 2)
                {
                    g.DrawEllipse(city2, ce.x * 20 + 8, ce.y * 20 + 8, 8, 8);
                }
            }
        }

        /// <summary>
        /// Draw line borders for cities
        /// </summary>
        /// <param name="g">Graphics device for main panel</param>
        /// <param name="city_to_process">City flag to process, 1 or 2</param>
        private void DrawBorders(ref Graphics g, byte city_to_process)
        {
            // temp edge used for ArrayList.Contains() call
            ArrayList points = new ArrayList();
            CityEdge containsEdge = new CityEdge(0, 0, 0);
            CityEdge tempEdge = new CityEdge(0, 0, 0);
            int cityEdgeCount = (city_to_process == 1) ? city1EdgeCount : city2EdgeCount;
            int tempindex = 0;
            Point[] pointarray;
            CityEdge currentEdge = new CityEdge(-2,-2, city_to_process);
            tempindex = BorderEdges.IndexOf(currentEdge);
            
            if (tempindex >= 0)
            {
                currentEdge = (CityEdge)BorderEdges[tempindex];
                tempEdge.cityflag = city_to_process;
                points.Add(new Point(currentEdge.x, currentEdge.y));
                #region City Border Search
                for (int x = 0; x < cityEdgeCount; x++)
                {
                    // Search x y-1, x+1 y-1, x+1 y, x+1 y+1, x y+1 (from noon to 6)
                    tempEdge.x = currentEdge.x;
                    tempEdge.y = currentEdge.y - 1;
                    tempindex = BorderEdges.IndexOf((CityEdge)tempEdge);
                    if (tempindex >= 0)
                    {
                        tempEdge = (CityEdge)BorderEdges[tempindex];
                        if (!tempEdge.processed)
                        {
                            tempEdge.processed = true;
                            BorderEdges[tempindex] = tempEdge;
                            points.Add(new Point(tempEdge.x, tempEdge.y));
                            currentEdge = tempEdge;
                            continue;
                        }
                    }

                    tempEdge.x = currentEdge.x + 1;
                    tempEdge.y = currentEdge.y - 1;
                    tempindex = BorderEdges.IndexOf((CityEdge)tempEdge);
                    if (tempindex >= 0)
                    {
                        tempEdge = (CityEdge)BorderEdges[tempindex];
                        if (!tempEdge.processed)
                        {
                            tempEdge.processed = true;
                            BorderEdges[tempindex] = tempEdge;
                            points.Add(new Point(tempEdge.x, tempEdge.y));
                            currentEdge = tempEdge;
                            continue;
                        }
                    }
                    tempEdge.x = currentEdge.x + 1;
                    tempEdge.y = currentEdge.y;
                    tempindex = BorderEdges.IndexOf((CityEdge)tempEdge);
                    if (tempindex >= 0)
                    {
                        tempEdge = (CityEdge)BorderEdges[tempindex];
                        if (!tempEdge.processed)
                        {
                            tempEdge.processed = true;
                            BorderEdges[tempindex] = tempEdge;
                            points.Add(new Point(tempEdge.x, tempEdge.y));
                            currentEdge = tempEdge;
                            continue;
                        }
                    }
                    tempEdge.x = currentEdge.x + 1;
                    tempEdge.y = currentEdge.y + 1;
                    tempindex = BorderEdges.IndexOf((CityEdge)tempEdge);
                    if (tempindex >= 0)
                    {
                        tempEdge = (CityEdge)BorderEdges[tempindex];
                        if (!tempEdge.processed)
                        {
                            tempEdge.processed = true;
                            BorderEdges[tempindex] = tempEdge;
                            points.Add(new Point(tempEdge.x, tempEdge.y));
                            currentEdge = tempEdge;
                            continue;
                        }
                    }
                    tempEdge.x = currentEdge.x;
                    tempEdge.y = currentEdge.y + 1;
                    tempindex = BorderEdges.IndexOf((CityEdge)tempEdge);
                    if (tempindex >= 0)
                    {
                        tempEdge = (CityEdge)BorderEdges[tempindex];
                        if (!tempEdge.processed)
                        {
                            tempEdge.processed = true;
                            BorderEdges[tempindex] = tempEdge;
                            points.Add(new Point(tempEdge.x, tempEdge.y));
                            currentEdge = tempEdge;
                            continue;
                        }
                    }
                    // search from 6 to 12
                    // x-1 y+1, x-1 y, x-1 y-1
                    tempEdge.x = currentEdge.x - 1;
                    tempEdge.y = currentEdge.y + 1;
                    tempindex = BorderEdges.IndexOf((CityEdge)tempEdge);
                    if (tempindex >= 0)
                    {
                        tempEdge = (CityEdge)BorderEdges[tempindex];
                        if (!tempEdge.processed)
                        {
                            tempEdge.processed = true;
                            BorderEdges[tempindex] = tempEdge;
                            points.Add(new Point(tempEdge.x, tempEdge.y));
                            currentEdge = tempEdge;
                            continue;
                        }
                    }
                    tempEdge.x = currentEdge.x - 1;
                    tempEdge.y = currentEdge.y;
                    tempindex = BorderEdges.IndexOf((CityEdge)tempEdge);
                    if (tempindex >= 0)
                    {
                        tempEdge = (CityEdge)BorderEdges[tempindex];
                        if (!tempEdge.processed)
                        {
                            tempEdge.processed = true;
                            BorderEdges[tempindex] = tempEdge;
                            points.Add(new Point(tempEdge.x, tempEdge.y));
                            currentEdge = tempEdge;
                            continue;
                        }
                    }
                    tempEdge.x = currentEdge.x - 1;
                    tempEdge.y = currentEdge.y - 1;
                    tempindex = BorderEdges.IndexOf((CityEdge)tempEdge);
                    if (tempindex >= 0)
                    {
                        tempEdge = (CityEdge)BorderEdges[tempindex];
                        if (!tempEdge.processed)
                        {
                            tempEdge.processed = true;
                            BorderEdges[tempindex] = tempEdge;
                            points.Add(new Point(tempEdge.x, tempEdge.y));
                            currentEdge = tempEdge;
                            continue;
                        }
                    }
                }
                #endregion
            }

            // draw city border
            if (points.Count > 0)
            {
                Pen cityPen = new Pen(Color.Red, 1);
                cityPen.Color = (city_to_process == 2) ? Color.Yellow : Color.Red;
                cityPen.LineJoin = LineJoin.Round;
                pointarray = new Point[points.Count];
                for (int x = 0; x < points.Count; x++)
                {
                    pointarray[x] = (Point)points[x];
                    pointarray[x].X = pointarray[x].X * 20 + 10;
                    pointarray[x].Y = pointarray[x].Y * 20 + 10;
                }
                g.DrawCurve(cityPen, pointarray, 0.9f);
                cityPen.Dispose();
            }
            points.Clear();
        }

        #region Control Events
        private void BorderTestForm_Paint(object sender, PaintEventArgs e)
        {
            // Do Nothing
        }

        private void BorderTestForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Capture Controls
            is_running = false;
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
            else if (e.KeyCode == Keys.G)
            {
                draw_grid = !draw_grid;
                RenderPanel();
            }
            else if (e.KeyCode == Keys.O)
            {
                show_ownership = !show_ownership;
                RenderPanel();
            }
            else if (e.KeyCode == Keys.E)
            {
                show_edges = !show_edges;
                RenderPanel();
            }
            else if (e.KeyCode == Keys.S)
            {
                show_strengths = !show_strengths;
                RenderPanel();
            }
            else if (e.KeyCode == Keys.D1)
            {
                show_city = 1;
                RenderPanel();
            }
            else if (e.KeyCode == Keys.D2)
            {
                show_city = 2;
                RenderPanel();
            }
            else if (e.KeyCode == Keys.D0)
            {
                show_city = 0;
                RenderPanel();
            }
            else if (e.KeyCode == Keys.B)
            {
                show_border = !show_border;
                RenderPanel();
            }
        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {
            // Main Panel Paint routine
            RenderPanel();
        }

        private void BorderTestForm_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void panelMain_MouseMove(object sender, MouseEventArgs e)
        {
            m_mouseXLoc = e.X;
            m_mouseYLoc = e.Y;
            m_mouseGridXLoc = e.X / 20;
            m_mouseGridYLoc = e.Y / 20;
            RenderPanel();
        }

        private void panelMain_MouseDown(object sender, MouseEventArgs e)
        {
            // register a click!
            int cityselected = comboCitySelect.SelectedIndex;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (!is_add_owner)
                {
                    // we already have grid locations so activate location
                    m_gridValues[m_mouseGridXLoc][m_mouseGridYLoc] = 254;
                    GridBase newbase = new GridBase(m_mouseGridXLoc, m_mouseGridYLoc, m_mouseXLoc, m_mouseYLoc, cityselected);

                    if (!BorderBases.Contains(newbase))
                    {
                        BorderBases.Add(newbase);
                        m_gridOwner[m_mouseGridXLoc][m_mouseGridYLoc] = Convert.ToByte(cityselected + 1);
                    }
                    RenderPanel();
                }
                else
                {
                    GridBase newowner = new GridBase(m_mouseGridXLoc, m_mouseGridYLoc, m_mouseXLoc, m_mouseYLoc, cityselected);
                    if (!BorderOwners.Contains(newowner))
                    {
                        BorderOwners.Add(newowner);
                        m_gridOwner[m_mouseGridXLoc][m_mouseGridYLoc] = Convert.ToByte(cityselected + 1);
                    }
                    RenderPanel();
                }
            }
        }        

        private void comboCitySelect_KeyDown(object sender, KeyEventArgs e)
        {
            BorderTestForm_KeyDown(sender, e);
        }

        private void btnAddOwner_Click(object sender, EventArgs e)
        {
            is_add_owner = !is_add_owner;
            cbAddingOwner.Invalidate();
            RenderPanel();
        }        

        private void cbAddingOwner_KeyDown(object sender, KeyEventArgs e)
        {
            BorderTestForm_KeyDown(sender, e);
        }

        private void btnAddOwner_KeyDown(object sender, KeyEventArgs e)
        {
            BorderTestForm_KeyDown(sender, e);            
        }
        #endregion
    }
}
