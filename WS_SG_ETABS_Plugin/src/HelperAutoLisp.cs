using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WS_SG_ETABS_Plugin.src
{
    internal class HelperAutoLisp
    {

        public static bool DrawLine(ref List<string> lisp, List<double> xCoords, List<double> yCoords)
        {
            bool isError = false;
            
            //Check if xCoords and yCoords same size
            if (xCoords.Count != yCoords.Count)
            {
                MessageBox.Show("X and Y coordinates must have same length");

                isError = true;
                return isError;
            }


            string line = "(command \"line\" ";

            for (int i = 0; i < xCoords.Count; i++)
            {
                line += $"'({xCoords[i]} {yCoords[i]}) ";
            }

            line += "\"\" )";

            lisp.Add(line);

            return isError;
        }

        public static bool DrawCircle(ref List<string> lisp, double xCoord, double yCoord, double diameter, bool solidHatch)
        {
            bool isError = false;

            string line = $"(command \"circle\" '({xCoord} {yCoord}) \"DIAMETER\" {diameter}) ";

            lisp.Add(line);

            if (solidHatch)
            {
                line = "(command \"-hatch\" \"select\" \"last\" \"\" \"properties\" \"solid\" \"\")";
                lisp.Add(line);
            }

            return isError;
        }

        public static bool AddText(ref List<string> lisp, double xCoord, double yCoord, double textSize, double textRotation, string text)
        {
            bool isError = false;

            string line = $"(command \"text\" '({xCoord} {yCoord}) {textSize} {textRotation} \"{text}\") ";

            lisp.Add(line);

            return isError;
        }
    }
}
