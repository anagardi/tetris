using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Tetris
{
    public class MovementHelper
    {
        public static int DefaultUnits { get { return Properties.Settings.Default.DefaultUnits; } }
        public static int TetrisUnits { get { return Properties.Settings.Default.TetrisUnits; } }

        public static bool CanMoveDown(Figure figure, Canvas canvas)
        {
            if (figure == null)
                return false;
            if (figure.Top + figure.Height >= canvas.Height)
                return false;

            int leftBound = figure.Left;
            int rightBound = figure.Left + figure.Width;
            int topBound = figure.Top;

            foreach (TextBlock block in canvas.Children)
            {
                if (figure.Blocks.Contains(block))
                    continue;

                double left = Canvas.GetLeft(block);
                double top = Canvas.GetTop(block);

                if (left < leftBound || left > rightBound - figure.Offset)
                    continue;
                for (int i = 0; i < figure.Blocks.Count; i++)
                {
                    TextBlock currentBlock = figure.Blocks[i];
                    double topCurrent = Canvas.GetTop(currentBlock);
                    double leftCurrent = Canvas.GetLeft(currentBlock);
                    if ((leftCurrent == left)
                        && (topCurrent + figure.Offset >= top)
                        && (top > topBound))
                        return false;
                }
            }

            return true;
        }

        public static bool CanMoveLeft(Figure figure, Canvas canvas)
        {
            if (figure == null)
                return false;
            if (figure.Left == 0)
                return false;

            foreach (TextBlock block in canvas.Children)
            {
                if (figure.Blocks.Contains(block))
                    continue;

                double left = Canvas.GetLeft(block);
                double top = Canvas.GetTop(block);            

                for (int i = 0; i < figure.Blocks.Count; i++)
                {
                    TextBlock currentBlock = figure.Blocks[i];
                    double topCurrent = Canvas.GetTop(currentBlock);
                    double leftCurrent = Canvas.GetLeft(currentBlock);
                   
                    if (leftCurrent == left + figure.Offset)
                    {
                        if (((topCurrent <= top) && (topCurrent + figure.Offset > top))
                            || ((topCurrent >= top) && (topCurrent < top + figure.Offset)))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static bool CanMoveRight(Figure figure, Canvas canvas)
        {
            if (figure == null)
                return false;
            if (figure.Left + figure.Width == canvas.Width)
                return false;            

            foreach (TextBlock block in canvas.Children)
            {
                if (figure.Blocks.Contains(block))
                    continue;

                double left = Canvas.GetLeft(block);
                double top = Canvas.GetTop(block);            

                for (int i = 0; i < figure.Blocks.Count; i++)
                {
                    TextBlock currentBlock = figure.Blocks[i];
                    double topCurrent = Canvas.GetTop(currentBlock);
                    double leftCurrent = Canvas.GetLeft(currentBlock);

                    if (leftCurrent == left - figure.Offset)
                        if (((topCurrent <= top) && (topCurrent + figure.Offset > top))
                            || ((topCurrent >= top) && (topCurrent < top + figure.Offset)))
                        {
                            return false;
                        }
                }
            }
            return true;
        }

        public static bool CanRotateLeft(Figure figure, Canvas canvas)
        {
            int id = GetNextLeftRotationId(figure, canvas);
            return RotatedFigureIntersectsWithOtherBlocks(id, figure, canvas);
        }

        public static bool CanRotateRight(Figure figure, Canvas canvas)
        {
            int id = GetNextRightRotationId(figure, canvas);
            return RotatedFigureIntersectsWithOtherBlocks(id, figure, canvas);
        }

        public static int GetNextLeftRotationId(Figure figure, Canvas canvas)
        {
            return figure.RotationId == 3 ? 0 : figure.RotationId + 1;
        }

        public static int GetNextRightRotationId(Figure figure, Canvas canvas)
        {
            return figure.RotationId == 0 ? 3 : figure.RotationId - 1;
        }

        public static bool RotatedFigureIntersectsWithOtherBlocks(int id, Figure figure, Canvas canvas)
        {
            foreach (TextBlock block in canvas.Children)
            {
                if (figure.Blocks.Contains(block))
                    continue;

                double left = Canvas.GetLeft(block);
                double top = Canvas.GetTop(block);

                for (int i = 0; i < figure.Blocks.Count; i++)
                {
                    int leftOffset = figure.Left + figure.GetLeftOffset(i, id);
                    int topOffset = figure.Top + figure.GetTopOffset(i, id);
                    if (((leftOffset == left) && (topOffset == top)) || ((leftOffset < 0) || (leftOffset >= canvas.Width)))
                        return false;
                }
            }
            return true;
        }

        public static int CalculateScore(Figure figure, Canvas canvas, int currentScore, out int linesCount)
        {
            linesCount = 0;
            //a single line clear is 100 points, clearing four lines at once is worth 800
            for (int offset = figure.Offset; offset < canvas.Height; offset += figure.Offset)
            {
                Collection<TextBlock> blocks = new Collection<TextBlock>();

                double count = canvas.Width / figure.Offset;
                foreach (TextBlock block in canvas.Children)
                {
                    double top = Canvas.GetTop(block);
                    if (top == offset)
                    {
                        count--;
                        blocks.Add(block);
                    }
                    if (count == 0)
                    {                        
                        foreach (TextBlock textBlock in blocks)
                        {
                            canvas.Children.Remove(textBlock);
                        }

                        foreach (TextBlock textBlock in canvas.Children)
                        {
                            double getTop = Canvas.GetTop(textBlock);
                            if (getTop < offset)
                            {
                                Canvas.SetTop(textBlock, getTop + figure.Offset);
                            }
                        }
                        linesCount++;                  
                        
                        if (Properties.Settings.Default.WithAcceleration)
                        {
                            if ((Properties.Settings.Default.SpeedWithAcceleration > 150)
                                && (currentScore > 500))
                            {
                                Properties.Settings.Default.SpeedWithAcceleration -= 25;
                                Properties.Settings.Default.Save();
                            }
                        }
                        break; 
                    }
                }
            }

            if ((linesCount > 0) && (linesCount % 4 == 0))
            {
                currentScore += Properties.Settings.Default.TetrisUnits;
            }
            else
            {
                currentScore += Properties.Settings.Default.DefaultUnits * linesCount;
            }

            return currentScore;
        }

        public static void MoveRight(Figure figure, Canvas canvas)
        {
            if (CanMoveRight(figure, canvas))
            {
                figure.Left += figure.Offset;
                figure.SetLocation(figure.Left, figure.Top);
            }
        }

        public static void MoveLeft(Figure figure, Canvas canvas)
        {
            if (CanMoveLeft(figure, canvas))
            {
                figure.Left -= figure.Offset;
                figure.SetLocation(figure.Left, figure.Top);
            }
        }

        public static void RotateRight(Figure figure, Canvas canvas)
        {
            if (CanRotateRight(figure, canvas))
            {
                figure.RotationId = GetNextRightRotationId(figure, canvas);
                figure.SetLocation(figure.Left, figure.Top);
            }
        }

        public static void RotateLeft(Figure figure, Canvas canvas)
        {
            if (CanRotateLeft(figure, canvas))
            {
                figure.RotationId = GetNextLeftRotationId(figure, canvas);
                figure.SetLocation(figure.Left, figure.Top);
            }
        }
    }
}
