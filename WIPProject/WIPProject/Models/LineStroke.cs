using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace WIPProject.Models
{
    public class LineStroke
    {
        private const int maxNumberOfLines = 100;
        //public int[] indices;
        public List<Line> lines;
        private int currentIndex = 0;

        public LineStroke()
        {
            //indices = new int[maxNumberOfLines];
            lines = new List<Line>();
        }

        public void AddLine(Line l)
        {
            //if (currentIndex >= maxNumberOfLines)
            //{
            //    RemoveFirstLine();
            //}

            //indices[currentIndex++] = index;
            lines.Add(l);
            //indices.Add(l);

            ++currentIndex;
        }

        private void RemoveFirstLine()
        {
            //lines[0] = null;

            //for (int i = 0; i < lines.Count - 1; ++i)
            //{
            //    Line top = lines[i + 1];
            //    Line bottom = lines[i];

            //    bottom = top;
            //}
            //lines[lines.Count - 1] = null;

            --currentIndex;

            lines.RemoveAt(0);
        }

        public void Reset()
        {
            currentIndex = 0;
        }
    }
}
