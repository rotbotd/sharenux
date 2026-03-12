#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using SkiaSharp;

namespace ShareX.HelpersLib
{
    /// <summary>
    /// 5x5 color transformation matrix compatible with System.Drawing.Imaging.ColorMatrix.
    /// Maps to SkiaSharp's SKColorFilter for actual rendering.
    /// </summary>
    public class ColorMatrix
    {
        private readonly float[][] _matrix;

        public ColorMatrix()
        {
            _matrix = new float[][]
            {
                new float[] { 1, 0, 0, 0, 0 },
                new float[] { 0, 1, 0, 0, 0 },
                new float[] { 0, 0, 1, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }
            };
        }

        public ColorMatrix(float[][] matrix)
        {
            if (matrix.Length != 5)
                throw new ArgumentException("Matrix must have 5 rows");
            foreach (var row in matrix)
            {
                if (row.Length != 5)
                    throw new ArgumentException("Each row must have 5 columns");
            }
            _matrix = matrix;
        }

        public float this[int row, int col]
        {
            get => _matrix[row][col];
            set => _matrix[row][col] = value;
        }

        /// <summary>
        /// Convert to SkiaSharp color filter.
        /// System.Drawing uses RGBA order with 5x5 matrix.
        /// SkiaSharp uses a flat 20-element array in row-major order (4x5, no 5th row).
        /// The 5th row in System.Drawing is the translation/offset row.
        /// </summary>
        public SKColorFilter ToSKColorFilter()
        {
            // SkiaSharp ColorMatrix is 4x5 (20 floats) in row-major order:
            // [ R' ]   [ m00 m01 m02 m03 m04 ] [ R ]
            // [ G' ] = [ m10 m11 m12 m13 m14 ] [ G ]
            // [ B' ]   [ m20 m21 m22 m23 m24 ] [ B ]
            // [ A' ]   [ m30 m31 m32 m33 m34 ] [ A ]
            //                                  [ 1 ]
            // 
            // System.Drawing ColorMatrix is 5x5:
            // Row 0: affects R
            // Row 1: affects G  
            // Row 2: affects B
            // Row 3: affects A
            // Row 4: translation (added after multiplication)
            //
            // Column order: R, G, B, A, W (where W=1)

            float[] skMatrix = new float[20];
            
            // Row 0 (R output): take from column 0 of each input row
            skMatrix[0] = _matrix[0][0];  // R contribution to R
            skMatrix[1] = _matrix[1][0];  // G contribution to R
            skMatrix[2] = _matrix[2][0];  // B contribution to R
            skMatrix[3] = _matrix[3][0];  // A contribution to R
            skMatrix[4] = _matrix[4][0];  // Translation for R

            // Row 1 (G output)
            skMatrix[5] = _matrix[0][1];
            skMatrix[6] = _matrix[1][1];
            skMatrix[7] = _matrix[2][1];
            skMatrix[8] = _matrix[3][1];
            skMatrix[9] = _matrix[4][1];

            // Row 2 (B output)
            skMatrix[10] = _matrix[0][2];
            skMatrix[11] = _matrix[1][2];
            skMatrix[12] = _matrix[2][2];
            skMatrix[13] = _matrix[3][2];
            skMatrix[14] = _matrix[4][2];

            // Row 3 (A output)
            skMatrix[15] = _matrix[0][3];
            skMatrix[16] = _matrix[1][3];
            skMatrix[17] = _matrix[2][3];
            skMatrix[18] = _matrix[3][3];
            skMatrix[19] = _matrix[4][3];

            return SKColorFilter.CreateColorMatrix(skMatrix);
        }

        /// <summary>
        /// Apply this color matrix to a bitmap, returning a new bitmap.
        /// </summary>
        public SKBitmap Apply(SKBitmap source)
        {
            var dest = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
            
            using var canvas = new SKCanvas(dest);
            using var paint = new SKPaint();
            paint.ColorFilter = ToSKColorFilter();
            
            canvas.DrawBitmap(source, 0, 0, paint);
            
            return dest;
        }
    }
}
